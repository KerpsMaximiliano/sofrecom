import { Component, OnDestroy, OnInit, Input, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { UtilsService } from "app/services/common/utils.service";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { UserService } from "app/services/admin/user.service";
import { Advancement, AdvancementDetail } from "app/models/advancement-and-refund/advancement";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { I18nService } from "app/services/common/i18n.service";
import { UserInfoService } from "app/services/common/user-info.service";

@Component({
    selector: 'advancement-form',
    templateUrl: './advancement-form.component.html'
})
export class AdvancementFormComponent implements OnInit, OnDestroy {

    @ViewChild('addDetailModal') addDetailModal;
    public addDetailModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "addDetailModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    ); 

    public currencies: any[] = new Array();
    public analytics: any[] = new Array();
    public advancementReturnForms: any[] = new Array();

    public userApplicantName: string;

    @Input() mode: string;

    public form: Advancement;
    public detailForms: AdvancementDetail[];
    public detailModalForm: AdvancementDetail;

    getCurrenciesSubscrip: Subscription;
    getAnalyticsSubscrip: Subscription;
    getAdvancementReturnFormsSubscrip: Subscription;

    constructor(private utilsService: UtilsService, 
                private analyticService: AnalyticService,
                public i18nService: I18nService,
                private userService: UserService){}

    ngOnInit(): void {
        this.form = new Advancement();
        this.detailForms = new Array();
        this.detailModalForm = new AdvancementDetail();

        const userInfo = UserInfoService.getUserInfo();
    
        if(userInfo && userInfo.id && userInfo.name){
            this.form.controls.userApplicantId.setValue(userInfo.id);
            this.userApplicantName = userInfo.name;
        }

        if(this.mode == 'salary'){
            var item = new AdvancementDetail();
            item.controls.date.setValue(new Date());
            item.controls.description.setValue('Adelanto Sueldo');
            item.controls.ammount.setValue(0);
            this.detailForms.push(item);
        }

        this.getCurrencies();
        this.getAnalytics();
        this.getAdvancementReturnForms();
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

    getAnalytics(){
        this.getAnalyticsSubscrip = this.analyticService.getByCurrentUser().subscribe(response => {
            this.analytics = response.data;
        });
    }

    getAdvancementReturnForms(){
        this.getAdvancementReturnFormsSubscrip = this.utilsService.getAdvancementReturnForms().subscribe(response => {
            this.advancementReturnForms = response;
        });
    }

    editDetail(detail){
        this.detailModalForm = detail;
        this.addDetailModal.show();
    }

    saveDetail(){
        this.addDetailModal.hide();
    }

    getClassProperty(form, property){
        if(form.controls[property].invalid && (form.controls[property].dirty || form.controls[property].touched)) return 'has-error';
        if(form.controls[property].valid && (form.controls[property].dirty || form.controls[property].touched)) return 'has-success';
    }

    hasErrors(form, property){
        if(form.controls[property].invalid && (form.controls[property].dirty || form.controls[property].touched)){
            return form.controls[property].errors;
        }

        return false;
    }

    hasError(form, property, validation){
        return form.controls[property].errors[validation];
    }

    canSave(){
        if(this.form.valid && this.detailForms.every(x => x.valid)) return true;
        
        return false;
    }

    getModel(){
        var advancement = this.form.getModel();
        
        this.detailForms.forEach(x => {
            advancement.details.push(x.getModel());
        });

        return advancement;
    }
}