import { Component, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Router } from "@angular/router";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { DataTableService } from "app/services/common/datatable.service";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from "app/services/common/message.service";
import { I18nService } from "app/services/common/i18n.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";

@Component({
    selector: 'analytic-search',
    templateUrl: './analytic-search.component.html'
})

export class AnalyticSearchComponent implements OnInit, OnDestroy {

    public model: any[] = new Array<any>();
    public resources: any[] = new Array<any>();
    public loading:  boolean = true;
    public loadingResources: boolean = true;
    public resource: any;

    getAllSubscrip: Subscription;
    getAllEmployeesSubscrip: Subscription;

    constructor(private analyticService: AnalyticService,
                private router: Router,
                private menuService: MenuService,
                private i18nService: I18nService,
                private messageService: MessageService,
                private employeeService: EmployeeService,
                private dataTableService: DataTableService,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void {
        this.getAllSubscrip = this.analyticService.getAll().subscribe(data => {
            this.model = data;
            this.loading = false;
            this.dataTableService.init('#analyticsTable', false);
        },
        error => this.errorHandlerService.handleErrors(error));

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

    goToAssignResource(analytic){
        if(this.menuService.hasFunctionality('ALLOC', 'ADRES')){
            sessionStorage.setItem("analytic", JSON.stringify(analytic));
            this.router.navigate([`/allocationManagement/analytics/${analytic.id}/allocations`]);
        }
        else{
            this.messageService.showError(this.i18nService.translate("allocationManagement.allocation.forbidden"));
        }
    }

    goToAssignAnalytics(resource){
        sessionStorage.setItem("resource", JSON.stringify(resource));
        this.router.navigate([`/allocationManagement/resources/${resource.id}/allocations`]);
    }
}