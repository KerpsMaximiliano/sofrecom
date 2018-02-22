import { Router } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Option } from "app/models/option";
import { CustomerService } from "app/services/billing/customer.service";
import { Cookie } from "ng2-cookies/ng2-cookies";
import { DataTableService } from "app/services/common/datatable.service";
import { MessageService } from "app/services/common/message.service";
import * as FileSaver from "file-saver";
import { CertificatesService } from 'app/services/billing/certificates.service';
declare var $: any;
declare var moment: any;

@Component({
  selector: 'certificate-search',
  templateUrl: './search-certificate.component.html'
})
export class CertificateSearchComponent implements OnInit, OnDestroy {
  
    public data: any[] = new Array();

    public customers: Option[] = new Array<Option>();

    public customerId: string = "0";
    public year;

    getAllSubscrip: Subscription;

    constructor(
        private router: Router,
        private customerService: CustomerService,
        private messageService: MessageService,
        private certificateService: CertificatesService,
        private datatableService: DataTableService,
        private errorHandlerService: ErrorHandlerService) {}

    ngOnInit() {
        this.getCustomers();
    }

    ngOnDestroy(){
      if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
    }

    gotToEdit(certificate) {
        this.router.navigate([`/billing/certificates/${certificate.id}`]);
    }

    goToAdd(){
        this.router.navigate([`/billing/certificates/new`]);
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

    search(){
        this.messageService.showLoading();

        var parameters = {
            clientId: this.customerId,
            year: this.year
        }

        this.getAllSubscrip = this.certificateService.search(parameters).subscribe(data => {

            setTimeout(() => {
                this.messageService.closeLoading();

                this.data = data;

               this.initGrid();
            }, 500)
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    initGrid(){
        var columns = [0, 1, 2];
        var title = `Certificados-${moment(new Date()).format("YYYYMMDD")}`;

        this.datatableService.destroy('#certificateTable');
        this.datatableService.initWithExportButtons('#certificateTable', columns, title);
    }

    clean(){
        this.customerId = "0";
        this.year = null;

        this.datatableService.destroy('#certificateTable');
        this.data = new Array();
    }

    export(certificate){
        this.certificateService.exportFile(certificate.fileId).subscribe(file => {
            FileSaver.saveAs(file, certificate.fileName);
        },
        err => this.errorHandlerService.handleErrors(err));
    }
}
