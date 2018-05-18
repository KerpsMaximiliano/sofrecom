import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Router, ActivatedRoute } from "@angular/router";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from "app/services/common/message.service";
import { AllocationService } from "app/services/allocation-management/allocation.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { EmployeeService } from "../../../../services/allocation-management/employee.service";
import { UserService } from "../../../../services/admin/user.service";
import { CategoryService } from "../../../../services/admin/category.service";

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
    public serviceName: string;
    public customerName: string;
    customerId: string;
    serviceId: string;

    public endDate: Date = new Date();
    public resourceSelected: any;

    getAllSubscrip: Subscription;
    paramsSubscrip: Subscription;
    getUsersSubscrip: Subscription;
    getAllEmployeesSubscrip: Subscription;
    getCategorySubscrip: Subscription;
    addCategoriesSubscrip: Subscription;

    constructor(private router: Router,
                public menuService: MenuService,
                private messageService: MessageService,
                private employeeService: EmployeeService,
                private usersService: UserService,
                private categoryService: CategoryService,
                private activatedRoute: ActivatedRoute,
                private allocationervice: AllocationService,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.customerId = params['customerId'];
            this.serviceId = params['serviceId'];
            this.customerName = sessionStorage.getItem('customerName');
            this.serviceName = sessionStorage.getItem('serviceName');
            this.getAll();
          });

        this.getUsersSubscrip = this.usersService.getOptions().subscribe(data => {  
            this.users = data;
        },
        error => this.errorHandlerService.handleErrors(error));

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

        this.getAllSubscrip = this.allocationervice.getAllocationsByService(this.serviceId).subscribe(data => {
            this.resources = data;
            this.messageService.closeLoading();
        },
        error => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(error)
        });
    }
 
    goToServices(){
        this.router.navigate([`/billing/customers/${this.customerId}/services`]);
    }

    goToProjects(){
        sessionStorage.setItem("customerId", this.customerId);
        sessionStorage.setItem("serviceId", this.serviceId);
        
        this.router.navigate([`/billing/customers/${this.customerId}/services/${this.serviceId}/projects`]);
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

    openModal(resource){
        this.resourceSelected = resource;
        this.confirmModal.show();
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
            if(data.messages) this.messageService.showMessages(data.messages);
        },
        error => {
            this.confirmModal.hide();
            this.errorHandlerService.handleErrors(error)
        });
    }

    getCategories(){
        this.getCategorySubscrip = this.categoryService.getOptions().subscribe(
            data => {
                this.categories = data;
            },
            err => this.errorHandlerService.handleErrors(err));
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

            this.messageService.showMessages(response.messages);
        },
        error => {
            this.messageService.closeLoading();
            this.categoriesModal.hide();
            this.errorHandlerService.handleErrors(error)
        });
    }

    canAddCategories(){
        return this.menuService.userIsDirector || this.menuService.userIsManager;
    }
}