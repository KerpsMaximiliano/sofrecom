import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs/Subscription";
import { Invoice } from "app/models/billing/invoice/invoice";
import { InvoiceService } from "app/services/billing/invoice.service";
import { MessageService } from "app/services/common/message.service";
import * as FileSaver from "file-saver";
import { FileUploader } from "ng2-file-upload";
import { Cookie } from "ng2-cookies/ng2-cookies";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { MenuService } from "app/services/admin/menu.service";
import { InvoiceStatus } from "app/models/enums/invoiceStatus";

declare var $: any;

@Component({
  selector: 'app-invoice-detail',
  templateUrl: './invoice-detail.component.html',
  styleUrls: ['./invoice-detail.component.scss']
})
export class InvoiceDetailComponent implements OnInit, OnDestroy {

    @ViewChild('selectedFile') selectedFile: any;
    @ViewChild('confirmModal') confirmModal;
    @ViewChild('history') history: any;
    
    public model: Invoice = new Invoice();
    paramsSubscrip: Subscription;
    getSubscrip: Subscription;

    projectId;
    public invoiceId;
    invoiceNumber: string;
    public uploader: FileUploader = new FileUploader({url:""});

    public showUploader: boolean = false;
    public isExcel: boolean = true;

    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "confirmModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );
 
    constructor(private router: Router,
                private activatedRoute: ActivatedRoute,
                private service: InvoiceService,
                public menuService: MenuService,
                private messageService: MessageService,
                private errorHandlerService: ErrorHandlerService) {}

    ngOnInit() {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {

            this.projectId = params['projectId'];
            this.invoiceId = params['id'];

            this.getSubscrip = this.service.getById(params['id']).subscribe(d => {
                this.model = d;

                this.configUploader();

                sessionStorage.setItem("serviceName", this.model.service);
                sessionStorage.setItem("serviceId", this.model.serviceId);
                sessionStorage.setItem("customerId", this.model.customerId);
                sessionStorage.setItem("customerName", this.model.accountName);
            },
            err => this.errorHandlerService.handleErrors(err));
        });
    }

    ngOnDestroy() {
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.getSubscrip) this.paramsSubscrip.unsubscribe();
    }

    configUploader(){
        this.showUploader = false;
        this.isExcel = true;
        var canUploadExcel = this.menuService.hasFunctionality("REM", "ADEXC");
        var canUploadPdf = this.menuService.hasFunctionality("REM", "ADPDF");

        if(this.model.invoiceStatus == InvoiceStatus[InvoiceStatus.Cancelled]) return;

        if(!this.model.excelFileName && canUploadExcel){
            this.excelConfig();
        }
        else{
            if((this.model.invoiceStatus == InvoiceStatus[InvoiceStatus.Rejected] || 
               this.model.invoiceStatus == InvoiceStatus[InvoiceStatus.SendPending]) && canUploadExcel){

                this.excelConfig();
            }
            else {
                if(this.menuService.hasFunctionality('REM', 'APROB') && canUploadPdf &&
                  (!this.model.pdfFileName || (this.model.pdfFileName && this.model.invoiceStatus == InvoiceStatus[InvoiceStatus.Sent]))){

                    this.pdfConfig();
                }
            }
        }
    }

    pdfConfig(){
        this.uploader = new FileUploader({url: this.service.getUrlForImportPdf(this.model.id), 
                                          authToken: 'Bearer ' + Cookie.get('access_token'),
                                          maxFileSize: 10*1024*1024,
                                          allowedMimeType: ['application/pdf'],
                                         });

        this.uploader.onCompleteItem = (item:any, response:any, status:any, headers:any) => {
            this.messageService.succes("billing.invoice.pdfAddedSucces");

            var dataJson = JSON.parse(response);
            
            if(dataJson){
                this.model.pdfFileName = dataJson.data.pdfFileName;
                this.model.pdfFileCreatedDate = new Date(dataJson.data.pdfFileCreatedDate).toLocaleDateString();
            }

            this.configUploader();
            this.clearSelectedFile();
        };

        this.uploader.onAfterAddingFile = (file)=> { file.withCredentials = false; };
        this.isExcel = false;
        this.showUploader = true;
    }

    excelConfig(){
        this.uploader = new FileUploader({url: this.service.getUrlForImportExcel(this.model.id),
                                          authToken: 'Bearer ' + Cookie.get('access_token') ,
                                          maxFileSize: 10*1024*1024,
                                          allowedMimeType: ['application/vnd.ms-excel','application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'],
                                        });

        this.uploader.onCompleteItem = (item:any, response:any, status:any, headers:any) => {
            this.messageService.succes("billing.invoice.excelAddedSucces");

            var dataJson = JSON.parse(response);

            if(dataJson){
                this.model.excelFileName = dataJson.data.excelFileName;
                this.model.excelFileCreatedDate = new Date(dataJson.data.excelFileCreatedDate).toLocaleDateString();
            }

            this.configUploader();
            this.clearSelectedFile();
        };

        this.uploader.onAfterAddingFile = (file)=> { file.withCredentials = false; };
        this.isExcel = true;
        this.showUploader = true;
    }

    goBack(){
        this.router.navigate([`/billing/customers/${this.model.customerId}/services/${this.model.serviceId}/projects/${this.projectId}`]);
    }
 
    exportExcel(){
        this.service.getExcel(this.model.id).subscribe(file => {
            FileSaver.saveAs(file, this.model.excelFileName);
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    exportPdf(){
        this.service.downloadPdf(this.model.id).subscribe(file => {
            FileSaver.saveAs(file, this.model.pdfFileName);
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    clearSelectedFile(){
        if(this.uploader.queue.length > 0){
            this.uploader.queue[0].remove();
        }
  
        this.selectedFile.nativeElement.value = '';
    }

    canDelete(){
        if(this.model.id > 0 && 
           (this.model.invoiceStatus == InvoiceStatus[InvoiceStatus.SendPending] ||
            this.model.invoiceStatus == InvoiceStatus[InvoiceStatus.Rejected]) &&
           this.menuService.hasFunctionality("REM", "RMV")){

            return true;
        }

        return false;
    }

    delete(){
        this.service.delete(this.model.id).subscribe(data => {
            this.confirmModal.hide();
            
            if(data.messages) this.messageService.showMessages(data.messages);

            setTimeout(() => { this.goBack(); }, 1500)
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    private getDateForFile(){
        var date = new Date();

        var yyyy = date.getFullYear().toString();
        var mm = (date.getMonth()+1).toString();
        var dd  = date.getDate().toString();

        var mmChars = mm.split('');
        var ddChars = dd.split('');

        return yyyy + (mmChars[1]?mm:"0"+mmChars[0]) + (ddChars[1]?dd:"0"+ddChars[0]);
    }

    updateStatus(event){
        if(event.invoiceStatus) this.model.invoiceStatus = event.invoiceStatus;
        if(event.invoiceNumber) this.model.invoiceNumber = event.invoiceNumber;

        if(event.reloadUploader && event.reloadUploader == true) this.configUploader();
    }

    goToSolfac(){
        this.router.navigate(['/billing/solfac/' + this.model.solfacId])
    }
} 