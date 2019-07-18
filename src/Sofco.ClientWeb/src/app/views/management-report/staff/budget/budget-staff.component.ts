import { Component, OnDestroy, OnInit, ViewChild, Output, EventEmitter } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Subscription } from "rxjs";
import { MenuService } from "app/services/admin/menu.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { MessageService } from "app/services/common/message.service";
import { FormControl, Validators } from "@angular/forms";

@Component({
    selector: 'budget-staff',
    templateUrl: './budget-staff.component.html',
    styleUrls: ['./budget-staff.component.scss']
})
export class BudgetStaffComponent implements OnInit, OnDestroy {

    months: any[] = new Array()
    readOnly: boolean = false
    categorySelected: any = { id: 0, name: '' }
    monthSelected: any = { value: 0, display: '' };

    categories: any[] = new Array()
    subCategories: any[] = new Array()
    subCategoriesFiltered: any[] = new Array()
    subCategorySelected: any = { id: 0, name: '' }
    typeBudget: string = "budget"

    costByMonth: any[] = new Array()


    @ViewChild('editItemModal') editItemModal;
    public editItemModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Editar Monto",
        "editItemModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    ngOnInit(): void {

        this.months = [
            {
                display: "May. 2019",
                monthYear: "2019-05-01"
            },
            {
                display: "Jun. 2019",
                monthYear: "2019-06-01"
            },
            {
                display: "Jul. 2019",
                monthYear: "2019-07-01"
            },
            {
                display: "Ago. 2019",
                monthYear: "2019-08-01"
            },
            {
                display: "Sep. 2019",
                monthYear: "2019-09-01"
            },
            {
                display: "Oct. 2019",
                monthYear: "2019-10-01"
            },
            {
                display: "Nov. 2019",
                monthYear: "2019-11-01"
            },
        ]

