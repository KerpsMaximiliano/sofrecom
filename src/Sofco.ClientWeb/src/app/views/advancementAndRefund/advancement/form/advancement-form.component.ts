import { Component, OnDestroy, OnInit, Input } from "@angular/core";
import { Subscription } from "rxjs";
import { UtilsService } from "app/services/common/utils.service";
import { Advancement } from "app/models/advancement-and-refund/advancement";
import { I18nService } from "app/services/common/i18n.service";
import { UserInfoService } from "app/services/common/user-info.service";
import { WorkflowStateType } from "app/models/enums/workflowStateType";

@Component({
    selector: 'advancement-form',
    templateUrl: './advancement-form.component.html'
})
export class AdvancementFormComponent implements OnInit, OnDestroy {

    public currencies: any[] = new Array();
    public advancementReturnForms: any[] = new Array();

    public userApplicantIdLogged: number;
    public userApplicantName: string;

    @Input() mode: string;

    public status: string;

    public form: Advancement;

    getCurrenciesSubscrip: Subscription;
    getAnalyticsSubscrip: Subscription;
    getAdvancementReturnFormsSubscrip: Subscription;

    constructor(private utilsService: UtilsService, 
                public i18nService: I18nService){}

    ngOnInit(): void {
        this.getCurrencies();
        this.getAdvancementReturnForms();

        if(this.mode == 'add'){
            this.form = new Advancement(false);
            this.form.controls.ammount.setValue(0);
        }

        const userInfo = UserInfoService.getUserInfo();
    
        if(userInfo && userInfo.id && userInfo.name){
            this.userApplicantIdLogged = userInfo.id;

            if(this.mode == 'add'){
                this.form.controls.userApplicantId.setValue(userInfo.id);
                this.userApplicantName = userInfo.name;
            }
        }
    }

    ngOnDestroy(): void {
        if(this.getCurrenciesSubscrip) this.getCurrenciesSubscrip.unsubscribe();
        if(this.getAnalyticsSubscrip) this.getAnalyticsSubscrip.unsubscribe();
        if(this.getAdvancementReturnFormsSubscrip) this.getAdvancementReturnFormsSubscrip.unsubscribe();
    }

    getCurrencies(){
        this.getCurrenciesSubscrip = this.utilsService.getCurrencies().subscribe(response => {
            this.currencies = response;
        });
    }

    getAdvancementReturnForms(){
        this.getAdvancementReturnFormsSubscrip = this.utilsService.getAdvancementReturnForms().subscribe(response => {
            this.advancementReturnForms = response;
        });
    }

    getClassProperty(property){
        if(this.form.controls[property].invalid && (this.form.controls[property].dirty || this.form.controls[property].touched)) return 'has-error';
        if(this.form.controls[property].valid && (this.form.controls[property].dirty || this.form.controls[property].touched)) return 'has-success';
    }

    hasErrors(property){
        if(this.form.controls[property].invalid && (this.form.controls[property].dirty || this.form.controls[property].touched)){
            return this.form.controls[property].errors;
        }

        return false;
    }

    hasError(property, validation){
        return this.form.controls[property].errors[validation];
    }

    canSave(){
        if(this.form.valid) return true;
        
        return false;
    }

    setModel(domain, isEdit){
        if(domain.statusId > 0){
            this.status = domain.statusDesc;
        }

        this.form = new Advancement(!isEdit, domain);

        if(this.mode == 'detail'){
            this.userApplicantName = domain.userApplicantDesc;
        }
    }

    getModel(){
        var advancement = this.form.getModel();
        
        return advancement;
    }

    getStatusClass(){
        switch(this.form.workflowStateType){
            case WorkflowStateType.Info: return "label-success";
            case WorkflowStateType.Warning: return "label-warning";
            case WorkflowStateType.Success: return "label-primary";
            case WorkflowStateType.Danger: return "label-danger";
        }
    }
}