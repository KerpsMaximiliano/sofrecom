import { Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { ManagementReportService } from "app/services/management-report/management-report.service";
import { Subscription } from "rxjs";
import { MenuService } from "app/services/admin/menu.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { MessageService } from "app/services/common/message.service";
import * as moment from 'moment';
import { modalConfigDefaults } from "ngx-bootstrap/modal/modal-options.class";

import { FormGroup, FormControl, Validators, FormsModule } from "@angular/forms";


@Component({
    selector: 'cost-detail',
    templateUrl: './cost-detail.component.html',
    styleUrls: ['./cost-detail.component.scss']
})
export class CostDetailComponent implements OnInit, OnDestroy {

    paramsSubscrip: Subscription;
    getCostSubscrip: Subscription;
    updateCostSubscrip: Subscription;

    //Propiedades
    serviceId: string;
    months: any[] = new Array();
    employees: any[] = new Array();
    employeesOriginal: any[] = new Array();
    fundedResources: any[] = new Array();
    otherResources: any[] = new Array();
    monthSelected: any = { value: 0, display: '' };
    itemSelected: any;
    indexSelected: number = 0;
    model: any;
    modalPercentage: boolean = false;
    modalEmployee: boolean = false;
    editItemMonto = new FormControl();
    editItemAdjustment = new FormControl();
    canEdit: boolean = false;

    otherResourceId: number;

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
        private menuService: MenuService) { }

    ngOnInit(): void {

        if (this.menuService.hasFunctionality('MANRE', 'EDIT-COST-DETAIL')) {
            this.canEdit = true
        }

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

            if (this.otherResources.length > 0) {
                this.otherResourceId = this.otherResources[0].typeId;
            }

        },
            error => {
                this.messageService.closeLoading();
            });

    }

    openEditItemModal(month, item, indexMonth) {

        if (this.canEdit) {
            if (item.typeName == 'Empleados' && !month.hasAlocation) {
                return false
            }
            else {
                this.editItemModal.show();
                this.monthSelected = month;
                this.indexSelected = indexMonth;
                this.itemSelected = item;
                this.editItemMonto.setValidators([Validators.min(0), Validators.max(999999)]);
                this.modalPercentage = false

                if (this.itemSelected.typeName == 'Empleados') {
                    this.modalEmployee = true
                    this.editItemMonto.setValue(month.originalValue)
                    this.editItemAdjustment.setValue(month.adjustment)
                    this.editItemAdjustment.setValidators([Validators.min(0), Validators.max(999)]);
                }
                else {
                    this.modalEmployee = false
                    this.editItemMonto.setValue(month.value)

                    if (this.itemSelected.typeName == '% Ajuste') {
                        this.editItemMonto.setValidators([Validators.min(0), Validators.max(999)]);
                        this.modalPercentage = true
                    }
                }
            }
        }

    }

    EditItem() {

        this.monthSelected.value = this.editItemMonto.value
        debugger
        //Si estoy editando un empleado se actualiza el sueldo para los meses que siguen
        if (this.itemSelected.typeName == 'Empleados') {

        this.monthSelected.originalValue = this.editItemMonto.value
            if (this.editItemAdjustment.value > 0) {
                this.monthSelected.adjustment = this.editItemAdjustment.value
                this.monthSelected.value = this.monthSelected.originalValue + this.monthSelected.originalValue * this.monthSelected.adjustment / 100
            }
            else {
                this.monthSelected.value = this.editItemMonto.value
            }

            for (let index = this.indexSelected + 1; index < this.itemSelected.monthsCost.length; index++) {

                if (this.itemSelected.monthsCost[index].hasAlocation) {
                    this.itemSelected.monthsCost[index].value = this.monthSelected.value;
                    this.itemSelected.monthsCost[index].originalValue = this.monthSelected.value;
                }
                else {
                    this.itemSelected.monthsCost[index].value = 0
                }
            }

            //Actualiza el sueldo
            this.salaryPlusIncrease(this.itemSelected, this.indexSelected + 1);
        }

        //Si estoy editando un aumento se actualiza el sueldo para todos los empleados
        if (this.itemSelected.typeName == '% Ajuste') {
            this.modalPercentage = true;
            this.employees.forEach(employee => {
                this.salaryPlusIncrease(employee, this.indexSelected);
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

        this.updateCostSubscrip = this.managementReportService.PostCostDetail(this.serviceId, this.model).subscribe(response => {
            this.messageService.closeLoading();

        },
            error => {
                this.messageService.closeLoading();
            });
    }

    getResourcesByMonth(month, year) {

        return this.employees.map(element => {

            var monthCost = element.monthsCost.find(x => {
                var dateSplitted = x.monthYear.split("-");

                if (dateSplitted[0] == year && dateSplitted[1] == month) {
                    return x;
                }
            });

            return {
                name: element.display,
                salary: monthCost.value || 0,
                charges: 0,
                total: monthCost.value || 0
            }
        });
    }


    CalculateSalary(monthData, index) {
        var SalaryPlusIncrese = monthData.value;

        var AjusteMensual = this.fundedResources.find(r => r.typeName == '% Ajuste');
        if (AjusteMensual) {
            if (AjusteMensual.monthsCost[index].value > 0) {
                SalaryPlusIncrese = monthData.value + (monthData.value * AjusteMensual.monthsCost[index].value / 100);
            }
        }
        return SalaryPlusIncrese;
    }

    salaryPlusIncrease(employee, pIndex) {
        //Verifico que exista la fila de ajustes
        var AjusteMensual = this.fundedResources.find(r => r.display == '% Ajuste');
        if (AjusteMensual) {
            //Si existe, Recorro todos los meses
            //El nuevo salario lo seteo como el primer salario
            let newSalary = employee.monthsCost[pIndex].value

            for (let index = pIndex; index < employee.monthsCost.length; index++) {
                
                //Verifico si tiene aumento en alguno
                if (AjusteMensual.monthsCost[index].value > 0) {
                    newSalary = employee.monthsCost[index].originalValue + (employee.monthsCost[index].originalValue * AjusteMensual.monthsCost[index].value / 100);
                }
                else {
                    newSalary = employee.monthsCost[index].originalValue
                }

                if (employee.monthsCost[index].hasAlocation) {
                    employee.monthsCost[index].value = newSalary;
                    employee.monthsCost[index].adjustment = AjusteMensual.monthsCost[index].value
                }

                if (employee.monthsCost[index + 1]) {
                    if (employee.monthsCost[index + 1].hasAlocation) {
                        employee.monthsCost[index + 1].value = newSalary;
                        employee.monthsCost[index + 1].originalValue = newSalary;
                    }
                }
            }
        }
    }

    calculateAllSalary(index) {
        var totalSalary = 0;
        this.employees.forEach(employee => {
            if (employee.monthsCost[index].value) {
                totalSalary += this.CalculateSalary(employee.monthsCost[index], index)
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

        return totalEmployees
    }

    calculateTotalCosts(index) {
        var totalCost = 0;
        //Sumo el totol de los sueldos
        this.employees.forEach(employee => {
            if (employee.monthsCost[index].value) {
                totalCost += this.CalculateSalary(employee.monthsCost[index], index)
            }
        })
        //Sumo los demas gastos excepto el % de Ajuste
        this.fundedResources.forEach(resource => {
            if (resource.typeName != '% Ajuste') {
                totalCost += resource.monthsCost[index].value
            }
        })

        return totalCost;
    }

    calculateLoads(index) {

        var totalSalary = 0;
        this.employees.forEach(employee => {
            if (employee.monthsCost[index].value) {
                totalSalary += this.CalculateSalary(employee.monthsCost[index], index)
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
                cssClass = 'not-allowed label-danger'
            }
        }

        return cssClass;
    }

    addOtherCost() {

        var resource = this.otherResources.find(r => r.typeId == this.otherResourceId)
        this.fundedResources.push(resource)

        var pos = this.otherResources.findIndex(r => r.typeId == this.otherResourceId);
        this.otherResources.splice(pos, 1)

        this.otherResourceId = this.otherResources[0].typeId;
    }

    resourcesClass(monthCost, item) {

        let cssClass;
        if (!this.canEdit) {
            cssClass = 'not-allowed'
        }
        else {
            cssClass = 'cursor-pointer'
            if (item.display == '% Ajuste') {
                if (monthCost.value > 0) {
                    cssClass += ' label-yellow'
                }
            }
        }

        return cssClass;
    }

}

