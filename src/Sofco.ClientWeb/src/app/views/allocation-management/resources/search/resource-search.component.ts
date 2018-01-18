import { Component, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Router } from "@angular/router";
import { DataTableService } from "app/services/common/datatable.service";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from "app/services/common/message.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";

@Component({
    selector: 'resource-search',
    templateUrl: './resource-search.component.html'
})

export class ResourceSearchComponent implements OnInit, OnDestroy {

    public model: any[] = new Array<any>();
    public resources: any[] = new Array<any>();
    public resource: any;

    public searchModel = {
        name: "",
        seniority: "",
        profile: "",
        technology: ""
    };

    getAllSubscrip: Subscription;
    getAllEmployeesSubscrip: Subscription;
    searchSubscrip: Subscription;

    constructor(private router: Router,
                private menuService: MenuService,
                private messageService: MessageService,
                private employeeService: EmployeeService,
                private dataTableService: DataTableService,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void {
        this.initGrid();
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

    sendUnsubscribeNotification(resource){
        this.messageService.showLoading();

        this.getAllEmployeesSubscrip = this.employeeService.sendUnsubscribeNotification(resource.name).subscribe(data => {
            this.messageService.closeLoading();
            if(data.messages) this.messageService.showMessages(data.messages);
        },
        error => {
            this.messageService.closeLoading();
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