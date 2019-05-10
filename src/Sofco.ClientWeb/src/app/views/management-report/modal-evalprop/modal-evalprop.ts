import { Component, OnInit, OnDestroy, ViewChild, Output, EventEmitter } from "@angular/core";
import { Subscription } from "rxjs";
import { FormControl, Validators } from "@angular/forms";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { ManagementReportService } from "app/services/management-report/management-report.service";
import { MenuService } from "app/services/admin/menu.service";

@Component({
    selector: 'modal-evalprop',
    templateUrl: './modal-evalprop.html',
})
export class ModalEvalPropComponent implements OnInit, OnDestroy {

    updateEvalpropValueSubscrip: Subscription;
    editEvalPropValue = new FormControl('', [Validators.required, Validators.min(0), Validators.max(999999999)]);
    
    monthSelectedDisplay: string = "";
    monthSelected: any;

    @ViewChild('editEvalPropModal') editEvalPropModal;
    public editEvalPropModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Editar Monto EvalProp",
        "editEvalPropModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    constructor(private managementReportService: ManagementReportService,
        private menuService: MenuService) { }

    ngOnInit(): void {
        this.editEvalPropModal.size = 'modal-sm'
    }

    ngOnDestroy(): void {
        if (this.updateEvalpropValueSubscrip) this.updateEvalpropValueSubscrip.unsubscribe();
    }

    openEditEvalProp(month){
        if(!this.menuService.userIsCdg) return;

        this.editEvalPropModal.show()
        this.editEvalPropValue.setValue(month.valueEvalProp);
        this.monthSelectedDisplay = month.display;
        this.monthSelected = month;
    }

    updateEvalPropValue(){
        var json = {
            id: this.monthSelected.billingMonthId,
            value: this.editEvalPropValue.value,
            type: this.monthSelected.type
        }

        this.updateEvalpropValueSubscrip = this.managementReportService.updateBilling(json).subscribe(response => {
            this.editEvalPropModal.hide()
            this.monthSelected.valueEvalProp = this.editEvalPropValue.value;
            this.monthSelectedDisplay = null;
            this.monthSelected = null;
            this.editEvalPropValue.setValue(null);
        }, 
        error => this.editEvalPropModal.hide());
    }
}