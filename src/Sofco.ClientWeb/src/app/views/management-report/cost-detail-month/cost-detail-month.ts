import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { I18nService } from "app/services/common/i18n.service";
import { MessageService } from "app/services/common/message.service";
import { Subscription } from "rxjs";
import { ManagementReportService } from "app/services/management-report/management-report.service";
import { ActivatedRoute } from "@angular/router";
import { ManagementReportDetailComponent } from "../detail/mr-detail"
import { EmployeeService } from "app/services/allocation-management/employee.service"
import { category } from "app/models/management-report/category";
import { ManagementReportStaffService } from "app/services/management-report/management-report-staff.service";
import { debug } from "util";

@Component({
    selector: 'cost-detail-month',
    templateUrl: './cost-detail-month.html',
    styleUrls: ['./cost-detail-month.scss']
})
export class CostDetailMonthComponent implements OnInit, OnDestroy {

    updateCostSubscrip: Subscription;
    getContratedSuscrip: Subscription;
    //getOtherSuscrip: Subscription;
    deleteContractedSuscrip: Subscription;
    deleteOtherSuscrip: Subscription;
    paramsSubscrip: Subscription;
    getEmployeesSubscrip: Subscription

    totalProvisioned: number = 0;
    totalProvisionedAux: number;
    totalCosts: number = 0;
    totalBilling: number = 0;
    totalBillingAux: number;
    provision: number = 0;
    provisionAux: number;
    totalChargesPercentage: number = 0;

    totalProvisionedEditabled: boolean = false;
    totalBillingEditabled: boolean = false;
    provisionEditabled: boolean = false;

    resources: any[] = new Array();
    expenses: any[] = new Array();
    users: any[] = new Array()

    serviceId: string;
    AnalyticId: any;
    fundedResources: any[] = new Array();
    // otherResources: any[] = new Array();
    // otherSelected: any;
    managementReportId: number;
    contracted: any[] = new Array();
    monthYear: Date;
    canSave: boolean = false;
    userSelected: any
    hasCostProfile: boolean = false

    getCategoriesSuscrip: Subscription;
    categories: category[] = new Array()
    categorySelected: category;
    subcategories: any[] = new Array();
    subcategorySelected: any;
    subCategoriesData: any[] = new Array()

    isReadOnly: boolean
    
