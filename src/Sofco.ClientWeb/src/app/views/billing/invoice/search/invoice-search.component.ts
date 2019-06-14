import { Router } from '@angular/router';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from "rxjs";
import { Option } from "../../../../models/option";
import { CustomerService } from "../../../../services/billing/customer.service";
import { ServiceService } from "../../../../services/billing/service.service";
import { ProjectService } from "../../../../services/billing/project.service";
import { DataTableService } from "../../../../services/common/datatable.service";
import { MessageService } from "../../../../services/common/message.service";
import { InvoiceService } from '../../../../services/billing/invoice.service';
import { MenuService } from '../../../../services/admin/menu.service';
import { EmployeeService } from '../../../../services/allocation-management/employee.service';
declare var $: any;
declare var moment: any;
import * as FileSaver from "file-saver";

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

    customerId: string = null;
    serviceId: string = null;
    projectId: string = null;
    userApplicantId: string = null;
    status: string = null;
    dateSince: Date = new Date();
    dateTo: Date = new Date();

    public filterByDates = false;

    isSingleClick: Boolean = true;

    constructor(
        private router: Router,
        private service: InvoiceService,
        private customerService: CustomerService,
        private messageService: MessageService,
        private menuService: MenuService,
        private serviceService: ServiceService,
        private projectService: ProjectService,
        private employeeService: EmployeeService,
        private datatableService: DataTableService) {}
 
    ngOnInit() {
        this.getCustomers();
        this.getUserOptions();
        this.getStatuses();

        const data = JSON.parse(sessionStorage.getItem('lastInvoiceQuery'));
        if (data) {
            this.customerId = data.customerId;
            this.serviceId = data.serviceId;
            this.projectId = data.projectId;
            this.userApplicantId = data.userApplicantId == 0 ? null : data.userApplicantId;
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

    goToDetailInNewTab(invoice){
        this.isSingleClick = false;
        
        window.open(`/#/billing/invoice/${invoice.id}/project/${invoice.projectId}`, "_blank");
    }

    goToDetail(invoice) {
        this.isSingleClick = true;

        setTimeout(() => {
            if(this.isSingleClick){
                this.router.navigate([`/billing/invoice/${invoice.id}/project/${invoice.projectId}`]);
            }
        }, 250);
    }

    getUserOptions(){
        this.employeeService.getOptions().subscribe(res => {
          this.userApplicants = res;
        });
    }

    getStatuses(){
        this.service.getStatus().subscribe(res => {
          this.statuses = res;
        });
    }

    getCustomers(){
        this.messageService.showLoading();

        this.customerService.getOptions().subscribe(res => {
            this.messageService.closeLoading();
            this.customers = res.data;
        },
        () => this.messageService.closeLoading());
    }

    customerChange(){
        this.serviceId = null;
        this.projectId = null;
        this.projects = [];
        this.services = [];

        if(this.customerId !){
            this.serviceService.getOptions(this.customerId).subscribe(res => {
                this.services = res.data;
            });
        }
    }

    serviceChange(){
        this.projectId = null;
        this.projects = [];

        if(this.serviceId){
            this.projectService.getOptions(this.serviceId).subscribe(res => {
                this.projects = res.data;
            });
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
            userApplicantId: this.userApplicantId ? this.userApplicantId : 0,
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

                if (!res.messages) {
                    this.data = res.map(item => {
                        item.selected = false;
                        return item;
                    });

                    sessionStorage.setItem('lastInvoiceQuery', JSON.stringify(parameters));
                }

               this.initGrid();
            }, 500);
        });
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
            columns: [1, 2, 3, 4, 5, 6],
            columnDefs: [ {"aTargets": [5], "sType": "date-uk"} ]
          }
    
          this.datatableService.destroy(params.selector);
          this.datatableService.initialize(params);
    }

    clean(){
        this.customerId = null;
        this.serviceId = null;
        this.projectId = null;
        this.userApplicantId = null;
        $('#invoiceNumber').val("");
        this.status = null;
        this.dateSince= new Date();
        this.dateTo = new Date();

        this.datatableService.destroy('#invoiceTable');
        this.data = null;
    }

    downloadZip(){
        var invoicesIds = this.data.filter(x => x.selected).map(x => x.fileId);

        if(!invoicesIds || invoicesIds == null || invoicesIds.length == 0) return;

        this.messageService.showLoading();

        this.service.downloadZip(invoicesIds).subscribe(file => {
            this.messageService.closeLoading();
            FileSaver.saveAs(file, "remitos.zip");
        },
        error => this.messageService.closeLoading());
    }

    canDownload(){
        if(this.data == null) return false; 

        var invoices = this.data.filter(x => x.selected && x.fileId > 0);

        if(invoices.length == 0) return false;

        return true;
    }
}
