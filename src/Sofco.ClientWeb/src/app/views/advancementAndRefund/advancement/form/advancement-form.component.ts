import { Component, OnDestroy, OnInit, Input } from "@angular/core";
import { Subscription } from "rxjs";
import { UtilsService } from "app/services/common/utils.service";
import { I18nService } from "app/services/common/i18n.service";
import { UserInfoService } from "app/services/common/user-info.service";
import { WorkflowStateType } from "app/models/enums/workflowStateType";
import { environment } from 'environments/environment'
import { Advancement } from "app/models/advancement-and-refund/advancement";
import { FormsService } from "app/services/forms/forms.service";
import { AdvancementService } from "app/services/advancement-and-refund/advancement.service";

@Component({
    selector: 'advancement-form',
    templateUrl: './advancement-form.component.html',
    styleUrls: ['./advancement-form.component.scss']
})
export class AdvancementFormComponent implements OnInit, OnDestroy {

    public currencies: any[] = new Array();
    public currenciesFiltered: any[] = new Array();
    public monthsReturns: any[] = new Array();

    userApplicantIdLogged: number;
    userApplicantName: string;
    userBank: string;
    userOffice: string;

    @Input() mode: string;

    public statusId: number;
    public status: string;
    public isSalary: boolean = true;
    public isLoading: boolean = true;
    public isCashEnable: boolean = false;

    public form: Advancement;

    private id: number;
    public workflowStateType: WorkflowStateType;

    getCurrenciesSubscrip: Subscription;
    getAnalyticsSubscrip: Subscription;
    getMonthsReturnSubscrip: Subscription;
    getCashEnableSubscrip: Subscription;

    constructor(private utilsService: UtilsService,
                public formsService: FormsService,
                private advancementService: AdvancementService,
                public i18nService: I18nService){}

    ngOnInit(): void {
        this.getMonthsReturn();

        if(this.mode == 'add'){
            this.getCashEnable();

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
        if(this.getCashEnableSubscrip) this.getCashEnableSubscrip.unsubscribe();
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

    getCashEnable(){
        this.getCashEnableSubscrip = this.advancementService.getCashEnable().subscribe(response => {
           this.isCashEnable = true;
        });
    }

    canSave(){
        return this.formsService.canSave(this.form);
    }

    setModel(domain, isEdit){
        if(domain.statusId > 0){
            this.status = domain.statusDesc;
            this.statusId = domain.statusId;

            if(this.statusId == environment.draftWorkflowStateId || this.statusId == environment.rejectedWorkflowStateId){
                this.getCashEnable();
            }
            else{
                this.isCashEnable = true;
            }
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
            this.userBank = userInfo.bank;
            this.userOffice = userInfo.office;
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
                    if(selectedValue == '2'){
                        this.currenciesFiltered = this.currencies.filter(x => x.id != environment.currencyPesosId);
                    }

                    if(selectedValue == '3'){
                        this.currenciesFiltered = this.currencies;
                        this.form.controls.currencyId.setValue(environment.currencyPesosId);
                    }
               
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
            
            if(this.form.controls.paymentForm.value == '1'){
                this.currenciesFiltered = this.currencies.filter(x => x.id == environment.currencyPesosId);
            }

            if(this.form.controls.paymentForm.value == '3'){
                this.currenciesFiltered = this.currencies;
            }
        }
    }
}