    @ViewChild('costDetailMonthModal') costDetailMonthModal;
    public costDetailMonthModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "managementReport.costDetailMonth",
        "costDetailMonthModal",
        true,
        true,
        "ACTIONS.save",
        "ACTIONS.cancel"
    );

    constructor(public i18nService: I18nService,
        private messageService: MessageService,
        private managementReportService: ManagementReportService,
        private activatedRoute: ActivatedRoute,
        private managementReport: ManagementReportDetailComponent,
        private managementReportStaffService: ManagementReportStaffService,
        private employeeService: EmployeeService
    ) { }

    ngOnInit(): void {

        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.serviceId = params['serviceId'];
        });

        // this.getOtherSuscrip = this.managementReportService.getOtherResources().subscribe(response => {
        //     this.otherResources = response.data;

        //     if (this.otherResources.length > 0) {
        //         this.otherSelected = this.otherResources[0];
        //     }
        // });

        this.getCategories()
        this.getUsers()
    }

    ngOnDestroy(): void {
        if (this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if (this.updateCostSubscrip) this.updateCostSubscrip.unsubscribe();
        if (this.deleteContractedSuscrip) this.deleteContractedSuscrip.unsubscribe();
        if (this.getContratedSuscrip) this.getContratedSuscrip.unsubscribe();
        if (this.getCategoriesSuscrip) this.getCategoriesSuscrip.unsubscribe()
        if (this.deleteOtherSuscrip) this.deleteOtherSuscrip.unsubscribe();
        if (this.getEmployeesSubscrip) this.getEmployeesSubscrip.unsubscribe()
    }

    totalProvisionedChanged() {
        this.totalProvisionedAux = this.totalProvisioned;
    }

    totalBillingChanged() {
        this.totalBillingAux = this.totalBilling;
    }

    provisionChanged() {
        this.provisionAux = this.provision;
    }

    addExpense() {

        var resource = {
            id: 0,
            subcategoryId: this.subcategorySelected.id,
            subcategoryName: this.subcategorySelected.name,
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
            this.deleteContractedSuscrip = this.managementReportService.deleteContracted(item.contractedId).subscribe(() => {
                this.contracted.splice(index, 1)
            },
                () => {
                });
        }
        else {
            this.contracted.splice(index, 1)
        }

        if (this.contracted.length == 0) {
            this.canSave = true;
        }
    }

    deleteResource(index, item) {

        //Si el item no esta en base de datos solo lo borro del array
        if (item.id == 0) {
            this.resources.splice(index, 1);
        }
    }

    deleteExpense(index, item) {

        //Si el item no esta en base de datos solo lo borro del array
        if (item.id == 0) {
            this.expenses.splice(index, 1);
        }
        else {
            //Si esta en base de datos borro el registio
            this.deleteContractedSuscrip = this.managementReportService.deleteOtherResources(item.id).subscribe(() => {
                this.expenses.splice(index, 1);
            },
                () => {
                });
        }

        this.calculateTotalCosts();
    }

    open(data, readOnly) {
        this.messageService.showLoading()
        this.expenses = [];
       
        this.costDetailMonthModal.otherTitle = `${data.monthDesc} ${data.year}`

        this.isReadOnly = readOnly;
        this.AnalyticId = data.AnalyticId;
        
        this.resources = data.resources.employees.filter( x=> x.hasAlocation == true || x.salary > 0 || x.charges > 0)
        this.totalBilling = data.totals.totalBilling;
        this.totalProvisioned = data.totals.totalProvisioned;
        this.provision = data.totals.provision;

        this.getContratedSuscrip = this.managementReportService.getCostDetailMonth(this.serviceId, data.month, data.year).subscribe(response => {

            this.managementReportId = response.data.managementReportId;
            this.monthYear = response.data.monthYear
            this.contracted = response.data.contracted;
            this.expenses = response.data.otherResources;
            this.hasCostProfile = response.data.hasCostProfile

            if (response.data.totalBilling && response.data.totalBilling != null) this.totalBilling = response.data.totalBilling;
            if (response.data.provision && response.data.provision != null) this.provision = response.data.provision;
            if (response.data.totalProvisioned && response.data.totalProvisioned != null) this.totalProvisioned = response.data.totalProvisioned;

            this.calculateTotalCosts();

            if(this.hasCostProfile){
                this.messageService.showWarning('managementReport.existProfiles')
            }

            this.messageService.closeLoading();
            this.costDetailMonthModal.show();
        },
            () => {
                this.messageService.closeLoading();
                this.costDetailMonthModal.hide();
            });
    }

    save() {

        if(this.hasCostProfile){
            this.messageService.showError('managementReport.existProfiles')
            this.costDetailMonthModal.resetButtons()
            return
        }

        this.messageService.showLoading();

        var model = {
            AnalyticId: 0,
            ManagementReportId: 0,
            MonthYear: new Date(),
            IsReal: true,
            Employees: [],
            OtherResources: [],
            Contracted: [],
            totalBilling: this.totalBillingAux != null ? this.totalBillingAux : null,
            totalProvisioned: this.totalProvisionedAux != null ? this.totalProvisionedAux : null,
            provision: this.provisionAux != null ? this.provisionAux : null,
        }

        model.AnalyticId = this.AnalyticId
        model.ManagementReportId = this.managementReportId
        model.MonthYear = this.monthYear
        model.Employees = this.resources
        model.OtherResources = this.expenses
        model.Contracted = this.contracted;

        this.updateCostSubscrip = this.managementReportService.PostCostDetailMonth(this.serviceId, model).subscribe(() => {
            this.messageService.closeLoading();
            this.costDetailMonthModal.hide();
            this.managementReport.updateDetailCost()
        },
            () => {
                this.messageService.closeLoading();
            });
    }

    subResourceChange(subResource) {
        subResource.total = subResource.salary + subResource.insurance;

        this.calculateTotalCosts();
    }

    resourceChange(resource) {
        resource.modified = true
        resource.total = resource.salary + resource.charges;

        if(resource.salary > 0){
            resource.chargesPercentage = (resource.charges/resource.salary)*100;
        }
        else{
            resource.chargesPercentage = 0;
        }

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

        var totalCharges = 0;
        var totalSalary= 0;

        this.resources.forEach(element => {
            totalCharges += element.charges;
            totalSalary += element.salary;
        });

        if(totalSalary > 0){
            this.totalChargesPercentage = (totalCharges/totalSalary)*100;
        }
    }

    getUsers() {
        this.getEmployeesSubscrip = this.employeeService.getListItems().subscribe(data => {
            this.users = data;
        });
    }

    addEmployee() {

        if (this.userSelected) {
            var existingEmployee = this.resources.find(e => e.employeeId === this.userSelected.id)
            if (!existingEmployee) {
                var costEmployee = {
                    id: 0,
                    costDetailId: 0,
                    employeeId: this.userSelected.id,
                    userId: this.userSelected.userId,
                    name: `${this.userSelected.text.toUpperCase()} - ${this.userSelected.employeeNumber}`,
                    salary: 0,
                    charges: 0,
                    chargesPercentage: 0,
                    total: 0,
                    hasAlocation: false,
                    new: true
                }

                this.resources.push(costEmployee)
            }
            else {
                this.messageService.showError("managementReport.existingEmployee")
            }
        }
        else {
            this.messageService.showError("managementReport.userRequired")
        }
    }

    getCategories() {
        this.getCategoriesSuscrip = this.managementReportStaffService.getCostDetailCategories().subscribe(
            data => {                
                this.categories = data.data;

                if(this.categories.length > 0){
                    this.categorySelected = this.categories[0]
                    this.categoryChange()
                }
            },
            () => { });
    }

    categoryChange() {        
        this.subcategorySelected = {}
        this.subcategories = new Array()
        this.subcategories = this.categorySelected.subcategories

        if(this.subcategories.length > 0){
            this.subcategorySelected = this.subcategories[0]
        }
    }

}