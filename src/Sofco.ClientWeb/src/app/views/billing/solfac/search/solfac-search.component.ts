import { Router } from '@angular/router';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from "rxjs";
import { SolfacService } from "../../../../services/billing/solfac.service";
import { Option } from "../../../../models/option";
import { CustomerService } from "../../../../services/billing/customer.service";
import { ServiceService } from "../../../../services/billing/service.service";
import { ProjectService } from "../../../../services/billing/project.service";
import { DataTableService } from "../../../../services/common/datatable.service";
import { MessageService } from "../../../../services/common/message.service";
import { SolfacStatus } from '../../../../models/enums/solfacStatus';
import { MenuService } from '../../../../services/admin/menu.service';
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

    customerId: string = null;
    serviceId: string = null;
    projectId: string = null;
    userApplicantId: string = null;
    analytic: string;
    status: string = null;
    dateSince: Date = new Date();
    dateTo: Date = new Date();

    public filterByDates = false;

    isSingleClick: Boolean = true;  

    constructor(
        private router: Router,
        private service: SolfacService,
        private customerService: CustomerService,
        private employeeService: EmployeeService,
        private messageService: MessageService,
        private serviceService: ServiceService,
        private projectService: ProjectService,
        private menuService: MenuService,
        private datatableService: DataTableService) {}

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

    goToDetailInNewTab(solfac){
        this.isSingleClick = false;
        
        if(this.menuService.hasFunctionality('SOLFA', 'ALTA') && 
        (solfac.statusName == SolfacStatus[SolfacStatus.SendPending] || 
         solfac.statusName == SolfacStatus[SolfacStatus.RejectedByDaf] ||
         solfac.statusName == SolfacStatus[SolfacStatus.ManagementControlRejected]))
        { 
            sessionStorage.setItem("customerId", solfac.customerId);
            sessionStorage.setItem("serviceId", solfac.serviceId);
            sessionStorage.setItem("projectId", solfac.projectId);
            window.open("/#/billing/solfac/" + solfac.id + "/edit", "_blank");
        }
        else{
            sessionStorage.setItem("customerId", solfac.customerId);
            sessionStorage.setItem("serviceId", solfac.serviceId);
            sessionStorage.setItem("projectId", solfac.projectId);
            window.open("/#/billing/solfac/" + solfac.id, "_blank");
        }
    }

    goToDetail(solfac) {
        this.isSingleClick = true;

        setTimeout(()=>{
            if(this.isSingleClick){
                if(this.menuService.hasFunctionality('SOLFA', 'ALTA') && 
                (solfac.statusName == SolfacStatus[SolfacStatus.SendPending] || 
                solfac.statusName == SolfacStatus[SolfacStatus.RejectedByDaf] ||
                solfac.statusName == SolfacStatus[SolfacStatus.ManagementControlRejected]))
                { 
                    sessionStorage.setItem("customerId", solfac.customerId);
                    sessionStorage.setItem("serviceId", solfac.serviceId);
                    sessionStorage.setItem("projectId", solfac.projectId);
                    this.router.navigate(["/billing/solfac/" + solfac.id + "/edit"]);
                }
                else{
                    sessionStorage.setItem("customerId", solfac.customerId);
                    sessionStorage.setItem("serviceId", solfac.serviceId);
                    sessionStorage.setItem("projectId", solfac.projectId);
                    this.router.navigate(["/billing/solfac/" + solfac.id]);
                }
            }

         },500)
    }

    getUserOptions() {
        this.employeeService.getManagers().subscribe(d => {
          this.userApplicants = d;
        });
    }

    getStatuses() {
        this.service.getStatus().subscribe(d => {
          this.statuses = d;
        });
    }

    getCustomers() {
        this.messageService.showLoading();

        this.customerService.getOptions().subscribe(d => {
            this.messageService.closeLoading();
            this.customers = d.data;
        },
        () => {
                this.messageService.closeLoading();
            });
    }

    customerChange(){
        this.serviceId = null;
        this.projectId = null;
        this.projects = [];
        this.services = [];

        if(this.customerId){
            this.serviceService.getOptions(this.customerId).subscribe(d => {
                this.services = d.data;
            });
        }
    }

    serviceChange(){
        this.projectId = null;
        this.projects = [];

        if(this.serviceId){
            this.projectService.getOptions(this.serviceId).subscribe(d => {
                this.projects = d.data;
            });
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
        if(this.userApplicantId){
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
            analytic: this.analytic,
            status: this.status ? this.status : 0,
            dateSince: this.filterByDates ? this.dateSince : null,
            dateTo: this.filterByDates ? this.dateTo : null
        }

        this.getAllSubscrip = this.service.search(parameters).subscribe(response => {

            setTimeout(() => {
                this.messageService.closeLoading();

                this.data = [];

                if (!response.messages) {
                    this.data = response;
                    sessionStorage.setItem('lastSolfacQuery', JSON.stringify(parameters));
                }

                this.initGrid();
            }, 500);
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
            withExport: true,
            currencyColumns: [6]
          }

        this.datatableService.destroy('#solfacsTable');
        this.datatableService.initialize(params);
    }

    clean() {
        this.customerId = null;
        this.serviceId = null;
        this.projectId = null;
        this.userApplicantId = null;
        this.analytic = "";
        this.status = null;
        this.dateSince= new Date();
        this.dateTo = new Date();

        this.datatableService.destroy('#solfacsTable');
        this.data = null;
    }
}
