import { Component, OnInit, OnDestroy, Input, ViewChild } from "@angular/core";
import { FormsService } from "app/services/forms/forms.service";
import { I18nService } from "app/services/common/i18n.service";
import { Refund } from "app/models/advancement-and-refund/refund";
import { WorkflowStateType } from "app/models/enums/workflowStateType";
import { Subscription } from "rxjs";
import { AdvancementService } from "app/services/advancement-and-refund/advancement.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { RefundDetail } from "app/models/advancement-and-refund/refund-detail";
import { UserInfoService } from "app/services/common/user-info.service";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { MessageService } from "app/services/common/message.service";
import { Validators } from "@angular/forms";
import { UtilsService } from "app/services/common/utils.service";
import { MenuService } from "app/services/admin/menu.service";

@Component({
    selector: 'refund-form',
    templateUrl: './refund-form.component.html',
    styleUrls: ['./refund-form.component.scss']
})
export class RefundFormComponent implements OnInit, OnDestroy {
    private defaultCurrencyId = 1;
    private defaultCurrencyDescription = "Pesos ($)";

    public advancements: any[] = new Array();
    public analytics: any[] = new Array();
    public creditCards: any[] = new Array();

    public userApplicantIdLogged: number;
    public userApplicantName: string;
    public status: string;
    public currencyDescription: string = this.defaultCurrencyDescription;

    public advancementSum = 0;
    public itemTotal = 0;
    public companyRefund = 0;
    public userRefund = 0;

    @ViewChild('addDetailModal') addDetailModal;
    public addDetailModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "addDetailModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    @Input() mode: string;

    public form: Refund;
    public detailForms: RefundDetail[];
    public detailModalForm: RefundDetail;

    private detailFormAux = {
        creationDate: null,
        description: '',
        ammount: null
    };

    private indexAux: number;

    public id: number;
    public workflowStateType: WorkflowStateType;

    public differentCurrenciesWereSelected = false;
    public hasCreditCard = false;
    public userHasCreditCard = false;
    public canUpdate = false;
    public isNewDetail = true;

    getAdvancementsSubscrip: Subscription;
    getAnalyticsSubscrip: Subscription;
    getCreditCardsSubscrip: Subscription;

    constructor(public formsService: FormsService,
        public advancementService: AdvancementService,
        private utilsService: UtilsService,
        private menuService: MenuService,
        public messageService: MessageService,
        public analyticService: AnalyticService,
        public i18nService: I18nService){}

    ngOnInit(): void {
        if(this.mode == 'add'){
            this.form = new Refund(false);
            this.setUserApplicant();
            this.canUpdate = true;
            this.hasCreditCardChanged(false);
            this.formConfiguration();
        }

        this.detailForms = new Array();
        this.detailModalForm = new RefundDetail();

        this.getAdvancementsUnrelated();
        this.getAnalytics();
        this.getCreditCards();
    }

    ngOnDestroy(): void {
        if(this.getAdvancementsSubscrip) this.getAdvancementsSubscrip.unsubscribe();
        if(this.getAnalyticsSubscrip) this.getAnalyticsSubscrip.unsubscribe();
        if(this.getCreditCardsSubscrip) this.getCreditCardsSubscrip.unsubscribe();
    }

    getAnalytics() {
        this.getAnalyticsSubscrip = this.analyticService.getOptions().subscribe(res => {
            this.analytics = res;
        });
    }

    getCreditCards() {
        this.getCreditCardsSubscrip = this.utilsService.getCreditCards().subscribe(res => {
            this.creditCards = res;
        });
    }

    getAdvancementsUnrelated(){
        this.getAdvancementsSubscrip = this.advancementService.getUnrelated().subscribe(response => {
            this.advancements = response.data;
        });
    }

    addDetail(){
        this.isNewDetail = true;

        this.addDetailModal.show();
    }

    editDetail(detail, index){
        this.isNewDetail = false;
        this.detailFormAux.creationDate = detail.controls.creationDate.value;
        this.detailFormAux.description = detail.controls.description.value;
        this.detailFormAux.ammount = detail.controls.ammount.value;
        this.indexAux = index;

        this.detailModalForm = detail;
        this.addDetailModal.show();
    }

    saveDetail(){
        if(this.isNewDetail){
            const detail = this.detailModalForm;
            this.detailForms.push(detail);
        }

        this.detailModalForm = new RefundDetail();
        this.addDetailModal.hide();
        this.calculateTotals();
    }

    removeDetail(index){
        this.messageService.showConfirm(() => {
            this.detailForms.splice(index, 1);
            this.calculateTotals();
        });
    }

