import { Component, OnDestroy, OnInit, ViewChild, Output, EventEmitter } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Subscription, VirtualTimeScheduler } from "rxjs";
import { MenuService } from "app/services/admin/menu.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { MessageService } from "app/services/common/message.service";
import { FormControl, Validators } from "@angular/forms";
import { months } from "moment";
import { DebugContext } from "@angular/core/src/view";
import { ManagementReportStaffService } from "app/services/management-report/management-report-staff.service";
import { ManagementReportDetailStaffComponent } from "app/views/management-report/staff/detail/detail-staff";

@Component({
    selector: 'budget-staff',
    templateUrl: './budget-staff.component.html',
    styleUrls: ['./budget-staff.component.scss']
})
export class BudgetStaffComponent implements OnInit, OnDestroy {

    paramsSubscrip: Subscription;
    getCostSubscrip: Subscription;
    updateCostSubscrip: Subscription;

    model: any
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
    typeBudgetSelected: any = { id: 0, name: '' }
    budgetTypes: any[] = new Array()
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
    constructor(private ManagementReportStaffService: ManagementReportStaffService,
        private activatedRoute: ActivatedRoute,
        private messageService: MessageService,
        private menuService: MenuService,
        private detailStaffComponent: ManagementReportDetailStaffComponent
    ) { }

    ngOnInit(): void {

        this.editItemModal.size = 'modal-lg'

        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.managementReportId = params['id'];
            this.getCost(this.managementReportId);
        });
    }

    ngOnDestroy(): void {
        if (this.getCostSubscrip) this.getCostSubscrip.unsubscribe();
        if (this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if (this.updateCostSubscrip) this.updateCostSubscrip.unsubscribe();
    }

    getCost(managementReportId) {
        this.messageService.showLoading();

        this.getCostSubscrip = this.ManagementReportStaffService.getCostDetailStaff(managementReportId).subscribe(response => {
            this.messageService.closeLoading();

            this.model = response.data
            this.months = response.data.monthsHeader;
            this.categories = response.data.costCategories;
            this.subCategories = response.data.allSubcategories;
            this.budgetTypes = response.data.budgetTypes;

            this.subCategoriesFiltered = this.subCategories

            this.calculateTotalCosts()
            this.sendDataToDetailView();
        },
            () => this.messageService.closeLoading());
    }

    openEditItemModal(category, typeBudget, month, item) {

        if (this.readOnly) return;

        if (month.closed) return;

        this.categorySelected = category
        this.monthSelected = month;
        this.typeBudgetSelected = this.budgetTypes.find(x => x.name.toUpperCase() == typeBudget.toUpperCase())

        this.subCategoriesFiltered = this.subCategories.filter(x => x.idCategory == this.categorySelected.id)
        if (this.subCategoriesFiltered.length > 0) {
            this.subCategorySelected = this.subCategoriesFiltered[0];
        }

        var subData = this.fillSubcategoriesData()
        this.subCategoriesData = new Array()
        subData.forEach(subcat => {

            var cost = {
                costDetailStaffId: subcat.costDetailStaffId,
                id: subcat.id,
                name: subcat.name,
                description: subcat.description,
                value: subcat.value,
                budgetTypeId: subcat.budgetTypeId,
                deleted: subcat.deleted
            }

            this.subCategoriesData.push(cost)
        });
    }

    fillSubcategoriesData() {


        switch (this.typeBudgetSelected.name.toUpperCase()) {
            case 'BUDGET':
                if (this.monthSelected.totalPfa1 > 0) {
                    this.messageService.showError("cannotUpdateBudget");
                }
                else {
                    this.editItemModal.show();
                    return this.monthSelected.subcategoriesBudget.filter(sub => sub.deleted == false);
                }

                break;
            case 'PFA1': {
                if (this.monthSelected.totalBudget == 0) {
                    this.messageService.showError("budgetRequired");
                }
                else {
                    if (this.monthSelected.totalPfa2 > 0) {
                        this.messageService.showError("cannotUpdatePfa1");
                    }
                    else {
                        this.editItemModal.show();
                        return this.monthSelected.subcategoriesPfa1.filter(sub => sub.deleted == false);
                    }
                }

                break;
            }
            case 'PFA2': {
                if (this.monthSelected.totalBudget == 0 || this.monthSelected.totalPfa1 == 0) {
                    this.messageService.showError("pfa1Required");
                }
                else {
                    this.editItemModal.show();
                    return this.monthSelected.subcategoriesPfa2.filter(sub => sub.deleted == false);
                }
                break;
            }
            case 'REAL':
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
            deleted: false
        }

        this.subCategoriesData.push(cost)
    }

    updateItem() {

        switch (this.typeBudgetSelected.name.toUpperCase()) {
            case 'BUDGET':
                this.monthSelected.subcategoriesBudget = this.subCategoriesData
                this.monthSelected.totalBudget = 0
                this.monthSelected.subcategoriesBudget.forEach(cost => {
                    this.monthSelected.totalBudget += cost.value
                })
                break;
            case 'PFA1':
                this.monthSelected.subcategoriesPfa1 = this.subCategoriesData
                this.monthSelected.totalPfa1 = 0
                this.monthSelected.subcategoriesPfa1.forEach(cost => {
                    this.monthSelected.totalPfa1 += cost.value
                })
                break;
            case 'PFA2':
                this.monthSelected.subcategoriesPfa2 = this.subCategoriesData
                this.monthSelected.totalPfa2 = 0
                this.monthSelected.subcategoriesPfa2.forEach(cost => {
                    this.monthSelected.totalPfa2 += cost.value
                })
                break;
            case 'REAL':
                this.monthSelected.subcategoriesReal = this.subCategoriesData
                this.monthSelected.totalReal = 0
                this.monthSelected.subcategoriesReal.forEach(cost => {
                    this.monthSelected.totalReal += cost.value
                })
                break;
        }

        this.calculateTotalCosts()
        this.sendDataToDetailView();
        this.editItemModal.hide()
    }

    calculateTotalCosts() {

        this.months.forEach(month => {
            let index = this.months.findIndex(cost => cost.monthYear === month.monthYear);
            let monthTotal = this.months.find(m => m.monthYear === month.monthYear)
            var totalCostBugdet = 0;
            var totalCostPfa1 = 0;
            var totalCostPfa2 = 0;
            var totalCostReal = 0;

            this.categories.forEach(category => {
                if (category.monthsCategory[index].totalBudget) {
                    totalCostBugdet += category.monthsCategory[index].totalBudget;
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
            });

            monthTotal.totalBudget = totalCostBugdet
            monthTotal.totalPfa1 = totalCostPfa1
            monthTotal.totalPfa2 = totalCostPfa2
            monthTotal.totalReal = totalCostReal
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
                categories: this.categories
            });
        }
    }

    save() {
        this.messageService.showLoading();

        this.updateCostSubscrip = this.ManagementReportStaffService.PostCostDetailStaff(this.model).subscribe(() => {
            this.messageService.closeLoading();

            this.detailStaffComponent.getDetail()

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

}