import { Component, OnDestroy, ViewChild, OnInit } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { MessageService } from "../../../../services/common/message.service";
import { Subscription } from "rxjs";
import { PurchaseOrderService } from "../../../../services/billing/purchaseOrder.service";
import { FileUploader } from "ng2-file-upload";
import { Cookie } from "ng2-cookies/ng2-cookies";
import * as FileSaver from "file-saver";
import { Ng2ModalConfig } from "../../../../components/modal/ng2modal-config";
import { I18nService } from "../../../../services/common/i18n.service";
import { PurchaseOrderStatus } from "../../../../models/enums/purchaseOrderStatus";
import { MenuService } from "../../../../services/admin/menu.service";
import { AuthService } from "../../../../services/common/auth.service";

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
    @ViewChild('ocAdjustment') ocAdjustment: any;
    @ViewChild('history') history: any;

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
                private i18nService: I18nService,
                private activatedRoute: ActivatedRoute,
                public menuService: MenuService,
                private authService: AuthService,
                private messageService: MessageService,
                private router: Router){
    }

    ngOnInit(): void {
        this.form.model.currencyId = 0;
        this.form.model.clientExternalId = 0;

        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.messageService.showLoading();
            this.form.getCustomers(false);

            this.getByIdSubscrip = this.purchaseOrderService.getById(params['id']).subscribe(data => {
                this.form.model = data;

                this.uploaderConfig();

                this.form.getCurrencies();

                this.history.getHistories(params['id']);

                if(this.form.model.clientExternalId && this.form.model.clientExternalId != ""){
                    this.form.getAnalytics();
                }

                setTimeout(() => {
                    $('#analytics').val(this.form.model.analyticIds).trigger('change');
                    this.form.searchOpportunities();
                }, 1000);

                if(this.form.model.status != PurchaseOrderStatus.Draft 
                    && this.form.model.status != PurchaseOrderStatus.Reject){
                    $('input').attr('disabled', 'disabled');
                    $('#customer-select select').attr('disabled', 'disabled');
                    $('#opportunity-select select').attr('disabled', 'disabled');
                    $('#analytics').attr('disabled', 'disabled');
                    $('#search-opportunity').attr('disabled', 'disabled');
                    $('input[type=file]').removeAttr('disabled');
                    $('#area-select select').attr('disabled', 'disabled');
                    $('#description').attr('disabled', 'disabled');
                    $('#comments').attr('disabled', 'disabled');
                    this.form.currencyDisabled = true;
                    this.form.isReadOnly = true;
                }
                this.messageService.closeLoading();
            },
            () => {
                this.messageService.closeLoading();
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
            if(status == 401){
                this.authService.refreshToken().subscribe(token => {
                    this.messageService.closeLoading();

                    if(token){
                        this.clearSelectedFile();
                        this.messageService.showErrorByFolder('common', 'fileMustReupload');
                        this.uploaderConfig();
                    }
                });

                return;
            }

            this.messageService.closeLoading();

            var dataJson = JSON.parse(response);
            
            if(dataJson){
                if(dataJson.messages) this.messageService.showMessages(dataJson.messages);
                
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
        this.messageService.showLoading();

        this.purchaseOrderService.exportFile(this.form.model.fileId).subscribe(file => {
            this.messageService.closeLoading();

            FileSaver.saveAs(file, this.form.model.fileName);
        },
        () => {
                this.messageService.closeLoading();
            });
    }

    deleteFile(){
        this.confirmModal.hide();
        this.messageService.showLoading();

        this.purchaseOrderService.deleteFile(this.form.model.id).subscribe(() => {
            this.form.model.fileId = null;
            this.form.model.fileName = null;
            this.messageService.closeLoading();
        },
        () => {
                this.messageService.closeLoading();
            });
    }
 
    viewFile(){
        if(this.form.model.fileName.endsWith('.pdf')){
            this.messageService.showLoading();

            this.purchaseOrderService.getFile(this.form.model.fileId).subscribe(response => {
                this.messageService.closeLoading();

                this.pdfViewer.renderFile(response.data);
            },
            () => {
                    this.messageService.closeLoading();
                });
        }
    }

    update() {
        this.messageService.showLoading();
        this.form.model.analyticIds = $('#analytics').val();

        this.updateSubscrip = this.purchaseOrderService.update(this.form.model).subscribe(
            () => {
                this.messageService.closeLoading();
                setTimeout(() => {
                    window.location.reload();
                }, 1000);
            },
            () => {
                this.messageService.closeLoading();
            });
    }

    openAdjustment(){
        var details = this.form.model.ammountDetails.map(item => {
            return { currencyId: item.currencyId, currencyDescription: item.currencyDescription, adjustment: 0, enable: true }
        })
        
        var settings = {
            id: this.form.model.id,
            details: details
        };

        this.ocAdjustment.show(settings);
    }

    back(){
        this.router.navigate(['/billing/purchaseOrders/pendings']);
    }

    goToQuery(){
        this.router.navigate(['/billing/purchaseOrders/query']);
    }

    getStatus(){
        switch(this.form.model.status){
            case 1: return this.i18nService.translateByKey("Valid");
            case 2: return this.i18nService.translateByKey("Consumed");
            case 3: return this.i18nService.translateByKey("Closed");
            case 4: return this.i18nService.translateByKey("Draft");
            case 5: return this.i18nService.translateByKey("ComercialPending");
            case 6: return this.i18nService.translateByKey("OperativePending");
            case 7: return this.i18nService.translateByKey("DafPending");
            case 8: return this.i18nService.translateByKey("Reject");
            case 9: return this.i18nService.translateByKey("CompliancePending");
        }
    }

    canDelete(){
        return this.form.model.status == PurchaseOrderStatus.Draft || this.form.model.status == PurchaseOrderStatus.Reject;
    }
}  