    onClose(){
        if(!this.isNewDetail){
            const detail = this.detailForms[this.indexAux];

            detail.controls.creationDate.setValue(this.detailFormAux.creationDate);
            detail.controls.description.setValue(this.detailFormAux.description);
            detail.controls.ammount.setValue(this.detailFormAux.ammount);
        }

        this.detailModalForm = new RefundDetail();
    }

    canSave(){
        if(this.form.valid
            && this.detailForms.length > 0
            && this.detailForms.every(x => x.valid)
            && !this.differentCurrenciesWereSelected) return true;

        return false;
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

        this.userHasCreditCard = userInfo.hasCreditCard;

        if(userInfo && userInfo.id && userInfo.name){
            this.userApplicantIdLogged = userInfo.id;

            this.form.controls.userApplicantId.setValue(userInfo.id);
            this.userApplicantName = userInfo.name;
        }
    }

    setModel(domain, isEdit){
        if(domain.statusId > 0){
            this.status = domain.statusDesc;
        }

        this.id = domain.id || 0;
        this.workflowStateType = domain.workflowStateType;
        this.canUpdate = isEdit;

        this.form = new Refund(!isEdit, domain);

        this.currencyDescription = domain.currencyDesc;

        if(domain.details && domain.details.length > 0){
            domain.details.forEach(detail => {
                this.detailForms.push(new RefundDetail(detail));
            });
        }

        if(domain.advancements && domain.advancements.length > 0){
            domain.advancements.forEach(advancement => {
                this.advancements.push(advancement);
            });
        }

        this.userApplicantName = domain.userApplicantDesc;
        this.calculateTotals();
    }

    getModel(){
        const advancement = this.form.getModel();

        this.detailForms.forEach(element => {
            advancement.details.push(element.getModel());
        });

        if(advancement.advancements == null){
            advancement.currencyId = this.defaultCurrencyId;
        }

        return advancement;
    }

    calculateTotals(){
        this.advancementSum = 0;
        this.itemTotal = 0;
        this.companyRefund = 0;
        this.userRefund = 0;

        if(this.differentCurrenciesWereSelected) return;

        if(this.form.controls.advancements.value != null
            && this.form.controls.advancements.value.length > 0){
            this.form.controls.advancements.value.forEach(element => {
                const advancement = this.advancements.find(x => x.id == element);

                if(advancement.ammount > 0){
                    this.advancementSum += advancement.ammount;
                }
            });
        }

        if(this.detailForms.length > 0){
            this.detailForms.forEach(element => {
                if(element.controls.ammount.value > 0){
                    this.itemTotal += element.controls.ammount.value;
                }
            });
        }

        const diffTotal = this.advancementSum - this.itemTotal;
        this.companyRefund = this.hasAdvancements() && diffTotal > 0
            ? Math.abs(diffTotal)
            : 0;
        this.userRefund = diffTotal < 0
            ? Math.abs(diffTotal) 
            : 0;
    }

    advancementsChanged(){
        this.differentCurrenciesWereSelected = false;

        if(this.form.controls.advancements.value.length > 0){

            let advancement = this.advancements.find(x => x.id == this.form.controls.advancements.value[0]);

            const currencyId = advancement.currencyId;
            const currencyDescription = advancement.currencyText;

            this.form.controls.advancements.value.forEach(element => {
                advancement = this.advancements.find(x => x.id == element);

                if(currencyId != advancement.currencyId){
                    this.differentCurrenciesWereSelected = true;
                }
            });

            if(!this.differentCurrenciesWereSelected){
                this.form.controls.currencyId.setValue(currencyId);
                this.currencyDescription = currencyDescription;
            }
            else{
                this.form.controls.currencyId.setValue(0);
                this.currencyDescription = "";
            }
        }
        else{
            this.form.controls.currencyId.setValue(this.defaultCurrencyId);
            this.currencyDescription = this.defaultCurrencyDescription;
        }

        this.calculateTotals();
    }

    hasAdvancements(){
        return this.form.controls.advancements.value != null
            && this.form.controls.advancements.value.length > 0;
    }

    formConfiguration(){
        this.form.controls['advancements'].valueChanges.subscribe(
            (selectedValue) => {
                if(selectedValue.length > 0){
                    this.hasCreditCard = false;
                    this.hasCreditCardChanged(false);
                }
            }
        );
    }

    hasCreditCardChanged(value){
        if(value == true){
            this.form.controls.advancements.setValue(new Array());

            this.form.controls.creditCardId.setValidators([Validators.required]);
            this.form.controls.creditCardId.updateValueAndValidity();
        }
        else {
            this.form.controls.creditCardId.clearValidators();
            this.form.controls.creditCardId.updateValueAndValidity();
            this.form.controls.creditCardId.setValue(null);
        }
    }
}
