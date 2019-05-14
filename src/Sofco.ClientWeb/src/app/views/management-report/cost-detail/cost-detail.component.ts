import { Component, OnDestroy, OnInit, ViewChild, Output, EventEmitter } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { ManagementReportService } from "app/services/management-report/management-report.service";
import { Subscription } from "rxjs";
import { MenuService } from "app/services/admin/menu.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { MessageService } from "app/services/common/message.service";
import { UtilsService } from "app/services/common/utils.service"
import { FormControl, Validators } from "@angular/forms";
import { UserService } from "app/services/admin/user.service";
import { EmployeeService } from "app/services/allocation-management/employee.service"
import { Profile } from "selenium-webdriver/firefox";

@Component({
    selector: 'cost-detail',
    templateUrl: './cost-detail.component.html',
    styleUrls: ['./cost-detail.component.scss']
})
export class CostDetailComponent implements OnInit, OnDestroy {

    paramsSubscrip: Subscription;
    getCostSubscrip: Subscription;
    updateCostSubscrip: Subscription;
    getProfileSuscrip: Subscription
    getUsersSubscrip: Subscription;
    getEmployeeSubscrip: Subscription;

    //Propiedades
    serviceId: string;
    months: any[] = new Array();
    employees: any[] = new Array();
    employeesOriginal: any[] = new Array();
    fundedResources: any[] = new Array();
    otherResources: any[] = new Array();
    costProfiles: any[] = new Array()
    monthSelected: any = { value: 0, display: '' };
    itemSelected: any;
    indexSelected: number = 0;
    model: any;
    modalPercentage: boolean = false;
    modalEmployee: boolean = false;
    editItemMonto = new FormControl();
    editItemAdjustment = new FormControl();
    canEdit: boolean = false;
    profiles: any[] = new Array()
    profileId: number;
    otherResourceId: number;
    users: any[] = new Array()
    userId: number
    showUsers: boolean = false
    showProfiles: boolean = false

    otherSelected: any
    userSelected: any
    profileSelected: any

    readonly generalAdjustment: string = "% Ajuste General";
    readonly typeEmployee: string = "Empleados"
    readonly AddResource: string = "Recursos"
    readonly AddProfile: string = "Perfiles"

    @Output() openEvalPropModal: EventEmitter<any> = new EventEmitter();

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
    constructor(private managementReportService: ManagementReportService,
        private activatedRoute: ActivatedRoute,
        private messageService: MessageService,
        private menuService: MenuService,
        private usersService: UserService,
        private utilsService: UtilsService,
        private employeeService: EmployeeService
    ) { }

    ngOnInit(): void {

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
    }

    getCost() {
        this.messageService.showLoading();

        this.getCostSubscrip = this.managementReportService.getCostDetail(this.serviceId).subscribe(response => {
            this.messageService.closeLoading();

            this.model = response.data;

            this.months = response.data.monthsHeader;
            this.employees = response.data.costEmployees;
            this.fundedResources = response.data.fundedResources;
            this.otherResources = response.data.otherResources;
            this.employeesOriginal = response.data.costEmployees;
            this.costProfiles = response.data.costProfiles;

            if (this.otherResources.length > 0) {
                this.otherSelected = this.otherResources[0];
            }
        },
            () => this.messageService.closeLoading());
    }

    openEditItemModal(month, item, indexMonth) {

        if (this.canEdit) {
            // if (item.typeName == 'Empleados' && !month.hasAlocation) {
            //     return false
            // }
            // else {
            this.editItemModal.show();
            this.monthSelected = month;
            this.indexSelected = indexMonth;
            this.itemSelected = item;
            this.editItemMonto.setValidators([Validators.min(0), Validators.max(999999)]);
            this.modalPercentage = false;

            if (this.itemSelected.typeName == this.typeEmployee) {
                this.modalEmployee = true
                this.editItemMonto.setValue(month.originalValue)
                this.editItemAdjustment.setValue(month.adjustment)
                this.editItemAdjustment.setValidators([Validators.min(0), Validators.max(999)]);
            }
            else {
                this.modalEmployee = false
                this.editItemMonto.setValue(month.value)

                if (this.itemSelected.typeName == this.generalAdjustment) {
                    this.editItemMonto.setValidators([Validators.min(0), Validators.max(999)]);
                    this.modalPercentage = true
                }
            }
            // }
        }

    }

    EditItem() {

        this.monthSelected.value = this.editItemMonto.value

        //Si estoy editando un empleado se actualiza el sueldo para los meses que siguen
        if (this.itemSelected.typeName == this.typeEmployee) {

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

                // if (this.itemSelected.monthsCost[index].hasAlocation) {
                this.itemSelected.monthsCost[index].value = this.monthSelected.value;
                this.itemSelected.monthsCost[index].originalValue = this.monthSelected.value;
                // }
                // else {
                //     this.itemSelected.monthsCost[index].value = 0
                // }
            }

            //Actualiza el sueldo
            this.salaryPlusIncrease(this.itemSelected, this.indexSelected, true);
        }

        //Si estoy editando un aumento se actualiza el sueldo para todos los empleados
        if (this.itemSelected.typeName == this.generalAdjustment) {
            this.modalPercentage = true;
            this.employees.forEach(employee => {
                this.salaryPlusIncrease(employee, this.indexSelected, false);
            })
        }
        else {
            this.modalPercentage = false;
        }

