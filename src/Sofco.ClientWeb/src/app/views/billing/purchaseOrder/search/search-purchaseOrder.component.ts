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

    @ViewChild('pdfViewer') pdfViewer;

    constructor(
        private router: Router,
        private customerService: CustomerService,
        private messageService: MessageService,
        private purchaseOrderService: PurchaseOrderService,
        private datatableService: DataTableService,
        private errorHandlerService: ErrorHandlerService) {}

    ngOnInit() {
        var data = JSON.parse(sessionStorage.getItem('lastPurchaseOrderQuery'));

        if(data){
            this.statusId = data.statusId;
            this.year = data.year;
            this.customerId = data.clientId;

            this.search();
        }

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

        this.customerService.getOptions().subscribe(data => {
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
            statusId: this.statusId,
            year: this.year | 0
        }

        this.getAllSubscrip = this.purchaseOrderService.search(parameters).subscribe(response => {

            setTimeout(() => {
                this.messageService.closeLoading();
                if(response.messages) this.messageService.showMessages(response.messages);

                this.data = response.data;
                sessionStorage.setItem('lastPurchaseOrderQuery', JSON.stringify(parameters));

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

        sessionStorage.removeItem('lastPurchaseOrderQuery');
    }

    export(purchaseOrder){
        this.purchaseOrderService.exportFile(purchaseOrder.fileId).subscribe(file => {
            FileSaver.saveAs(file, purchaseOrder.fileName);
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    viewFile(purchaseOrder){
        if(purchaseOrder.fileName.endsWith('.pdf')){
            this.purchaseOrderService.getFile(purchaseOrder.fileId).subscribe(file => {
                this.pdfViewer.renderFile(file);
            },
            err => this.errorHandlerService.handleErrors(err));
        }
    }
}
