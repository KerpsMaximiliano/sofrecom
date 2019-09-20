import { Component, OnDestroy, OnInit, ViewChild, Output, EventEmitter } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { ManagementReportService } from "app/services/management-report/management-report.service";
import { Subscription } from "rxjs";
import { MenuService } from "app/services/admin/menu.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { MessageService } from "app/services/common/message.service";
import { UtilsService } from "app/services/common/utils.service"
import { FormControl, Validators } from "@angular/forms";
import { EmployeeService } from "app/services/allocation-management/employee.service"
import { evaluate } from 'mathjs/number'

@Component({
    selector: 'cost-detail',
    templateUrl: './cost-detail.component.html',
    styleUrls: ['./cost-detail.component.scss']
})
export class CostDetailComponent implements OnInit, OnDestroy {

    paramsSubscrip: Subscription;
    getCostSubscrip: Subscription;
    updateCostSubscrip: Subscription;
    updateMonthSubscrip: Subscription;
    getProfileSuscrip: Subscription
    getEmployeeSubscrip: Subscription;
    getOtherByMonthSuscrip: Subscription;
    deleteProfileSubscrip: Subscription;
    deleteOthersByMonthSubscrip: Subscription;
    getCurrenciesSubscrip: Subscription;

    //Propiedades
    serviceId: string;
    months: any[] = new Array();
    employees: any[] = new Array();
    employeesOriginal: any[] = new Array();
    fundedResources: any[] = new Array();
    fundedResourcesEmployees: any[] = new Array();
    otherResources: any[] = new Array();
    costProfiles: any[] = new Array()
    monthSelected: any = { value: 0, display: '' };
    itemSelected: any;
    indexSelected: number = 0;
    model: any;

    modalPercentage: boolean = false;
    modalEmployee: boolean = false;
    modalOther: boolean = false;
    modalProfile: boolean = false;

    editItemMonto = new FormControl();
    editItemAdjustment = new FormControl();
    canEdit: boolean = false;
    profiles: any[] = new Array();
    profileId: number;
    otherResourceId: number;
    users: any[] = new Array();
    userId: number;
    showUsers: boolean = false;
    showProfiles: boolean = false;
    readOnly: boolean = false;
    managementReportId: number;

    otherSelected: any
    userSelected: any
    profileSelected: any
    othersByMonth: any[] = new Array()
    today: Date = new Date()
    fromMonth: Date = new Date()
    employeesHide: boolean = true;
    showColumn = {
        real: true
    }

    intAux: number = 0
    categorySelected: any = { id: 0, name: '' }
    subtypes: any[] = new Array()
    subtypeSelected: any = { id: 0, name: '' }
    currencies: any[] = new Array()
    totalCostsExchanges: any = {
        exchanges: [],
        currencies: [],
        total: 0
    };

    readonly generalAdjustment: string = "% Ajuste General";
    readonly typeEmployee: string = "Empleados"
    readonly typeResource: string = "Recursos"
    readonly typeProfile: string = "Perfiles"

    @Output() openEvalPropModal: EventEmitter<any> = new EventEmitter();
    @Output() getData: EventEmitter<any> = new EventEmitter();

