import { Component, OnInit,  OnDestroy, ViewChild, Output, EventEmitter, Input } from '@angular/core';
import { Option } from "../../../../models/option";
import { CustomerService } from "../../../../services/billing/customer.service";
import { PurchaseOrderService } from '../../../../services/billing/purchaseOrder.service';
import { EmployeeService } from '../../../../services/allocation-management/employee.service';
import { DateRangePickerComponent } from '../../../../components/date-range-picker/date-range-picker.component';
import { UserService } from '../../../../services/admin/user.service';
import { MenuService } from '../../../../services/admin/menu.service';
import { PurchaseOrderViewComponent } from './purchase-order-view.component';
import { Subscription } from 'rxjs';
declare var $: any;
declare var moment: any;

@Component({
  selector: 'purchase-order-view-filter',
  templateUrl: './purchase-order-view-filter.component.html'
})
export class PurchaseOrderViewFilterComponent implements OnInit, OnDestroy {

    @Output() change:EventEmitter<any> = new EventEmitter();
    @Input() storeSessionName = "purchaseOrderViewFilter";
    @Input() useStatus = true;

    public analytics: any[] = new Array();
    public purchaseOrders: any[] = new Array();
    public projectManagers: any[] = new Array();
    public commercialManagers: any[] = new Array();

    public customers: Option[] = new Array<Option>();
    public statuses: Option[] = new Array<Option>();

    public customerId: any;
    public analyticId: any;
    public opportunityId: any;
    public purchaseOrderId: any;
    public projectManagerId: any;
    public commercialManagerId: any;
    public startDate: Date;
    public endDate: Date;
    public dateFilter = true;
    public filterByDates = true;

    public statusId: number = 1;
    
    public year;

    suscription: Subscription;

    @ViewChild('dateRangePicker') dateRangePicker:DateRangePickerComponent;
    @ViewChild('purchaseOrderView') purchaseOrderView:PurchaseOrderViewComponent;

    constructor(
        private customerService: CustomerService,
        private purchaseOrderService: PurchaseOrderService,
        private employeeService: EmployeeService,
        private userService: UserService,
        public menuService: MenuService) {}

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
        if(!this.useStatus) return;
        this.purchaseOrderService.getStatuses().subscribe(res => {
          this.statuses = res;
        });
    }

    getCustomers() {
        this.customerService.getAllOptions().subscribe(res => {
            this.customers = res.data;
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
        });
    }

    getManagers() {
        this.suscription = this.employeeService.getManagers().subscribe(data => {
            this.projectManagers = data;
        });
    }

    getCommercialManagers() {
        this.suscription = this.userService.getCommercialManagers().subscribe(res => {
            this.commercialManagers = res.data;
        });
    }

    clean() {
        this.customerId = null;
        this.analyticId = null;
        this.projectManagerId = null;
        this.commercialManagerId = null;
        this.statusId = null;
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
