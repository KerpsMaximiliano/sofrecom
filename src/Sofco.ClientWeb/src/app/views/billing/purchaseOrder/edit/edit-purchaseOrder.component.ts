import { Component, OnDestroy, ViewChild, OnInit } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { MessageService } from "app/services/common/message.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs/Subscription";
import { PurchaseOrderService } from "app/services/billing/purchaseOrder.service";
import { FileUploader } from "ng2-file-upload";
import { Cookie } from "ng2-cookies/ng2-cookies";
import * as FileSaver from "file-saver";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";

declare var $: any;

@Component({
    selector: 'edit-purchaseOrder',
    templateUrl: './edit-purchaseOrder.component.html',
    styleUrls: ['./edit-purchaseOrder.component.scss']
})
export class EditPurchaseOrderComponent implements OnInit, OnDestroy {

    @ViewChild('form') form;
    @ViewChild('pdfViewer') pdfViewer;
    @ViewChild('selectedFile') selectedFile: any;

    updateSubscrip: Subscription;
    getSubscrip: Subscription;
    paramsSubscrip: Subscription;
    getByIdSubscrip: Subscription;

    public uploader: FileUploader = new FileUploader({url:""});
    public showUploader: boolean = false;

    public fileName: string;
    public creationDate: string;
    public fileId: number;

    @ViewChild('confirmDeleteFileModal') confirmModal;
    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "confirmDeleteFileModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    constructor(private purchaseOrderService: PurchaseOrderService,
                private router: Router,
                private activatedRoute: ActivatedRoute,
                private messageService: MessageService,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void {
        this.form.model.currencyId = 0;
        this.form.model.clientExternalId = 0;

        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.messageService.showLoading();

            this.getByIdSubscrip = this.purchaseOrderService.getById(params['id']).subscribe(data => {
                this.messageService.closeLoading();
                this.form.model = data;

                this.uploaderConfig();

                if(this.form.model.clientExternalId && this.form.model.clientExternalId != ""){
                    this.form.getAnalytics();
                }

                setTimeout(() => {
                    $('#analytics').val(this.form.model.analyticIds).trigger('change');
                }, 1000);

                $('input').attr('disabled', 'disabled');
                $('#customer-select select').attr('disabled', 'disabled');
                $('input[type=file]').removeAttr('disabled');
            },
            error => {
                this.messageService.closeLoading();
                this.errorHandlerService.handleErrors(error);
            });
        });
    }

    ngOnDestroy(): void {
        if(this.updateSubscrip) this.updateSubscrip.unsubscribe();
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.getByIdSubscrip) this.getByIdSubscrip.unsubscribe();
    }

    uploaderConfig(){
        this.uploader = new FileUploader({url: this.purchaseOrderService.getUrlForImportExcel(this.form.model.id),
                                          authToken: 'Bearer ' + Cookie.get('access_token') ,
                                          maxFileSize: 50*1024*1024
                                        });

        this.uploader.onCompleteItem = (item:any, response:any, status:any, headers:any) => {
            var dataJson = JSON.parse(response);
            
            if(dataJson.messages) this.messageService.showMessages(dataJson.messages);

            if(dataJson){
                this.form.model.fileName = dataJson.data.fileName;
                this.form.model.creationDate = new Date(dataJson.data.creationDate).toLocaleDateString();
                this.form.model.fileId = dataJson.data.id;
            }

            this.clearSelectedFile();
        };

        this.uploader.onAfterAddingFile = (file)=> { file.withCredentials = false; };
        this.showUploader = true;
    }

    clearSelectedFile(){
        if(this.uploader.queue.length > 0){
            this.uploader.queue[0].remove();
        }
  
        this.selectedFile.nativeElement.value = '';
    }

    exportExcel(){
        this.purchaseOrderService.exportFile(this.form.model.fileId).subscribe(file => {
            FileSaver.saveAs(file, this.form.model.fileName);
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    deleteFile(){
        this.confirmModal.hide();
        this.messageService.showLoading();

        this.purchaseOrderService.deleteFile(this.form.model.id).subscribe(response => {
            if(response.messages) this.messageService.showMessages(response.messages);

            this.form.model.fileId = null;
            this.form.model.fileName = null;

            this.messageService.closeLoading();
        },
        err => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(err)
        });
    }
 
    viewFile(){
        if(this.form.model.fileName.endsWith('.pdf')){
            this.purchaseOrderService.getFile(this.form.model.fileId).subscribe(response => {
                this.pdfViewer.renderFile(response.data);
            },
            err => this.errorHandlerService.handleErrors(err));
        }
    }

    update() {
        this.messageService.showLoading();
        this.form.model.analyticIds = $('#analytics').val();

        this.updateSubscrip = this.purchaseOrderService.update(this.form.model).subscribe(
            response => {
                this.messageService.closeLoading();
                if(response.messages) this.messageService.showMessages(response.messages);
            },
            err => {
                this.messageService.closeLoading();
                this.errorHandlerService.handleErrors(err);
            });
    }
}  