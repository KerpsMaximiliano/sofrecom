import { Router } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Option } from "app/models/option";
import { CustomerService } from "app/services/billing/customer.service";
import { Cookie } from "ng2-cookies/ng2-cookies";
import { DataTableService } from "app/services/common/datatable.service";
import { MessageService } from "app/services/common/message.service";
import { PurchaseOrderService } from '../../../../services/billing/purchaseOrder.service';
import * as FileSaver from "file-saver";
declare var $: any;
declare var moment: any;

@Component({
  selector: 'purchase-order-search',
  templateUrl: './search-purchaseOrder.component.html'
})
export class PurchaseOrderSearchComponent implements OnInit, OnDestroy {
  
    public data: any[] = new Array();

    public customers: Option[] = new Array<Option>();
    public statuses: Option[] = new Array<Option>();

    public customerId: string = "0";
    public statusId: string = "0";
    public year;

    getAllSubscrip: Subscription;

    constructor(
        private router: Router,
        private customerService: CustomerService,
        private messageService: MessageService,
        private purchaseOrderService: PurchaseOrderService,
        private datatableService: DataTableService,
        private errorHandlerService: ErrorHandlerService) {
         }

    ngOnInit() {
        this.getCustomers();
        this.getStatuses();
    }

    ngOnDestroy(){
      if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
    }

    gotToEdit(purchaseOrder) {
        this.router.navigate([`/billing/purchaseOrders/${purchaseOrder.id}`]);
    }

    goToAdd(){
        this.router.navigate([`/billing/purchaseOrders/new`]);
    }

    getStatuses(){
        this.purchaseOrderService.getStatuses().subscribe(data => {
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

    search(){
        this.messageService.showLoading();

        var parameters = {
            customerId: this.customerId,
            statusId: this.statusId,
            year: this.year
        }

        this.getAllSubscrip = this.purchaseOrderService.search(parameters).subscribe(data => {

            setTimeout(() => {
                this.messageService.closeLoading();

                this.data = data;

               this.initGrid();
            }, 500)
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    initGrid(){
        var columns = [0, 1, 2, 3, 4];
        var title = `OrdenesDeCompra-${moment(new Date()).format("YYYYMMDD")}`;

        this.datatableService.destroy('#purchaseOrderTable');
        this.datatableService.initWithExportButtons('#purchaseOrderTable', columns, title);
    }

    clean(){
        this.customerId = "0";
        this.statusId = "0";
        this.year = null;

        this.datatableService.destroy('#purchaseOrderTable');
        this.data = new Array();
    }

    export(purchaseOrder){
        this.purchaseOrderService.exportFile(purchaseOrder.fileId).subscribe(file => {
            FileSaver.saveAs(file, purchaseOrder.fileName);
        },
        err => this.errorHandlerService.handleErrors(err));
    }
}
