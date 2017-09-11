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

@Component({
  selector: 'app-invoice-detail',
  templateUrl: './invoice-detail.component.html',
  styleUrls: ['./invoice-detail.component.scss']
})
export class InvoiceDetailComponent implements OnInit, OnDestroy {

    @ViewChild('selectedFile') selectedFile: any;
    @ViewChild('confirmModal') confirmModal;

    public model: Invoice = new Invoice();
    paramsSubscrip: Subscription;
    getSubscrip: Subscription;

    projectId;
    invoiceNumber: string;
    public uploader: FileUploader = new FileUploader({url:""});

    public showUploader: boolean = false;
    public isExcel: boolean = true;

    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Incluya número de remito antes de aprobar",
        "confirmModal",
        true,
        true,
        "Aceptar",
        "Cancelar"
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

            this.getSubscrip = this.service.getById(params['id']).subscribe(d => {
                this.model = d;

                this.configUploader();
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

        if(!this.model.excelFileName){
            this.excelConfig();
        }
        else{
            if(this.model.invoiceStatus == InvoiceStatus[InvoiceStatus.Rejected] || this.model.invoiceStatus == InvoiceStatus[InvoiceStatus.SendPending]){
                this.excelConfig();
            }
            else {
                if(!this.model.pdfFileName || (this.model.pdfFileName && this.model.invoiceStatus == InvoiceStatus[InvoiceStatus.Sent])){
                    this.pdfConfig();
                }
            }
        }
    }

    pdfConfig(){
        this.uploader = new FileUploader({url: this.service.getUrlForImportPdf(this.model.id), authToken: Cookie.get('access_token') });

        this.uploader.onCompleteItem = (item:any, response:any, status:any, headers:any) => {
            this.model.pdfFileName = `${this.model.accountName}_${this.model.project}_${new Date().toLocaleDateString()}.pdf`;
            this.model.pdfFileCreatedDate = new Date().toLocaleDateString();
            this.configUploader();
            this.clearSelectedFile();
        };

        this.uploader.onAfterAddingFile = (file)=> { file.withCredentials = false; };
        this.isExcel = false;
        this.showUploader = true;
    }

    excelConfig(){
        this.uploader = new FileUploader({url: this.service.getUrlForImportExcel(this.model.id), authToken: Cookie.get('access_token') });

        this.uploader.onCompleteItem = (item:any, response:any, status:any, headers:any) => {
            this.model.excelFileName = `${this.model.accountName}_${this.model.project}_${new Date().toLocaleDateString()}.xlsx`;
            this.model.excelFileCreatedDate = new Date().toLocaleDateString();
            this.configUploader();
            this.clearSelectedFile();
        };

        this.uploader.onAfterAddingFile = (file)=> { file.withCredentials = false; };
        this.isExcel = true;
        this.showUploader = true;
    }

    goBack(){
        this.router.navigate([`/billing/project/${this.projectId}`]);
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
        this.service.sendToDaf(this.model.id).subscribe(data => {
            if(data.messages) this.messageService.showMessages(data.messages);
            this.model.invoiceStatus = InvoiceStatus[InvoiceStatus.Sent];
            this.configUploader();
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    reject(){
        this.service.reject(this.model.id).subscribe(data => {
            if(data.messages) this.messageService.showMessages(data.messages);
            this.model.invoiceStatus = InvoiceStatus[InvoiceStatus.Rejected];
            this.configUploader();
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    annulment(){
        this.service.annulment(this.model.id).subscribe(data => {
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

    openModal(){
        this.confirmModal.show();
    }

    approve(){
        if(this.invoiceNumber && this.invoiceNumber != ""){

            this.service.approve(this.model.id, this.invoiceNumber).subscribe(data => {
                this.confirmModal.hide();
                if(data.messages) this.messageService.showMessages(data.messages);
                this.model.invoiceStatus = InvoiceStatus[InvoiceStatus.Approved];
                this.model.invoiceNumber = this.invoiceNumber;

                this.configUploader();
            },
            err => this.errorHandlerService.handleErrors(err));
        }
        else{
            this.messageService.showError("El número de remito es requerido");
        }
    }

    canRejectInvoice(){
        if(this.menuService.hasFunctionality('REM', 'REJEC') && this.model.invoiceStatus == InvoiceStatus[InvoiceStatus.Sent]){
            return true;
        }

        return false;
    }

    canApprovedInvoice(){
        if(this.menuService.hasFunctionality('REM', 'APROB') && this.model.invoiceStatus == InvoiceStatus[InvoiceStatus.Sent] && this.model.pdfFileName){
            return true;
        }

        return false;
    }

    delete(){
        this.service.delete(this.model.id).subscribe(data => {
            if(data.messages) this.messageService.showMessages(data.messages);

            setTimeout(() => { this.goBack(); }, 1500)
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    canCancel(){
        return this.model.id > 0 && (this.model.invoiceStatus == InvoiceStatus[InvoiceStatus.Sent] 
                                  || this.model.invoiceStatus == InvoiceStatus[InvoiceStatus.Approved]
                                  || this.model.invoiceStatus == InvoiceStatus[InvoiceStatus.Rejected])
    }
}