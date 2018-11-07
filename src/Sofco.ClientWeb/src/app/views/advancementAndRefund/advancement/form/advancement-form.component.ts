import { Component, OnDestroy, OnInit, Input, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { UtilsService } from "app/services/common/utils.service";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { UserService } from "app/services/admin/user.service";
import { Advancement, AdvancementDetail } from "app/models/advancement-and-refund/advancement";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";

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

    public users: any[] = new Array();
    public currencies: any[] = new Array();
    public analytics: any[] = new Array();
    public advancementReturnForms: any[] = new Array();

    @Input() mode: string;

    public form: Advancement;
    public detailForms: AdvancementDetail[];
    public detailModalForm: AdvancementDetail;

    getUsersSubscrip: Subscription;
    getCurrenciesSubscrip: Subscription;
    getAnalyticsSubscrip: Subscription;
    getAdvancementReturnFormsSubscrip: Subscription;

    constructor(private utilsService: UtilsService, 
                private analyticService: AnalyticService,
                private userService: UserService){}

    ngOnInit(): void {
        this.form = new Advancement();
        this.detailForms = new Array();
        this.detailModalForm = new AdvancementDetail();

        if(this.mode == 'salary'){
            var item = new AdvancementDetail();
            item.controls.date.setValue(new Date());
            item.controls.description.setValue('Adelanto Sueldo');
            item.controls.ammount.setValue(0);
            this.detailForms.push(item);
        }

        this.getUsers();
        this.getCurrencies();
        this.getAnalytics();
        this.getAdvancementReturnForms();
    }

    ngOnDestroy(): void {
        if(this.getUsersSubscrip) this.getUsersSubscrip.unsubscribe();
        if(this.getCurrenciesSubscrip) this.getCurrenciesSubscrip.unsubscribe();
        if(this.getAnalyticsSubscrip) this.getAnalyticsSubscrip.unsubscribe();
        if(this.getAdvancementReturnFormsSubscrip) this.getAdvancementReturnFormsSubscrip.unsubscribe();
    }

    getUsers(){
        this.getUsersSubscrip = this.userService.getOptions().subscribe(response => {
            this.users = response;
        });
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

    validateProperty(form, property){
        if(form.controls[property].invalid && (form.controls[property].dirty || form.controls[property].touched)) return 'has-error';
        if(form.controls[property].valid && (form.controls[property].dirty || form.controls[property].touched)) return 'has-success';
    }

    canSave(){
        if(this.form.valid && this.detailForms.every(x => x.valid)) return true;
        
        return false;
    }
}