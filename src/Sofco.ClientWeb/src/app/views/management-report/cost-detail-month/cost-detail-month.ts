import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";

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
    subResources: any[] = new Array();
    expenses: any[] = new Array();

    isReadOnly: boolean 

    constructor(){}

    ngOnInit(): void {
    }

    ngOnDestroy(): void {
    }

    addSubResource(){
        this.subResources.push({ name: "", salary: 0, insurance: 0, total: 0 });
    }

    deleteSubResource(index){
        this.subResources.splice(index, 1);
        this.calculateTotalCosts();
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

        this.subResources.forEach(element => {
            this.totalCosts += element.total;
        });

        this.expenses.forEach(element => {
            this.totalCosts += element.total;
        });

        this.resources.forEach(element => {
            this.totalCosts += element.total;
        });
    }
}