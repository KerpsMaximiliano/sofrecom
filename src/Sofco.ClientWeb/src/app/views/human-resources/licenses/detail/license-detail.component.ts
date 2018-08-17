import { Component, OnDestroy, ViewChild, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { MessageService } from "../../../../services/common/message.service";
import { Subscription } from "rxjs";
import { FileUploader } from "ng2-file-upload";
import { LicenseService } from "../../../../services/human-resources/licenses.service";
import { MenuService } from "../../../../services/admin/menu.service";
import { Cookie } from "ng2-cookies/ng2-cookies";
import { Ng2ModalConfig } from "../../../../components/modal/ng2modal-config";
import { LicenseDetail } from "../../../../models/rrhh/licenseDetail";
import * as FileSaver from "file-saver";

@Component({
    selector: 'license-detail',
    templateUrl: './license-detail.component.html',
    styleUrls: ['./license-detail.component.scss']
})
export class LicenseDetailComponent implements OnInit, OnDestroy {

    @ViewChild('selectedFile') selectedFile: any;
    @ViewChild('history') history: any;

    getSubscrip: Subscription;
    paramsSubscrip: Subscription;
    deleteFileSubscrip: Subscription;
    fileDeliveredSubscrip: Subscription;

    public uploader: FileUploader = new FileUploader({url:""});
    public showUploader: boolean = false;
    public fileIdToDelete: number;
    public indexToDelete: number;

    public model: LicenseDetail = new LicenseDetail();

    @ViewChild('confirmDeleteFileModal') confirmModal;
    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "confirmDeleteFileModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );
 
    constructor(private licenseService: LicenseService,
                public menuService: MenuService,
                private activatedRoute: ActivatedRoute,
                private messageService: MessageService){
    }

    ngOnInit(): void {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {

            this.messageService.showLoading();

            this.getSubscrip = this.licenseService.getById(params['id']).subscribe(response => {
                this.model = response.data;

                this.configUploader();

                this.history.getHistories(params['id']);
            },
            () => { },
            () => this.messageService.closeLoading());
        });
    }

    ngOnDestroy(): void {
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.deleteFileSubscrip) this.deleteFileSubscrip.unsubscribe();
        if(this.fileDeliveredSubscrip) this.fileDeliveredSubscrip.unsubscribe();
    }

    configUploader(){
        this.uploader = new FileUploader({url: this.licenseService.getUrlForImportFile(this.model.id), authToken: `Bearer ${Cookie.get('access_token')}`, maxFileSize: 10*1024*1024 });

        this.uploader.onCompleteItem = (item:any, response:any, status:any, headers:any) => {
            var json = JSON.parse(response);

            if(json.messages) this.messageService.showMessages(json.messages);
            
            var file = json.data;
            var option = { id: file.id, text: file.fileName };
            this.model.files.push(option);
        };

        this.uploader.onSuccessItem = (item: any) => {
            item.remove();
            this.selectedFile.nativeElement.value = '';
        };
    }

    openConfirmModal(fileId, index){
        this.fileIdToDelete = fileId;
        this.indexToDelete = index;
        this.confirmModal.show();
    }

    deleteFile(){
        this.deleteFileSubscrip = this.licenseService.deleteFile(this.fileIdToDelete).subscribe(() => {
            this.model.files.splice(this.indexToDelete, 1);
        },
          () => { },
         () => this.confirmModal.hide());
    }

    
    updateStatus(event){
        if(event.statusId) this.model.status = event.statusId;
        if(event.statusName) this.model.statusName = event.statusName;
    }

    fileDelivered(){
        this.messageService.showLoading();

        this.fileDeliveredSubscrip = this.licenseService.fileDelivered(this.model.id).subscribe(() => {
            this.model.hasCertificate = true;
        },
          () => { },
          () => this.messageService.closeLoading());
    }

    exportExcel(id, name){
        this.licenseService.exportFile(id).subscribe(file => {
            FileSaver.saveAs(file, name);
        });
    }
} 