import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { Router } from "@angular/router";
import { DataTableService } from "../../../../services/common/datatable.service";
import { MenuService } from "../../../../services/admin/menu.service";
import { MessageService } from "../../../../services/common/message.service";
import { EmployeeService } from "../../../../services/allocation-management/employee.service";
import { Ng2ModalConfig } from "../../../../components/modal/ng2modal-config";
import { UserService } from "../../../../services/admin/user.service";
import { CategoryService } from "../../../../services/admin/category.service";
import { AllocationService } from "../../../../services/allocation-management/allocation.service";
import * as FileSaver from "file-saver";

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

    @ViewChild('allocationsModal') allocationsModal;
    public allocationsModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.reassingAllocation",
        "allocationsModal",
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
        employeeNumber: "",
        unassigned: false,
        externalOnly: false
    };

    public allocationModel = {
        analyticId: 0,
        percentage: 100,
        startDate: new Date(),
        endDate: new Date()
    }

    public endDate: Date = new Date();
    public pendingWorkingHours = false;

    getAllSubscrip: Subscription;
    getAllEmployeesSubscrip: Subscription;
    searchSubscrip: Subscription;
    getUsersSubscrip: Subscription;
    getAnalyticSubscrip: Subscription;
    getCategorySubscrip: Subscription;
    addCategoriesSubscrip: Subscription;
    subscrip: Subscription;
    allocationsSubscrip: Subscription;

    constructor(private router: Router,
                public menuService: MenuService,
                private messageService: MessageService,
                private employeeService: EmployeeService,
                private allocationService: AllocationService,
                private usersService: UserService,
                private categoryService: CategoryService,
                private dataTableService: DataTableService) {
    }

    ngOnInit(): void {
        this.getUsersSubscrip = this.usersService.getOptions().subscribe(data => {
            this.users = data;
        });

        this.getAnalytics();
        this.getCategories();
    }

    ngOnDestroy(): void {
        if(this.subscrip) this.subscrip.unsubscribe();
        if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
        if(this.searchSubscrip) this.searchSubscrip.unsubscribe();
        if(this.getUsersSubscrip) this.getUsersSubscrip.unsubscribe();
        if(this.getAnalyticSubscrip) this.getAnalyticSubscrip.unsubscribe();
        if(this.getCategorySubscrip) this.getCategorySubscrip.unsubscribe();
        if(this.addCategoriesSubscrip) this.addCategoriesSubscrip.unsubscribe();
        if(this.allocationsSubscrip) this.allocationsSubscrip.unsubscribe();
        if(this.getAllEmployeesSubscrip) this.getAllEmployeesSubscrip.unsubscribe();
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
        () => this.confirmModal.hide());
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
        },
        () => this.confirmModal.hide());
    }

    getAnalytics(){
        this.getAnalyticSubscrip = this.employeeService.getAnalytics(this.menuService.user.id).subscribe(
            data => {
                this.analytics = data;

                this.setLastQuery();
            });
    }

    getCategories(){
        this.getCategorySubscrip = this.categoryService.getOptions().subscribe(
            data => {
                this.categories = data;
            });
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
        this.searchModel.unassigned = false;
        this.searchModel.externalOnly = false;
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
           !this.searchModel.percentage && this.searchModel.percentage == null && 
           !this.searchModel.unassigned && !this.searchModel.externalOnly){
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
            }

            this.messageService.closeLoading();
            this.collapse();

            sessionStorage.setItem('lastResourceQuery', JSON.stringify(this.searchModel));
        },
        () => this.messageService.closeLoading());
    }

    searchAll(){
        this.messageService.showLoading();

        this.getAllEmployeesSubscrip = this.employeeService.getAll().subscribe(data => {
            this.resources = data.map(item => {
                item.selected = false;
                return item;
            });

            this.initGrid();
            this.messageService.closeLoading();

            this.collapse();
        },
        () => this.messageService.closeLoading());
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

    noneResourseSelected(){
        return this.resources.filter(x => x.selected == true).length == 0;
    }

    saveCategories(){
        var categoriesSelected = this.categories.filter(x => x.selected == true).map(item => item.id);
        var usersSelected = this.resources.filter(x => x.selected == true).map(item => item.id);

        if(categoriesSelected.length == 0) return;

        var json = {
            categoriesToAdd: categoriesSelected,
            categoriesToRemove: [],
            employees: usersSelected
        }

        this.addCategoriesSubscrip = this.employeeService.addCategories(json).subscribe(response => {
            this.categoriesModal.hide();
        },
        () => this.categoriesModal.hide());
    }

    canAddCategories(){
        return this.menuService.userIsDirector || this.menuService.userIsManager;
    }

    areAllSelected(){
        return this.resources.every(item => {
            return item.selected == true;
        });
    }

    areAllUnselected(){
        return this.resources.every(item => {
            return item.selected == false;
        });
    }

    selectAll(){
        this.resources.forEach((item, index) => {
            item.selected = true;
        });
    }

    unselectAll(){
        this.resources.forEach((item, index) => {
            item.selected = false;
        });
    }

    sendNewAllocations(){
        var json = {
            employeeIds: this.resources.filter(x => x.selected == true).map(item => item.id),
            analyticId: this.allocationModel.analyticId,
            startDate: this.allocationModel.startDate,
            endDate: this.allocationModel.endDate,
            percentage: this.allocationModel.percentage
        }

        this.allocationsSubscrip = this.allocationService.addMassive(json).subscribe(file => {
            this.allocationsModal.hide();
            
            if(file.size > 0){
                this.messageService.showWarningByFolder('allocationManagement/allocation', 'employeeWithErrors');
                FileSaver.saveAs(file, 'asignaciones con error.xlsx');
            }
            else{
                this.messageService.showSuccessByFolder('allocationManagement/allocation', 'massiveSuccess');
            }
        },
        () => this.allocationsModal.hide());
    }

    onKeydown(event) {
        if(this.searchDisable()) return;

        if (event.key === "Enter") {
            setTimeout(() => {
                this.search();
            }, 100);
        }
      }

    openCategoriesModal(){
        this.categories = this.categories.map(item => {
            item.selected = false;
            return item;
        });

        this.categoriesModal.show();
    }
} 