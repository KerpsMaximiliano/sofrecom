import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { I18nService } from "app/services/common/i18n.service";
import { MessageService } from "app/services/common/message.service";
import { Subscription } from "rxjs";
import { EmployeeService } from "app/services/allocation-management/employee.service"
import { ManagementReportDetailStaffComponent } from "../detail/detail-staff";
import { ManagementReportStaffService } from "app/services/management-report/management-report-staff.service";
import { category } from "app/models/management-report/category";

@Component({
    selector: 'cost-detail-month-staff',
    templateUrl: './cost-month-staff.html',
    styleUrls: ['./cost-month-staff.scss']
})
export class CostDetailMonthStaffComponent implements OnInit, OnDestroy {

    updateCostSubscrip: Subscription;
    getContratedSuscrip: Subscription;
    deleteContractedSuscrip: Subscription;
    deleteOtherSuscrip: Subscription;
    paramsSubscrip: Subscription;
    getEmployeesSubscrip: Subscription

    getCategoriesSuscrip: Subscription;
    categories: category[] = new Array()
    categorySelected: category;
    subcategories: any[] = new Array();
    subcategorySelected: any;
    subCategoriesData: any[] = new Array()

    totalCosts: number = 0;
    totalProvisioned: number = 0;
    totalProvisionedAux: number = 0;
    totalChargesPercentage: number = 0;

    totalProvisionedEditabled: boolean = false;

    resources: any[] = new Array();
    // expenses: any[] = new Array();
    users: any[] = new Array()

    AnalyticId: any;
    fundedResources: any[] = new Array();
    otherResources: any[] = new Array();
    otherSelected: any;
    managementReportId: number;
    //contracted: any[] = new Array();
    monthYear: Date;
    canSave: boolean = true;
    userSelected: any

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
        private managementReportStaffService: ManagementReportStaffService,
        private managementReport: ManagementReportDetailStaffComponent,
        private employeeService: EmployeeService
    ) { }

    ngOnInit(): void {
        this.getUsers();
        this.getCategories();
    }

    ngOnDestroy(): void {
        if (this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if (this.updateCostSubscrip) this.updateCostSubscrip.unsubscribe();
        if (this.getContratedSuscrip) this.getContratedSuscrip.unsubscribe();
        if (this.deleteOtherSuscrip) this.deleteOtherSuscrip.unsubscribe();
        if (this.getEmployeesSubscrip) this.getEmployeesSubscrip.unsubscribe()
        if (this.getCategoriesSuscrip) this.getCategoriesSuscrip.unsubscribe()
    }

    totalProvisionedChanged() {
        this.totalProvisionedAux = this.totalProvisioned;
    }

    deleteResource(index, item) {
        //Si el item no esta en base de datos solo lo borro del array
        if (item.id == 0) {
            this.resources.splice(index, 1);
        }
    }

    open(data, readOnly) {
        this.messageService.showLoading()
        //this.expenses = [];

        this.managementReportId = data.managementReportId;
        this.costDetailMonthModal.otherTitle = `${data.monthDesc} ${data.year}`
      
        this.isReadOnly = readOnly;
        this.AnalyticId = data.AnalyticId;

        this.getContratedSuscrip = this.managementReportStaffService.getCostDetailMonth(this.managementReportId, data.month, data.year).subscribe(response => {
            this.monthYear = response.data.monthYear
            this.resources = response.data.employees;
            this.subCategoriesData = response.data.subcategories;
            this.totalProvisioned = response.data.totalProvisioned;

            this.calculateTotalCosts();

            this.messageService.closeLoading();
            this.costDetailMonthModal.show();
        },
            () => {
                this.messageService.closeLoading();
                this.costDetailMonthModal.hide();
            });
    }

    save() {
        var model = {
            AnalyticId: 0,
            ManagementReportId: 0,
            MonthYear: new Date(),
            Employees: [],
            OtherResources: [],
            Contracted: [],
            totalBilling: 0,
            totalProvisioned: this.totalProvisionedAux != null ? this.totalProvisionedAux : null,
            provision: 0,
            Subcategories: this.subCategoriesData
        }

        model.AnalyticId = this.AnalyticId
        model.ManagementReportId = this.managementReportId
        model.MonthYear = this.monthYear
        model.Employees = this.resources

        this.updateCostSubscrip = this.managementReportStaffService.PostCostDetailStaffMonth(this.managementReportId, model).subscribe(() => {
            this.costDetailMonthModal.hide();
            this.managementReport.updateBudgetView();
        },
            () => {
                this.costDetailMonthModal.resetButtons();
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

        this.resources.forEach(element => {
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
        },
        () => { });
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
            },
            () => { });
    }

    categoryChange() {
        this.subcategorySelected = {}
        this.subcategories = new Array()
        this.subcategories = this.categorySelected.subcategories
    }

    addSubcategoryData() {

        var cost = {
            costDetailStaffId: 0,
            id: this.subcategorySelected.id,
            name: this.subcategorySelected.name,
            nameCategory: this.categorySelected.name,
            monthYear: this.monthYear,
            description: "",
            value: 0,
            deleted: false
        }

        this.subCategoriesData.push(cost)
    }

    deleteSubcategory(index) {
        
        if (this.subCategoriesData[index].costDetailStaffId == 0) {
            this.subCategoriesData.splice(index, 1)
        }
        else {
            this.subCategoriesData[index].value = 0
            this.subCategoriesData[index].deleted = true
        }

    }
}