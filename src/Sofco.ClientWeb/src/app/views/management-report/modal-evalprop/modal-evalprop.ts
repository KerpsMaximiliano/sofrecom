import { Component, OnInit, OnDestroy, ViewChild, Output, EventEmitter } from "@angular/core";
import { Subscription } from "rxjs";
import { FormControl, Validators } from "@angular/forms";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { ManagementReportService } from "app/services/management-report/management-report.service";
import { MenuService } from "app/services/admin/menu.service";
import * as moment from 'moment';

@Component({
    selector: 'modal-evalprop',
    templateUrl: './modal-evalprop.html',
})
export class ModalEvalPropComponent implements OnInit, OnDestroy {

    updateEvalpropValueSubscrip: Subscription;
    editEvalPropValue = new FormControl('', [Validators.required, Validators.min(0), Validators.max(999999999)]);
    
    monthSelectedDisplay: string = "";
    monthSelected: any;
    managementReportId: number;
    icon: string = "$";
    replicate: boolean = false;
    months: any;

    @Output() getData: EventEmitter<any> = new EventEmitter();

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

    openEditEvalProp(month, months){
        if(!this.menuService.userIsCdg) return;

        if(month.icon){
            this.icon = month.icon;
            this.editEvalPropValue.setValidators([Validators.required, Validators.min(-100), Validators.max(100)]);
        }
        else{
            this.icon = "$"
            this.editEvalPropValue.setValidators([Validators.required, Validators.min(0), Validators.max(999999999)])
        }

        this.editEvalPropModal.show()
        this.editEvalPropValue.setValue(month.valueEvalProp);
        this.monthSelectedDisplay = month.display;
        this.monthSelected = month;
        this.months = months;
    }

    updateEvalPropValue(){
        var json = {
            id: this.monthSelected.billingMonthId,
            value: this.editEvalPropValue.value,
            type: this.monthSelected.type,
            managementReportId: this.managementReportId,
            monthYear: this.monthSelected.monthYear,
            replicate: this.replicate
        }

        this.updateEvalpropValueSubscrip = this.managementReportService.updateBilling(json).subscribe(response => {
            this.editEvalPropModal.hide()

            this.sendDataToDetailView(this.monthSelected, this.editEvalPropValue.value);

            this.monthSelected.valueEvalProp = this.editEvalPropValue.value;
            this.monthSelected.billingMonthId = response.data;

            this.months.forEach(month => {
                if(new Date(month.year, month.month, 1) > moment(this.monthSelected.monthYear).toDate()){
                    month.valueEvalProp = this.editEvalPropValue.value
                }
            });

            this.monthSelectedDisplay = null;
            this.monthSelected = null;
            this.editEvalPropValue.setValue(null);
        }, 
        error => this.editEvalPropModal.hide());
    }

    sendDataToDetailView(monthSelected, value){
        if (this.getData.observers.length > 0) {
            this.getData.emit({
                month: monthSelected.month,
                year: monthSelected.year,
                type: monthSelected.type,
                value
            });
        }
    }
}