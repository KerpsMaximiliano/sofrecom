import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Router } from "@angular/router";
import { DataTableService } from "app/services/common/datatable.service";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from "app/services/common/message.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { UserService } from "app/services/admin/user.service";
import { AnalyticService } from "../../../../services/allocation-management/analytic.service";
import { CategoryService } from "../../../../services/admin/category.service";

declare var moment: any;
declare var $: any;

@Component({
    selector: 'resource-search',
    templateUrl: './resource-search.component.html'
})
export class ResourceSearchComponent implements OnInit, OnDestroy {

    @ViewChild('accordion') accordion;

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

    public model: any[] = new Array<any>();
    public resources: any[] = new Array<any>();
    public users: any[] = new Array<any>();
    public analytics: any[] = new Array<any>();
    public categories: any[] = new Array<any>();
    public resource: any;
    public resourceSelected: any;

    public searchModel = {
        name: "",
        seniority: "",
        profile: "",
        technology: "",
        percentage: null,
        analyticId: 0,
        employeeNumber: ""
    };

    public endDate: Date = new Date();
    public isLoading = false;
    public pendingWorkingHours = false;

    getAllSubscrip: Subscription;
    getAllEmployeesSubscrip: Subscription;
    searchSubscrip: Subscription;
    getUsersSubscrip: Subscription;
    getAnalyticSubscrip: Subscription;
    getCategorySubscrip: Subscription;
    addCategoriesSubscrip: Subscription;
    subscrip: Subscription;

    constructor(private router: Router,
                public menuService: MenuService,
                private messageService: MessageService,
                private employeeService: EmployeeService,
                private usersService: UserService,
                private categoryService: CategoryService,
                private analyticService: AnalyticService,
                private dataTableService: DataTableService,
                private errorHandlerService: ErrorHandlerService) {
    }

    ngOnInit(): void {
        this.getUsersSubscrip = this.usersService.getOptions().subscribe(data => {
            this.users = data;
        },
        error => this.errorHandlerService.handleErrors(error));

        this.getAnalytics();
        this.getCategories();
    }

    ngOnDestroy(): void {
        if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
        if(this.searchSubscrip) this.searchSubscrip.unsubscribe();
        if(this.getAnalyticSubscrip) this.getAnalyticSubscrip.unsubscribe();
        if(this.getCategorySubscrip) this.getCategorySubscrip.unsubscribe();
        if(this.addCategoriesSubscrip) this.addCategoriesSubscrip.unsubscribe();
    }

    goToAssignAnalytics(resource){
        sessionStorage.setItem("resource", JSON.stringify(resource));
        this.router.navigate([`/allocationManagement/resources/${resource.id}/allocations`]);
    }

    canSendUnsubscribeNotification(){
        return this.menuService.hasFunctionality('ALLOC', 'NUNEM');
    }

    openEndEmployeeModal(resource) {
        this.resourceSelected = resource;
        this.subscrip = this.employeeService.getPendingHours(resource.id).subscribe(res => {
            this.showEndEmployeeModal(res.data);
        },
        error => {
            this.confirmModal.hide();
            this.errorHandlerService.handleErrors(error)
        });
    }

    showEndEmployeeModal(data) {
        this.pendingWorkingHours = data.pendingHours > 0;
        this.confirmModal.show();
    }

    sendUnsubscribeNotification() {
        const json = {
            receipents: $('#userId').val(),
            endDate: this.endDate
        }

        this.getAllEmployeesSubscrip = this.employeeService.sendUnsubscribeNotification(this.resourceSelected.name, json).subscribe(data => {
            this.confirmModal.hide();
            if (data.messages) this.messageService.showMessages(data.messages);
        },
        error => {
            this.confirmModal.hide();
            this.errorHandlerService.handleErrors(error)
        });
    }

    getAnalytics(){
        this.getAnalyticSubscrip = this.employeeService.getAnalytics(this.menuService.user.id).subscribe(
            data => {
                this.analytics = data;

                this.setLastQuery();
            },
            err => this.errorHandlerService.handleErrors(err));
    }

