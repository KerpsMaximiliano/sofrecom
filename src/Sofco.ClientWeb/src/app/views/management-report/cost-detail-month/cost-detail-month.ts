import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { I18nService } from "app/services/common/i18n.service";
import { MessageService } from "app/services/common/message.service";
import { Subscription } from "rxjs";
import { ManagementReportService } from "app/services/management-report/management-report.service";
import { ActivatedRoute } from "@angular/router";
import { detectBody } from "app/app.helpers";
import { ManagementReportDetailComponent } from "../detail/mr-detail"
import { datepickerLocale } from "fullcalendar";

@Component({
    selector: 'cost-detail-month',
    templateUrl: './cost-detail-month.html',
    styleUrls: ['./cost-detail-month.scss']
})
export class CostDetailMonthComponent implements OnInit, OnDestroy {

    updateCostSubscrip: Subscription;
    getContratedSuscrip: Subscription;
    getOtherSuscrip: Subscription;
    deleteContractedSuscrip: Subscription;
    deleteOtherSuscrip: Subscription;
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
    otherSelected: any;
    managementReportId: number;
    contracted: any[] = new Array();
    monthYear: Date;
    canSave: boolean = false;

    isReadOnly: boolean

    constructor(public i18nService: I18nService,
        private messageService: MessageService,
        private managementReportService: ManagementReportService,
        private activatedRoute: ActivatedRoute,
        private managementReport: ManagementReportDetailComponent
    ) { }

    ngOnInit(): void {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.serviceId = params['serviceId'];
        });

        this.getOtherSuscrip = this.managementReportService.getOtherResources().subscribe(response => {

            this.otherResources = response.data;

            if (this.otherResources.length > 0) {
                this.otherSelected = this.otherResources[0];
            }

            this.messageService.closeLoading();
        },
            error => {
                this.messageService.closeLoading();
            });
    }

    ngOnDestroy(): void {
        if (this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if (this.updateCostSubscrip) this.updateCostSubscrip.unsubscribe();
        if (this.deleteContractedSuscrip) this.deleteContractedSuscrip.unsubscribe();
        if (this.getContratedSuscrip) this.getContratedSuscrip.unsubscribe();
        if (this.getOtherSuscrip) this.getOtherSuscrip.unsubscribe();
        if (this.deleteOtherSuscrip) this.deleteOtherSuscrip.unsubscribe();
    }

    addExpense() {

        var resource = {
            id: 0,
            typeId: this.otherSelected.typeId,
            typeName: this.otherSelected.typeName,
            value: 0,
            description: ""
        }

        this.expenses.push(resource)
    }

    addContracted() {
        this.canSave = false;
        this.contracted.push({ contractedId: 0, name: "", honorary: 0, insurance: 0, total: 0, monthYear: this.monthYear })
    }

    contractedChange(hire) {
        hire.total = hire.honorary + hire.insurance;

        this.calculateTotalCosts();
    }

    deleteContracted(index, item) {

        if (item.contractedId > 0) {
            this.deleteContractedSuscrip = this.managementReportService.deleteContracted(item.contractedId).subscribe(response => {
                this.contracted.splice(index, 1)
            },
                error => {
                });
        }
        else {
            this.contracted.splice(index, 1)
        }

        if (this.contracted.length == 0) {
            this.canSave = true;
        }
    }

    deleteExpense(index, item) {

        console.log(item)
        //Si el item no esta en base de datos solo lo borro del array
        if (item.id == 0) {
            this.expenses.splice(index, 1);
        }
        else {
            //Si esta en base de datos borro el registio
            this.deleteContractedSuscrip = this.managementReportService.deleteOtherResources(item.id).subscribe(response => {
                this.expenses.splice(index, 1);
            },
            error => {
            });
        }

        this.calculateTotalCosts();
    }

    open(data) {
        this.messageService.showLoading()
        this.expenses = [];

        this.isReadOnly = !data.isCdg;
        this.AnalyticId = data.AnalyticId;
        this.resources = data.resources.employees.filter(e => e.hasAlocation == true)
        this.totalBilling = data.totals.totalBilling;
        this.totalProvisioned = data.totals.totalProvisioned;

        this.getContratedSuscrip = this.managementReportService.getCostDetailMonth(this.serviceId, data.month, data.year).subscribe(response => {

            this.managementReportId = response.data.managementReportId;
            this.monthYear = response.data.monthYear
            this.contracted = response.data.contracted;
            this.expenses = response.data.otherResources;

            this.calculateTotalCosts();

            this.messageService.closeLoading();
            this.costDetailMonthModal.show();
        },
            error => {
                this.messageService.closeLoading();
                this.costDetailMonthModal.hide();
            });
    }

    save() {

        this.messageService.showLoading();
        var model = {
            AnalyticId: 0,
            ManagementReportId: 0,
            MonthYear: new Date(),
            Employees: [],
            OtherResources: [],
            Contracted: []
        }

        model.AnalyticId = this.AnalyticId
        model.ManagementReportId = this.managementReportId
        model.MonthYear = this.monthYear
        model.Employees = this.resources
        model.OtherResources = this.expenses
        model.Contracted = this.contracted;

        this.updateCostSubscrip = this.managementReportService.PostCostDetailMonth(this.serviceId, model).subscribe(response => {
            this.messageService.closeLoading();
            this.costDetailMonthModal.hide();
            this.managementReport.updateDetailCost()
        },
            error => {
                this.messageService.closeLoading();
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
            this.totalCosts += element.value;
        });

        this.resources.forEach(element => {
            this.totalCosts += element.total;
        });

        this.contracted.forEach(element => {
            this.totalCosts += element.total;
        });
    }


}