import { Component, OnDestroy, OnInit, ViewChild, Output, EventEmitter } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Subscription, VirtualTimeScheduler } from "rxjs";
import { MenuService } from "app/services/admin/menu.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { MessageService } from "app/services/common/message.service";
import { ManagementReportStaffService } from "app/services/management-report/management-report-staff.service";
import { ManagementReportDetailStaffComponent } from "app/views/management-report/staff/detail/detail-staff";
import { environment } from "environments/environment";
import { ManagementReportStatus } from "app/models/enums/managementReportStatus";
import { UtilsService } from "app/services/common/utils.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { FormControl, Validators } from "@angular/forms";
import { evaluate } from 'mathjs/number'

@Component({
    selector: 'budget-staff',
    templateUrl: './budget-staff.component.html',
    styleUrls: ['./budget-staff.component.scss']
})
export class BudgetStaffComponent implements OnInit, OnDestroy {

    paramsSubscrip: Subscription;
    getCostSubscrip: Subscription;
    updateCostSubscrip: Subscription;
    generatePFASuscrip: Subscription;
    getCurrenciesSubscrip: Subscription;
    getEmployeeSubscrip: Subscription;

    showColumn = {
        budget: true,
        projected: true,
        pfa1: false,
        pfa2: false,
        real: false
    }
    dateSelected: Date
    model: any
    managementReportId: string;
    months: any[] = new Array()
    readOnly: boolean = false
    isCdg: boolean = false
    categorySelected: any = { id: 0, name: '' }
    monthSelected: any = { value: 0, display: '' };

    employees: any[] = new Array();
    categoriesEmployees: any[] = new Array();
    employeesHide: boolean = false;

    modalPercentage: boolean = false;
    modalEmployee: boolean = false;
    modalOther: boolean = false;
    modalProfile: boolean = false;

    otherCategories: any[] = new Array();
    otherSelected: any

    itemSelected: any;
    indexSelected: number = 0;
    replicateCosts: boolean = false;

    editItemMonto = new FormControl();
    editItemAdjustment = new FormControl();

    monthsToReplicateSelected: any[] = new Array();
    monthsToReplicate: any[] = new Array();
    categories: any[] = new Array();
    categoriesRedInfra: any[] = new Array();
    subCategories: any[] = new Array();
    subCategoriesData: any[] = new Array();
    subCategoriesFiltered: any[] = new Array();
    subCategorySelected: any = { id: 0, name: '' }
    typeBudgetSelected: any = { id: 0, name: '' }
    budgetTypes: any[] = new Array();
    costByMonth: any[] = new Array();
    currencies: any[] = new Array();
    actualState: string
    monthExchanges: any[] = new Array();
    sinceMonth: Date = new Date();

    totalCostsExchanges: any = {
        exchanges: [],
        currencies: [],
        total: 0
    };

    readonly generalAdjustment: string = "% Ajuste General";
    readonly typeEmployee: string = "Empleados"
    readonly typeResource: string = "Recursos"
    readonly typeProfile: string = "Perfiles"

    @Output() getData: EventEmitter<any> = new EventEmitter();

    @ViewChild('totalCostsExchangesModal') totalCostsExchangesModal;
    public totalCostsExchangesModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Detalle costos mensual",
        "totalCostsExchangesModal",
        false,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    @ViewChild('editItemModal') editItemModal;
    public editItemModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Editar Monto",
        "editItemModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    //Constructor
    constructor(private ManagementReportStaffService: ManagementReportStaffService,
        private activatedRoute: ActivatedRoute,
        private messageService: MessageService,
        private menuService: MenuService,
        private utilsService: UtilsService,
        private detailStaffComponent: ManagementReportDetailStaffComponent,
        private employeeService: EmployeeService
    ) { }

