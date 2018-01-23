import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { SolfacService } from "app/services/billing/solfac.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Option } from "app/models/option";
import { CustomerService } from "app/services/billing/customer.service";
import { ServiceService } from "app/services/billing/service.service";
import { ProjectService } from "app/services/billing/project.service";
import { UserService } from "app/services/admin/user.service";
import { Cookie } from "ng2-cookies/ng2-cookies";
import { DataTableService } from "app/services/common/datatable.service";
import { MessageService } from "app/services/common/message.service";
import { SolfacStatus } from 'app/models/enums/solfacStatus';
import { MenuService } from 'app/services/admin/menu.service';

declare var moment: any;

@Component({
  selector: 'app-solfacSearch',
  templateUrl: './solfac-search.component.html'
})
export class SolfacSearchComponent implements OnInit, OnDestroy {
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
    analytic: string;
    status: string = "0";
    dateSince: Date = new Date();
    dateTo: Date = new Date();
    
    public dateOptions;

    public filterByDates: boolean = true;

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private service: SolfacService,
        private customerService: CustomerService,
        private messageService: MessageService,
        private serviceService: ServiceService,
        private projectService: ProjectService,
        private menuService: MenuService,
        private datatableService: DataTableService,
        private userService: UserService,
        private errorHandlerService: ErrorHandlerService) {

            this.dateOptions = this.menuService.getDatePickerOptions();
         }

    ngOnInit() {
        this.getCustomers();
        this.getUserOptions();
        this.getStatuses();

        var data = JSON.parse(sessionStorage.getItem('lastSolfacQuery'))
        if(data && data.length > 0){
            this.data = data;
            this.initGrid();
        }
    }

    ngOnDestroy(){
      if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
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
        (solfac.statusName == SolfacStatus[SolfacStatus.SendPending] || solfac.statusName == SolfacStatus[SolfacStatus.ManagementControlRejected]))
        {
            this.router.navigate(["/billing/solfac/" + solfac.id + "/edit"]);
        }
        else{
            this.router.navigate(["/billing/solfac/" + solfac.id]);
        }
    }

    getUserOptions(){
        this.userService.getOptions().subscribe(data => {
          this.userApplicants = data;
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    getStatuses(){
        this.service.getStatus().subscribe(data => {
          this.statuses = data;
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    getCustomers(){
        this.messageService.showLoading();

        this.customerService.getOptions(Cookie.get("currentUserMail")).subscribe(data => {
            this.messageService.closeLoading();
            this.customers = data;
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
            this.serviceService.getOptions(this.customerId).subscribe(data => {
                this.services = data;
            },
            err => this.errorHandlerService.handleErrors(err));
        }
    }

    serviceChange(){
        this.projectId = "0";
        this.projects = [];

        if(this.serviceId != "0"){
            this.projectService.getOptions(this.serviceId).subscribe(data => {
                this.projects = data;
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

        var parameters = {
            customerId: this.customerId,
            serviceId: this.serviceId,
            projectId: this.projectId,
            userApplicantId: this.userApplicantId,
            analytic: this.analytic,
            status: this.status,
            dateSince: this.filterByDates ? this.dateSince : null,
            dateTo: this.filterByDates ? this.dateTo : null
        }

        this.getAllSubscrip = this.service.search(parameters).subscribe(data => {

            setTimeout(() => {
                this.messageService.closeLoading();

                this.data = [];

                if(data.messages) {
                    this.messageService.showMessages(data.messages);
                }      
                else{
                    this.data = data;
                    sessionStorage.setItem('lastSolfacQuery', JSON.stringify(data));
                }      

                this.initGrid();
            }, 500)
        },
        err => {
            this.errorHandlerService.handleErrors(err)
        });
    }

    initGrid(){
        var columns = [0, 1, 2, 3, 4, 5, 6];
        var title = `SOLFACs-${moment(new Date()).format("YYYYMMDD")}`;

        var params = {
            selector: '#solfacsTable',
            columnDefs: [ {"aTargets": [4], "sType": "date-uk"} ],
            columns: columns,
            title: title,
            withExport: true
          }

        this.datatableService.destroy('#solfacsTable');
        this.datatableService.init2(params);
    }

    clean(){
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