    getCategories(){
        this.getCategorySubscrip = this.categoryService.getOptions().subscribe(
            data => {
                this.categories = data;
            },
            err => this.errorHandlerService.handleErrors(err));
    }

    setLastQuery(){
        setTimeout(() => {
            var data = JSON.parse(sessionStorage.getItem('lastResourceQuery'));

            if(data){
                this.searchModel = data;
                this.search();
            }
        }, 0);
    }

    clean(){
        this.searchModel.name = "";
        this.searchModel.profile = "";
        this.searchModel.seniority = "";
        this.searchModel.technology = "";
        this.searchModel.employeeNumber = "";
        this.searchModel.percentage = null;
        this.searchModel.analyticId = 0;
        this.resources = [];
        sessionStorage.removeItem('lastResourceQuery');
    }

    searchDisable(){
        if(!this.searchModel.name && this.searchModel.name == "" &&
           !this.searchModel.profile && this.searchModel.profile == "" &&
           !this.searchModel.seniority && this.searchModel.seniority == "" &&
           !this.searchModel.technology && this.searchModel.technology == "" && 
           !this.searchModel.employeeNumber && this.searchModel.employeeNumber == "" && 
           !this.searchModel.analyticId && this.searchModel.analyticId == 0 && 
           !this.searchModel.percentage && this.searchModel.percentage == null){
               return true;
           }

           return false;
    }

    search() {
        this.messageService.showLoading();

        this.getAllEmployeesSubscrip = this.employeeService.search(this.searchModel).subscribe(response => {
            this.resources = response.data;

            if (response.messages) {
                this.initGrid();
                this.messageService.showMessages(response.messages);
            }

            this.messageService.closeLoading();
            this.collapse();

            sessionStorage.setItem('lastResourceQuery', JSON.stringify(this.searchModel));
        },
        error => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(error)
        });
    }

    searchAll(){
        this.messageService.showLoading();

        this.getAllEmployeesSubscrip = this.employeeService.getAll().subscribe(data => {
            this.resources = data;
            this.initGrid();
            this.messageService.closeLoading();

            this.collapse();
        },
        error => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(error)
        });
    }

    initGrid(){
        var columns = [1, 2, 3, 4, 5];
        var title = `Recursos-${moment(new Date()).format("YYYYMMDD")}`;

        var options = { 
            selector: "#resourcesTable",
            columns: columns,
            title: title,
            columnDefs: [ {'aTargets': [2], "sType": "date-uk"} ],
            withExport: true,
         };

        this.dataTableService.destroy(options.selector);
        this.dataTableService.initialize(options);
    }

    goToProfile(resource){
        this.router.navigate([`/allocationManagement/resources/${resource.id}`]);
    }

    canViewProfile(){
        return this.menuService.hasFunctionality('PROFI', 'VWPRO');
    }

    collapse(){
        if($("#collapseOne").hasClass('in')){
            $("#collapseOne").removeClass('in');
        }
        else{
            $("#collapseOne").addClass('in');
        }

        this.changeIcon();
    }

    changeIcon(){
        if($("#collapseOne").hasClass('in')){
            $("#search-icon").toggleClass('fa-caret-down').toggleClass('fa-caret-up');
        }
        else{
            $("#search-icon").toggleClass('fa-caret-up').toggleClass('fa-caret-down');
        }
    }

    addCategoryDisabled(){
        return this.resources.filter(x => x.selected == true).length == 0;
    }

    saveCategories(){
        var categoriesSelected = this.categories.filter(x => x.selected == true).map(item => item.id);
        var usersSelected = this.resources.filter(x => x.selected == true).map(item => item.id);

        if(categoriesSelected.length == 0) return;

        var json = {
            categories: categoriesSelected,
            employees: usersSelected
        }

        this.isLoading = true;

        this.addCategoriesSubscrip = this.employeeService.addCategories(json).subscribe(response => {
            this.isLoading = false;
            this.categoriesModal.hide();

            this.messageService.showMessages(response.messages);
        },
        error => {
            this.isLoading = false;
            this.categoriesModal.hide();
            this.errorHandlerService.handleErrors(error)
        });
    }

    canAddCategories(){
        return this.menuService.userIsDirector || this.menuService.userIsManager;
    }
}