import { Component, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { Router } from "@angular/router";
import { AnalyticService } from "../../../../services/allocation-management/analytic.service";
import { DataTableService } from "../../../../services/common/datatable.service";
import { MenuService } from "../../../../services/admin/menu.service";
import { MessageService } from "../../../../services/common/message.service";
import { I18nService } from "../../../../services/common/i18n.service";
import { CustomerService } from "../../../../services/billing/customer.service";
import { ServiceService } from "../../../../services/billing/service.service";
import { EmployeeService } from "../../../../services/allocation-management/employee.service";
import * as FileSaver from "file-saver";
declare var moment: any;

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
                private dataTableService: DataTableService){
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
        });
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

    goToManagementReport(analytic){
        sessionStorage.setItem('customerName', analytic.clientExternalName);
        sessionStorage.setItem('serviceName', analytic.serviceName);
        this.router.navigate([`/managementReport/${analytic.clientId}/service/${analytic.serviceId}/detail`]);
    }

    canGoToManagementReport(analytic){
        if(this.menuService.hasFunctionality('MANRE', 'VIEW-DETAIL') && analytic.clientId && analytic.serviceId){
            return true;
        }

        return false;
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
            $("#search-icon").toggleClass('fa-caret-down').toggleClass('fa-caret-up');
        } else {
            $("#search-icon").toggleClass('fa-caret-up').toggleClass('fa-caret-down');
        }
    }

    mapAnalyticToSelect(data: Array<any>): Array<any> {
        data.forEach(x => x.text = x.title + ' - ' + x.name);
        return data;
    }

    getCustomers() {
        this.customerService.getAllOptions().subscribe(d => {
            this.customers = d.data;
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
        });
    }

    searchCriteriaChange() {
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
 
            var columns = [0, 1, 2, 3, 4];
            var title = `Analiticas-${moment(new Date()).format("YYYYMMDD")}`;

            const options = {
                selector: "#analyticsTable",
                columns: columns,
                title: title,
                withExport: true
            };

            this.dataTableService.destroy(options.selector);
            this.dataTableService.initialize(options);
            this.storeSearchCriteria(searchCriteria);
        });
    }

    storeSearchCriteria(searchCriteria) {
        sessionStorage.setItem('analyticSearchCriteria', JSON.stringify(searchCriteria));
    }

    export(){
        const ids = this.model.map(item => item.id);

        this.messageService.showLoading();

        this.analyticService.createReport(ids).subscribe(file => {
            this.messageService.closeLoading();
            FileSaver.saveAs(file, `Reporte Analiticas.xlsx`);
        },
        err => {
            this.messageService.closeLoading();
        });
    }

    clean() {
        this.customerId = null;
        this.analyticId = null;
        this.serviceId = null;
        this.analyticStatusId = null;
        this.managerId = null;

        sessionStorage.removeItem('analyticSearchCriteria');

        this.searchCriteriaChange();
    }
}
