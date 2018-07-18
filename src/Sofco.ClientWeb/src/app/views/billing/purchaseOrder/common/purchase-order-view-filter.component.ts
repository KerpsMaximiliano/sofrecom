import { Component, OnInit,  OnDestroy, ViewChild, Output, EventEmitter, Input } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Option } from "app/models/option";
import { CustomerService } from "app/services/billing/customer.service";
import { PurchaseOrderService } from 'app/services/billing/purchaseOrder.service';
import { EmployeeService } from 'app/services/allocation-management/employee.service';
import { DateRangePickerComponent } from 'app/components/date-range-picker/date-range-picker.component';
import { UserService } from 'app/services/admin/user.service';
import { MenuService } from 'app/services/admin/menu.service';
import { PurchaseOrderViewComponent } from '../common/purchase-order-view.component';
declare var $: any;
declare var moment: any;

@Component({
  selector: 'purchase-order-view-filter',
  templateUrl: './purchase-order-view-filter.component.html'
})
export class PurchaseOrderViewFilterComponent implements OnInit, OnDestroy {

    @Output() change:EventEmitter<any> = new EventEmitter();
    @Input() private storeSessionName = "purchaseOrderViewFilter";

    public analytics: any[] = new Array();
    public purchaseOrders: any[] = new Array();
    public projectManagers: any[] = new Array();
    public commercialManagers: any[] = new Array();

    public customers: Option[] = new Array<Option>();
    public statuses: Option[] = new Array<Option>();

    public analyticId: any;
    public opportunityId: any;
    public purchaseOrderId: any;
    public projectManagerId: any;
    public commercialManagerId: any;
    public startDate: Date;
    public endDate: Date;
    public dateFilter = true;
    public filterByDates = true;

    public customerId = "0";
    public statusId = "1";
    public year;

    suscription: Subscription;

    @ViewChild('dateRangePicker') dateRangePicker:DateRangePickerComponent;
    @ViewChild('purchaseOrderView') purchaseOrderView:PurchaseOrderViewComponent;

    constructor(
        private customerService: CustomerService,
        private purchaseOrderService: PurchaseOrderService,
        private employeeService: EmployeeService,
        private userService: UserService,
        public menuService: MenuService,
        private errorHandlerService: ErrorHandlerService) {}

    ngOnInit() {
        this.getCustomers();
        this.getAnalytics();
        this.getManagers();
        this.getCommercialManagers();
        this.getStatuses();
        const data = JSON.parse(sessionStorage.getItem(this.storeSessionName));
        if (data){
            this.statusId = data.statusId;
            this.customerId = data.clientId;
            this.analyticId = data.analyticId;
            this.projectManagerId = data.managerId;
            this.commercialManagerId = data.commercialManagerId;
            this.startDate = data.startDate;
            this.endDate = data.endDate;
            this.filterByDates = data.filterByDates;
        }
    }

    ngAfterViewInit() {
        const data = JSON.parse(sessionStorage.getItem(this.storeSessionName));
        if (data){
            if(this.startDate) {
                this.dateRangePicker.start = moment(this.startDate);
            }
            if(this.endDate) {
                this.dateRangePicker.end = moment(this.endDate);
            }
        }
    }

    ngOnDestroy(){
      if(this.suscription) this.suscription.unsubscribe();
    }

    getStatuses() {
        this.purchaseOrderService.getStatuses().subscribe(res => {
          this.statuses = res;
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    getCustomers() {
        this.customerService.getAllOptions().subscribe(res => {
            this.customers = res.data;
        },
        err => {
            this.errorHandlerService.handleErrors(err);
        });
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
            $("#search-icon").toggleClass('fa-caret-up').toggleClass('fa-caret-down');
        } else {
            $("#search-icon").toggleClass('fa-caret-down').toggleClass('fa-caret-up');
        }
    }

    searchCriteriaChange() {
        this.customerId = this.customerId === "0" ? null : this.customerId;
        this.statusId = this.statusId === "0" ? null : this.statusId;
        this.analyticId = this.analyticId === "0" ? null : this.analyticId;
        this.projectManagerId = this.projectManagerId === "0" ? null : this.projectManagerId;
        this.commercialManagerId = this.commercialManagerId === "0" ? null : this.commercialManagerId;
        this.startDate = null;
        this.endDate = null;
        if(this.dateRangePicker) {
            this.startDate = this.filterByDates ? this.dateRangePicker.start.toDate() : null;
            this.endDate = this.filterByDates ? this.dateRangePicker.end.toDate() : null;
        }
        const searchCriteria = {
            clientId: this.customerId,
            statusId: this.statusId,
            analyticId: this.analyticId,
            managerId: this.projectManagerId,
            commercialManagerId: this.commercialManagerId,
            startDate: this.startDate,
            endDate: this.endDate
        };

        this.storeSearchCriteria(searchCriteria);

        this.change.emit(searchCriteria);
    }

    storeSearchCriteria(searchCriteria) {
        searchCriteria.filterByDates = this.filterByDates;
        sessionStorage.setItem(this.storeSessionName, JSON.stringify(searchCriteria));
    }

    getAnalytics() {
        this.suscription = this.purchaseOrderService.getAnalyticsByCurrentUser().subscribe(res => {
            this.analytics = res.data;
            this.searchCriteriaChange();
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    getManagers() {
        this.suscription = this.employeeService.getManagers().subscribe(data => {
            this.projectManagers = data;
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    getCommercialManagers() {
        this.suscription = this.userService.getCommercialManagers().subscribe(res => {
            this.commercialManagers = res.data;
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    clean() {
        this.customerId = "0";
        this.analyticId = "0";
        this.projectManagerId = "0";
        this.commercialManagerId = "0";
        this.statusId = "0";
        this.filterByDates = true;

        sessionStorage.removeItem(this.storeSessionName);

        setTimeout(() => {
            this.searchCriteriaChange();
        }, 100);
    }

    public getFilterData():any {
        const searchCriteria = {
            clientId: this.customerId,
            statusId: this.statusId,
            analyticId: this.analyticId,
            managerId: this.projectManagerId,
            commercialManagerId: this.commercialManagerId,
            startDate: this.startDate,
            endDate: this.endDate
        };
        return searchCriteria;
    }
}