        this.editItemModal.hide();
    }

    save() {
        this.messageService.showLoading();

        this.model.employees = this.employees;
        this.model.fundedResources = this.fundedResources;

        this.updateCostSubscrip = this.managementReportService.PostCostDetail(this.serviceId, this.model).subscribe(() => {
            this.messageService.closeLoading();

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
                total: monthCost.value + monthCost.charges || 0
            }
        });

        resources.fundedResources = this.fundedResources.map(element => {

            var monthCost = element.monthsCost.find(x => {
                var dateSplitted = x.monthYear.split("-");

                if (dateSplitted[0] == year && dateSplitted[1] == month) {
                    return x;
                }
            });

            return {
                typeId: element.typeId,
                typeName: element.typeName,
                monthYear: monthCost.monthYear,
                id: monthCost.id,
                value: monthCost.value || 0,
                otherResource: element.otherResource
            }
        });

        resources.otherResources = this.otherResources.map(element => {

            var monthCost = element.monthsCost.find(x => {
                var dateSplitted = x.monthYear.split("-");

                if (dateSplitted[0] == year && dateSplitted[1] == month) {
                    return x;
                }
            });

            return {
                typeId: element.typeId,
                typeName: element.typeName,
                MonthYear: monthCost.monthYear,
                costDetailId: monthCost.costDetailId,
                salary: monthCost.value || 0,
                otherResource: element.otherResource
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

    calculateAllSalary(index) {
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

        return totalSalary
    }

    calculateAssignedEmployees(index) {
        var totalEmployees = 0;
        this.employees.forEach(employee => {
            if (employee.monthsCost[index].value) {
                if (employee.monthsCost[index].value > 0) {
                    totalEmployees++
                }
            }
        })

        this.costProfiles.forEach(Profile => {
            if (Profile.monthsCost[index].value) {
                if (Profile.monthsCost[index].value > 0) {
                    totalEmployees++
                }
            }
        })

        return totalEmployees
    }

    calculateTotalCosts(index) {
        var totalCost = 0;
        //Sumo el totol de los sueldos
        this.employees.forEach(employee => {
            if (employee.monthsCost[index].value) {
                totalCost += employee.monthsCost[index].value
            }
        })

        //Sumo los sueldos de los perfiles
        this.costProfiles.forEach(profile => {
            if (profile.monthsCost[index].value) {
                totalCost += profile.monthsCost[index].value
            }
        })

        //Sumo los demas gastos excepto el % de Ajuste
        this.fundedResources.forEach(resource => {
            if (resource.typeName != this.generalAdjustment) {
                totalCost += resource.monthsCost[index].value
            }
        })

        return totalCost;
    }

    calculateLoads(index) {

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
            case this.AddResource:
                this.addEmployee()
                break;

            case this.AddProfile:
                this.addProfile()
                break;

            default:
                var resource = this.otherResources.find(r => r.typeId == this.otherSelected.typeId)
                this.fundedResources.push(resource)
                break;
        }

        // if (this.otherSelected.typeName == this.AddResource) {


        //     this.addEmployee()
        // }
        // else {
        //     var resource = this.otherResources.find(r => r.typeId == this.otherSelected.typeId)
        //     this.fundedResources.push(resource)
        // }

        // var pos = this.otherResources.findIndex(r => r.typeId == this.otherResourceId);
        // this.otherResources.splice(pos, 1)

        // this.otherResourceId = this.otherResources[0].typeId;
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

        this.save()

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
            if (this.profiles.length > 0) {
                this.profileSelected = this.profiles[0];
            }
        },
            () => {
                this.messageService.closeLoading();
            })
    }

    getUsers() {
        this.messageService.showLoading();

        this.getUsersSubscrip = this.usersService.getOptions().subscribe(data => {
            this.messageService.closeLoading();

            this.users = data;
            if (this.users.length > 0) {
                this.userSelected = this.users[0];
            }
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
            case this.AddResource:
                this.showUsers = true
                this.showProfiles = false
                break;

            case this.AddProfile:
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
        if (this.openEvalPropModal.observers.length > 0) {
            month.type = 2;
            this.openEvalPropModal.emit(month);
        }
    }

    addEmployee() {

        this.messageService.showLoading();

        this.getEmployeeSubscrip = this.employeeService.getByEmail(this.userSelected.email).subscribe(response => {
            this.messageService.closeLoading();

            var existingEmployee =  this.employees.find(e => e.employeeId === response.data.id)
            if (!existingEmployee) {
                var costEmployee = {

                    employeeId: response.data.id,
                    userId: parseInt(this.userSelected.id),
                    typeName: this.typeEmployee,
                    display: `${this.userSelected.text.toUpperCase()} - ${response.data.employeeNumber}`,
                    monthsCost: this.months
                }

                this.employees.push(costEmployee)
            }
            else{
                this.messageService.showError("managementReport.existingEmployee")
            }
        },
            error => {
                this.messageService.closeLoading();
            })
    }

    addProfile() {

        var costProfile = {
            display: this.profileSelected.text,
            employeeProfileId: this.profileSelected.id,
            monthsCost: this.months
        }

        this.costProfiles.push(costProfile)
    }

}

