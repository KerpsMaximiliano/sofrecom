import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { I18nService } from "app/services/common/i18n.service";

@Component({
    selector: 'cost-detail-month',
    templateUrl: './cost-detail-month.html',
    styleUrls: ['./cost-detail-month.scss']
})
export class CostDetailMonthComponent implements OnInit, OnDestroy {

    @ViewChild('costDetailMonthModal') costDetailMonthModal;
    public costDetailMonthModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "managementReport.costDetailMonth",
        "costDetailMonthModal",
        true,
        true,
        "ACTIONS.save",
        "ACTIONS.cancel"
    );

    totalProvisioned: number = 0;
    totalCosts: number = 0;
    totalBilling: number = 0;

    resources: any[] = new Array();
    expenses: any[] = new Array();

    isReadOnly: boolean 

    constructor(public i18nService: I18nService){}

    ngOnInit(): void {
    }

    ngOnDestroy(): void {
    }

    addExpense(){
        this.expenses.push({ type: "Gasto x", description: "", total: 0 });
    }

    deleteExpense(index){
        this.expenses.splice(index, 1);
        this.calculateTotalCosts();
    }

    open(data){
        this.isReadOnly = !data.isCdg;
        this.resources = data.resources;
        this.totalBilling = data.totals.totalBilling;
        this.totalProvisioned = data.totals.totalProvisioned;

        this.calculateTotalCosts();
        this.costDetailMonthModal.show();
    }

    save(){
        
    }

    subResourceChange(subResource){
        subResource.total = subResource.salary + subResource.insurance;

        this.calculateTotalCosts();
    }

    resourceChange(resource){
        resource.total = resource.salary + resource.charges;

        this.calculateTotalCosts();
    }

    calculateTotalCosts(){
        this. totalCosts = 0;

        this.expenses.forEach(element => {
            this.totalCosts += element.total;
        });

        this.resources.forEach(element => {
            this.totalCosts += element.total;
        });
    }
}