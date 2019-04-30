import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { I18nService } from "app/services/common/i18n.service";
import { MessageService } from "app/services/common/message.service";
import { Subscription } from "rxjs";
import { ManagementReportService } from "app/services/management-report/management-report.service";
import { ActivatedRoute } from "@angular/router";
import { detectBody } from "app/app.helpers";

@Component({
    selector: 'cost-detail-month',
    templateUrl: './cost-detail-month.html',
    styleUrls: ['./cost-detail-month.scss']
})
export class CostDetailMonthComponent implements OnInit, OnDestroy {

    updateCostSubscrip: Subscription;
    paramsSubscrip: Subscription;

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

    serviceId: string;
    AnalyticId: any;
    fundedResources: any[] = new Array();
    otherResources: any[] = new Array();
    otherResourceId: number;

    isReadOnly: boolean

    constructor(public i18nService: I18nService,
        private messageService: MessageService,
        private managementReportService: ManagementReportService,
        private activatedRoute: ActivatedRoute,
    ) { }

    ngOnInit(): void {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.serviceId = params['serviceId'];
        });
    }

    ngOnDestroy(): void {
        if (this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if (this.updateCostSubscrip) this.updateCostSubscrip.unsubscribe();
    }

    addExpense() {
        //  this.expenses.push({ type: "Gasto x", description: "", total: 0 });
        debugger
        var resource = this.otherResources.find(r => r.typeId == this.otherResourceId)
        this.expenses.push(resource)

        var pos = this.otherResources.findIndex(r => r.typeId == this.otherResourceId);
        this.otherResources.splice(pos, 1)

        this.otherResourceId = this.otherResources[0].typeId;
    }

    deleteExpense(index, item) {

        item.salary = 0;
        this.expenses.splice(index, 1);
        this.otherResources.push(item)

        this.otherResources.sort(function (a, b) {
            if (a.typeName > b.typeName) {
                return 1;
            }
            if (a.typeName < b.typeName) {
                return -1;
            }
            return 0;
        });

        this.otherResourceId = this.otherResources[0].typeId;

        this.calculateTotalCosts();
    }

    open(data) {

        this.isReadOnly = !data.isCdg;
        this.AnalyticId = data.AnalyticId;
        this.resources = data.resources;
        this.fundedResources = data.fundedResources;
        this.otherResources = data.otherResources;
        this.otherResourceId = this.otherResources[0].typeId;
        this.totalBilling = data.totals.totalBilling;
        this.totalProvisioned = data.totals.totalProvisioned;

        this.fundedResources.forEach(resource => {
            if (resource.otherResource == true) {
                if (resource.salary > 0) {
                    this.expenses.push(resource);
                }
                else {
                    this.otherResources.push(resource)
                }
            }
        })

        this.calculateTotalCosts();
        this.costDetailMonthModal.show();
    }

    save() {

        this.messageService.showLoading();
        var model = {
            AnalyticId: 0,
            Employees: [],
            OtherResources: []
        }

        model.AnalyticId = this.AnalyticId
        model.Employees = this.resources;
        model.OtherResources = this.expenses.concat(this.otherResources);
        
        this.updateCostSubscrip = this.managementReportService.PostCostDetailMonth(this.serviceId, model).subscribe(response => {
            this.messageService.closeLoading();
            this.costDetailMonthModal.hide();
            this.expenses = []
        },
        error => {
            this.messageService.closeLoading();
            this.costDetailMonthModal.hide();
        });
    }

    subResourceChange(subResource) {
        subResource.total = subResource.salary + subResource.insurance;

        this.calculateTotalCosts();
    }

    resourceChange(resource) {
        resource.total = resource.salary + resource.charges;

        this.calculateTotalCosts();
    }

    calculateTotalCosts() {
        this.totalCosts = 0;

        this.expenses.forEach(element => {
            this.totalCosts += element.salary;
        });

        this.resources.forEach(element => {
            this.totalCosts += element.total;
        });
    }
}