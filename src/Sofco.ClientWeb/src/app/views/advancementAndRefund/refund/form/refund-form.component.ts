import { Component, OnInit, OnDestroy, Input, ViewChild } from "@angular/core";
import { FormsService } from "app/services/forms/forms.service";
import { I18nService } from "app/services/common/i18n.service";
import { Refund } from "app/models/advancement-and-refund/refund";
import { WorkflowStateType } from "app/models/enums/workflowStateType";
import { Subscription } from "rxjs";
import { AdvancementService } from "app/services/advancement-and-refund/advancement.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { RefundDetail } from "app/models/advancement-and-refund/refund-detail";

@Component({
    selector: 'refund-form',
    templateUrl: './refund-form.component.html'
})
export class RefundFormComponent implements OnInit, OnDestroy {

    public advancements: any[] = new Array();

    public userApplicantIdLogged: number;
    public userApplicantName: string;
    public status: string;
    public currencyDescription: string;

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

    private id: number;
    public workflowStateType: WorkflowStateType;

    getAdvancementsSubscrip: Subscription;

    constructor(public formsService: FormsService,
        public advancementService: AdvancementService,
        public i18nService: I18nService){}

    ngOnInit(): void {
        if(this.mode == 'add'){
            this.form = new Refund(false);
            this.detailForms = new Array();
            this.detailModalForm = new RefundDetail(false);
        }
    }

    ngOnDestroy(): void {
        if(this.getAdvancementsSubscrip) this.getAdvancementsSubscrip.unsubscribe();
    }

    // getAdvancementsUnrelated(){
    //     this.getAdvancementsSubscrip = this.advancementService.getUnrelated().subscribe(response => {
    //         this.advancements = response;
    //     });
    // }

    addDetail(){
        var detail = new RefundDetail(false);
        this.detailForms.push(detail);
        this.editDetail(detail);
    }

    editDetail(detail){
        this.detailModalForm = detail;
        this.addDetailModal.show();
    }

    saveDetail(){
        this.detailModalForm = new RefundDetail(false);
        this.addDetailModal.hide();
    }

    canSave(){
        if(this.form.valid && this.detailForms.every(x => x.valid)) return true;
        
        return false;
    }
}