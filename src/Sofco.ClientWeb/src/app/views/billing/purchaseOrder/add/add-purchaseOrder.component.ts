import { Component, OnDestroy, ViewChild, OnInit } from "@angular/core";
import { MessageService } from "app/services/common/message.service";
import { Subscription } from "rxjs";
import { PurchaseOrderService } from "app/services/billing/purchaseOrder.service";
import { FileUploader } from "ng2-file-upload";
import { Cookie } from "ng2-cookies/ng2-cookies";
import * as FileSaver from "file-saver";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { PurchaseOrderStatus } from "app/models/enums/purchaseOrderStatus";

declare var $: any;

@Component({
    selector: 'add-purchaseOrder',
    templateUrl: './add-purchaseOrder.component.html',
    styleUrls: ['./add-purchaseOrder.component.scss']
})
export class NewPurchaseOrderComponent implements OnInit, OnDestroy {

    @ViewChild('form') form;
    @ViewChild('selectedFile') selectedFile: any;
    @ViewChild('pdfViewer') pdfViewer;

    @ViewChild('confirmDeleteFileModal') confirmModal;
    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "confirmDeleteFileModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    addSubscrip: Subscription;

    public uploader: FileUploader = new FileUploader({url:""});
    public showUploader: boolean = false;

    public fileName: string;
    public creationDate: string;
    public fileId: number;

    public alertDisable: boolean = true;
 
    constructor(private purchaseOrderService: PurchaseOrderService,
                private messageService: MessageService){
    }

    ngOnInit(): void {
        this.form.model.currencyId = 0;
        this.form.model.clientExternalId = 0;
        this.form.getCustomers(true);
    }

    ngOnDestroy(): void {
        if(this.addSubscrip) this.addSubscrip.unsubscribe();
    }

    add() {
        this.messageService.showLoading();

        var client = this.form.customers.find(x => x.id == this.form.model.clientExternalId);

        this.form.model.clientExternalName = client ? client.text : '';

        this.form.model.analyticIds = $('#analytics').val();

        this.addSubscrip = this.purchaseOrderService.add(this.form.model).subscribe(
            response => {
                this.messageService.closeLoading();

                this.form.model.id = response.data.id;
                this.form.model.status = response.data.status;
                this.uploaderConfig();

                this.alertDisable = false;
            },
            err => {
                this.messageService.closeLoading();
            });
    }

    uploaderConfig(){
        this.uploader = new FileUploader({url: this.purchaseOrderService.getUrlForImportExcel(this.form.model.id),
                                          authToken: 'Bearer ' + Cookie.get('access_token'),
                                          maxFileSize: 50*1024*1024
                                        });

        this.uploader.onCompleteItem = (item:any, response:any, status:any, headers:any) => {
            var dataJson = JSON.parse(response);
            
            if(dataJson.messages) this.messageService.showMessages(dataJson.messages);

            if(dataJson){
                this.alertDisable = true;
                this.fileName = dataJson.data.fileName;
                this.creationDate = new Date(dataJson.data.creationDate).toLocaleDateString();
                this.fileId = dataJson.data.id;
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
        this.purchaseOrderService.exportFile(this.fileId).subscribe(file => {
            FileSaver.saveAs(file, this.fileName);
        });
    }
    
    viewFile(){
        if(this.fileName.endsWith('.pdf')){
            this.purchaseOrderService.getFile(this.fileId).subscribe(response => {
                this.pdfViewer.renderFile(response.data);
            });
        }
    }

    deleteFile(){
        this.confirmModal.hide();
        this.messageService.showLoading();

        this.purchaseOrderService.deleteFile(this.form.model.id).subscribe(response => {
            if(response.messages) this.messageService.showMessages(response.messages);

            this.fileId = null;
            this.fileName = null;

            this.messageService.closeLoading();
        },
        err => {
            this.messageService.closeLoading();
        });
    }

    canDelete(){
        return this.form.model.status == PurchaseOrderStatus.Draft || this.form.model.status == PurchaseOrderStatus.Reject;
    }
} 