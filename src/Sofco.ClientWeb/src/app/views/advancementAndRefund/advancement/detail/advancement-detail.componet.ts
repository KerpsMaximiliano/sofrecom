import { Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { AdvancementService } from "app/services/advancement-and-refund/advancement.service";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { ActivatedRoute, Router } from "@angular/router";
import { environment } from 'environments/environment'
import { MenuService } from "app/services/admin/menu.service";
import { UserInfoService } from "app/services/common/user-info.service";

@Component({
    selector: 'advancement-detail',
    templateUrl: './advancement-detail.component.html'
})
export class AdvancementDetailComponent implements OnInit, OnDestroy {
  
    @ViewChild('form') form;
    @ViewChild('workflow') workflow;
    @ViewChild('history') history;
    @ViewChild('refundRelated') refundRelated;

    public entityId: number;
    public actualStateId: number;
    public userApplicantId: number;

    getSubscrip: Subscription;
    editSubscrip: Subscription;

    constructor(private advancementService: AdvancementService,
                private activateRoute: ActivatedRoute,
                private menuService: MenuService,
                private router: Router,
                private messageService: MessageService){}

    ngOnInit(): void {
        this.activateRoute.params.subscribe(routeParams => {
            this.entityId = routeParams.id;
            this.getData(routeParams.id);
        });
    }
                
    ngOnDestroy(): void {
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
        if(this.editSubscrip) this.editSubscrip.unsubscribe();
    }

    getData(id){
        this.refundRelated.init([id]);

        this.messageService.showLoading();

        this.getSubscrip = this.advancementService.get(id).subscribe(response => {
            this.messageService.closeLoading();

            var model = {
                workflowId: response.data.workflowId,
                entityController: "advancement",
                entityId: response.data.id,
                actualStateId: response.data.statusId
            }

            this.actualStateId = response.data.statusId;
            this.userApplicantId = response.data.userApplicantId;

            this.form.setModel(response.data, this.canUpdate());

            this.workflow.init(model);

            this.history.getHistories(id);
        }, 
        error => this.messageService.closeLoading());
    }

    back(){
        window.history.back();
    }

    canBack(){
        return this.menuService.hasFunctionality('ADVAN', 'QUERY');
    }
    
    canUpdate(){
        const userInfo = UserInfoService.getUserInfo();
    
        if(userInfo && userInfo.id && userInfo.name){
            if(environment.draftWorkflowStateId == this.actualStateId || environment.rejectedWorkflowStateId == this.actualStateId){
                if(userInfo.id == this.userApplicantId){
                    return true;
                }
            }
        }

        return false;
    }

    update(){
        var model = this.form.getModel();

        this.messageService.showLoading();

        this.editSubscrip = this.advancementService.edit(model).subscribe(response => {
            this.messageService.closeLoading();
        },
        error => {
            this.messageService.closeLoading();
        });
    }

    onTransitionSuccess(){
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

    goToProfile(){
        const userInfo = UserInfoService.getUserInfo();
        this.router.navigate(['/profile/' + userInfo.employeeId]);
    }

    delete(){
        this.messageService.showLoading();

        this.editSubscrip = this.advancementService.delete(this.entityId).subscribe(
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
}