import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { Router, ActivatedRoute } from "@angular/router";
import { MenuService } from "../../../../services/admin/menu.service";
import { MessageService } from "../../../../services/common/message.service";
import { AllocationService } from "../../../../services/allocation-management/allocation.service";
import { Ng2ModalConfig } from "../../../../components/modal/ng2modal-config";
import { EmployeeService } from "../../../../services/allocation-management/employee.service";
import { UserService } from "../../../../services/admin/user.service";
import { CategoryService } from "../../../../services/admin/category.service";
import { DataTableService } from "app/services/common/datatable.service";

declare var $: any;

@Component({
    selector: 'resource-by-service',
    templateUrl: './resource-by-service.component.html',
    styleUrls: ['./resource-by-service.component.scss']
})

export class ResourceByServiceComponent implements OnInit, OnDestroy {

    @ViewChild('confirmModal') confirmModal;
    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "confirmModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    @ViewChild('categoriesModal') categoriesModal;
    public categoriesModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ADMIN.category.list",
        "categoriesModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    public resources: any[] = new Array<any>();
    public categories: any[] = new Array<any>();
    public users: any[] = new Array<any>();
    
    public analyticName: string;
    public analyticId: number;

    public endDate: Date = new Date();
    public resourceSelected: any;

    getAllSubscrip: Subscription;
    paramsSubscrip: Subscription;
    getUsersSubscrip: Subscription;
    getAllEmployeesSubscrip: Subscription;
    getCategorySubscrip: Subscription;
    addCategoriesSubscrip: Subscription;
    public pendingWorkingHours = false;

    constructor(private router: Router,
                public menuService: MenuService,
                private messageService: MessageService,
                private employeeService: EmployeeService,
                private usersService: UserService,
                private dataTableService: DataTableService,
                private categoryService: CategoryService,
                private activatedRoute: ActivatedRoute,
                private allocationervice: AllocationService){
    }

    ngOnInit(): void {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.analyticId = params['id'];
            this.analyticName = sessionStorage.getItem('analyticName');
            this.getAll();
          });

        this.getUsersSubscrip = this.usersService.getOptions().subscribe(data => {  
            this.users = data;
        });

        this.getCategories();
    }

    ngOnDestroy(): void {
        if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.getAllEmployeesSubscrip) this.getAllEmployeesSubscrip.unsubscribe();
        if(this.getUsersSubscrip) this.getUsersSubscrip.unsubscribe();
        if(this.getCategorySubscrip) this.getCategorySubscrip.unsubscribe();
        if(this.addCategoriesSubscrip) this.addCategoriesSubscrip.unsubscribe();
    }

    initGrid(){
        var options = { 
            selector: "#resourcesTable",
         };

        this.dataTableService.destroy(options.selector);
        this.dataTableService.initialize(options);
    }
 
    getAll(){
        this.messageService.showLoading();

        this.getAllSubscrip = this.allocationervice.getAllocationsByAnalytic(this.analyticId).subscribe(data => {
            this.resources = data;
            this.messageService.closeLoading();

            this.initGrid();
        },
        () => this.messageService.closeLoading());
    }
 
    goToProfile(resource){
        this.router.navigate([`/allocationManagement/resources/${resource.id}`]);
    }

    canViewProfile(){
        return this.menuService.hasFunctionality('PROFI', 'VWPRO');
    }

    canSendUnsubscribeNotification(){
        return this.menuService.hasFunctionality('ALLOC', 'NUNEM');
    }

    canAssingCategories(){
        return this.menuService.hasFunctionality('ALLOC', 'EMP-CAT');
    }

    openModal(data) {
        this.pendingWorkingHours = data.pendingHours > 0;
        this.confirmModal.show();
    }

    openEndEmployeeModal(resource) {
        this.resourceSelected = resource;
        this.getAllEmployeesSubscrip = this.employeeService.getPendingHours(resource.id).subscribe(res => {
            this.openModal(res.data);
        },
        () => this.confirmModal.hide());
    }

    openCategoryModal(resource){
        this.resourceSelected = resource;

        this.categories.forEach(item => {
            item.selected = false;

            if(resource.categories.includes(item.id)){
                item.selected = true;
            }
        });

        this.categoriesModal.show();
    }

    sendUnsubscribeNotification(){
        const model = {
            employeeId: this.resourceSelected.id,
            employeeName: this.resourceSelected.name,
            recipients: $('#userId').val(),
            endDate: this.endDate
        };

        this.getAllEmployeesSubscrip = this.employeeService.sendUnsubscribeNotification(model).subscribe(data => {
            this.confirmModal.hide();
        },
        error => this.confirmModal.hide());
    }

    getCategories(){
        this.getCategorySubscrip = this.categoryService.getOptions().subscribe(
            data => {
                this.categories = data;
            });
    }

    addCategoryDisabled(){
        return this.resources.filter(x => x.selected == true).length == 0;
    }

    saveCategories(){
        var categoriesSelected = this.categories.filter(x => x.selected == true).map(item => item.id);
        var categoriesNotSelected = this.categories.filter(x => x.selected == false).map(item => item.id);
        var usersSelected = [this.resourceSelected.id];

        if(categoriesSelected.length == 0) return;

        var json = {
            categoriesToAdd: categoriesSelected,
            categoriesToRemove: categoriesNotSelected,
            employees: usersSelected
        }

        this.addCategoriesSubscrip = this.employeeService.addCategories(json).subscribe(response => {
            this.messageService.closeLoading();
            this.categoriesModal.hide();

            var user = this.resources.find(x => x.id == this.resourceSelected.id);

            if(user){
                user.categories = [];
                categoriesSelected.forEach(item => {
                    user.categories.push(item);
                });
            }
        },
        () => {
            this.messageService.closeLoading();
            this.categoriesModal.hide();
        });
    }

    canAddCategories(){
        return this.menuService.userIsDirector || this.menuService.userIsManager;
    }
}