        this.categories = [
            {
                id: 1,
                name: 'Sueldos',
                months: [
                    {
                        display: "May. 2019",
                        monthYear: "2019-05-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Jun. 2019",
                        monthYear: "2019-06-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Jul. 2019",
                        monthYear: "2019-07-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Ago. 2019",
                        monthYear: "2019-08-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Sep. 2019",
                        monthYear: "2019-09-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Oct. 2019",
                        monthYear: "2019-10-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Nov. 2019",
                        monthYear: "2019-11-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                ]
            },
            {
                id: 2,
                name: 'Viáticos',
                months: [
                    {
                        display: "May. 2019",
                        monthYear: "2019-05-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Jun. 2019",
                        monthYear: "2019-06-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Jul. 2019",
                        monthYear: "2019-07-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Ago. 2019",
                        monthYear: "2019-08-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Sep. 2019",
                        monthYear: "2019-09-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Oct. 2019",
                        monthYear: "2019-10-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Nov. 2019",
                        monthYear: "2019-11-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                ]
            },
            {
                id: 3,
                name: 'Gastos Vehículos',
                months: [
                    {
                        display: "May. 2019",
                        monthYear: "2019-05-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Jun. 2019",
                        monthYear: "2019-06-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Jul. 2019",
                        monthYear: "2019-07-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Ago. 2019",
                        monthYear: "2019-08-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Sep. 2019",
                        monthYear: "2019-09-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Oct. 2019",
                        monthYear: "2019-10-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Nov. 2019",
                        monthYear: "2019-11-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                ]
            },
            {
                id: 4,
                name: 'Edificio',
                months: [
                    {
                        display: "May. 2019",
                        monthYear: "2019-05-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Jun. 2019",
                        monthYear: "2019-06-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Jul. 2019",
                        monthYear: "2019-07-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Ago. 2019",
                        monthYear: "2019-08-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Sep. 2019",
                        monthYear: "2019-09-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Oct. 2019",
                        monthYear: "2019-10-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Nov. 2019",
                        monthYear: "2019-11-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                ]
            },
            {
                id: 5,
                name: 'Seguros',
                months: [
                    {
                        display: "May. 2019",
                        monthYear: "2019-05-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Jun. 2019",
                        monthYear: "2019-06-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Jul. 2019",
                        monthYear: "2019-07-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Ago. 2019",
                        monthYear: "2019-08-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Sep. 2019",
                        monthYear: "2019-09-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Oct. 2019",
                        monthYear: "2019-10-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Nov. 2019",
                        monthYear: "2019-11-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                ]
            },
            {
                id: 5,
                name: 'Servicios',
                months: [
                    {
                        display: "May. 2019",
                        monthYear: "2019-05-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Jun. 2019",
                        monthYear: "2019-06-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Jul. 2019",
                        monthYear: "2019-07-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Ago. 2019",
                        monthYear: "2019-08-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Sep. 2019",
                        monthYear: "2019-09-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Oct. 2019",
                        monthYear: "2019-10-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Nov. 2019",
                        monthYear: "2019-11-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                ]
            },
            {
                id: 6,
                name: 'Muebles y útiles',
                months: [
                    {
                        display: "May. 2019",
                        monthYear: "2019-05-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Jun. 2019",
                        monthYear: "2019-06-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Jul. 2019",
                        monthYear: "2019-07-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Ago. 2019",
                        monthYear: "2019-08-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Sep. 2019",
                        monthYear: "2019-09-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Oct. 2019",
                        monthYear: "2019-10-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Nov. 2019",
                        monthYear: "2019-11-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                ]
            },
            {
                id: 7,
                name: 'Hardware y Software',
                months: [
                    {
                        display: "May. 2019",
                        monthYear: "2019-05-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Jun. 2019",
                        monthYear: "2019-06-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Jul. 2019",
                        monthYear: "2019-07-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Ago. 2019",
                        monthYear: "2019-08-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Sep. 2019",
                        monthYear: "2019-09-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Oct. 2019",
                        monthYear: "2019-10-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Nov. 2019",
                        monthYear: "2019-11-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                ]
            },
            {
                id: 8,
                name: 'Capacitación',
                months: [
                    {
                        display: "May. 2019",
                        monthYear: "2019-05-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Jun. 2019",
                        monthYear: "2019-06-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Jul. 2019",
                        monthYear: "2019-07-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Ago. 2019",
                        monthYear: "2019-08-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Sep. 2019",
                        monthYear: "2019-09-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Oct. 2019",
                        monthYear: "2019-10-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Nov. 2019",
                        monthYear: "2019-11-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                ]
            },
            {
                id: 9,
                name: 'Honorarios',
                months: [
                    {
                        display: "May. 2019",
                        monthYear: "2019-05-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Jun. 2019",
                        monthYear: "2019-06-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Jul. 2019",
                        monthYear: "2019-07-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Ago. 2019",
                        monthYear: "2019-08-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Sep. 2019",
                        monthYear: "2019-09-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Oct. 2019",
                        monthYear: "2019-10-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Nov. 2019",
                        monthYear: "2019-11-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                ]
            },
            {
                id: 10,
                name: 'Gastos reclutamiento personal',
                months: [
                    {
                        display: "May. 2019",
                        monthYear: "2019-05-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Jun. 2019",
                        monthYear: "2019-06-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Jul. 2019",
                        monthYear: "2019-07-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Ago. 2019",
                        monthYear: "2019-08-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Sep. 2019",
                        monthYear: "2019-09-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Oct. 2019",
                        monthYear: "2019-10-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Nov. 2019",
                        monthYear: "2019-11-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                ]
            },
            {
                id: 11,
                name: 'Red',
                months: [
                    {
                        display: "May. 2019",
                        monthYear: "2019-05-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Jun. 2019",
                        monthYear: "2019-06-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Jul. 2019",
                        monthYear: "2019-07-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Ago. 2019",
                        monthYear: "2019-08-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Sep. 2019",
                        monthYear: "2019-09-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Oct. 2019",
                        monthYear: "2019-10-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Nov. 2019",
                        monthYear: "2019-11-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                ]
            },
            {
                id: 12,
                name: 'Infraestructura',
                months: [
                    {
                        display: "May. 2019",
                        monthYear: "2019-05-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Jun. 2019",
                        monthYear: "2019-06-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Jul. 2019",
                        monthYear: "2019-07-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Ago. 2019",
                        monthYear: "2019-08-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Sep. 2019",
                        monthYear: "2019-09-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Oct. 2019",
                        monthYear: "2019-10-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                    {
                        display: "Nov. 2019",
                        monthYear: "2019-11-01",
                        budget: 0,
                        pfa1: 0,
                        pfa2: 0,
                        real: 0,
                        subCategories: []
                    },
                ]
            }
        ]
        this.subCategories = [
            { id: 1, name: 'Sueldos - Honorarios - Pasantías', idCategory: 1, Category: 'Sueldos' },
            { id: 2, name: 'Viajes y Traslados', idCategory: 2, Category: 'Viáticos' },
            { id: 3, name: 'Otros gastos ligados a Misiones', idCategory: 2, Category: 'Viáticos' },
            { id: 4, name: 'Taxi, peaje y estacionamiento', idCategory: 2, Category: 'Viáticos' },
            { id: 5, name: 'Recepciones', idCategory: 2, Category: 'Viáticos' },
            { id: 6, name: 'Amortización Rodados', idCategory: 3, Category: 'Gastos Vehículos' },
            { id: 7, name: 'Combustible', idCategory: 3, Category: 'Gastos Vehículos' },
            { id: 8, name: 'Conservación y reparación de rodados', idCategory: 3, Category: 'Gastos Vehículos' },
            { id: 9, name: 'Patentes', idCategory: 3, Category: 'Gastos Vehículos' },
            { id: 10, name: 'Seguro Rodados', idCategory: 3, Category: 'Gastos Vehículos' },
            { id: 11, name: 'Alquiler - Expensas - Limpieza', idCategory: 4, Category: 'Edificio' },
            { id: 12, name: 'Reparación y conservación de bienes inmuebles', idCategory: 4, Category: 'Edificio' },
            { id: 13, name: 'Amortizaciones instalaciones general', idCategory: 4, Category: 'Edificio' },
            { id: 14, name: 'Multiriesgo', idCategory: 5, Category: 'Seguros' },
            { id: 15, name: 'Seguro Vida Obligatorio', idCategory: 5, Category: 'Seguros' },
            { id: 16, name: 'Responsabilidad civil', idCategory: 5, Category: 'Seguros' },
            { id: 17, name: 'Caución', idCategory: 5, Category: 'Seguros' },
            { id: 18, name: 'Agua - Luz - Gas', idCategory: 5, Category: 'Servicios' },
            { id: 19, name: 'Telecom Argentina / Phillips', idCategory: 5, Category: 'Servicios' },
            { id: 20, name: 'Celulares, Nextel y otros', idCategory: 5, Category: 'Servicios' },
            { id: 21, name: 'Amortización de muebles', idCategory: 6, Category: 'Muebles y útiles' },
            { id: 22, name: 'Reparación y conservación de mobiliarios', idCategory: 6, Category: 'Muebles y útiles' },
            { id: 23, name: 'Amortización materiales de oficina', idCategory: 6, Category: 'Muebles y útiles' },
            { id: 24, name: 'Proveedores artículos de oficina', idCategory: 6, Category: 'Muebles y útiles' },
            { id: 25, name: 'Otros proveedores y materiales', idCategory: 6, Category: 'Muebles y útiles' },
            { id: 26, name: 'Fotocopias', idCategory: 6, Category: 'Muebles y útiles' },
            { id: 27, name: 'Correo', idCategory: 6, Category: 'Muebles y útiles' },
            { id: 28, name: 'Amortización materiales informática', idCategory: 7, Category: 'Hardware y Software' },
            { id: 29, name: 'Amortización de Software', idCategory: 7, Category: 'Hardware y Software' },
            { id: 30, name: 'Hardware TII', idCategory: 7, Category: 'Hardware y Software' },
            { id: 31, name: 'Mantenimiento de software', idCategory: 7, Category: 'Hardware y Software' },
            { id: 32, name: 'Mantenimiento de equipos', idCategory: 7, Category: 'Hardware y Software' },
            { id: 33, name: 'Capacitación', idCategory: 8, Category: 'Capacitación' },
            { id: 34, name: 'Honorarios ', idCategory: 9, Category: 'Honorarios ' },
            { id: 35, name: 'Gastos reclutamiento personal', idCategory: 10, Category: 'Gastos reclutamiento personal' },
            { id: 36, name: 'Red', idCategory: 11, Category: 'Red' },
            { id: 37, name: 'Infraestructura', idCategory: 12, Category: 'Infraestructura' }
        ]

        this.subCategoriesFiltered = this.subCategories

    }

    ngOnDestroy(): void {

    }

    openEditItemModal(category, typeBudget, month, item) {

        if (this.readOnly) return;

        if (month.closed) return;

        this.categorySelected = category
       //this.subCategorySelected = 
        this.subCategoriesFiltered = this.subCategories.filter(x => x.idCategory == this.categorySelected.id)
         if (this.subCategoriesFiltered.length > 0) {
             this.subCategorySelected = this.subCategoriesFiltered[0];
         }
        this.monthSelected = month;
        this.typeBudget = typeBudget
        this.editItemModal.show();
    }

    addCostByMonth() {

        var cost = {
            id: 0,
            CostDetailId: 0,
            idCategory: this.categorySelected.id,
            nameCategory: this.categorySelected.name,
            idsubCategory: this.subCategorySelected.id,
            nameSubCategory: this.subCategorySelected.name,
            monthYear: this.monthSelected.monthYear,
            description: "",
            value: 0
        }

        this.monthSelected.subCategories.push(cost)
    }

    updateItem() {

        this.monthSelected.subCategories.forEach(cost => {
            switch (this.typeBudget) {
                case 'budget':
                    this.monthSelected.budget += cost.value
                    break;
                case 'pfa1':
                    this.monthSelected.pfa1 += cost.value
                    break;
                case 'pfa2':
                    this.monthSelected.pfa2 += cost.value
                    break;
                case 'real':
                    this.monthSelected.real += cost.value
                    break;

                default:
                    break;
            }
        });

        //this.monthSelected.subCategories = this.costByMonth

        this.editItemModal.hide()
    }

    calculateTotalCosts(month, typeBudget) {
        const index = this.months.findIndex(cost => cost.monthYear === month.monthYear);
        var totalCost = 0;

        this.categories.forEach(category => {
            switch (typeBudget) {
                case 'budget':
                    if (category.months[index].budget) {
                        totalCost += category.months[index].budget;
                    }
                    break;
                case 'pfa1':
                    if (category.months[index].pfa1) {
                        totalCost += category.months[index].budget;
                    }
                    break;
                case 'pfa2':
                    if (category.months[index].pfa2) {
                        totalCost += category.months[index].budget;
                    }
                    break;
                case 'real':
                    if (category.months[index].real) {
                        totalCost += category.months[index].budget;
                    }
                    break;

                default:
                    break;
            }
        });

        return totalCost
    }
}