    @ViewChild('editItemModal') editItemModal;
    public editItemModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Editar Monto",
        "editItemModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    @ViewChild('editResourceQuantity') editResourceQuantityModal;
    public editResourceQuantityConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Editar Costos",
        "editResourceQuantity",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    @ViewChild('totalCostsExchangesModal') totalCostsExchangesModal;
    public totalCostsExchangesModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Detalle costos mensual",
        "totalCostsExchangesModal",
        false,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );
    //Constructor
    constructor(private managementReportService: ManagementReportService,
        private activatedRoute: ActivatedRoute,
        private messageService: MessageService,
        private menuService: MenuService,
        private utilsService: UtilsService,
        private employeeService: EmployeeService
    ) { }

    ngOnInit(): void {

        //  this.fromMonth = new Date(this.today.getFullYear(), this.today.getMonth() - 2, 1)
        if (this.menuService.hasFunctionality('MANRE', 'EDIT-COST-DETAIL') && this.menuService.userIsDirector || this.menuService.userIsManager || this.menuService.isManagementReportDelegate) {
            this.canEdit = true
        }

        this.getUsers()
        this.getProfiles()
        this.getCurrencies();

        this.editItemModal.size = 'modal-sm'

        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.serviceId = params['serviceId'];

            this.getCost();
        });
    }

    ngOnDestroy(): void {
        if (this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if (this.getCostSubscrip) this.getCostSubscrip.unsubscribe();
        if (this.updateCostSubscrip) this.updateCostSubscrip.unsubscribe();
        if (this.updateMonthSubscrip) this.updateMonthSubscrip.unsubscribe();
        if (this.getProfileSuscrip) this.getProfileSuscrip.unsubscribe();
        if (this.getEmployeeSubscrip) this.getEmployeeSubscrip.unsubscribe();
        if (this.getOtherByMonthSuscrip) this.getOtherByMonthSuscrip.unsubscribe();
        if (this.deleteProfileSubscrip) this.getOtherByMonthSuscrip.unsubscribe();
        if (this.deleteOthersByMonthSubscrip) this.getOtherByMonthSuscrip.unsubscribe();
    }

    getCurrencies() {
        this.getCurrenciesSubscrip = this.utilsService.getCurrencies().subscribe(d => {
            this.currencies = d;
        });
    }

    setFromDate(date: Date) {
        this.fromMonth = new Date(date.getFullYear(), date.getMonth() - 2, 1)
    }

    getCost() {
        this.getCostSubscrip = this.managementReportService.getCostDetail(this.serviceId).subscribe(response => {
            this.model = response.data;

            this.months = response.data.monthsHeader;
            this.employees = response.data.costEmployees;
            this.fundedResources = response.data.fundedResources;
            this.fundedResourcesEmployees = response.data.fundedResourcesEmployees
            this.otherResources = response.data.otherResources;
            this.employeesOriginal = response.data.costEmployees;
            this.costProfiles = response.data.costProfiles;

            if (this.otherResources.length > 0) {
                this.otherSelected = this.otherResources[0];
            }

            if (this.employees.length > 0) {
                this.addClassEmployee();
            }

            this.calculateTotalReal();
            this.calculateTotalCosts();
            this.sendDataToDetailView();
        },
        () => this.messageService.closeLoading());
    }

    openEditItemModal(month, item) {
        
        if (this.readOnly) return;

        if (month.closed) return;

        if (this.canEdit) {
            // if (item.typeName == 'Empleados' && !month.hasAlocation) {
            //     return false
            // }
            // else {
            //this.editItemModal.show();
            const indexMonth = item.monthsCost.findIndex(cost => cost === month);

            this.monthSelected = month;
            this.indexSelected = indexMonth;
            this.itemSelected = item;
            this.editItemMonto.setValidators([Validators.min(0), Validators.max(999999)]);

            this.modalPercentage = false;
            this.modalOther = false
            this.modalEmployee = false
            this.modalProfile = false;
            this.editItemModal.size = 'modal-sm'

            switch (this.itemSelected.typeName) {
                case this.typeEmployee:
                    this.modalEmployee = true
                    this.editItemMonto.setValue(month.budget.originalValue)
                    this.editItemAdjustment.setValue(month.budget.adjustment)
                    this.editItemAdjustment.setValidators([Validators.min(0), Validators.max(999)]);
                    this.editItemModal.show();
                    break;

                case this.generalAdjustment:
                    this.modalPercentage = true
                    this.editItemMonto.setValue(month.budget.value)
                    this.editItemMonto.setValidators([Validators.min(0), Validators.max(999)]);
                    this.editItemModal.show();
                    break;

                case this.typeProfile:
                    this.modalProfile = true
                    this.editItemMonto.setValue(month.budget.value)
                    this.editItemAdjustment.setValidators([Validators.min(0), Validators.max(999)]);
                    this.editItemModal.show();
                    break;

                default:                    
                    this.subtypes = []
                    this.modalOther = true
                    this.messageService.showLoading();
                    this.editItemModal.size = 'modal-lg'
                    this.categorySelected.id = item.typeId
                    this.categorySelected.name = item.typeName
                    this.getOtherByMonthSuscrip = this.managementReportService.GetOtherByMonth(item.typeId, month.costDetailId).subscribe(response => {
                        this.othersByMonth = response.data.costMonthOther;
                        this.subtypes = response.data.subtypes

                        if (this.subtypes.length > 0) {
                            this.subtypeSelected = this.subtypes[0]
                        }
                        // if (this.othersByMonth.length == 0) {
                        //     this.addOtherByMonth()
                        // }
                        this.editItemModal.show();
                        this.messageService.closeLoading();
                        setTimeout(() => {
                            $('.input-billing-modal.ng-select .ng-select-container').css('min-height', '26px');
                            $('.input-billing-modal.ng-select .ng-select-container').css('height', '26px');
                        }, 200);
                    },
                        error => {
                            this.messageService.closeLoading();
                        });



                    break;
            }
        }

    }

    EditItem() {

        this.monthSelected.budget.value = this.editItemMonto.value
        switch (this.itemSelected.typeName) {

            case this.typeEmployee:
                this.monthSelected.budget.originalValue = this.editItemMonto.value
                if (this.editItemAdjustment.value > 0) {
                    this.monthSelected.budget.adjustment = this.editItemAdjustment.value
                    this.monthSelected.budget.value = this.monthSelected.budget.originalValue + this.monthSelected.budget.originalValue * this.monthSelected.budget.adjustment / 100
                }
                else {
                    this.monthSelected.budget.adjustment = 0;
                    this.monthSelected.budget.value = this.editItemMonto.value;
                }

                for (let index = this.indexSelected + 1; index < this.itemSelected.monthsCost.length; index++) {

                    if (this.itemSelected.monthsCost[index].hasAlocation) {
                        this.itemSelected.monthsCost[index].budget.value = this.monthSelected.budget.value;
                        this.itemSelected.monthsCost[index].budget.originalValue = this.monthSelected.budget.value;
                    }
                    else {
                        this.itemSelected.monthsCost[index].budget.value = 0
                        this.itemSelected.monthsCost[index].budget.originalValue = 0
                        this.itemSelected.monthsCost[index].budget.adjustment = null
                    }
                }

                //Actualiza el sueldo
                this.salaryPlusIncrease(this.itemSelected, this.indexSelected, true);
                //Guarda el empleado
                var listAux = [];
                listAux.push(this.itemSelected);

                this.save(listAux, [], [])
                this.editItemModal.hide();
                break;

            case this.generalAdjustment:
                this.modalPercentage = true;
                this.employees.forEach(employee => {
                    this.salaryPlusIncrease(employee, this.indexSelected, false);
                })

                var listAux = [];
                listAux.push(this.itemSelected);

                this.save(this.employees, [], listAux)
                this.editItemModal.hide();
                break

            case this.typeProfile:

                for (let index = this.indexSelected + 1; index < this.itemSelected.monthsCost.length; index++) {
                    this.itemSelected.monthsCost[index].budget.value = this.monthSelected.budget.value;
                    this.itemSelected.monthsCost[index].budget.originalValue = this.monthSelected.budget.value;
                }

                var listAux = [];
                listAux.push(this.itemSelected);

                this.modalPercentage = false;
                if (this.itemSelected.typeName == this.typeProfile) {
                    this.save([], listAux, [])
                }
                this.editItemModal.hide();
                break

            default:
                var modelMonth = {
                    AnalyticId: this.model.analyticId,
                    ManagementReportId: this.model.managementReportId,
                    MonthYear: this.monthSelected.monthYear,
                    IsReal: false,
                    Employees: [],
                    OtherResources: this.othersByMonth,
                    Contracted: []
                }

                var month = this.months.find(x => x.month == this.monthSelected.month && x.year == this.monthSelected.year);

                this.updateMonthSubscrip = this.managementReportService.PostCostDetailMonth(this.serviceId, modelMonth).subscribe(response => {
                    this.messageService.closeLoading();

                    this.monthSelected.budget.value = 0
                    // this.othersByMonth.forEach(element => {
                    //     this.monthSelected.budget.value += element.value
                    // });

                    this.othersByMonth.forEach(cost => {
                        this.monthSelected.budget.value += this.setExchangeValue(month.currencyMonth, cost);
                    })

                    this.calculateTotalCosts();

                    setTimeout(() => {
                        this.sendDataToDetailView();
                    }, 1500);

                    this.editItemModal.hide();
                },
                    error => {
                        this.messageService.closeLoading();
                    });
                break;
        }

    }

    save(pEmployees, pProfiles, pFunded) {
        this.messageService.showLoading();

        var model = {
            ManagementReportId: this.model.managementReportId,
            ManagerId: this.model.managerId,
            AnalyticId: this.model.analyticId,
            CostEmployees: pEmployees,
            CostProfiles: pProfiles,
            FundedResources: pFunded
        }

        this.updateCostSubscrip = this.managementReportService.PostCostDetail(model).subscribe(() => {
            this.messageService.closeLoading();

            this.calculateTotalCosts();
            this.addClassEmployee();

            setTimeout(() => {
                this.sendDataToDetailView();
            }, 1500);
        },
            () => {
                this.messageService.closeLoading();
            });
    }

    getResourcesByMonth(month, year) {
        var resources = { employees: [], fundedResources: [], otherResources: [] }

        var monthHeader = this.months.find(x => {
            var dateSplitted = x.monthYear.split("-");
            if (dateSplitted[0] == year && dateSplitted[1] == month) {
                return x;
            }
        });

        resources.employees = this.employees.map(element => {

            var monthCost = element.monthsCost.find(x => {
                var dateSplitted = x.monthYear.split("-");

                if (dateSplitted[0] == year && dateSplitted[1] == month) {
                    return x;
                }
            });

            var _id = 0
            var _salary = monthCost.budget.value || 0
            var _charges = monthCost.budget.charges || 0
            var _total = monthCost.budget.value + monthCost.budget.charges || 0

            if (monthHeader.hasReal) {
                _id = monthCost.real.id
                _salary = monthCost.real.value || 0
                _charges = monthCost.real.charges || 0
                _total = monthCost.real.value + monthCost.real.charges || 0
            }

            return {
                employeeId: element.employeeId,
                userId: element.userId,
                monthYear: monthCost.monthYear,
                hasAlocation: monthCost.hasAlocation,
                name: element.display,
                id: _id,
                salary: _salary,
                charges: _charges,
                chargesPercentage: monthCost.chargesPercentage,
                total: _total
            }
        });

        return resources;
    }

    getIdAnalytic() {
        return this.model.analyticId
    }

    CalculateSalary(monthData, index) {
        var SalaryPlusIncrese = monthData.value;

        var AjusteMensual = this.fundedResources.find(r => r.typeName == this.generalAdjustment);
        if (AjusteMensual) {
            if (AjusteMensual.monthsCost[index].budget.value > 0) {
                SalaryPlusIncrese = monthData.budget.value + (monthData.budget.value * AjusteMensual.monthsCost[index].budget.value / 100);
            }
        }
        return SalaryPlusIncrese;
    }

    salaryPlusIncrease(employee, pIndex, isSalaryEmployee) {
        //Verifico que exista la fila de ajustes
        var AjusteMensual = this.fundedResources.find(r => r.display == this.generalAdjustment);
        if (AjusteMensual) {
            //Si existe, Recorro todos los meses
            let newSalary = 0;
            //El nuevo salario lo seteo como el primer salario
            if (isSalaryEmployee == true) {
                newSalary = employee.monthsCost[pIndex].budget.value
                pIndex += 1
            }

            for (let index = pIndex; index < employee.monthsCost.length; index++) {

                //Verifico si tiene aumento en alguno
                if (AjusteMensual.monthsCost[index].budget.value > 0) {
                    newSalary = employee.monthsCost[index].budget.originalValue + (employee.monthsCost[index].budget.originalValue * AjusteMensual.monthsCost[index].budget.value / 100);
                }
                else {
                    //Si el aumento es cero el salario nuevo es igual al salario anterior
                    if (AjusteMensual.monthsCost[index].value == 0) {
                        newSalary = employee.monthsCost[index].originalValue
                    }
                }

                if (employee.monthsCost[index].budget.value > 0) {
                    employee.monthsCost[index].budget.value = newSalary;
                    employee.monthsCost[index].budget.adjustment = AjusteMensual.monthsCost[index].budget.value

                    for (let newindex = index + 1; newindex < employee.monthsCost.length; newindex++) {

                        if (employee.monthsCost[newindex].budget.value > 0) {
                            employee.monthsCost[newindex].budget.value = newSalary;
                            employee.monthsCost[newindex].budget.originalValue = newSalary;
                        }
                        else {
                            employee.monthsCost[newindex].value = 0
                        }
                    }
                }
            }
        }
    }

    calculateAllSalary(month) {
        const index = this.months.findIndex(cost => cost.monthYear === month.monthYear);
        var totalSalary = 0;
        this.employees.forEach(employee => {
            if (employee.monthsCost[index].budget.value) {
                totalSalary += employee.monthsCost[index].budget.value
            }
        })

        this.costProfiles.forEach(Profile => {
            if (Profile.monthsCost[index].budget.value) {
                totalSalary += Profile.monthsCost[index].budget.value
            }
        })

        this.fundedResourcesEmployees.forEach(resourceEmpleyee => {
            if (resourceEmpleyee.monthsCost[index].budget.value) {
                totalSalary += resourceEmpleyee.monthsCost[index].budget.value
            }
        })

        month.budget.totalSalary = totalSalary
    }

    calculateTotalCosts() {
        this.months.forEach((month, index) => {
            var totalCost = 0;
            var totalSalary = 0;
            var asignacion = 0

            //Sumo el totol de los sueldos
            this.employees.forEach(employee => {
                if (employee.monthsCost[index].budget.value) {
                    totalCost += employee.monthsCost[index].budget.value;
                    totalSalary += employee.monthsCost[index].budget.value;
                }

                asignacion += employee.monthsCost[index].allocationPercentage
            })

            //Sumo los sueldos de los perfiles
            this.costProfiles.forEach(profile => {
                if (profile.monthsCost[index].budget.value) {
                    totalCost += profile.monthsCost[index].budget.value;
                    totalSalary += profile.monthsCost[index].budget.value;
                }
            })

            //Sumo los demas gastos excepto el % de Ajuste
            this.fundedResources.forEach(resource => {
                if (resource.typeName != this.generalAdjustment) {
                    totalCost += resource.monthsCost[index].budget.value;
                }
            })

            //Sumo los gastos de los empleados
            this.fundedResourcesEmployees.forEach(resourceEmpleyee => {
                if (resourceEmpleyee.monthsCost[index].budget.value) {
                    totalCost += resourceEmpleyee.monthsCost[index].budget.value
                    totalSalary += resourceEmpleyee.monthsCost[index].budget.value;
                }
            })

            month.budget.totalCost = totalCost + (totalSalary * 0.51);
            month.budget.totalSalary = totalSalary
            month.budget.totalLoads = (totalSalary * 0.51)
            month.resourceQuantity = asignacion / 100
        })
    }

    calculateTotalReal() {
        this.months.forEach((month, index) => {
            var totalCost = 0;
            var totalSalary = 0;
            var totalCharges = 0;

            //Sumo el totol de los sueldos
            this.employees.forEach(employee => {
                if (employee.monthsCost[index].real.value) {
                    totalCost += employee.monthsCost[index].real.value;
                    totalSalary += employee.monthsCost[index].real.value;
                }
                if (employee.monthsCost[index].real.charges) {
                    totalCharges += employee.monthsCost[index].real.charges
                }
            })

            //Sumo los demas gastos excepto el % de Ajuste
            this.fundedResources.forEach(resource => {
                if (resource.typeName != this.generalAdjustment) {
                    totalCost += resource.monthsCost[index].real.value;
                }
            })

            //Sumo los gastos de los empleados
            this.fundedResourcesEmployees.forEach(resourceEmpleyee => {
                if (resourceEmpleyee.monthsCost[index].real.value) {
                    totalCost += resourceEmpleyee.monthsCost[index].real.value
                    totalSalary += resourceEmpleyee.monthsCost[index].real.value
                }
            })

            month.real.totalCost = totalCost + (totalSalary * 0.51)
            month.real.totalSalary = totalSalary
            month.real.totalLoads = totalCharges
        })
    }

    calculateLoads(month) {
        const index = this.months.findIndex(cost => cost.monthYear === month.monthYear);
        var totalSalary = 0;
        this.employees.forEach(employee => {
            if (employee.monthsCost[index].budget.value) {
                totalSalary += employee.monthsCost[index].budget.value
            }
        })

        this.costProfiles.forEach(profile => {
            if (profile.monthsCost[index].budget.value) {
                totalSalary += profile.monthsCost[index].budget.value
            }
        })

        this.fundedResourcesEmployees.forEach(resourceEmpleyee => {
            if (resourceEmpleyee.monthsCost[index].budget.value) {
                totalSalary += resourceEmpleyee.monthsCost[index].budget.value
            }
        })

        month.budget.totalLoads = totalSalary * 0.51;
    }

    EditItemOnClose() {
    }

    addOtherCost() {

        switch (this.otherSelected.typeName) {
            case this.typeResource:
                this.addEmployee()
                break;

            case this.typeProfile:
                this.addProfile()
                break;

            default:
                var resource = this.otherResources.find(r => r.typeId == this.otherSelected.typeId)
                this.fundedResources.push(resource)

                var pos = this.otherResources.findIndex(r => r.typeId == this.otherSelected.typeId);
                this.otherResources.splice(pos, 1)

                if (this.otherResources.length > 0) {
                    this.otherSelected = this.otherResources[0];
                    this.otherResourceChange()
                }
                break;
        }
    }

    resourcesClass(monthCost, item) {

        let cssClass;
        if (!this.canEdit) {
            cssClass = 'not-allowed'
        }
        else {
            cssClass = 'cursor-pointer'
            if (item.display == this.generalAdjustment) {
                if (monthCost.value > 0) {
                    cssClass += ' label-yellow'
                }
            }
        }

        return cssClass;
    }

    canDeleteResources(item) {

        var canEdit = false;
        if (item.otherResource) {
            canEdit = true;
        }
        item.monthsCost.forEach(month => {
            if (month.budget.value > 0 || month.real.value > 0) {
                canEdit = false
                return canEdit
            }
        });

        return canEdit
    }

    deleteResources(item, index) {

        this.fundedResources.splice(index, 1)
        this.otherResources.push(item)

        this.save([], [], this.fundedResources)

        this.otherResources.sort(function (a, b) {
            if (a.display > b.display) {
                return 1;
            }
            if (a.display < b.display) {
                return -1;
            }
            // a must be equal to b
            return 0;
        });
    }

    getProfiles() {
        this.getProfileSuscrip = this.utilsService.getEmployeeProfiles().subscribe(response => {
            this.profiles = response;
        });
    }

    getUsers() {
        this.getEmployeeSubscrip = this.employeeService.getListItems().subscribe(data => {
            this.users = data;
        });
    }

    canEditCdg() {
        return this.menuService.userIsCdg;
    }

    otherResourceChange() {

        switch (this.otherSelected.typeName) {
            case this.typeResource:
                this.showUsers = true
                this.showProfiles = false
                break;

            case this.typeProfile:
                this.showUsers = false
                this.showProfiles = true
                break;

            default:
                this.showUsers = false
                this.showProfiles = false
                break;
        }
    }

    openEditEvalProp(month) {
        if (this.readOnly) return;

        if (month.closed) return;

        if (this.openEvalPropModal.observers.length > 0) {
            month.type = 2;
            this.openEvalPropModal.emit(month);
        }
    }

    addEmployee() {

        if (this.userSelected) {
            var existingEmployee = this.employees.find(e => e.employeeId === this.userSelected.id)
            if (!existingEmployee) {
                var monthsEmpty = this.createEmptyMonths()
                var costEmployee = {

                    employeeId: this.userSelected.id,
                    userId: this.userSelected.userId,
                    typeName: this.typeEmployee,
                    display: `${this.userSelected.text.toUpperCase()} - ${this.userSelected.employeeNumber}`,
                    monthsCost: monthsEmpty
                }

                this.employees.push(costEmployee)
            }
            else {
                this.messageService.showError("managementReport.existingEmployee")
            }
        }
        else {
            this.messageService.showError("managementReport.userRequired")
        }
    }

    addProfile() {

        if (this.profileSelected) {
            var monthsEmpty = this.createEmptyMonths()
            var costProfile = {
                display: this.profileSelected.text,
                employeeProfileId: this.profileSelected.id,
                guid: "",
                typeName: this.typeProfile,
                monthsCost: monthsEmpty
            }

            this.costProfiles.push(costProfile)
        }
        else {
            this.messageService.showError("managementReport.profileRequired")
        }
    }

    sendDataToDetailView() {
        if (this.getData.observers.length > 0) {
            this.getData.emit({
                model: this.model,
                employees: this.employees,
                months: this.months,
                otherResources: this.otherResources,
                costProfiles: this.costProfiles,
                fundedResources: this.fundedResources.concat(this.fundedResourcesEmployees)
            });
        }
    }

    addOtherByMonth() {
        var resource = {
            id: 0,
            CostDetailId: this.monthSelected.costDetailId,
            subtypeId: this.subtypeSelected.id,
            subtypeName: this.subtypeSelected.name,
            value: 0,
            description: "",
            currencyId: this.currencies[0].id
        }

        this.othersByMonth.push(resource)

        setTimeout(() => {
            $('.input-billing-modal.ng-select .ng-select-container').css('min-height', '26px');
            $('.input-billing-modal.ng-select .ng-select-container').css('height', '26px');
        }, 200);
    }

    // openEditResourceQuantity(month) {
    //     if (this.readOnly) return;

    //     if (month.closed) return;

    //     this.monthSelected = month;
    //     this.editResourceQuantityModal.show()
    // }

    // updateResourceQuantity() {
    //     this.updateCostSubscrip = this.managementReportService.updateQuantityResources(this.monthSelected.billingMonthId, parseInt(this.monthSelected.resourceQuantity)).subscribe(
    //         () => {
    //             this.editResourceQuantityModal.hide()
    //             this.sendDataToDetailView()
    //             this.messageService.closeLoading();
    //         },
    //         () => {
    //             this.editResourceQuantityModal.hide()
    //             this.messageService.closeLoading();
    //         });
    // }

    // setResourceQuantity(months) {
    //     months.forEach(month => {
    //         var monthValue = this.months.find(x => x.month == month.month && x.year == month.year);
    //         if (monthValue) {
    //             monthValue.resourceQuantity = month.resourceQuantity;
    //         }
    //     });
    // }

    getId(date: Date) {
        var item = this.months.find(x => x.month == (date.getMonth() + 1) && x.year == date.getFullYear());

        if (item) {
            return item.costDetailId;
        }

        return 0;
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

    createEmptyMonths() {
        var monthsEmpty = new Array()
        this.months.forEach(element => {
            let month = {
                canViewSensibleData: false,
                costDetailId: element.costDetailId,
                display: element.display,
                month: element.month,
                monthYear: element.monthYear,
                year: element.year,
                budget: {
                    id: element.id,
                    value: 0,
                    originalValue: null,
                    adjustment: 0,
                    charges: null,
                    description: null,
                }
            }
            monthsEmpty.push(month)
        });

        return monthsEmpty
    }

    deleteProfile(profile, index) {

        if (profile.guid == "") {
            this.costProfiles.splice(index, 1)
        }
        else {
            this.messageService.showLoading();

            this.deleteProfileSubscrip = this.managementReportService.deleteProfile(profile.guid).subscribe(
                () => {
                    this.messageService.closeLoading();
                    this.costProfiles.splice(index, 1)
                },
                () => {
                    this.messageService.closeLoading();
                });
        }
    }

    deleteOthersByMonth(index, item) {

        //Si el item no esta en base de datos solo lo borro del array
        if (item.id == 0) {
            this.othersByMonth.splice(index, 1);
        }
        else {
            //Si esta en base de datos borro el registio
            this.deleteOthersByMonthSubscrip = this.managementReportService.deleteOtherResources(item.id).subscribe(() => {
                this.othersByMonth.splice(index, 1);
            },
                () => {
                });
        }

    }

    addClassEmployee() {

        let arrayClass = ['label-yellow', 'label-warning']

        this.employees.forEach(employee => {
            let aux = 0
            employee.monthsCost.forEach((month, index) => {

                if (month.hasAlocation) {
                    if (index > 0) {
                        if (month.budget.value != employee.monthsCost[index - 1].budget.value) {
                            if (aux >= arrayClass.length - 1) {
                                aux = 0
                            }
                            else {
                                aux++
                            }
                        }
                    }
                    month.class = arrayClass[aux]
                }
                else {
                    month.class = 'label-danger'
                }

                if (!this.canEdit) {
                    month.class += ' not-allowed'
                }
                else {
                    month.class += ' cursor-pointer'
                }

            })
        });
    }

    setExchangeValue(currencyMonth, cost) {
        var total = 0;
        cost.originalValue = cost.value;

        if (currencyMonth) {
            var currencyExchange = currencyMonth.find(x => x.currencyId == cost.currencyId);
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

    setAllCosts(pMonth, total, type) {
        debugger
        var exchanges = [];
        var currencies = [];

        this.currencies.forEach(currency => {
            currencies.push({ value: 0, valuePesos: 0, currencyName: currency.text, id: currency.id });
        });

        var currencyMonth = pMonth.currencyMonth
        if (currencyMonth) {
            currencyMonth.forEach(item => {
                exchanges.push({ currencyName: item.currencyDesc, exchange: item.exchange });
            });
        }

        var currPesos = currencies.find(x => x.currencyName.toUpperCase() == 'PESOS ($)');
        var totalSalary = 0

        //Sumo el totol de los sueldos (solo estan en pesos Ars)
        this.employees.forEach(employee => {
            var monthCost = employee.monthsCost.find(x => x.month == pMonth.month && x.year == pMonth.year);
            totalSalary += monthCost[type].value;
            if (monthCost[type].value) {
                currPesos.value += monthCost[type].value;
                currPesos.valuePesos += monthCost[type].value;
            }
        })

        //Sumo los sueldos de los perfiles (solo estan en pesos ARs)
        this.costProfiles.forEach(profile => {
            var monthCost = profile.monthsCost.find(x => x.month == pMonth.month && x.year == pMonth.year);
            totalSalary += monthCost[type].value;
            if (monthCost[type].value) {
                currPesos.value += monthCost[type].value;
                currPesos.valuePesos += monthCost[type].value;
            }
        })

        // Al total de pesos le sumo las cargas (51% del salario)
        currPesos.value = currPesos.value + (totalSalary * 0.51)
        currPesos.valuePesos = currPesos.valuePesos + (totalSalary * 0.51)

        var otherResourceValues = new Array()

         //Sumo los demas gastos excepto el % de Ajuste
        this.fundedResources.forEach(resource => {
            if (resource.typeName != this.generalAdjustment) {
            var monthCost = resource.monthsCost.find(x => x.month == pMonth.month && x.year == pMonth.year);
            if (monthCost[type].value) {
                if (monthCost[type].value > 0) {
                    otherResourceValues.push(resource)
                }
            }
            }
        })

         //Sumo los gastos de los empleados
        this.fundedResourcesEmployees.forEach(resourceEmpleyee => {
            var monthCost = resourceEmpleyee.monthsCost.find(x => x.month == pMonth.month && x.year == pMonth.year);
            if (monthCost[type].value) {
                if (monthCost[type].value > 0) {
                    otherResourceValues.push(resourceEmpleyee)
                }
            }
        })

        otherResourceValues.forEach(resourceEmpleyee => {
            this.getOtherByMonthSuscrip = this.managementReportService.GetOtherByMonth(resourceEmpleyee.typeId, pMonth.costDetailId).subscribe(
                response => {
                    var subCategories = response.data.costMonthOther                            
                    if (subCategories) {
                        subCategories.forEach(subcategory => {
                            var curr = currencies.find(x => x.id == subcategory.currencyId);
                            var valueCurrency = currencyMonth.find(x => x.currencyId == subcategory.currencyId)
                            if (curr) {
                                curr.value += subcategory.value;
                                if(valueCurrency){
                                    curr.valuePesos += subcategory.value * valueCurrency.exchange;    
                                }
                                else{
                                    curr.valuePesos += subcategory.value
                                }
                            }
                        });
                    }
                    this.messageService.closeLoading();
                },
                error => {
                    this.messageService.closeLoading();
                });
            })

        this.totalCostsExchanges = {
            exchanges: exchanges,
            currencies: currencies,
            total: total
        };

        this.totalCostsExchangesModal.show();
    }

    getSubcategoriesMonth(typeId, month) {
        var subCategories = new Array()
        this.getOtherByMonthSuscrip = this.managementReportService.GetOtherByMonth(typeId, month.costDetailId).subscribe(
            response => {
                subCategories = response.data.costMonthOther

                this.messageService.closeLoading();
            },
            error => {
                this.messageService.closeLoading();
            });

        return subCategories
    }
}

