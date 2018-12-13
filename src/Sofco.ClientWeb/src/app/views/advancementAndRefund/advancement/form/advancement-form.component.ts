import { Component, OnDestroy, OnInit, Input } from "@angular/core";
import { Subscription } from "rxjs";
import { UtilsService } from "app/services/common/utils.service";
import { I18nService } from "app/services/common/i18n.service";
import { UserInfoService } from "app/services/common/user-info.service";
import { WorkflowStateType } from "app/models/enums/workflowStateType";
import { environment } from 'environments/environment'
import { Advancement } from "app/models/advancement-and-refund/advancement";

@Component({
    selector: 'advancement-form',
    templateUrl: './advancement-form.component.html'
})
export class AdvancementFormComponent implements OnInit, OnDestroy {

    public currencies: any[] = new Array();
    public currenciesFiltered: any[] = new Array();
    public monthsReturns: any[] = new Array();

    public userApplicantIdLogged: number;
    public userApplicantName: string;

    @Input() mode: string;

    public status: string;
    public isSalary: boolean = true;
    public isLoading: boolean = true;

    public form: Advancement;

    private id: number;
    public workflowStateType: WorkflowStateType;

    getCurrenciesSubscrip: Subscription;
    getAnalyticsSubscrip: Subscription;
    getMonthsReturnSubscrip: Subscription;

    constructor(private utilsService: UtilsService,
                public i18nService: I18nService){}

    ngOnInit(): void {
        this.getMonthsReturn();
     
        if(this.mode == 'add'){
            this.isLoading = false;
            this.form = new Advancement(false);

            this.form.controls.paymentForm.disable();

            this.getCurrencies();
            this.form.handleSalaryForm();
            this.formConfiguration();
            this.setUserApplicant();
        }
    }

    ngOnDestroy(): void {
        if(this.getCurrenciesSubscrip) this.getCurrenciesSubscrip.unsubscribe();
        if(this.getAnalyticsSubscrip) this.getAnalyticsSubscrip.unsubscribe();
        if(this.getMonthsReturnSubscrip) this.getMonthsReturnSubscrip.unsubscribe();
    }

    getCurrencies(){
        this.getCurrenciesSubscrip = this.utilsService.getCurrencies().subscribe(response => {
            this.currencies = response;

            if(this.form){
                this.filterCurrency();
            }
        });
    }

    getMonthsReturn(){
        this.getMonthsReturnSubscrip = this.utilsService.getMonthsReturn().subscribe(response => {
            this.monthsReturns = response;
        });
    }

    getClassProperty(property){
        if(!this.form || !this.form.controls[property]) return;

        if(this.form.controls[property].invalid && (this.form.controls[property].dirty || this.form.controls[property].touched)) return 'has-error';
        if(this.form.controls[property].valid && (this.form.controls[property].dirty || this.form.controls[property].touched)) return 'has-success';
    }

    hasErrors(property){
        if(!this.form || !this.form.controls[property]) return;

        if(this.form.controls[property].invalid && (this.form.controls[property].dirty || this.form.controls[property].touched)){
            return this.form.controls[property].errors;
        }

        return false;
    }

    hasError(property, validation){
        if(!this.form || !this.form.controls[property]) return;

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

        this.id = domain.id || 0;
        this.workflowStateType = domain.workflowStateType;

        this.form = new Advancement(!isEdit, domain);

        this.getCurrencies();
        this.formConfiguration();
        this.form.controls.type.disable();

        if(domain.type == '1'){
            this.isSalary = true;
        }
        else{
            this.isSalary = false;
        }

        this.userApplicantName = domain.userApplicantDesc;
    }

    getModel(){
        var advancement = this.form.getModel();
        
        return advancement;
    }

    getStatusClass(){
        switch(this.workflowStateType){
            case WorkflowStateType.Info: return "label-success";
            case WorkflowStateType.Warning: return "label-warning";
            case WorkflowStateType.Success: return "label-primary";
            case WorkflowStateType.Danger: return "label-danger";
        }
    }

    setUserApplicant(){
        const userInfo = UserInfoService.getUserInfo();
    
        if(userInfo && userInfo.id && userInfo.name){
            this.userApplicantIdLogged = userInfo.id;

            this.form.controls.userApplicantId.setValue(userInfo.id);
            this.userApplicantName = userInfo.name;
        }
    }

    formConfiguration(){
        this.form.controls['type'].valueChanges.subscribe(
            (selectedValue) => {
                if(selectedValue == '1'){
                    this.isSalary = true;
                    this.form.handleSalaryForm();
                }
                else{
                    this.isSalary = false;
                    this.form.handleViaticumForm();
                }

                this.filterCurrency();
            }
        );

        this.form.controls['paymentForm'].valueChanges.subscribe(
            (selectedValue) => {
                if(selectedValue == '1'){
                    this.currenciesFiltered = this.currencies;
                    this.form.controls.currencyId.setValue(environment.currencyPesosId);
                }
                else{
                    this.currenciesFiltered = this.currencies.filter(x => x.id != environment.currencyPesosId);

                    if(!this.isLoading){
                        this.form.controls.currencyId.setValue(null);
                    }

                    this.isLoading = false;
                }
            }
        );
    }

    filterCurrency(){
        if(this.isSalary){
            this.currenciesFiltered = this.currencies.filter(x => x.id == environment.currencyPesosId);
            this.form.controls.currencyId.setValue(environment.currencyPesosId);
        }
        else{
            if(this.form.controls.paymentForm.value == '2'){
                this.currenciesFiltered = this.currencies.filter(x => x.id != environment.currencyPesosId);
            }
            else{
                this.currenciesFiltered = this.currencies.filter(x => x.id == environment.currencyPesosId);
            }
        }
    }
}