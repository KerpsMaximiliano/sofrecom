import { Component, OnDestroy, OnInit, ViewChild, Output, EventEmitter } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Subscription } from "rxjs";
import { MenuService } from "app/services/admin/menu.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { MessageService } from "app/services/common/message.service";
import { FormControl, Validators } from "@angular/forms";
import { months } from "moment";
import { DebugContext } from "@angular/core/src/view";
import { ManagementReportService } from "app/services/management-report/management-report.service";

@Component({
    selector: 'budget-staff',
    templateUrl: './budget-staff.component.html',
    styleUrls: ['./budget-staff.component.scss']
})
export class BudgetStaffComponent implements OnInit, OnDestroy {

    paramsSubscrip: Subscription;
    getCostSubscrip: Subscription;

    managementReportId: string;
    months: any[] = new Array()
    readOnly: boolean = false
    categorySelected: any = { id: 0, name: '' }
    monthSelected: any = { value: 0, display: '' };

    categories: any[] = new Array()
    subCategories: any[] = new Array()
    subCategoriesData: any[] = new Array()
    subCategoriesFiltered: any[] = new Array()
    subCategorySelected: any = { id: 0, name: '' }
    typeBudget: string = "BUDGET"
    costByMonth: any[] = new Array()

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

    
    //Constructor
    constructor(private managementReportService: ManagementReportService,
        private activatedRoute: ActivatedRoute,
        private messageService: MessageService,
        private menuService: MenuService,
    ) { }

    ngOnInit(): void {

        this.editItemModal.size = 'modal-lg'

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


        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.managementReportId = params['id'];

            this.getCost();
        });
    }

    ngOnDestroy(): void {
        if (this.getCostSubscrip) this.getCostSubscrip.unsubscribe();
        if (this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
    }

    getCost() {
        this.messageService.showLoading();

        this.getCostSubscrip = this.managementReportService.getCostDetailStaff(this.managementReportId).subscribe(response => {
            this.messageService.closeLoading();

            //this.model = response.data;

            console.log(response.data)
            this.months = response.data.monthsHeader;
            this.categories = response.data.costCategories;

            console.log(this.categories)
            // this.employees = response.data.costEmployees;
            // this.fundedResources = response.data.fundedResources;
            // this.otherResources = response.data.otherResources;
            // this.employeesOriginal = response.data.costEmployees;
            // this.costProfiles = response.data.costProfiles;

            // if (this.otherResources.length > 0) {
            //     this.otherSelected = this.otherResources[0];
            // }

            this.sendDataToDetailView();
        },
            () => this.messageService.closeLoading());
    }

    openEditItemModal(category, typeBudget, month, item) {

        if (this.readOnly) return;

        if (month.closed) return;
     
        this.categorySelected = category
        this.monthSelected = month;
        this.typeBudget = typeBudget

        this.subCategoriesFiltered = this.subCategories.filter(x => x.idCategory == this.categorySelected.id)
        if (this.subCategoriesFiltered.length > 0) {
            this.subCategorySelected = this.subCategoriesFiltered[0];
        }

        this.editItemModal.show();
        //this.fillSubCategories()

        switch (this.typeBudget.toUpperCase()) {
            case 'BUDGET':
                this.subCategoriesData = this.monthSelected.subcategoriesBudget
                break;
            case 'PFA1':
                this.subCategoriesData = this.monthSelected.subcategoriesPfa1
                break;
            case 'PFA2':
                this.subCategoriesData = this.monthSelected.subcategoriesPfa2
                break;
            case 'REAL':
                this.subCategoriesData = this.monthSelected.subcategoriesReal
                break;
        }
        // this.subCategoriesData = this.monthSelected.subcategories
        // console.log(this.subCategoriesData)
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

        switch (this.typeBudget) {
            case 'budget':
                this.monthSelected.budget = 0
                break;
            case 'pfa1':
                this.monthSelected.pfa1 = 0
                break;
            case 'pfa2':
                this.monthSelected.pfa2 = 0
                break;
            case 'real':
                this.monthSelected.real = 0
                break;
        }

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

        this.calculateTotalCosts(this.monthSelected)
        this.editItemModal.hide()
        this.sendDataToDetailView();
    }

    calculateTotalCosts(month) {

        //  this.months.forEach(month => {

        let index = this.months.findIndex(cost => cost.monthYear === month.monthYear);
        let monthTotal = this.months.find(m => m.monthYear === month.monthYear)
        var totalCostBugdet = 0;
        var totalCostPfa1 = 0;
        var totalCostPfa2 = 0;
        var totalCostReal = 0;

        this.categories.forEach(category => {
            if (category.months[index].budget) {
                totalCostBugdet += category.months[index].budget;
            }
            if (category.months[index].pfa1) {
                totalCostPfa1 += category.months[index].pfa1;
            }
            if (category.months[index].pfa2) {
                totalCostPfa2 += category.months[index].pfa2;
            }
            if (category.months[index].real) {
                totalCostReal += category.months[index].real;
            }
        });

        monthTotal.totalBudget = totalCostBugdet
        monthTotal.totalPfa1 = totalCostPfa1
        monthTotal.totalPfa2 = totalCostPfa2
        monthTotal.totalReal = totalCostReal
        // })
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
                    typeBudget: this.typeBudget
                }

                this.monthSelected.subCategories.push(cost)
            }
        })
    }

    sendDataToDetailView() {
        if (this.getData.observers.length > 0) {
            this.getData.emit({
                months: this.months,
                categories: this.categories
            });
        }
    }

}