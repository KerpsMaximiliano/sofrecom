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
        if (this.menuService.hasFunctionality('MANRE', 'EDIT-COST-DETAIL')) {
            this.canEdit = true
        }

        this.getUsers()
        this.getProfiles()

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
    }

    setFromDate(date: Date) {
        this.fromMonth = new Date(date.getFullYear(), date.getMonth() - 2, 1)
    }

    getCost() {
        this.messageService.showLoading();

        this.getCostSubscrip = this.managementReportService.getCostDetail(this.serviceId).subscribe(response => {
            this.messageService.closeLoading();

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
                    this.editItemMonto.setValue(month.originalValue)
                    this.editItemAdjustment.setValue(month.adjustment)
                    this.editItemAdjustment.setValidators([Validators.min(0), Validators.max(999)]);
                    this.editItemModal.show();
                    break;

                case this.generalAdjustment:
                    this.modalPercentage = true
                    this.editItemMonto.setValue(month.value)
                    this.editItemMonto.setValidators([Validators.min(0), Validators.max(999)]);
                    this.editItemModal.show();
                    break;

                case this.typeProfile:
                    this.modalProfile = true
                    this.editItemMonto.setValue(month.value)
                    this.editItemAdjustment.setValidators([Validators.min(0), Validators.max(999)]);
                    this.editItemModal.show();
                    break;

                default:
                    this.modalOther = true
                    this.messageService.showLoading();
                    this.editItemModal.size = 'modal-md'
                    this.getOtherByMonthSuscrip = this.managementReportService.GetOtherByMonth(item.typeId, month.costDetailId).subscribe(response => {
                        this.othersByMonth = response.data;

                        if (this.othersByMonth.length == 0) {
                            this.addOtherByMonth()
                        }
                        this.editItemModal.show();
                        this.messageService.closeLoading();
                    },
                        error => {
                            this.messageService.closeLoading();
                        });
                    break;
            }
        }

    }

    EditItem() {

        this.monthSelected.value = this.editItemMonto.value
        switch (this.itemSelected.typeName) {

            case this.typeEmployee:
                this.monthSelected.originalValue = this.editItemMonto.value
                if (this.editItemAdjustment.value > 0) {
                    this.monthSelected.adjustment = this.editItemAdjustment.value
                    this.monthSelected.value = this.monthSelected.originalValue + this.monthSelected.originalValue * this.monthSelected.adjustment / 100
                }
                else {
                    this.monthSelected.adjustment = 0;
                    this.monthSelected.value = this.editItemMonto.value;
                }

                for (let index = this.indexSelected + 1; index < this.itemSelected.monthsCost.length; index++) {

                    if (this.itemSelected.monthsCost[index].hasAlocation) {
                        this.itemSelected.monthsCost[index].value = this.monthSelected.value;
                        this.itemSelected.monthsCost[index].originalValue = this.monthSelected.value;
                    }
                    else {
                        this.itemSelected.monthsCost[index].value = 0
                        this.itemSelected.monthsCost[index].originalValue = 0
                        this.itemSelected.monthsCost[index].adjustment = null
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
                    this.itemSelected.monthsCost[index].value = this.monthSelected.value;
                    this.itemSelected.monthsCost[index].originalValue = this.monthSelected.value;
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
                    Employees: [],
                    OtherResources: this.othersByMonth,
                    Contracted: []
                }

                this.updateMonthSubscrip = this.managementReportService.PostCostDetailMonth(this.serviceId, modelMonth).subscribe(response => {
                    this.messageService.closeLoading();

                    this.monthSelected.value = 0
                    this.othersByMonth.forEach(element => {
                        this.monthSelected.value += element.value
                    });

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
            MonthsHeader: this.model.monthsHeader,
            CostEmployees: pEmployees,
            CostProfiles: pProfiles,
            FundedResources: pFunded
        }
        this.updateCostSubscrip = this.managementReportService.PostCostDetail(model).subscribe(() => {
            this.messageService.closeLoading();

            this.calculateTotalCosts();

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

        resources.employees = this.employees.map(element => {

            var monthCost = element.monthsCost.find(x => {
                var dateSplitted = x.monthYear.split("-");

                if (dateSplitted[0] == year && dateSplitted[1] == month) {
                    return x;
                }
            });

            return {
                employeeId: element.employeeId,
                userId: element.userId,
                monthYear: monthCost.monthYear,
                hasAlocation: monthCost.hasAlocation,
                id: monthCost.id,
                name: element.display,
                salary: monthCost.value || 0,
                charges: monthCost.charges || 0,
                chargesPercentage: monthCost.chargesPercentage,
                total: monthCost.value + monthCost.charges || 0
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
            if (AjusteMensual.monthsCost[index].value > 0) {
                SalaryPlusIncrese = monthData.value + (monthData.value * AjusteMensual.monthsCost[index].value / 100);
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
                newSalary = employee.monthsCost[pIndex].value
                pIndex += 1
            }

            for (let index = pIndex; index < employee.monthsCost.length; index++) {

                //Verifico si tiene aumento en alguno
                if (AjusteMensual.monthsCost[index].value > 0) {
                    newSalary = employee.monthsCost[index].originalValue + (employee.monthsCost[index].originalValue * AjusteMensual.monthsCost[index].value / 100);
                }
                else {
                    //Si el aumento es cero el salario nuevo es igual al salario anterior
                    if (AjusteMensual.monthsCost[index].value == 0) {
                        newSalary = employee.monthsCost[index].originalValue
                    }
                }

                if (employee.monthsCost[index].value > 0) {
                    employee.monthsCost[index].value = newSalary;
                    employee.monthsCost[index].adjustment = AjusteMensual.monthsCost[index].value

                    for (let newindex = index + 1; newindex < employee.monthsCost.length; newindex++) {

                        if (employee.monthsCost[newindex].value > 0) {
                            employee.monthsCost[newindex].value = newSalary;
                            employee.monthsCost[newindex].originalValue = newSalary;
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
            if (employee.monthsCost[index].value) {
                totalSalary += employee.monthsCost[index].value
            }
        })

        this.costProfiles.forEach(Profile => {
            if (Profile.monthsCost[index].value) {
                totalSalary += Profile.monthsCost[index].value
            }
        })

        this.fundedResourcesEmployees.forEach(resourceEmpleyee => {
            if (resourceEmpleyee.monthsCost[index].value) {
                totalSalary += resourceEmpleyee.monthsCost[index].value
            }
        })

        return totalSalary
    }

    calculateTotalCosts() {
        this.months.forEach((month, index) => {
            var totalCost = 0;
            var totalSalary = 0;

            //Sumo el totol de los sueldos
            this.employees.forEach(employee => {
                if (employee.monthsCost[index].value) {
                    totalCost += employee.monthsCost[index].value;
                    totalSalary += employee.monthsCost[index].value;
                }
            })

            //Sumo los sueldos de los perfiles
            this.costProfiles.forEach(profile => {
                if (profile.monthsCost[index].value) {
                    totalCost += profile.monthsCost[index].value;
                    totalSalary += profile.monthsCost[index].value;
                }
            })

            //Sumo los demas gastos excepto el % de Ajuste
            this.fundedResources.forEach(resource => {
                if (resource.typeName != this.generalAdjustment) {
                    totalCost += resource.monthsCost[index].value;
                }
            })

            //Sumo los gastos de los empleados
            this.fundedResourcesEmployees.forEach(resourceEmpleyee => {
                if (resourceEmpleyee.monthsCost[index].value) {
                    totalCost += resourceEmpleyee.monthsCost[index].value
                }
            })

            month.value = totalCost + (totalSalary * 0.51);
        })
    }

    calculateLoads(month) {
        const index = this.months.findIndex(cost => cost.monthYear === month.monthYear);
        var totalSalary = 0;
        this.employees.forEach(employee => {
            if (employee.monthsCost[index].value) {
                totalSalary += employee.monthsCost[index].value
            }
        })

        this.costProfiles.forEach(profile => {
            if (profile.monthsCost[index].value) {
                totalSalary += profile.monthsCost[index].value
            }
        })

        this.fundedResourcesEmployees.forEach(resourceEmpleyee => {
            if (resourceEmpleyee.monthsCost[index].value) {
                totalSalary += resourceEmpleyee.monthsCost[index].value
            }
        })

        return totalSalary * 0.51;
    }

    EditItemOnClose() {
    }

    AccessEmployeeClass(monthCost) {

        let cssClass;
        if (!this.canEdit) {
            cssClass = 'not-allowed'
        }
        else {
            cssClass = 'cursor-pointer'
            if (!monthCost.hasAlocation) {
                cssClass += ' label-danger'
                // cssClass = 'not-allowed label-danger'
            }
        }

        return cssClass;
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
            if (month.value > 0) {
                canEdit = false
                return canEdit
            }
        });

        return canEdit
    }

    deleteResources(item, index) {

        this.save([], [], this.fundedResources)

        this.fundedResources.splice(index, 1)
        this.otherResources.push(item)

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
        this.messageService.showLoading();

        this.getProfileSuscrip = this.utilsService.getEmployeeProfiles().subscribe(response => {
            this.messageService.closeLoading();

            this.profiles = response;
        },
            () => {
                this.messageService.closeLoading();
            })
    }

    getUsers() {
        this.messageService.showLoading();

        this.getEmployeeSubscrip = this.employeeService.getListItems().subscribe(data => {
            this.messageService.closeLoading();

            this.users = data;
        },
            () => {
                this.messageService.closeLoading();
            })
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
            typeId: this.itemSelected.typeId,
            typeName: this.itemSelected.typeName,
            value: 0,
            description: ""
        }

        this.othersByMonth.push(resource)
    }

    openEditResourceQuantity(month) {
        if (this.readOnly) return;

        if (month.closed) return;

        this.monthSelected = month;
        this.editResourceQuantityModal.show()
    }

    updateResourceQuantity() {
        this.updateCostSubscrip = this.managementReportService.updateQuantityResources(this.monthSelected.billingMonthId, parseInt(this.monthSelected.resourceQuantity)).subscribe(
            () => {
                this.editResourceQuantityModal.hide()
                this.sendDataToDetailView()
                this.messageService.closeLoading();
            },
            () => {
                this.editResourceQuantityModal.hide()
                this.messageService.closeLoading();
            });
    }

    setResourceQuantity(months) {
        months.forEach(month => {
            var monthValue = this.months.find(x => x.month == month.month && x.year == month.year);
            if (monthValue) {
                monthValue.resourceQuantity = month.resourceQuantity;
            }
        });
    }

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
                charges: null,
                costDetailId: element.costDetailId,
                description: null,
                display: element.display,
                id: element.id,
                month: element.month,
                monthYear: element.monthYear,
                originalValue: null,
                value: 0,
                year: element.year
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
}

