import { Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { ActivatedRoute, Router } from "@angular/router";
import { environment } from 'environments/environment'
import { MenuService } from "app/services/admin/menu.service";
import { UserInfoService } from "app/services/common/user-info.service";
import { RefundService } from "app/services/advancement-and-refund/refund.service";
import { FileUploader } from "ng2-file-upload";
import { Cookie } from "ng2-cookies/ng2-cookies";
import { AuthService } from "app/services/common/auth.service";
import * as FileSaver from "file-saver";


@Component({
    selector: 'refund-refund',
    templateUrl: './refund-detail.component.html'
})
export class RefundDetailComponent implements OnInit, OnDestroy {
  
    @ViewChild('form') form;
    @ViewChild('selectedFile') selectedFile: any;
    @ViewChild('workflow') workflow;
    @ViewChild('history') history;
    @ViewChild('refundRelated') refundRelated;
 
    public entityId: number;
    public actualStateId: number;
    public userApplicantId: number;
   

    getSubscrip: Subscription;
    editSubscrip: Subscription;

    public uploader: FileUploader = new FileUploader({url:""});

    public files: any[] = new Array();

    private workflowModel: any;
    inWorkflowProcess: boolean;

    constructor(private refundService: RefundService,
                private activateRoute: ActivatedRoute,
                private menuService: MenuService,
                private authService: AuthService,
                private router: Router,
                private messageService: MessageService){}

    ngOnInit(): void {
        this.activateRoute.params.subscribe(routeParams => {
            this.getData(routeParams.id);
        });
    }
                
    ngOnDestroy(): void {
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
        if(this.editSubscrip) this.editSubscrip.unsubscribe();
    }

    canDownload(){
        return this.menuService.userIsGaf && !this.inWorkflowProcess;
    }

    getData(id){
        this.messageService.showLoading();

        this.getSubscrip = this.refundService.get(id).subscribe(response => {
            this.messageService.closeLoading();

            this.actualStateId = response.data.statusId;
            this.entityId = response.data.id;
            this.userApplicantId = response.data.userApplicantId;
            this.form.userOffice = response.data.office;
            this.form.userBank = response.data.bank;
            this.inWorkflowProcess = response.data.inWorkflowProcess;

            this.form.setModel(response.data, this.canUpdate());

            if(response.data && response.data.files && response.data.files.length > 0){
                this.files = response.data.files;
            }

            this.uploaderConfig();

            this.workflowModel = {
                workflowId: response.data.workflowId,
                entityController: "refund",
                entityId: response.data.id,
                actualStateId: response.data.statusId
            }

            if(response.data.advancementIds && response.data.advancementIds.length > 0){
                this.refundRelated.init(response.data.advancementIds);
            }
            else{
                this.refundRelated.advancements = [];
                this.refundRelated.refunds = [];
            }

            this.workflow.init(this.workflowModel);

            this.history.getHistories(id);
        }, 
        error => this.messageService.closeLoading());
    }

    canUpdate(){
        if(environment.gafWorkflowStateId == this.actualStateId && this.menuService.userIsGaf) {
            this.form.canGafUpdate = true;
        }

        if(environment.draftWorkflowStateId == this.actualStateId || environment.rejectedWorkflowStateId == this.actualStateId){
            const userInfo = UserInfoService.getUserInfo();
    
            if(userInfo && userInfo.id && userInfo.name){

                if(userInfo.id == this.userApplicantId){
                    return true;
                }
            }
        }

        return false;
    }

    update(){
        var model = this.form.getModel();
        model.hasCreditCard = this.form.hasCreditCard;
        
        this.messageService.showLoading();

        this.editSubscrip = this.refundService.edit(model).subscribe(response => {
            this.messageService.closeLoading();
            this.workflow.init(this.workflowModel);
            this.refundRelated.init(this.form.form.controls.advancements.value);
        },
        error => {
            this.messageService.closeLoading();
        });
    }
    
    onTransitionSuccess(){
        
        if(this.workflowModel.actualStateId == 10)
         {
             this.downloadZip()
         }
        const userInfo = UserInfoService.getUserInfo();
        if(userInfo.id == this.form.form.controls.userApplicantId.value){
            if(userInfo && userInfo.employeeId){
                this.router.navigate(['/profile/' + userInfo.employeeId]);
            }
        }
        else{
            if(this.canBack()){
                this.back();
            }
        }
    }

    back(){
        this.router.navigate(['/advancementAndRefund/refund/search']);
    }

    canBack(){
        return this.menuService.hasFunctionality('ADVAN', 'REFUND-LIST-VIEW');
    }

    goToProfile(){
        const userInfo = UserInfoService.getUserInfo();
        this.router.navigate(['/profile/' + userInfo.employeeId]);
    }

    uploaderConfig(){
        this.uploader = new FileUploader({url: this.refundService.getUrlForImportExcel(this.entityId),
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
                
                this.files.push({ id: dataJson.data.id, text: dataJson.data.fileName });
            }

            this.clearSelectedFile();
        };

        this.uploader.onAfterAddingFile = (file)=> { file.withCredentials = false; };
    }

    clearSelectedFile(){
        if(this.uploader.queue.length > 0){
            this.uploader.queue[0].remove();
        }
  
        this.selectedFile.nativeElement.value = '';
    }

    removeFile(file, index){
        this.messageService.showConfirm(() => {
            this.messageService.showLoading();

            this.editSubscrip = this.refundService.deleteFile(this.entityId, file.id).subscribe(
                response => {
                    this.messageService.closeLoading();
                    this.files.splice(index, 1);
                },
                error => {
                    this.messageService.closeLoading();
                });

        });
    }

    delete(){
        this.messageService.showLoading();

        this.editSubscrip = this.refundService.delete(this.entityId).subscribe(
            response => {
                this.messageService.closeLoading();
                this.goToProfile();
            },
            error => {
                this.messageService.closeLoading();
            });
    }

    canDelete(){
        const userInfo = UserInfoService.getUserInfo();
    
        if(userInfo && userInfo.id && userInfo.name){
            if(environment.draftWorkflowStateId == this.actualStateId){
                if(userInfo.id == this.userApplicantId){
                    return true;
                }
            }
        }

        return false;
    }

    export(file){
        this.messageService.showLoading();

        this.refundService.exportFile(file.id).subscribe(response => {
            this.messageService.closeLoading();

            FileSaver.saveAs(response, file.text);
        },
        () => {
            this.messageService.closeLoading();
        });
    }

    downloadZip(){
        this.messageService.showLoading();
 
        this.refundService.downloadZip(this.entityId).subscribe(file => {
            this.messageService.closeLoading();
            FileSaver.saveAs(file, `reint-${this.entityId}.zip`);
        },
        error => {
            this.messageService.showMessage("Archivos no encontrados", 1);
            this.messageService.closeLoading();
        });
    }
}