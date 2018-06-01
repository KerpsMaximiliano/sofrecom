import { Component, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Router } from "@angular/router";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { DataTableService } from "app/services/common/datatable.service";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from "app/services/common/message.service";
import { I18nService } from "app/services/common/i18n.service";
import { Option } from "app/models/option";
import { CustomerService } from "app/services/billing/customer.service";
import { ServiceService } from "app/services/billing/service.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";

@Component({
    selector: 'analytic-search',
    templateUrl: './analytic-search.component.html'
})

export class AnalyticSearchComponent implements OnInit, OnDestroy {

    public analytics: any[] = new Array();
    public customers: any[] = new Array();
    public services: any[] = new Array();
    public analyticStatus: any[] = new Array();
    public managers: any[] = new Array();
    public analyticId: any;
    public customerId: any;
    public serviceId: any;
    public analyticStatusId: any;
    public managerId: any;

    public model: any[] = new Array<any>();
    public resources: any[] = new Array<any>();
    public loading:  boolean = true;
    public loadingResources: boolean = true;
    public resource: any;
    private nullId = '';
    private idKey = 'id';
    private textKey = 'text';

    getAllSubscrip: Subscription;
    getAllEmployeesSubscrip: Subscription;
    suscription: Subscription;

    constructor(private analyticService: AnalyticService,
                private customerService: CustomerService,
                private serviceService: ServiceService,
                private employeeService: EmployeeService,
                private router: Router,
                private i18nService: I18nService,
                public menuService: MenuService,
                private messageService: MessageService,
                private dataTableService: DataTableService,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void {
        const data = JSON.parse(sessionStorage.getItem('analyticSearchCriteria'));
        if (data) {
            this.analyticId = data.analyticId;
            this.customerId = data.customerId;
            this.serviceId = data.serviceId;
            this.analyticStatusId = data.analyticStatusId;
            this.managerId = data.managerId;
        }
        this.getCustomers();
        this.getAnalytics();
        this.getAnalyticStatus();
        this.getManagers();
        if (this.customerId != null) {
            this.getServices();
        }
    }

    getAnalytics() {
        this.getAllSubscrip = this.analyticService.getAll().subscribe(data => {
            this.analytics = this.mapAnalyticToSelect(data);
            this.loading = false;
            this.searchCriteriaChange();
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    ngOnDestroy(): void {
        if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
    }

    gotToEdit(analytic){
        this.router.navigate([`/contracts/analytics/${analytic.id}/edit`]);
    }

    gotToView(analytic){
        this.router.navigate([`/contracts/analytics/${analytic.id}/view`]);
    }

    goToAssignResource(analytic){
        if(this.menuService.hasFunctionality('ALLOC', 'ADRES')){
            sessionStorage.setItem("analytic", JSON.stringify(analytic));
            this.router.navigate([`/contracts/analytics/${analytic.id}/allocations`]);
        }
        else{
            this.messageService.showError("allocationManagement.allocation.forbidden");
        }
    }

    goToAdd(){
        sessionStorage.setItem('analyticWithProject', 'no');
        this.router.navigate(['/contracts/analytics/new']);
    }
 
    getStatus(analytic){
        switch(analytic.status){
            case 1: return this.i18nService.translateByKey("allocationManagement.analytics.status.open");
            case 2: return this.i18nService.translateByKey("allocationManagement.analytics.status.close");
            case 3: return this.i18nService.translateByKey("allocationManagement.analytics.status.closeForExpenses");
        }
    }

    collapse() {
        if ($("#collapseOne").hasClass('in')) {
            $("#collapseOne").removeClass('in');
        } else {
            $("#collapseOne").addClass('in');
        }
        this.changeIcon();
    }

    changeIcon() {
        if ($("#collapseOne").hasClass('in')) {
            $("#search-icon").toggleClass('fa-minus').toggleClass('fa-plus');
        } else {
            $("#search-icon").toggleClass('fa-plus').toggleClass('fa-minus');
        }
    }

    mapAnalyticToSelect(data: Array<any>): Array<any> {
        data.forEach(x => x.text = x.title + ' - ' + x.name);
        return data;
    }

    getCustomers() {
        this.customerService.getOptions().subscribe(d => {
            this.customers = d.data;
        },
        err => {
            this.errorHandlerService.handleErrors(err)
        });
    }

    customerChange() {
        this.serviceId = null;
        this.searchCriteriaChange();
        this.getServices();
    }

    getServices() {
        this.services = [];

        if (this.customerId == 0 || this.customerId == null) return;

        this.messageService.showLoading();

        this.serviceService.getOptions(this.customerId).subscribe(d => {
            this.messageService.closeLoading();
            this.services = d.data;
        },
        err => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(err);
        });
    }

    getAnalyticStatus() {
        const openStatus = 'allocationManagement.analytics.status.open';
        const closeStatus = 'allocationManagement.analytics.status.close';
        const closeForExpensesStatus = 'allocationManagement.analytics.status.closeForExpenses';

        this.analyticStatus = [
            { 'id': 1, 'text': openStatus},
            { 'id': 2, 'text': closeStatus},
            { 'id': 3, 'text': closeForExpensesStatus},
        ];
    }

    getManagers() {
        this.suscription = this.employeeService.getManagers().subscribe(data => {
            this.managers = data;
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    searchCriteriaChange() {
        this.serviceId = this.serviceId === "0" ? null : this.serviceId;
        this.customerId = this.customerId === "0" ? null : this.customerId;
        const searchCriteria = {
            analyticId: this.analyticId,
            customerId: this.customerId,
            serviceId: this.serviceId,
            analyticStatusId: this.analyticStatusId,
            managerId: this.managerId
        };

        this.getAnalyticsBySearchCriteria(searchCriteria);
    }

    getAnalyticsBySearchCriteria(searchCriteria) {
        this.getAllSubscrip = this.analyticService.get(searchCriteria).subscribe(res => {
            this.model = res.data;
            this.loading = false;

            const options = {
                selector: "#analyticsTable",
                withExport: true,
                title: "Analiticas",
                columns: [0, 1, 2, 3, 4]
            };

            this.dataTableService.destroy(options.selector);
            this.dataTableService.init2(options);
            this.storeSearchCriteria(searchCriteria);
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    storeSearchCriteria(searchCriteria) {
        sessionStorage.setItem('analyticSearchCriteria', JSON.stringify(searchCriteria));
    }
}
