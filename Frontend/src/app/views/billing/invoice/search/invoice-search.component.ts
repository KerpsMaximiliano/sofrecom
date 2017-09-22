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

@Component({
  selector: 'app-invoice-search',
  templateUrl: './invoice-search.component.html'
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
    invoiceNumber: string;
    status: string = "";

    public loading:  boolean = false;

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private service: InvoiceService,
        private customerService: CustomerService,
        private messageService: MessageService,
        private serviceService: ServiceService,
        private projectService: ProjectService,
        private datatableService: DataTableService,
        private userService: UserService,
        private errorHandlerService: ErrorHandlerService) { }

    ngOnInit() {
        this.getCustomers();
        this.getUserOptions();
        this.getStatuses();
    }

    ngOnDestroy(){
      if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
    }

    goToDetail(invoice) {
        this.router.navigate([`/billing/invoice/${invoice.id}/project/${invoice.projectId}`]);
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
        this.customerService.getOptions(Cookie.get("currentUserMail")).subscribe(data => {
          this.customers = data;
        },
        err => this.errorHandlerService.handleErrors(err));
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

    search(){
        var parameters = {
            customerId: this.customerId,
            serviceId: this.serviceId,
            projectId: this.projectId,
            userApplicantId: this.userApplicantId,
            invoiceNumber: this.invoiceNumber,
            status: this.status
        }

        this.loading = true;

        this.getAllSubscrip = this.service.search(parameters).subscribe(data => {

            setTimeout(() => {
                this.data = [];

                if(data.messages) {
                    this.messageService.showMessages(data.messages);
                }      
                else{
                    this.data = data;
                }      

                this.datatableService.destroy('#invoiceTable');
                this.datatableService.init('#invoiceTable');

                this.loading = false;
            }, 500)
        },
        err => {
            this.loading = false;
            this.errorHandlerService.handleErrors(err)
        });
    }
}
