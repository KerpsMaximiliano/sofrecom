import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { SolfacService } from "app/services/billing/solfac.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Option } from "app/models/option";
import { CustomerService } from "app/services/billing/customer.service";
import { ServiceService } from "app/services/billing/service.service";
import { ProjectService } from "app/services/billing/project.service";
import { UserService } from "app/services/admin/user.service";
import { DataTableService } from "app/services/common/datatable.service";
import { MessageService } from "app/services/common/message.service";
import { SolfacStatus } from 'app/models/enums/solfacStatus';
import { MenuService } from 'app/services/admin/menu.service';
import { EmployeeService } from '../../../../services/allocation-management/employee.service';

declare var moment: any;

@Component({
  selector: 'app-solfac-search',
  templateUrl: './solfac-search.component.html'
})
export class SolfacSearchComponent implements OnInit, OnDestroy {
    getAllSubscrip: Subscription;
    data;

    public customers: Option[] = new Array<Option>();
    public services: Option[] = new Array<Option>();
    public projects: Option[] = new Array<Option>();
    public userApplicants: any[] = new Array();
    public statuses: Option[] = new Array<Option>();

    customerId: string = "0";
    serviceId: string = "0";
    projectId: string = "0";
    userApplicantId: string = "0";
    analytic: string;
    status: string = "0";
    dateSince: Date = new Date();
    dateTo: Date = new Date();

    public filterByDates = false;

    constructor(
        private router: Router,
        private service: SolfacService,
        private customerService: CustomerService,
        private employeeService: EmployeeService,
        private messageService: MessageService,
        private serviceService: ServiceService,
        private projectService: ProjectService,
        private menuService: MenuService,
        private datatableService: DataTableService,
        private errorHandlerService: ErrorHandlerService) {}

    ngOnInit() {
        this.getCustomers();
        this.getUserOptions();
        this.getStatuses();

        const data = JSON.parse(sessionStorage.getItem('lastSolfacQuery'));
        if (data) {
            this.customerId = data.customerId;
            this.serviceId = data.serviceId;
            this.projectId = data.projectId;
            this.userApplicantId = data.userApplicantId;
            this.analytic = data.analytic;
            this.status = data.status;
            this.filterByDates = data.filterByDates;

            if (this.filterByDates) {
                this.dateSince = data.dateSince;
                this.dateTo = data.dateTo;
            } else {
                this.dateSince = new Date();
                this.dateTo = new Date();
            }

            this.search();
        }
    }

    ngOnDestroy() {
      if (this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
    }

    setCurrencySymbol(currencyId){
      switch(currencyId){
        case 1: { return "$";  }
        case 2: { return "U$D"; }
        case 3: { return "€"; }
      }
    }

    goToDetail(solfac) {
        if(this.menuService.hasFunctionality('SOLFA', 'ALTA') && 
        (solfac.statusName == SolfacStatus[SolfacStatus.SendPending] || 
         solfac.statusName == SolfacStatus[SolfacStatus.RejectedByDaf] ||
         solfac.statusName == SolfacStatus[SolfacStatus.ManagementControlRejected]))
        {
            this.router.navigate(["/billing/solfac/" + solfac.id + "/edit"]);
        }
        else{
            this.router.navigate(["/billing/solfac/" + solfac.id]);
        }
    }

    getUserOptions() {
        this.employeeService.getManagers().subscribe(d => {
          this.userApplicants = d;
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    getStatuses() {
        this.service.getStatus().subscribe(d => {
          this.statuses = d;
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    getCustomers() {
        this.messageService.showLoading();

        this.customerService.getOptions().subscribe(d => {
            this.messageService.closeLoading();
            this.customers = d.data;
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
            this.serviceService.getOptions(this.customerId).subscribe(d => {
                this.services = d.data;
            },
            err => this.errorHandlerService.handleErrors(err));
        }
    }

    serviceChange(){
        this.projectId = "0";
        this.projects = [];

        if(this.serviceId != "0"){
            this.projectService.getOptions(this.serviceId).subscribe(d => {
                this.projects = d.data;
            },
            err => this.errorHandlerService.handleErrors(err));
        }
    }

    showUserApplicantFilter(){
        return this.menuService.userIsDirector || this.menuService.userIsDaf || this.menuService.userIsCdg;
    }

    search(){
        if(this.dateTo < this.dateSince){
            this.messageService.showError("dateToLessThanSince");
            return;
        }

        this.messageService.showLoading();

        let managerId = "";
        if(this.userApplicantId != '0'){
            let manager = this.userApplicants.filter(item => item.id == this.userApplicantId);

            if(manager && manager.length > 0){
                managerId = manager[0].externalId;
            }
        }

        var parameters = {
            customerId: this.customerId,
            serviceId: this.serviceId,
            projectId: this.projectId,
            managerId: managerId,
            userApplicantId: this.userApplicantId,
            analytic: this.analytic,
            status: this.status,
            dateSince: this.filterByDates ? this.dateSince : null,
            dateTo: this.filterByDates ? this.dateTo : null,
            filterByDates: this.filterByDates
        }

        this.getAllSubscrip = this.service.search(parameters).subscribe(response => {

            setTimeout(() => {
                this.messageService.closeLoading();

                this.data = [];

                if (response.messages) {
                    this.messageService.showMessages(response.messages);
                } else {
                    this.data = response;
                    sessionStorage.setItem('lastSolfacQuery', JSON.stringify(parameters));
                }

                this.initGrid();
            }, 500);
        },
        err => {
            this.errorHandlerService.handleErrors(err)
        });
    }

    initGrid() {
        var columns = [0, 1, 2, 3, 4, 5, 6];
        var title = `SOLFACs-${moment(new Date()).format("YYYYMMDD")}`;

        var params = {
            selector: '#solfacsTable',
            columnDefs: [ {"aTargets": [4], "sType": "date-uk"} ],
            columns: columns,
            order: [[ 4, "desc" ]],
            title: title,
            withExport: true
          }

        this.datatableService.destroy('#solfacsTable');
        this.datatableService.initialize(params);
    }

    clean() {
        this.customerId = "0";
        this.serviceId = "0";
        this.projectId = "0";
        this.userApplicantId = "0";
        this.analytic = "";
        this.status = "0";
        this.dateSince= new Date();
        this.dateTo = new Date();

        this.datatableService.destroy('#solfacsTable');
        this.data = null;
    }
}
