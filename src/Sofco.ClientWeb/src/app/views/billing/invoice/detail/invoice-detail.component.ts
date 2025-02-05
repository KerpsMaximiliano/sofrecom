import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs";
import { Invoice } from "../../../../models/billing/invoice/invoice";
import { InvoiceService } from "../../../../services/billing/invoice.service";
import { MessageService } from "../../../../services/common/message.service";
import * as FileSaver from "file-saver";
import { FileUploader } from "ng2-file-upload";
import { Cookie } from "ng2-cookies/ng2-cookies";
import { Ng2ModalConfig } from "../../../../components/modal/ng2modal-config";
import { MenuService } from "../../../../services/admin/menu.service";
import { InvoiceStatus } from "../../../../models/enums/invoiceStatus";
import { AuthService } from '../../../../services/common/auth.service';
import * as moment from 'moment';

@Component({
  selector: 'app-invoice-detail',
  templateUrl: './invoice-detail.component.html',
  styleUrls: ['./invoice-detail.component.scss']
})
export class InvoiceDetailComponent implements OnInit, OnDestroy {

    @ViewChild('selectedFile') selectedFile: any;
    @ViewChild('confirmModal') confirmModal;
    @ViewChild('history') history: any;
    @ViewChild('pdfViewer') pdfViewer: any;
    
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
                private authService: AuthService,
                public menuService: MenuService,
                private messageService: MessageService) {}

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
            });
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
        this.uploader = new FileUploader({url: this.service.getUrlForImportFile(this.model.id), 
                                          authToken: 'Bearer ' + Cookie.get('access_token'),
                                          maxFileSize: 10*1024*1024,
                                          allowedMimeType: ['application/pdf'],
                                         });

        this.uploader.onCompleteItem = (item:any, response:any, status:any, headers:any) => {
            if(status == 401){
                this.authService.refreshToken().subscribe(token => {
                    this.messageService.closeLoading();

                    if(token){
                        this.clearSelectedFile();
                        this.messageService.showErrorByFolder('common', 'fileMustReupload');
                        this.pdfConfig();
                    }
                });

                return;
            }

            this.messageService.closeLoading();

            var dataJson = JSON.parse(response);

            if(dataJson){
                if(dataJson.messages) this.messageService.showMessages(dataJson.messages);

                this.model.pdfFileName = dataJson.data.fileName;
                this.model.pdfFileCreatedDate = new Date(dataJson.data.creationDate).toLocaleDateString();
                this.model.pdfFileId = dataJson.data.id;
            }

            this.configUploader();
            this.clearSelectedFile();
        };

        this.uploader.onAfterAddingFile = (file)=> { file.withCredentials = false; };
        this.isExcel = false;
        this.showUploader = true;
    }

    excelConfig(){
        this.uploader = new FileUploader({url: this.service.getUrlForImportFile(this.model.id),
                                          authToken: 'Bearer ' + Cookie.get('access_token') ,
                                          maxFileSize: 10*1024*1024,
                                          allowedMimeType: ['application/vnd.ms-excel','application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'],
                                        });

        this.uploader.onCompleteItem = (item:any, response:any, status:any, headers:any) => {
            if(status == 401){
                this.authService.refreshToken().subscribe(token => {
                    this.messageService.closeLoading();

                    if(token){
                        this.clearSelectedFile();
                        this.messageService.showErrorByFolder('common', 'fileMustReupload');
                        this.excelConfig();
                    }
                });

                return;
            }

            var dataJson = JSON.parse(response);

            if(dataJson){
                if(dataJson.messages) this.messageService.showMessages(dataJson.messages);
                
                this.model.excelFileName = dataJson.data.fileName;
                this.model.excelFileCreatedDate = moment(dataJson.data.creationDate).format('DD/MM/YYYY'),
                this.model.excelFileId = dataJson.data.id;
            }

            this.configUploader();
            this.clearSelectedFile();
        };

        this.uploader.onAfterAddingFile = (file)=> { file.withCredentials = false; };
        this.isExcel = true;
        this.showUploader = true;
    }

    goToProject(){
        this.router.navigate([`/billing/customers/${this.model.customerId}/services/${this.model.serviceId}/projects/${this.projectId}`]);
    }

    goToSearch(){
        this.router.navigate([`/billing/invoice/search`]);
    }
 
    exportExcel(){
        this.service.exportExcelFile(this.model.excelFileId).subscribe(file => {
            FileSaver.saveAs(file, this.model.excelFileName);
        });
    }
 
    exportPdf(){
        this.service.exportPdfFile(this.model.pdfFileId).subscribe(file => {
            FileSaver.saveAs(file, this.model.pdfFileName);
        });
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

            setTimeout(() => { this.goToProject(); }, 1500)
        });
    }

    updateStatus(event){
        if(event.invoiceStatus) this.model.invoiceStatus = event.invoiceStatus;
        if(event.invoiceNumber) this.model.invoiceNumber = event.invoiceNumber;

        if(event.reloadUploader && event.reloadUploader == true) this.configUploader();
    }

    goToSolfac(){
        this.router.navigate(['/billing/solfac/' + this.model.solfacId])
    }

    viewPdf(){
        if(this.model.pdfFileName.endsWith('.pdf')){
            this.service.getPdfFile(this.model.pdfFileId).subscribe(response => {
                this.pdfViewer.renderFile(response.data);
            });
        }
    }
} 