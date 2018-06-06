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
  templateUrl: './search-purchaseOrder.component.html',
  styleUrls: ['./search-purchaseOrder.component.css']
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

    getStatuses() {
        this.purchaseOrderService.getStatuses().subscribe(res => {
          this.statuses = res;
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    getCustomers() {
        this.messageService.showLoading();

        this.customerService.getOptions().subscribe(res => {
            this.messageService.closeLoading();
            this.customers = res.data;
        },
        err => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(err);
        });
    }

    search() {
        this.messageService.showLoading();

        var parameters = {
            clientId: this.customerId,
            statusId: this.statusId
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
        const self = this;

        const columns = [{
            "className":      'details-control',
            "orderable":      false,
            "data":           null,
            "defaultContent": ''
        }, 0, 1, 2, 3, 4];

        const title = `OrdenesDeCompra-${moment(new Date()).format("YYYYMMDD")}`;

        this.datatableService.destroy('#purchaseOrderTable');

        this.datatableService.initWithExportButtons('#purchaseOrderTable', columns, title);

        $(document).ready(function() {
            $('#purchaseOrderTable tbody').on('click', 'td.details-control', function () {
                const datatable = $('#purchaseOrderTable').DataTable();
                const tr = $(this).closest('tr');
                const tdi = tr.find("i.fa");
                const row = datatable.row(tr);

                if (row.child.isShown()) {
                    // This row is already open - close it
                    row.child.hide();
                    tr.removeClass('shown');
                    tdi.first().removeClass('fa-minus-square');
                    tdi.first().addClass('fa-plus-square');
                } else {
                    // Open this row
                    row.child(self.format(row.data())).show();
                    tr.addClass('shown');
                    tdi.first().removeClass('fa-plus-square');
                    tdi.first().addClass('fa-minus-square');
                }
            });
        });
    }

    clean(){
        this.customerId = "0";
        this.statusId = "0";

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
            this.purchaseOrderService.getFile(purchaseOrder.fileId).subscribe(response => {
                this.pdfViewer.renderFile(response.data);
            },
            err => this.errorHandlerService.handleErrors(err));
        }
    }

    format( data ) {
        return '<table cellpadding="5" cellspacing="0" border="0" style="margin-left:50px;padding-left:50px; width:100%">' +
            '<thead>' +
                '<th>Solfac</th>' +
                '<th>Hito</th>' +
                '<th>Fecha</th>' +
                '<th>Moneda</th>' +
                '<th>Monto</th>' +
                '<th>Estado</th>' +
            '</thead>' +
            '<tbody><tr>' +
                '<td>129</td>' +
                '<td>Mes 1</td>' +
                '<td>02/04/2018</td>' +
                '<td>$</td>' +
                '<td>101</td>' +
                '<td>Facturado</td>' +
            '</tr></tbody>' +
        '</table>';
    }
}
