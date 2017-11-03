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
    public loading:  boolean = true;
    public loadingResources: boolean = true;
    public resource: any;

    getAllSubscrip: Subscription;
    getAllEmployeesSubscrip: Subscription;

    constructor(private router: Router,
                private menuService: MenuService,
                private messageService: MessageService,
                private employeeService: EmployeeService,
                private dataTableService: DataTableService,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void {
        this.getAllEmployeesSubscrip = this.employeeService.getOptions().subscribe(data => {
            this.loadingResources = false;
            this.resources = data;
            this.dataTableService.init('#resourcesTable', false);
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    ngOnDestroy(): void {
        if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
    }

    goToAssignAnalytics(resource){
        sessionStorage.setItem("resource", JSON.stringify(resource));
        this.router.navigate([`/allocationManagement/resources/${resource.id}/allocations`]);
    }
}