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

@Component({
    selector: 'refund-form',
    templateUrl: './refund-form.component.html'
})
export class RefundFormComponent implements OnInit, OnDestroy {

    public advancements: any[] = new Array();
    public analytics: any[] = new Array();

    public userApplicantIdLogged: number;
    public userApplicantName: string;
    public status: string;
    public currencyDescription: string;

    public advancementSum: number = 0;
    public refundSum: number = 0;
    public differenceSum: number = 0;



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

    private id: number;
    public workflowStateType: WorkflowStateType;

    public differentCurrenciesWereSelected: boolean = false;
    public canUpdate: boolean = false;

    getAdvancementsSubscrip: Subscription;
    getAnalyticsSubscrip: Subscription;

    constructor(public formsService: FormsService,
        public advancementService: AdvancementService,
        public messageService: MessageService,
        public analyticService: AnalyticService,
        public i18nService: I18nService){}

    ngOnInit(): void {
        if(this.mode == 'add'){
            this.form = new Refund(false);
            this.setUserApplicant();
            this.canUpdate = true;
        }

        this.detailForms = new Array();
        this.detailModalForm = new RefundDetail();

        this.getAdvancementsUnrelated();
        this.getAnalytics();
    }

    ngOnDestroy(): void {
        if(this.getAdvancementsSubscrip) this.getAdvancementsSubscrip.unsubscribe();
        if(this.getAnalyticsSubscrip) this.getAnalyticsSubscrip.unsubscribe();
    }

    getAnalytics() {
        this.getAnalyticsSubscrip = this.analyticService.getByCurrentUser().subscribe(res => {
            this.analytics = res.data;
        });
      }

    getAdvancementsUnrelated(){
        this.getAdvancementsSubscrip = this.advancementService.getUnrelated().subscribe(response => {
            this.advancements = response.data;
        });
    }

    addDetail(){
        var detail = new RefundDetail();
        this.detailForms.push(detail);
        this.editDetail(detail, this.detailForms.length-1);
    }

    editDetail(detail, index){
        this.detailFormAux.creationDate = detail.controls.creationDate.value;
        this.detailFormAux.description = detail.controls.description.value;
        this.detailFormAux.ammount = detail.controls.ammount.value;
        this.indexAux = index;

        this.detailModalForm = detail;
        this.addDetailModal.show();
    }

    saveDetail(){
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
        var detail = this.detailForms[this.indexAux];

        detail.controls.creationDate.setValue(this.detailFormAux.creationDate);
        detail.controls.description.setValue(this.detailFormAux.description);
        detail.controls.ammount.setValue(this.detailFormAux.ammount);

        this.detailModalForm = new RefundDetail();
    }

    canSave(){
        if(this.form.valid && this.detailForms.length > 0 && this.detailForms.every(x => x.valid) && !this.differentCurrenciesWereSelected) return true;
        
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
        var advancement = this.form.getModel();
        
        this.detailForms.forEach(element => {
            advancement.details.push(element.getModel());
        });

        return advancement;
    }

    calculateTotals(){
        this.advancementSum = 0;
        this.refundSum = 0;
        this.differenceSum = 0;

        if(this.differentCurrenciesWereSelected) return;

        if(this.form.controls.advancements.value.length > 0){
            this.form.controls.advancements.value.forEach(element => {
                var advancement = this.advancements.find(x => x.id == element);

                if(advancement.ammount > 0){
                    this.advancementSum += advancement.ammount;
                }
            });
        }

        if(this.detailForms.length > 0){
            this.detailForms.forEach(element => {
                if(element.controls.ammount.value > 0){
                    this.refundSum += element.controls.ammount.value;    
                }
            });
        }

        this.differenceSum = this.advancementSum - this.refundSum;
    }

    advancementsChanged(){
        this.differentCurrenciesWereSelected = false;

        if(this.form.controls.advancements.value.length > 0){

            var advancement = this.advancements.find(x => x.id == this.form.controls.advancements.value[0]);

            var currencyId = advancement.currencyId;
            var currencyDescription = advancement.currencyText;

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
            this.form.controls.currencyId.setValue(0);
            this.currencyDescription = "";
        }

        this.calculateTotals();
    }
}