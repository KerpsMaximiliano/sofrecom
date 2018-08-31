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

declare var $: any;

@Component({
    selector: 'resource-by-analytic',
    templateUrl: './resource-by-analytic.component.html',
    styleUrls: ['./resource-by-analytic.component.scss']
})

export class ResourceByAnalyticComponent implements OnInit, OnDestroy {

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
    analyticId: number;

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
 
    getAll(){
        this.messageService.showLoading();

        this.getAllSubscrip = this.allocationervice.getAllocationsByAnalytic(this.analyticId).subscribe(data => {
            this.resources = data;
            this.messageService.closeLoading();
        },
        () => this.messageService.closeLoading());
    }
 
    goToProfile(resource){
        this.router.navigate([`/allocationManagement/resources/${resource.id}`]);
    }

    canViewProfile(){
        return this.menuService.hasFunctionality('ALLOC', 'VWPRO');
    }

    goToAssignAnalytics(resource){
        sessionStorage.setItem("resource", JSON.stringify(resource));
        this.router.navigate([`/allocationManagement/resources/${resource.id}/allocations`]);
    }

    canSendUnsubscribeNotification(){
        return this.menuService.hasFunctionality('ALLOC', 'NUNEM');
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
        this.categoriesModal.show();
    }

    sendUnsubscribeNotification(){
        var json = {
            receipents: $('#userId').val(),
            endDate: this.endDate
        }

        this.getAllEmployeesSubscrip = this.employeeService.sendUnsubscribeNotification(this.resourceSelected.name, json).subscribe(data => {
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
        var usersSelected = [this.resourceSelected.id];

        if(categoriesSelected.length == 0) return;

        var json = {
            categories: categoriesSelected,
            employees: usersSelected
        }

        this.addCategoriesSubscrip = this.employeeService.addCategories(json).subscribe(response => {
            this.messageService.closeLoading();
            this.categoriesModal.hide();
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