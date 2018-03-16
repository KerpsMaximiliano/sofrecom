import { Component, OnDestroy, ViewChild, OnInit } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { MessageService } from "app/services/common/message.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs/Subscription";
import { FileUploader } from "ng2-file-upload";
import { LicenseService } from "app/services/human-resources/licenses.service";
import { MenuService } from "app/services/admin/menu.service";
import { License } from "../../../../models/rrhh/license";
import { Cookie } from "ng2-cookies/ng2-cookies";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { LicenseDetail } from "app/models/rrhh/licenseDetail";
import { Option } from "../../../../models/option";

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
                private router: Router,
                private menuService: MenuService,
                private activatedRoute: ActivatedRoute,
                private messageService: MessageService,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {

            this.messageService.showLoading();

            this.getSubscrip = this.licenseService.getById(params['id']).subscribe(response => {
                this.model = response.data;

                this.configUploader();

                this.history.getHistories(params['id']);
            },
            error => this.errorHandlerService.handleErrors(error),
            () => this.messageService.closeLoading());
        });
    }

    ngOnDestroy(): void {
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.deleteFileSubscrip) this.deleteFileSubscrip.unsubscribe();
    }

    back(){
        if(this.menuService.userIsRrhh){
            this.router.navigate(['/allocationManagement/licenses/rrhh']);
        }
        else{
            this.router.navigate(['/profile/' + this.model.employeeId]);
        }
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

        this.uploader.onSuccessItem = (item: any, response: any, status: any, headers: any) => {
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
        this.deleteFileSubscrip = this.licenseService.deleteFile(this.fileIdToDelete).subscribe(response => {
            if(response.messages) this.messageService.showMessages(response.messages);
            this.model.files.splice(this.indexToDelete, 1);
          },
          err => this.errorHandlerService.handleErrors(err),
         () => this.confirmModal.hide());
    }

    
    updateStatus(event){
        if(event.statusId) this.model.status = event.statusId;
        if(event.statusName) this.model.statusName = event.statusName;
    }
} 