import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Option } from "app/models/option";
import { CustomerService } from "app/services/billing/customer.service";
import { ServiceService } from "app/services/billing/service.service";
import { ProjectService } from "app/services/billing/project.service";
import { UserService } from "app/services/admin/user.service";
import { Cookie } from "ng2-cookies/ng2-cookies";
import { DataTableService } from "app/services/common/datatable.service";
import { MessageService } from "app/services/common/message.service";
import { InvoiceService } from 'app/services/billing/invoice.service';
import { MenuService } from 'app/services/admin/menu.service';
import { I18nService } from 'app/services/common/i18n.service';
import { EmployeeService } from '../../../../services/allocation-management/employee.service';
declare var $: any;
declare var moment: any;

@Component({
  selector: 'app-invoice-search',
  templateUrl: './invoice-search.component.html',
  styleUrls: ['./invoice-search.component.scss']
})
export class InvoiceSearchComponent implements OnInit, OnDestroy {
    getAllSubscrip: Subscription;
    data;

    public customers: Option[] = new Array<Option>();
    public services: Option[] = new Array<Option>();
    public projects: Option[] = new Array<Option>();
    public userApplicants: Option[] = new Array<Option>();
    public statuses: Option[] = new Array<Option>();

    customerId: string = "0";
    serviceId: string = "0";
    projectId: string = "0";
    userApplicantId: string = "0";
    status: string = "";
    dateSince: Date = new Date();
    dateTo: Date = new Date();

    public filterByDates = false;

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private service: InvoiceService,
        private customerService: CustomerService,
        private messageService: MessageService,
        private menuService: MenuService,
        private serviceService: ServiceService,
        private i18nService: I18nService,
        private projectService: ProjectService,
        private employeeService: EmployeeService,
        private datatableService: DataTableService,
        private userService: UserService,
        private errorHandlerService: ErrorHandlerService) {}

    ngOnInit() {
        this.getCustomers();
        this.getUserOptions();
        this.getStatuses();

        const data = JSON.parse(sessionStorage.getItem('lastInvoiceQuery'));
        if (data) {
            this.customerId = data.customerId;
            this.serviceId = data.serviceId;
            this.projectId = data.projectId;
            this.userApplicantId = data.userApplicantId;
            $('#invoiceNumber').val(data.invoiceNumber);
            this.status = data.status,
            this.filterByDates = data.filterByDates;

            if(this.filterByDates){
                this.dateSince = data.dateSince;
                this.dateTo = data.dateTo;
            }
            else{
                this.dateSince = new Date();
                this.dateTo = new Date();
            }

            this.search();
        }
    }

    ngOnDestroy(){
      if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
    }

    goToDetail(invoice) {
        this.router.navigate([`/billing/invoice/${invoice.id}/project/${invoice.projectId}`]);
    }

    getUserOptions(){
        this.employeeService.getOptions().subscribe(res => {
          this.userApplicants = res;
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    getStatuses(){
        this.service.getStatus().subscribe(res => {
          this.statuses = res;
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    getCustomers(){
        this.messageService.showLoading();

        this.customerService.getOptions().subscribe(res => {
            this.messageService.closeLoading();
            this.customers = res.data;
        },
        err => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(err)
        });
    }

    customerChange(){
        this.serviceId = "0";
        this.projectId = "0";
        this.projects = [];
        this.services = [];

        if(this.customerId != "0"){
            this.serviceService.getOptions(this.customerId).subscribe(res => {
                this.services = res.data;
            },
            err => this.errorHandlerService.handleErrors(err));
        }
    }

    serviceChange(){
        this.projectId = "0";
        this.projects = [];

        if(this.serviceId != "0"){
            this.projectService.getOptions(this.serviceId).subscribe(res => {
                this.projects = res.data;
            },
            err => this.errorHandlerService.handleErrors(err));
        }
    }

    search(){
        if(this.dateTo < this.dateSince){
            this.messageService.showError("dateToLessThanSince");
            return;
        }

        this.messageService.showLoading();

        var parameters = {
            customerId: this.customerId,
            serviceId: this.serviceId,
            projectId: this.projectId,
            userApplicantId: this.userApplicantId,
            invoiceNumber: $('#invoiceNumber').val(),
            status: this.status,
            dateSince: this.filterByDates ? this.dateSince : null,
            dateTo: this.filterByDates ? this.dateTo : null,
            filterByDates: this.filterByDates
        }

        this.getAllSubscrip = this.service.search(parameters).subscribe(res => {

            setTimeout(() => {
                this.messageService.closeLoading();

                this.data = [];

                if (res.messages) {
                    this.messageService.showMessages(res.messages);
                } else {
                    this.data = res;
                    sessionStorage.setItem('lastInvoiceQuery', JSON.stringify(parameters));
                }

               this.initGrid();
            }, 500);
        },
        err => this.errorHandlerService.handleErrors(err));
    }
 
    showUserApplicantFilter(){
        return this.menuService.userIsDirector || this.menuService.userIsDaf || this.menuService.userIsCdg;
    }

    initGrid(){
        var params = {
            selector: '#invoiceTable',
            withExport: true,
            order: [[ 4, "desc" ]],
            title: `REMITOS-${moment(new Date()).format("YYYYMMDD")}`,
            columns: [0, 1, 2, 3, 4, 5],
            columnDefs: [ {"aTargets": [4], "sType": "date-uk"} ]
          }
    
          this.datatableService.initialize(params);
    }

    clean(){
        this.customerId = "0";
        this.serviceId = "0";
        this.projectId = "0";
        this.userApplicantId = "0";
        $('#invoiceNumber').val("");
        this.status = "";
        this.dateSince= new Date();
        this.dateTo = new Date();

        this.datatableService.destroy('#invoiceTable');
        this.data = null;
    }
}
