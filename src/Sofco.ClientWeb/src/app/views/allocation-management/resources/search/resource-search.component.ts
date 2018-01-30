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

declare var $: any;

@Component({
    selector: 'resource-search',
    templateUrl: './resource-search.component.html'
})
export class ResourceSearchComponent implements OnInit, OnDestroy {

    @ViewChild('confirmModal') confirmModal;
    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "confirmModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    ); 

    public model: any[] = new Array<any>();
    public resources: any[] = new Array<any>();
    public users: any[] = new Array<any>();
    public resource: any;
    public resourceSelected: any;

    public searchModel = {
        name: "",
        seniority: "",
        profile: "",
        technology: ""
    };

    public options: any;
    public endDate: Date = new Date();

    getAllSubscrip: Subscription;
    getAllEmployeesSubscrip: Subscription;
    searchSubscrip: Subscription;
    getUsersSubscrip: Subscription;

    constructor(private router: Router,
                public menuService: MenuService,
                private messageService: MessageService,
                private employeeService: EmployeeService,
                private usersService: UserService,
                private dataTableService: DataTableService,
                private errorHandlerService: ErrorHandlerService){

                this.options = this.menuService.getDatePickerOptions();
    }
 
    ngOnInit(): void {

        var data = JSON.parse(sessionStorage.getItem('lastResourceQuery'));

        if(data){
            this.searchModel = data;
            this.search();
            this.initGrid();
        }

        this.getUsersSubscrip = this.usersService.getOptions().subscribe(data => {  
            this.users = data;
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    ngOnDestroy(): void {
        if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
        if(this.searchSubscrip) this.searchSubscrip.unsubscribe();
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

    clean(){
        this.searchModel.name = "";
        this.searchModel.profile = "";
        this.searchModel.seniority = "";
        this.searchModel.technology = "";
    }

    searchDisable(){
        if(!this.searchModel.name && this.searchModel.name == "" &&
           !this.searchModel.profile && this.searchModel.profile == "" &&
           !this.searchModel.seniority && this.searchModel.seniority == "" &&
           !this.searchModel.technology && this.searchModel.technology == ""){
               return true;
           }

           return false;
    }

    search(){
        this.messageService.showLoading();

        this.getAllEmployeesSubscrip = this.employeeService.search(this.searchModel).subscribe(data => {
            this.resources = data;
            this.initGrid();
            this.messageService.closeLoading();

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
        },
        error => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(error)
        });
    }

    initGrid(){
        var options = { selector: "#resourcesTable" };
        this.dataTableService.destroy(options.selector);
        this.dataTableService.init2(options);
    }
}