    ngOnInit(): void {

        this.editItemModal.size = 'modal-lg'
        this.isCdg = this.menuService.userIsCdg

        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.managementReportId = params['id'];
            this.getCost(this.managementReportId);
        });

        this.getCurrencies();

    }

    ngOnDestroy(): void {
        if (this.getCostSubscrip) this.getCostSubscrip.unsubscribe();
        if (this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if (this.updateCostSubscrip) this.updateCostSubscrip.unsubscribe();
        if (this.generatePFASuscrip) this.generatePFASuscrip.unsubscribe();
        if (this.getCurrenciesSubscrip) this.getCurrenciesSubscrip.unsubscribe();
        if (this.getEmployeeSubscrip) this.getEmployeeSubscrip.unsubscribe();
    }

    getCurrencies() {
        this.getCurrenciesSubscrip = this.utilsService.getCurrencies().subscribe(d => {
            this.currencies = d;
        });
    }

    getCost(managementReportId) {
        this.messageService.showLoading();

        this.getCostSubscrip = this.ManagementReportStaffService.getCostDetailStaff(managementReportId).subscribe(response => {
            this.messageService.closeLoading();

            this.model = response.data
            this.months = response.data.monthsHeader; 
            this.employees = response.data.costEmployees;
            this.categoriesEmployees = response.data.costCategoriesEmployees
            this.otherCategories = response.data.otherCategories;
            this.categories = response.data.costCategories;
            this.categoriesRedInfra = response.data.costCategoriesRedInfra;
            this.subCategories = response.data.allSubcategories;
            this.budgetTypes = response.data.budgetTypes;
            this.actualState = response.data.state;

            this.otherCategories = response.data.otherCategories;
            if (this.otherCategories.length > 0) {
                this.otherSelected = this.otherCategories[0];
            }

            this.subCategoriesFiltered = this.subCategories

            // this.selectDefaultColumn(this.dateSelected)
            this.calculateTotalCosts()
            this.sendDataToDetailView();
        },
        () => this.messageService.closeLoading());
    }

    openEditItemModal(item, typeBudget, month) {
        if (month.closed) return;
        this.replicateCosts = false;
        this.monthsToReplicateSelected = null;

        const totalType = `total${typeBudget}`;
        this.subCategoriesData = new Array()

        var today = new Date();
        if (typeBudget == 'projected') {
            if (month.year < today.getFullYear()) {
                this.messageService.showError("onlyCdgCanModify");
                return;
            }
            if (month.year == today.getFullYear() && month.month < today.getMonth() + 1 && !this.isCdg) {
                this.messageService.showError("onlyCdgCanModify");
                return;
            }
        }

        this.itemSelected = item;

        let indexMonth = 0
        if (this.itemSelected.typeName == this.typeEmployee) {
            indexMonth = item.monthsCost.findIndex(cost => cost === month);
        } else {
            indexMonth = item.monthsCategory.findIndex(cost => cost === month);
        }

        this.monthSelected = month;
        this.indexSelected = indexMonth;
        this.editItemMonto.setValidators([Validators.min(0), Validators.max(999999)]);

        this.monthsToReplicate = [];
        this.months.forEach((month2, index) => {
            if((month2.year == this.monthSelected.year && month2.month > this.monthSelected.month) || 
               (month2.year > this.monthSelected.year)){
                this.monthsToReplicate.push({
                    id: index,
                    month: month2.month,
                    year: month2.year,
                    display: month2.display,
                });
            }
        });

        this.modalPercentage = false;
        this.modalOther = false
        this.modalEmployee = false
        this.modalProfile = false;
        this.editItemModal.size = 'modal-sm'
        this.typeBudgetSelected = this.budgetTypes.find(x => x.name.toUpperCase() == typeBudget.toUpperCase())

        switch (this.itemSelected.typeName) {
            case this.typeEmployee:
                this.modalEmployee = true
                this.editItemMonto.setValue(month[typeBudget.toLowerCase()].originalValue)
                // this.editItemAdjustment.setValue(month[typeBudget.toLowerCase()].adjustment)
                this.editItemAdjustment.setValue(null);
                this.editItemAdjustment.setValidators([Validators.min(0), Validators.max(999)]);
                this.editItemModal.show();
                break;

            case this.generalAdjustment:
                this.categorySelected = item
                this.subCategoriesFiltered = this.subCategories.filter(x => x.idCategory == this.categorySelected.id)
                if (this.subCategoriesFiltered.length > 0) {
                    this.subCategorySelected = this.subCategoriesFiltered[0];
                }

                switch (this.typeBudgetSelected.name.toUpperCase()) {
                    case 'BUDGET':
                        if (!this.monthSelected.subcategoriesBudget[0]) {
                            this.addCostByMonth();
                        }
                        else {
                            this.subCategoriesData = this.monthSelected.subcategoriesBudget;
                        }
                        break;
                    case 'PROJECTED':
                        if (!this.monthSelected.subcategoriesProjected[0]) {
                            this.addCostByMonth();
                        }
                        else {
                            this.subCategoriesData = this.monthSelected.subcategoriesProjected
                        }
                        break;
                    case 'PFA1':
                        if (!this.monthSelected.subcategoriesPfa1[0]) {
                            this.addCostByMonth();
                        }
                        else {
                            this.subCategoriesData = this.monthSelected.subcategoriesPfa1;
                        }
                        break;
                    case 'PFA2':
                        if (!this.monthSelected.subcategoriesPfa2[0]) {
                            this.addCostByMonth();
                        }
                        else {
                            this.subCategoriesData = this.monthSelected.subcategoriesPfa2;
                        }
                        break;
                    case 'REAL':
                        if (!this.monthSelected.subcategoriesReal[0]) {
                            this.addCostByMonth();
                        }
                        else {
                            this.subCategoriesData = this.monthSelected.subcategoriesReal;
                        }
                        break;
                }

                this.modalPercentage = true;
                this.editItemMonto.setValue(month[totalType]);
                this.editItemMonto.setValidators([Validators.min(0), Validators.max(999)]);
                this.editItemModal.show();
                break;

            default:
                this.editItemModal.size = 'modal-lg'
                this.modalOther = true
                this.categorySelected = item
                if (typeBudget.toUpperCase() == 'PROJECTED' && (environment.infrastructureCategoryId == this.categorySelected.id || environment.redCategoryId == this.categorySelected.id)) {
                    this.messageService.showWarning("autocompletedData");
                    return;
                }

                this.subCategoriesFiltered = this.subCategories.filter(x => x.idCategory == this.categorySelected.id)
                if (this.subCategoriesFiltered.length > 0) {
                    this.subCategorySelected = this.subCategoriesFiltered[0];
                }

                var subData = this.fillSubcategoriesData()
                if (subData) {
                    this.subCategoriesData = new Array()
                    subData.forEach(subcat => {

                        var cost = {
                            costDetailStaffId: subcat.costDetailStaffId,
                            id: subcat.id,
                            name: subcat.name,
                            description: subcat.description,
                            value: subcat.value,
                            budgetTypeId: subcat.budgetTypeId,
                            deleted: subcat.deleted,
                            currencyId: subcat.currencyId
                        }

                        this.subCategoriesData.push(cost)
                    });

                    setTimeout(() => {
                        $('.input-billing-modal.ng-select .ng-select-container').css('min-height', '28px');
                        $('.input-billing-modal.ng-select .ng-select-container').css('height', '28px');
                    }, 200);
                }
                break;
        }
    }

    fillSubcategoriesData() {

        let isCdg = this.menuService.userIsCdg
        this.readOnly = true

        switch (this.typeBudgetSelected.name.toUpperCase()) {
            case 'BUDGET':
                if (isCdg && this.actualState == 'budget') { this.readOnly = false }
                if (isCdg && this.actualState != 'budget') { this.messageService.showWarning("managementReport.budgetClosed") }
                this.editItemModal.show();
                return this.monthSelected.subcategoriesBudget.filter(sub => sub.deleted == false);
                break;

            case 'PROJECTED':

                if (this.menuService.userIsDirector || this.menuService.userIsManager || this.menuService.isManagementReportDelegate) {
                    this.readOnly = false
                    if (this.model.status == ManagementReportStatus.CdgPending) {
                        this.messageService.showWarning("managementReport.cdgPending")
                        this.readOnly = true
                    }
                }

                this.editItemModal.show();
                return this.monthSelected.subcategoriesProjected.filter(sub => sub.deleted == false);
                break;

            case 'PFA1':
                if (isCdg && this.actualState == 'pfa1') { this.readOnly = false }
                if (isCdg && this.actualState == 'pfa2' || this.actualState == 'real') {
                    this.messageService.showWarning("managementReport.pfa1Closed")
                }
                if (this.actualState == 'budget') {
                    this.messageService.showWarning("managementReport.pfa1NotAvailable")
                    return
                }

                this.editItemModal.show();
                return this.monthSelected.subcategoriesPfa1.filter(sub => sub.deleted == false);
                break;

            case 'PFA2':
                if (isCdg && this.actualState == 'pfa2') { this.readOnly = false }
                if (isCdg && this.actualState == 'real') {
                    this.messageService.showWarning("managementReport.pfa2Closed")
                }
                if (this.actualState == 'budget' || this.actualState == 'pfa1') {
                    this.messageService.showWarning("managementReport.pfa2NotAvailable")
                    return
                }

                this.editItemModal.show();
                return this.monthSelected.subcategoriesPfa2.filter(sub => sub.deleted == false);
                break;

            case 'REAL':
                this.readOnly = true
                this.editItemModal.show();
                return this.monthSelected.subcategoriesReal.filter(sub => sub.deleted == false);
                break;
        }
    }

    addCostByMonth() {
        var cost = {
            costDetailStaffId: 0,
            id: this.subCategorySelected.id,
            name: this.subCategorySelected.name,
            description: "",
            value: 0,
            budgetTypeId: this.typeBudgetSelected.id,
            currencyId: this.currencies[0].id,
            deleted: false
        }

        this.subCategoriesData.push(cost);

        setTimeout(() => {
            $('.input-billing-modal.ng-select .ng-select-container').css('min-height', '28px');
            $('.input-billing-modal.ng-select .ng-select-container').css('height', '28px');
        }, 100);
    }

    updateItem() {
        this.isCdg = this.menuService.userIsCdg
        var hasError = false;

        switch (this.itemSelected.typeName) {

            case this.typeEmployee:
                this.monthSelected[this.typeBudgetSelected.name.toLowerCase()].value = this.editItemMonto.value
                this.monthSelected[this.typeBudgetSelected.name.toLowerCase()].originalValue = this.editItemMonto.value
                if (this.editItemAdjustment.value > 0) {
                    this.monthSelected[this.typeBudgetSelected.name.toLowerCase()].adjustment = this.editItemAdjustment.value
                    this.monthSelected[this.typeBudgetSelected.name.toLowerCase()].value = this.editItemMonto.value + (this.editItemMonto.value * this.editItemAdjustment.value) / 100
                }
                else {
                    this.monthSelected[this.typeBudgetSelected.name.toLowerCase()].adjustment = 0;
                    this.monthSelected[this.typeBudgetSelected.name.toLowerCase()].value = this.editItemMonto.value;
                }

                for (let index = this.indexSelected + 1; index < this.itemSelected.monthsCost.length; index++) {

                    if (this.itemSelected.monthsCost[index].hasAlocation) {
                        this.itemSelected.monthsCost[index][this.typeBudgetSelected.name.toLowerCase()].value = this.monthSelected[this.typeBudgetSelected.name.toLowerCase()].value;
                        this.itemSelected.monthsCost[index][this.typeBudgetSelected.name.toLowerCase()].originalValue = this.monthSelected[this.typeBudgetSelected.name.toLowerCase()].value;
                    }
                    else {
                        this.itemSelected.monthsCost[index][this.typeBudgetSelected.name.toLowerCase()].value = 0
                        this.itemSelected.monthsCost[index][this.typeBudgetSelected.name.toLowerCase()].originalValue = 0
                        this.itemSelected.monthsCost[index][this.typeBudgetSelected.name.toLowerCase()].adjustment = null
                    }
                }

                //Actualiza el sueldo
                this.salaryPlusIncrease(this.itemSelected, this.indexSelected, true);
                break;

            case this.generalAdjustment:

                this.subCategoriesData[0].value = this.editItemMonto.value
                this.subCategoriesData[0].originalValue = this.editItemMonto.value

                switch (this.typeBudgetSelected.name.toUpperCase()) {
                    case 'BUDGET':
                        this.monthSelected.subcategoriesBudget = this.subCategoriesData
                        this.monthSelected.totalBudget = this.editItemMonto.value
                        break;
                    case 'PROJECTED':
                        this.monthSelected.subcategoriesProjected = this.subCategoriesData
                        this.monthSelected.totalProjected = this.editItemMonto.value
                        break;
                    case 'PFA1':
                        this.monthSelected.subcategoriesPfa1 = this.subCategoriesData
                        this.monthSelected.totalPfa1 = this.editItemMonto.value
                        break;
                    case 'PFA2':
                        this.monthSelected.subcategoriesPfa2 = this.subCategoriesData
                        this.monthSelected.totalPfa2 = this.editItemMonto.value
                        break;
                    case 'REAL':
                        this.monthSelected.subcategoriesReal = this.subCategoriesData
                        this.monthSelected.totalReal = this.editItemMonto.value
                        break;
                }
                // this.monthSelected.subcategoriesBudget[0].value = this.editItemMonto.value
                // this.monthSelected.subcategoriesBudget[0].originalValue = this.editItemMonto.value
                // this.monthSelected.totalBudget = this.editItemMonto.value

                this.modalPercentage = true;
                this.employees.forEach(employee => {
                    this.salaryPlusIncrease(employee, this.indexSelected, false);
                })
                break

            default:

                this.subCategoriesData.forEach(cost => {
                    if (!cost.currencyId || cost.currencyId == 0) {
                        hasError = true;
                        this.messageService.showErrorByFolder('managementReport/currencyExchange', 'currencyRequired');
                    }
                });

                if (hasError) {
                    this.editItemModal.resetButtons();
                    return;
                };

                var currencyMonth = this.monthExchanges.find(x => x.month == this.monthSelected.month && x.year == this.monthSelected.year);

                switch (this.typeBudgetSelected.name.toUpperCase()) {
                    case 'BUDGET':
                        this.monthSelected.subcategoriesBudget = this.subCategoriesData;
                        this.monthSelected.totalBudget = 0;
                        this.monthSelected.subcategoriesBudget.forEach(cost => {
                            this.monthSelected.totalBudget += this.setExchangeValue(currencyMonth, cost);

                            if (this.isCdg) {
                                if (this.categorySelected.name == "Infraestructura" && cost.name == "Infraestructura") {
                                    this.setProjectedInfrastructureOrRed(cost, currencyMonth, "Infraestructura", this.monthSelected);
                                }

                                if (this.categorySelected.name == "Red" && cost.name == "Red") {
                                    this.setProjectedInfrastructureOrRed(cost, currencyMonth, "Red", this.monthSelected);
                                }
                            }
                        });

                        this.replicateCategories(currencyMonth, 'totalBudget', 'subcategoriesBudget');

                        if (this.monthSelected.totalBudget == 0 && (this.categorySelected.name == "Infraestructura" || this.categorySelected.name == "Red")) {
                            this.deleteRedInfra()
                         }
                        break;
                    case 'PROJECTED':
                        this.monthSelected.subcategoriesProjected = this.subCategoriesData
                        this.monthSelected.totalProjected = 0
                        this.monthSelected.subcategoriesProjected.forEach(cost => {
                            this.monthSelected.totalProjected += this.setExchangeValue(currencyMonth, cost);
                        })
                        break;
                    case 'PFA1':
                        this.monthSelected.subcategoriesPfa1 = this.subCategoriesData
                        this.monthSelected.totalPfa1 = 0
                        this.monthSelected.subcategoriesPfa1.forEach(cost => {
                            this.monthSelected.totalPfa1 += this.setExchangeValue(currencyMonth, cost);

                            if (this.isCdg) {
                                if (this.categorySelected.name == "Infraestructura" && cost.name == "Infraestructura") {
                                    this.setProjectedInfrastructureOrRed(cost, currencyMonth, "Infraestructura", this.monthSelected);
                                }

                                if (this.categorySelected.name == "Red" && cost.name == "Red") {
                                    this.setProjectedInfrastructureOrRed(cost, currencyMonth, "Red", this.monthSelected);
                                }
                            }

                            this.replicateCategories(currencyMonth, 'totalPfa1', 'subcategoriesPfa1');
                        });
                        
                        if (this.monthSelected.totalPfa1 == 0 && (this.categorySelected.name == "Infraestructura" || this.categorySelected.name == "Red")) {
                           this.deleteRedInfra()
                        }
                        break;
                    case 'PFA2':
                        this.monthSelected.subcategoriesPfa2 = this.subCategoriesData
                        this.monthSelected.totalPfa2 = 0
                        this.monthSelected.subcategoriesPfa2.forEach(cost => {
                            this.monthSelected.totalPfa2 += this.setExchangeValue(currencyMonth, cost);

                            if (this.isCdg) {
                                if (this.categorySelected.name == "Infraestructura" && cost.name == "Infraestructura") {
                                    this.setProjectedInfrastructureOrRed(cost, currencyMonth, "Infraestructura", this.monthSelected);
                                }

                                if (this.categorySelected.name == "Red" && cost.name == "Red") {
                                    this.setProjectedInfrastructureOrRed(cost, currencyMonth, "Red", this.monthSelected);
                                }
                            }

                            this.replicateCategories(currencyMonth, 'totalPfa2', 'subcategoriesPfa2');
                        });
                        if (this.monthSelected.totalPfa2 == 0 && (this.categorySelected.name == "Infraestructura" || this.categorySelected.name == "Red")) {
                            this.deleteRedInfra()
                         }
                        break;
                    case 'REAL':
                        this.monthSelected.subcategoriesReal = this.subCategoriesData
                        this.monthSelected.totalReal = 0
                        this.monthSelected.subcategoriesReal.forEach(cost => {
                            this.monthSelected.totalReal += this.setExchangeValue(currencyMonth, cost);

                            if (this.isCdg) {
                                if (this.categorySelected.name == "Infraestructura" && cost.name == "Infraestructura") {
                                    this.setProjectedInfrastructureOrRed(cost, currencyMonth, "Infraestructura", this.monthSelected);
                                }

                                if (this.categorySelected.name == "Red" && cost.name == "Red") {
                                    this.setProjectedInfrastructureOrRed(cost, currencyMonth, "Red", this.monthSelected);
                                }
                            }

                            this.replicateCategories(currencyMonth, 'totalReal', 'subcategoriesReal');
                        })
                        break;
                }
        }
        this.calculateTotalCosts()
        this.sendDataToDetailView();
        this.editItemModal.hide()
    }

    private replicateCategories(currencyMonth: any, total, subcat) {
        if (this.replicateCosts && this.monthsToReplicateSelected) {

            this.monthsToReplicateSelected.forEach(monthReplicateId => {

                var monthReplicateSelected = this.monthsToReplicate.find(x => x.id == monthReplicateId);
                if (monthReplicateSelected) {
                    var monthCategory = this.itemSelected.monthsCategory.find(x => x.month == monthReplicateSelected.month && x.year == monthReplicateSelected.year);

                    if (monthCategory.display != this.monthSelected.display) {
                        monthCategory[total] = 0;

                        if (monthCategory[subcat].length > 0) {
                            monthCategory[subcat].forEach(x => x.deleted = true);
                        }

                        this.subCategoriesData.forEach(x => {
                            var exist = monthCategory[subcat].find(s => s.budgetTypeId == x.budgetTypeId && s.name == x.name && x.description == s.description);
                            if (exist) {
                                exist.deleted = x.deleted;
                                exist.value = x.value;
                            }
                            else {
                                let newItem = {
                                    budgetTypeId: x.budgetTypeId,
                                    costDetailStaffId: 0,
                                    currencyId: x.currencyId,
                                    deleted: x.deleted,
                                    description: x.description,
                                    id: x.id,
                                    name: x.name,
                                    originalValue: x.originalValue,
                                    value: x.value,
                                    idCategory: 0,
                                };

                                monthCategory[subcat].push(newItem);
                            }

                            if (this.isCdg) {
                                if (this.categorySelected.name == "Infraestructura" && x.name == "Infraestructura") {
                                    this.setProjectedInfrastructureOrRed(x, currencyMonth, "Infraestructura", monthCategory);
                                }
    
                                if (this.categorySelected.name == "Red" && x.name == "Red") {
                                    this.setProjectedInfrastructureOrRed(x, currencyMonth, "Red", monthCategory);
                                }
                            }
                        });

                        monthCategory[subcat].forEach(x => {
                            if(!x.deleted){
                                monthCategory[total] += this.setExchangeValue(currencyMonth, x);
                            }
                        });
                    }
                }
            });
        }
    }

    setProjectedInfrastructureOrRed(cost, currencyMonth, type, monthCategory) {
        var infraProyected = monthCategory.subcategoriesProjected.find(x => x.name == type);

        if (infraProyected) {
            monthCategory.totalProjected -= infraProyected.originalValue;
            infraProyected.value = cost.value;
            monthCategory.totalProjected += this.setExchangeValue(currencyMonth, infraProyected);
        }
        else {
            var projType = this.budgetTypes.find(x => x.name.toUpperCase() == "PROJECTED");

            var costToAdd = {
                costDetailStaffId: cost.costDetailStaffId,
                id: cost.id,
                name: cost.name,
                description: cost.description,
                value: cost.originalValue,
                budgetTypeId: projType.id,
                deleted: false,
                currencyId: cost.currencyId
            };

            monthCategory.totalProjected += this.setExchangeValue(currencyMonth, costToAdd);
            monthCategory.subcategoriesProjected.push(costToAdd);
        }
    }

    setExchangeValue(currencyMonth, cost) {
        var total = 0;

        cost.originalValue = cost.value;

        if (currencyMonth) {
            var currencyExchange = currencyMonth.items.find(x => x.currencyId == cost.currencyId);

            if (currencyExchange) {
                cost.value *= currencyExchange.exchange;
                total += cost.value;
            }
            else {
                total += cost.value
            }
        }
        else {
            total += cost.value
        }

        return total;
    }

    calculateTotalCosts() {

        this.months.forEach(month => {
            let index = this.months.findIndex(cost => cost.monthYear === month.monthYear);
            let monthTotal = this.months.find(m => m.monthYear === month.monthYear)

            var totalCostBugdet = 0;
            var totalCostProjected = 0
            var totalCostPfa1 = 0;
            var totalCostPfa2 = 0;
            var totalCostReal = 0;

            var totalInfraRedBugdet = 0;
            var totalInfraRedProjected = 0
            var totalInfraRedPfa1 = 0;
            var totalInfraRedPfa2 = 0;
            var totalInfraRedReal = 0;

            var totalSalaryBudget = 0
            var totalSalaryProjected = 0
            var totalSalaryPfa1 = 0;
            var totalSalaryPfa2 = 0;
            var totalSalaryReal = 0;

            var charges = 0;
            var chargesPfa1 = 0;
            var chargesPfa2 = 0;

            this.categories.forEach(category => {
                if (category.name != this.generalAdjustment) {
                    if (category.monthsCategory[index].totalBudget) {
                        totalCostBugdet += category.monthsCategory[index].totalBudget;
                    }
                    if (category.monthsCategory[index].totalProjected) {
                        totalCostProjected += category.monthsCategory[index].totalProjected;
                    }
                    if (category.monthsCategory[index].totalPfa1) {
                        totalCostPfa1 += category.monthsCategory[index].totalPfa1;
                    }
                    if (category.monthsCategory[index].totalPfa2) {
                        totalCostPfa2 += category.monthsCategory[index].totalPfa2;
                    }
                    if (category.monthsCategory[index].totalReal) {
                        totalCostReal += category.monthsCategory[index].totalReal;
                    }
                }

                if(category.typeName == this.typeProfile){
                    if (category.monthsCategory[index].totalBudget) {
                        totalSalaryBudget += category.monthsCategory[index].totalBudget;
                    }
                    if (category.monthsCategory[index].totalProjected) {
                        totalSalaryProjected += category.monthsCategory[index].totalProjected;
                    }
                    if (category.monthsCategory[index].totalPfa1) {
                        totalSalaryPfa1 += category.monthsCategory[index].totalPfa1;
                    }
                    if (category.monthsCategory[index].totalPfa2) {
                        totalSalaryPfa2 += category.monthsCategory[index].totalPfa2;
                    }
                    if (category.monthsCategory[index].totalReal) {
                        totalSalaryReal += category.monthsCategory[index].totalReal;
                    }
                }
            });

            //Sumo el totol de los sueldos
            this.employees.forEach(employee => {
                if (employee.monthsCost[index].budget.value) {
                    totalCostBugdet += employee.monthsCost[index].budget.value;
                    totalSalaryBudget += employee.monthsCost[index].budget.value;
                }

                if (employee.monthsCost[index].projected.value) {
                    totalCostProjected += employee.monthsCost[index].projected.value;
                    totalSalaryProjected += employee.monthsCost[index].projected.value;
                }

                if (employee.monthsCost[index].pfa1.value) {
                    totalCostPfa1 += employee.monthsCost[index].pfa1.value;
                    totalSalaryPfa1 += employee.monthsCost[index].pfa1.value;
                    chargesPfa1 += employee.monthsCost[index].pfa1.charges;
                }

                if (employee.monthsCost[index].pfa2.value) {
                    totalCostPfa2 += employee.monthsCost[index].pfa2.value;
                    totalSalaryPfa2 += employee.monthsCost[index].pfa2.value;
                    chargesPfa2 += employee.monthsCost[index].pfa2.charges;
                }

                if (employee.monthsCost[index].real.value) {
                    totalCostReal += employee.monthsCost[index].real.value;
                    totalSalaryReal += employee.monthsCost[index].real.value;
                    charges += employee.monthsCost[index].real.charges;
                }

                //asignacion += employee.monthsCost[index].allocationPercentage
            })

            //Sumo los gastos de los empleados
            this.categoriesEmployees.forEach(category => {
                if (category.monthsCategory[index].totalBudget) {
                    totalCostBugdet += category.monthsCategory[index].totalBudget;
                    totalSalaryBudget += category.monthsCategory[index].totalBudget;
                }
                if (category.monthsCategory[index].totalProjected) {
                    totalCostProjected += category.monthsCategory[index].totalProjected;
                    totalSalaryProjected += category.monthsCategory[index].totalProjected;
                }
                if (category.monthsCategory[index].totalPfa1) {
                    totalCostPfa1 += category.monthsCategory[index].totalPfa1;
                    totalSalaryPfa1 += category.monthsCategory[index].totalPfa1;
                }
                if (category.monthsCategory[index].totalPfa2) {
                    totalCostPfa2 += category.monthsCategory[index].totalPfa2;
                    totalSalaryPfa2 += category.monthsCategory[index].totalPfa2;
                }
                if (category.monthsCategory[index].totalReal) {
                    totalCostReal += category.monthsCategory[index].totalReal;
                    totalSalaryReal += category.monthsCategory[index].totalReal;
                }
            })

            //sumo el total de Infraestructura y Red
            this.categoriesRedInfra.forEach(category => {
                if (category.name != this.generalAdjustment) {
                    if (category.monthsCategory[index].totalBudget) {
                        totalCostBugdet += category.monthsCategory[index].totalBudget;
                        totalInfraRedBugdet += category.monthsCategory[index].totalBudget;
                    }
                    if (category.monthsCategory[index].totalProjected) {
                        totalCostProjected += category.monthsCategory[index].totalProjected;
                        totalInfraRedProjected += category.monthsCategory[index].totalProjected;
                    }
                    if (category.monthsCategory[index].totalPfa1) {
                        totalCostPfa1 += category.monthsCategory[index].totalPfa1;
                        totalInfraRedPfa1 += category.monthsCategory[index].totalPfa1;
                    }
                    if (category.monthsCategory[index].totalPfa2) {
                        totalCostPfa2 += category.monthsCategory[index].totalPfa2;
                        totalInfraRedPfa2 += category.monthsCategory[index].totalPfa2;
                    }
                    if (category.monthsCategory[index].totalReal) {
                        totalCostReal += category.monthsCategory[index].totalReal;
                        totalInfraRedReal += category.monthsCategory[index].totalReal;
                    }
                }
            });


            if(chargesPfa1 == 0){
                monthTotal.pfa1.totalCost = totalCostPfa1 + (totalSalaryPfa1 * 0.85);
                month.pfa1.totalLoads = (totalSalaryPfa1 * 0.85);
            }
            else{
                monthTotal.pfa1.totalCost = totalCostPfa1 + chargesPfa1;
                month.pfa1.totalLoads = chargesPfa1;
            }
            
            if(chargesPfa2 == 0){
                monthTotal.pfa2.totalCost = totalCostPfa2 + (totalSalaryPfa2 * 0.85);
                month.pfa2.totalLoads = (totalSalaryPfa2 * 0.85);
            }
            else{
                monthTotal.pfa2.totalCost = totalCostPfa2 + chargesPfa2;
                month.pfa2.totalLoads = chargesPfa2;
            }

            monthTotal.budget.totalCost = totalCostBugdet + (totalSalaryBudget * 0.85);
            monthTotal.projected.totalCost = totalCostProjected + (totalSalaryProjected * 0.85);
            monthTotal.real.totalCost = totalCostReal + charges;

            month.budget.totalSalary = totalSalaryBudget
            month.projected.totalSalary = totalSalaryProjected
            month.pfa1.totalSalary = totalSalaryPfa1
            month.pfa2.totalSalary = totalSalaryPfa2
            month.real.totalSalary = totalSalaryReal

            month.budget.totalLoads = (totalSalaryBudget * 0.85)
            month.projected.totalLoads = (totalSalaryProjected * 0.85)
         
            month.real.totalLoads = charges

            monthTotal.budget.subTotalCost = monthTotal.budget.totalCost - totalInfraRedBugdet;
            monthTotal.projected.subTotalCost = monthTotal.projected.totalCost - totalInfraRedProjected;
            monthTotal.pfa1.subTotalCost = monthTotal.pfa1.totalCost - totalInfraRedPfa1;
            monthTotal.pfa2.subTotalCost = monthTotal.pfa2.totalCost - totalInfraRedPfa2;
            monthTotal.real.subTotalCost = monthTotal.real.totalCost - totalInfraRedReal;
        })
    }

    fillSubCategories() {

        let subCategoriesFiltered = this.subCategories.filter(x => x.idCategory == this.categorySelected.id)
        let index = this.months.findIndex(cost => cost.monthYear === this.monthSelected.monthYear);

        subCategoriesFiltered.forEach(sub => {

            //Si la subcategoria existe no agregarla
            if (this.categorySelected.months[index].subCategories.findIndex(x => x.idsubCategory == sub.id) == -1) {
                var cost = {
                    id: 0,
                    CostDetailId: 0,
                    idCategory: this.categorySelected.id,
                    nameCategory: this.categorySelected.name,
                    idsubCategory: sub.id,
                    nameSubCategory: sub.name,
                    monthYear: this.monthSelected.monthYear,
                    description: "",
                    value: 0,
                    typeBudget: this.typeBudgetSelected
                }

                this.monthSelected.subCategories.push(cost)
            }
        })
    }

    sendDataToDetailView() {
        if (this.getData.observers.length > 0) {
            this.getData.emit({
                months: this.months,
                categories: this.categories,
                actualState: this.actualState
            });
        }
    }

    save(endState = false) {
        this.messageService.showLoading();

        this.model.closeState = endState

        this.updateCostSubscrip = this.ManagementReportStaffService.PostCostDetailStaff(this.model).subscribe(() => {
            this.messageService.closeLoading();

            this.detailStaffComponent.getDetail()
            this.getCost(this.managementReportId);

            setTimeout(() => {
                this.sendDataToDetailView();
            }, 1500);
        },
            () => {
                this.messageService.closeLoading();
            });
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

    getId(date: Date) {
        var item = this.months.find(x => x.month == (date.getMonth() + 1) && x.year == date.getFullYear());

        if (item) {
            return item.costDetailId;
        }

        return 0;
    }

    isClosed(date: Date) {
        var item = this.months.find(x => x.month == (date.getMonth() + 1) && x.year == date.getFullYear());

        if (item) {
            return item.closed;
        }

        return false;
    }

    isFirstColumn(column, index) {
        var isFirst = false

        if (index == 0) {
            switch (column) {
                case "budget":
                    if (this.showColumn.budget) {
                        isFirst = true
                    }
                    break;
                case "projected":
                    if (!this.showColumn.budget) {
                        isFirst = true
                    }
                    break;
                case "pfa1":
                    if (!this.showColumn.budget && !this.showColumn.projected) {
                        isFirst = true
                    }
                    break;
                case "pfa2":
                    if (!this.showColumn.budget && !this.showColumn.projected && !this.showColumn.pfa1) {
                        isFirst = true
                    }
                    break;
                case "real":
                    if (!this.showColumn.budget && !this.showColumn.projected && !this.showColumn.pfa1 && !this.showColumn.pfa2) {
                        isFirst = true
                    }
                    break;

                default:
                    break;
            }
        }

        return isFirst
    }

    toggleColumn(column) {

        this.showColumn[column] = !this.showColumn[column]
        if (!this.showColumn.budget && !this.showColumn.pfa1 && !this.showColumn.pfa2
            && !this.showColumn.real && !this.showColumn.projected) {
            this.showColumn[column] = true
        }
    }

    generatePfa(typePfa, callback) {
        this.messageService.showLoading()

        var model = {
            IdManagementReport: this.managementReportId,
            TypePFA: typePfa
        }

        this.generatePFASuscrip = this.ManagementReportStaffService.PostGeneratePfa(model).subscribe(
            () => {
                this.messageService.closeLoading()
                this.getCost(this.managementReportId);

                if(callback) callback();
            },
            () => {
                this.messageService.closeLoading()
            }
        )
    }

    setAllCosts(month, total, type) {
        var exchanges = [];
        var currencies = [];

        this.currencies.forEach(currency => {
            currencies.push({ value: 0, valuePesos: 0, currencyName: currency.text, id: currency.id });
        });

        var currencyMonth = this.monthExchanges.find(x => x.month == month.month && x.year == month.year);

        if (currencyMonth) {
            currencyMonth.items.forEach(item => {
                exchanges.push({ currencyName: item.currencyDesc, exchange: item.exchange });
            });
        }

        this.categories.forEach(category => {
            var monthCategory = category.monthsCategory.find(x => x.month == month.month && x.year == month.year);

            if (monthCategory) {
                var subCategories = this.getSubcategories(monthCategory, type);

                subCategories.forEach(subcategory => {
                    var curr = currencies.find(x => x.id == subcategory.currencyId);

                    if (curr) {
                        curr.value += subcategory.originalValue;
                        curr.valuePesos += subcategory.value;
                    }
                });
            }
        });

        this.totalCostsExchanges = {
            exchanges: exchanges,
            currencies: currencies,
            total: total
        };

        this.totalCostsExchangesModal.show();
    }

    getSubcategories(monthCategory, type) {
        switch (type) {
            case 'budget': return monthCategory.subcategoriesBudget;
            case 'projected': return monthCategory.subcategoriesProjected;
            case 'pfa1': return monthCategory.subcategoriesPfa1;
            case 'pfa2': return monthCategory.subcategoriesPfa2;
            case 'real': return monthCategory.subcategoriesReal;
        }
    }


    salaryPlusIncrease(employee, pIndex, isSalaryEmployee) {
        //Verifico que exista la fila de ajustes
        var AjusteMensual = this.categories.find(r => r.name == this.generalAdjustment);
        if (AjusteMensual) {
            //Si existe, Recorro todos los meses
            let newSalary = 0;
            //El nuevo salario lo seteo como el primer salario
            if (isSalaryEmployee == true) {
                newSalary = employee.monthsCost[pIndex][this.typeBudgetSelected.name.toLowerCase()].value
                pIndex += 1
            }

            for (let index = pIndex; index < employee.monthsCost.length; index++) {

                //Verifico si tiene aumento en alguno
                if (AjusteMensual.monthsCategory[index].totalBudget > 0) {
                    newSalary = employee.monthsCost[index][this.typeBudgetSelected.name.toLowerCase()].originalValue + (employee.monthsCost[index][this.typeBudgetSelected.name.toLowerCase()].originalValue * AjusteMensual.monthsCategory[index].totalBudget / 100);
                }
                else {
                    //Si el aumento es cero el salario nuevo es igual al salario anterior
                    if (AjusteMensual.monthsCategory[index].totalBudget == 0) {
                        newSalary = employee.monthsCost[index][this.typeBudgetSelected.name.toLowerCase()].originalValue
                    }
                }

                if (employee.monthsCost[index][this.typeBudgetSelected.name.toLowerCase()].value > 0) {
                    employee.monthsCost[index][this.typeBudgetSelected.name.toLowerCase()].value = newSalary;
                    employee.monthsCost[index][this.typeBudgetSelected.name.toLowerCase()].adjustment = AjusteMensual.monthsCategory[index].totalBudget

                    for (let newindex = index + 1; newindex < employee.monthsCost.length; newindex++) {

                        if (employee.monthsCost[newindex][this.typeBudgetSelected.name.toLowerCase()].value > 0) {
                            employee.monthsCost[newindex][this.typeBudgetSelected.name.toLowerCase()].value = newSalary;
                            employee.monthsCost[newindex][this.typeBudgetSelected.name.toLowerCase()].originalValue = newSalary;
                        }
                        else {
                            employee.monthsCost[newindex].value = 0
                        }
                    }
                }
            }
        }
    }

    onEnter(mathBox, value: string) {
        var result;

        try {
            if (value == null || value == "") {
                result = 0
            }
            else {
                result = evaluate(value)
            }
        }
        catch (error) {
            result = 0

            mathBox.value = result
        }

        this.editItemMonto.setValue(result)
    }

    addOtherCost() {
        var category = this.otherCategories.find(r => r.id == this.otherSelected.id)

        var pos = this.otherCategories.findIndex(r => r.id == this.otherSelected.id);
        this.otherCategories.splice(pos, 1)

        var auxCategories = this.otherCategories;
        this.otherCategories = []

        auxCategories.forEach(category => {
            this.otherCategories.push(category)
        });

        if (this.otherCategories.length > 0) {
            this.otherSelected = this.otherCategories[0];
        }

        if (category.name == "Infraestructura" || category.name == "Red") {
            this.categoriesRedInfra.push(category)
        }
        else {
            this.categories.push(category)
        }

    }

    canDeleteCategory(item) {
        var canDelete = true;
        if (item.default == true) {
            canDelete = false
            return canDelete
        }

        item.monthsCategory.forEach(month => {
            if (month.totalBudget > 0 || month.totalProjected > 0
                || month.totalPfa1 > 0 || month.totalPfa2 > 0 || month.totalReal > 0) {
                canDelete = false
                return canDelete
            }
        });

        return canDelete
    }

    deleteCategory(item, index) {

        if (item.name == "Infraestructura" || item.name == "Red") {
            this.categoriesRedInfra.splice(index, 1)
        }
        else {
            this.categories.splice(index, 1)
        }

        this.otherCategories.push(item)

        this.save()

        this.otherCategories.sort(function (a, b) {
            if (a.display > b.display) {
                return 1;
            }
            if (a.display < b.display) {
                return -1;
            }
            return 0;
        });
    }

    setSinceDate(date: Date) {
        this.sinceMonth = new Date(date.getFullYear(), date.getMonth() - 2, 1)
    }

    deleteRedInfra(){
        this.monthSelected.totalProjected = 0
        this.monthSelected.subcategoriesProjected.forEach(projected => {
            projected.value = 0
            projected.originalValue = 0
            if (projected.costDetailStaffId == 0) {
                projected.deleted = true
            }
        })
    }

    createWorksheet(workbook){
        let worksheet = workbook.addWorksheet('Presupuesto');

        this.buildHeader(worksheet);
        this.buildResources(worksheet);
        this.buildSalaryAndCharges(worksheet);
        this.buildCategories(worksheet);

        var columnCount = worksheet.columnCount;
        var columnIndex = 1;

        while(columnIndex <= columnCount){
            var column = worksheet.getColumn(columnIndex);
            column.eachCell(cell => {
                this.drawBorder(cell, 'right');
            });

            columnIndex+=4;
        }

        var row1 = worksheet.getRow(1);
        var row2 = worksheet.getRow(2);
        var row3 = worksheet.getRow(2);

        row1.eachCell(cell => {
            this.drawBorder(cell, 'bottom');
        });

        row2.eachCell(cell => {
            this.drawBorder(cell, 'bottom');
        });

        row3.eachCell(cell => {
            this.drawBorder(cell, 'bottom');
        });
        
        var lastRow = worksheet.getRow(worksheet.rowCount);

        lastRow.eachCell(cell => {
            this.drawBorder(cell, 'bottom');
        });
    }

    private drawBorder(cell, position) {
        const borderBlack = "FF000000";

        if (cell.border) {
            if (cell.border[position]) {
                cell.border[position].style = 'thin';
                cell.border[position].color.argb = borderBlack;
            }
            else {
                cell.border[position] = { style: 'thin', color: { argb: borderBlack } };
            }
        }
        else {
            cell.border = {};
            cell.border[`${position}`] = {};
            cell.border[position] = { style: 'thin', color: { argb: borderBlack } };
        }
    }

    private buildResources(worksheet) {
        worksheet.addRow(["Recursos"]);

        this.employees.forEach(employee => {
            var resource = [employee.display];

            employee.monthsCost.forEach(monthCost => {
                resource.push(monthCost.budget.value || 0);
                resource.push(monthCost.projected.value || 0);
                resource.push(monthCost.pfa1.value || 0);
                resource.push(monthCost.pfa2.value || 0);
                // resource.push(monthCost.real.value || 0);
            });

            worksheet.addRow(resource);
        });

        if(this.employees.length > 0){
            var lastRow = worksheet.getRow(worksheet.rowCount);

            lastRow.eachCell(cell => {
                this.drawBorder(cell, 'bottom');
            });
        }

        this.categoriesEmployees.forEach(fundedResource => {
            var item = [fundedResource.name];

            fundedResource.monthsCategory.forEach(monthCost => {
                item.push(monthCost.totalBudget || 0);
                item.push(monthCost.totalProjected || 0);
                item.push(monthCost.totalPfa1 || 0);
                item.push(monthCost.totalPfa2 || 0);
                // item.push(monthCost.totalReal || 0);
            });

            worksheet.addRow(item);
        });

        if(this.categoriesEmployees.length > 0){
            var lastRow = worksheet.getRow(worksheet.rowCount);

            lastRow.eachCell(cell => {
                this.drawBorder(cell, 'bottom');
            });
        }
    }

    private buildCategories(worksheet) {
        this.categories.forEach(category => {
            var item = [category.name];
            category.monthsCategory.forEach(monthCost => {
                item.push(monthCost.totalBudget || 0);
                item.push(monthCost.totalProjected || 0);
                item.push(monthCost.totalPfa1 || 0);
                item.push(monthCost.totalPfa2 || 0);
                // item.push(monthCost.totalReal || 0);
            });
            worksheet.addRow(item);
        });

        var totalSalary = ["Sub-total Gastos"];

        this.months.forEach(month => {
            totalSalary.push(month.budget.subTotalCost);
            totalSalary.push(month.projected.subTotalCost);
            totalSalary.push(month.pfa1.subTotalCost);
            totalSalary.push(month.pfa2.subTotalCost);
            // totalSalary.push(month.real.subTotalCost);
        });

        worksheet.addRow(totalSalary);

        this.categoriesRedInfra.forEach(category => {
            var item = [category.name];
            category.monthsCategory.forEach(monthCost => {
                item.push(monthCost.totalBudget || 0);
                item.push(monthCost.totalProjected || 0);
                item.push(monthCost.totalPfa1 || 0);
                item.push(monthCost.totalPfa2 || 0);
                // item.push(monthCost.totalReal || 0);
            });
            worksheet.addRow(item);
        });
    }

    private buildSalaryAndCharges(worksheet) {
        var totalSalary = ["Total Sueldo"];
        var totalLoads = ["Cargas (0.85 del sueldo)"];

        this.months.forEach(month => {
            totalSalary.push(month.budget.totalSalary);
            totalSalary.push(month.projected.totalSalary);
            totalSalary.push(month.pfa1.totalSalary);
            totalSalary.push(month.pfa2.totalSalary);
            // totalSalary.push(month.real.totalSalary);

            totalLoads.push(month.budget.totalLoads);
            totalLoads.push(month.projected.totalLoads);
            totalLoads.push(month.pfa1.totalLoads);
            totalLoads.push(month.pfa2.totalLoads);
            // totalLoads.push(month.real.totalLoads);
        });
        worksheet.addRow(totalSalary);
        worksheet.addRow(totalLoads);

        var lastRow = worksheet.getRow(worksheet.rowCount);

        lastRow.eachCell(cell => {
            this.drawBorder(cell, 'bottom');
        });
    }

    private buildHeader(worksheet) {
        var columns = [];
        var monthItem = { header: "Meses", width: 50 };
        columns.push(monthItem);

        var subHeader = ["Tipo"];
        var totalCosts = ["Total Gastos"];

        this.months.forEach(month => {
            columns.push({ header: month.display, width: 15, style: { numFmt: '#,##0.00' } });
            columns.push({ header: "", width: 15, style: { numFmt: '#,##0.00' } });
            columns.push({ header: "", width: 15, style: { numFmt: '#,##0.00' } });
            columns.push({ header: "", width: 15, style: { numFmt: '#,##0.00' } });
            // columns.push({ header: "", width: 15, style: { numFmt: '#,##0.00' } });

            subHeader.push("BUDGET");
            subHeader.push("PROYECTADO");
            subHeader.push("PFA1");
            subHeader.push("PFA2");
            // subHeader.push("REAL");

            totalCosts.push(month.budget.totalCost || 0);
            totalCosts.push(month.projected.totalCost || 0);
            totalCosts.push(month.pfa1.totalCost || 0);
            totalCosts.push(month.pfa2.totalCost || 0);
            // totalCosts.push(month.real.totalCost || 0);
        });

        worksheet.columns = columns;
        worksheet.addRow(subHeader);
        worksheet.addRow(totalCosts);

        var count = columns.length - 1;
        for (var i = 2; i < count; i += 4) {
            var row = worksheet.getRow(1);
            var firstCell = row.getCell(i);
            var lastCell = row.getCell(i + 3);

            worksheet.mergeCells(`${firstCell.address}:${lastCell.address}`);
            firstCell.alignment = { horizontal: 'center' };

            for (var j = i; j <= (i + 3); j++) {
                var row2 = worksheet.getRow(2);
                var cell = row2.getCell(j);
                cell.alignment = { horizontal: 'center' };
            }
        }
    }
}