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
import { I18nService } from 'app/services/common/i18n.service';

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
                private i18nService: I18nService,
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

        if(this.model.invoiceStatus == InvoiceStatus[InvoiceStatus.Cancelled]) return;

        if(!this.model.excelFileName){
            this.excelConfig();
        }
        else{
            if(this.model.invoiceStatus == InvoiceStatus[InvoiceStatus.Rejected] || this.model.invoiceStatus == InvoiceStatus[InvoiceStatus.SendPending]){
                this.excelConfig();
            }
            else {
                if(this.menuService.hasFunctionality('REM', 'APROB') && 
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
            this.messageService.succes(this.i18nService.translate("billing.invoice.pdfAddedSucces"));

            this.model.pdfFileName = `REMITO_${this.model.accountName}_${this.model.service}_${this.model.project}_${this.getDateForFile()}.pdf`;
            this.model.pdfFileCreatedDate = new Date().toLocaleDateString();
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
            this.messageService.succes(this.i18nService.translate("billing.invoice.excelAddedSucces"));

            this.model.excelFileName = `REMITO_${this.model.accountName}_${this.model.service}_${this.model.project}_${this.getDateForFile()}.xlsx`;
            this.model.excelFileCreatedDate = new Date().toLocaleDateString();
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
        this.service.getPdf(this.model.id).subscribe(file => {
            FileSaver.saveAs(file, this.model.pdfFileName);
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    sendToDaf(){
        this.service.changeStatus(this.model.id, InvoiceStatus.Sent, "", "").subscribe(data => {
            this.confirmModal.hide();
            if(data.messages) this.messageService.showMessages(data.messages);
            this.model.invoiceStatus = InvoiceStatus[InvoiceStatus.Sent];
            this.configUploader();
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    reject(){
        this.service.changeStatus(this.model.id, InvoiceStatus.Rejected, "", "").subscribe(data => {
            this.confirmModal.hide();
            if(data.messages) this.messageService.showMessages(data.messages);
            this.model.invoiceStatus = InvoiceStatus[InvoiceStatus.Rejected];
            this.configUploader();
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    annulment(){
        this.service.changeStatus(this.model.id, InvoiceStatus.Cancelled, "", "").subscribe(data => {
            this.confirmModal.hide();
            if(data.messages) this.messageService.showMessages(data.messages);
            this.model.invoiceStatus = InvoiceStatus[InvoiceStatus.Cancelled];
            this.configUploader();
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    clearSelectedFile(){
        if(this.uploader.queue.length > 0){
            this.uploader.queue[0].remove();
        }
  
        this.selectedFile.nativeElement.value = '';
    }

    canRejectInvoice(){
        if(this.menuService.hasFunctionality('REM', 'REJEC') && this.model.invoiceStatus == InvoiceStatus[InvoiceStatus.Sent]){
            return true;
        }

        return false;
    }

    canSendToDaf(){
        if(this.model.excelFileName && 
           this.menuService.hasFunctionality('REM', 'SEND') &&
          (this.model.invoiceStatus == 'SendPending' || this.model.invoiceStatus == 'Rejected')){
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

    canCancel(){
        return this.model.id > 0 && this.menuService.hasFunctionality('REM', 'ANNUL') 
                                 && (this.model.invoiceStatus == InvoiceStatus[InvoiceStatus.Sent] 
                                 || this.model.invoiceStatus == InvoiceStatus[InvoiceStatus.Approved]
                                 || this.model.invoiceStatus == InvoiceStatus[InvoiceStatus.Rejected])
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

    confirm() {}

    showConfirmDelete(){
        this.confirm = this.delete;
        this.confirmModal.show();
    }

    showConfirmReject(){
        this.confirm = this.reject;
        this.confirmModal.show();
    }

    showConfirmSendToDaf(){
        this.confirm = this.sendToDaf;
        this.confirmModal.show();
    }

    showConfirmAnnulment(){
        this.confirm = this.annulment;
        this.confirmModal.show();
    }

    updateStatus(event){
        if(event.invoiceStatus) this.model.invoiceStatus = event.invoiceStatus;
        if(event.invoiceNumber) this.model.invoiceNumber = event.invoiceNumber;

        if(event.reloadUploader && event.reloadUploader == true) this.configUploader();
    }
} 