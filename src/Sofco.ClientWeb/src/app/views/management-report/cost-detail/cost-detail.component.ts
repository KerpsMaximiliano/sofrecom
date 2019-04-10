import { Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { ManagementReportService } from "app/services/management-report/management-report.service";
import { Subscription } from "rxjs";
import { MenuService } from "app/services/admin/menu.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { MessageService } from "app/services/common/message.service";
import * as moment from 'moment';
import { modalConfigDefaults } from "ngx-bootstrap/modal/modal-options.class";

import {  FormGroup, FormControl, Validators } from "@angular/forms";


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
    fundedResourses: any[] = new Array();
    monthSelected: any = { value: 0, display: '' };
    itemSelected: any;
    model: any;
    modalPercentage: boolean = false;
  

   

    @ViewChild('editItemModal') editItemModal;
    @ViewChild("editItemModal") modal;

    public editItemModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Editar Monto",
        "editItemModal",
        true,
        true,
        "ACTIONS.save",
        "ACTIONS.cancel"
    );

    //Constructor
    constructor(private managementReportService: ManagementReportService,
        private activatedRoute: ActivatedRoute,
        private messageService: MessageService,
        private menuService: MenuService) { }

    ngOnInit(): void {

        const control = new FormControl(16, Validators.max(15));

        console.log(control.errors); 
    

        this.modal.size = 'modal-sm'
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
            this.fundedResourses = response.data.fundedResources;
            this.employeesOriginal = response.data.costEmployees;

        },
            error => {
                this.messageService.closeLoading();
            });
    }

    openEditItemModal(month, item) {

        if (this.menuService.hasFunctionality('MANRE', 'EDIT-COST-DETAIL')) {
            this.editItemModal.show();
            this.monthSelected = month;
            this.itemSelected = item;
        }
    }

    EditItem() {
        if (this.itemSelected.monthsCost.TypeName == 'Empleados') {
            this.itemSelected.monthsCost.forEach(monthRow => {
                if (monthRow.monthYear > this.monthSelected.monthYear) {
                    monthRow.value = this.monthSelected.value;
                }
            });
        }

        this.editItemModal.hide();
        if (this.itemSelected.display == '% Ajuste') {
            this.modalPercentage = true;
        }
        else {
            this.modalPercentage = false;

        }
        // this.CalculateRows();
    }

    save() {
        this.messageService.showLoading();

        this.model.employees = this.employees;
        this.model.fundedResources = this.fundedResourses;

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

    CalculateRows() {
        if (this.itemSelected.display == '% Ajuste') {
            this.employees.forEach(employee => {
                employee.monthsCost.forEach(monthEmployee => {

                    if (monthEmployee.monthYear > this.monthSelected.monthYear) {
                        var percentage = (monthEmployee.value * this.monthSelected.value) / 100;
                        monthEmployee.value = monthEmployee.value + percentage;
                    }
                });
            })
        }
    }

    CalculateSalary(pSalary) {
        
        //var AjusteMensual = []
        var newSalary = pSalary.value;

        var AjusteMensual = this.fundedResourses.find(r => r.display == '% Ajuste');
        if (AjusteMensual) {
            var ajuste = AjusteMensual.monthsCost.find(a => a.monthYear == pSalary.monthYear)
            newSalary = pSalary.value + (pSalary.value * ajuste.value / 100);
        }

        return newSalary;
    }

}

