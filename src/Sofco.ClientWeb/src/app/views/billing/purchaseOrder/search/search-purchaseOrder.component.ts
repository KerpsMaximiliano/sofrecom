import { Router } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Option } from "app/models/option";
import { CustomerService } from "app/services/billing/customer.service";
import { Cookie } from "ng2-cookies/ng2-cookies";
import { DataTableService } from "app/services/common/datatable.service";
import { MessageService } from "app/services/common/message.service";
import { PurchaseOrderService } from 'app/services/billing/purchaseOrder.service';
import * as FileSaver from "file-saver";
import { I18nService } from 'app/services/common/i18n.service';
import { AnalyticService } from 'app/services/allocation-management/analytic.service';
declare var $: any;
declare var moment: any;

@Component({
  selector: 'purchase-order-search',
  templateUrl: './search-purchaseOrder.component.html',
  styleUrls: ['./search-purchaseOrder.component.css']
})
export class PurchaseOrderSearchComponent implements OnInit, OnDestroy {

    public data: any[] = new Array();
    
    public analytics: any[] = new Array();
    public opportunities: any[] = new Array();
    public purchaseOrders: any[] = new Array();
    public projectManagers: any[] = new Array();
    public accountManagers: any[] = new Array();

    public customers: Option[] = new Array<Option>();
    public statuses: Option[] = new Array<Option>();

    public analyticId: any;
    public opportunityId: any;
    public purchaseOrderId: any;
    public projectManagerId: any;
    public accountManagerId: any;
    public startDate: Date;
    public endDate: Date;
    public dateFilter = true;

    public customerId = "0";
    public statusId = "0";
    public year;

    getAllSubscrip: Subscription;

    @ViewChild('pdfViewer') pdfViewer;

    constructor(
        private router: Router,
        private customerService: CustomerService,
        private messageService: MessageService,
        private purchaseOrderService: PurchaseOrderService,
        private analyticService: AnalyticService,
        private datatableService: DataTableService,
        private i18nService: I18nService,
        private errorHandlerService: ErrorHandlerService) {}

    ngOnInit() {
        const data = JSON.parse(sessionStorage.getItem('lastPurchaseOrderQuery'));
        if (data){
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

    gotToEdit(data) {
        this.router.navigate([`/billing/purchaseOrders/${data.purchaseOrderId}`]);
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

        const parameters = {
            clientId: this.customerId,
            statusId: this.statusId
        }

        this.getAllSubscrip = this.purchaseOrderService.getReport(parameters).subscribe(response => {
            setTimeout(() => {
                this.messageService.closeLoading();
                if(response.messages) this.messageService.showMessages(response.messages);

                this.data = response.data;
                sessionStorage.setItem('lastPurchaseOrderQuery', JSON.stringify(parameters));
                this.initGrid();
            }, 500);
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    initGrid(){
        const columns = [{
            "className": 'details-control',
            "orderable": false,
            "data": null,
            "defaultContent": ''
        }, 0, 1, 2, 3, 4, 5, 6, 7];

        const title = `OrdenesDeCompra-${moment(new Date()).format("YYYYMMDD")}`;

        const self = this;

        const params = {
            selector: '#purchaseOrderTable',
            columns: columns,
            title: title,
            withExport: true,
            customizeExcelExport: this.customizeExcelExport,
            customizeExcelExportData: function(data) {
                self.customizeExcelExportData(data);
            },
            columnDefs: [{
                "targets": [ 1 ], "visible": false, "searchable": false
            }]
        }

        this.datatableService.destroy('#purchaseOrderTable');

        this.datatableService.init2(params);

        this.updateTableDetail();
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
        const id = $(data[0]).data("id")
        const item = <any>this.data.find(x => x.id == id);

        const details = item != null ? item.details : [];

        let tbody = "";

        details.forEach(x => {
            tbody += this.getRowDetailForma(x);
        });

        return '<table class="table table-striped">' +
            '<thead>' +
                '<th>' + this.i18nService.translateByKey('report.solfacs.title') + '</th>' +
                '<th>' + this.i18nService.translateByKey('billing.hito.title') + '</th>' +
                '<th>' + this.i18nService.translateByKey('billing.solfac.date') + '</th>' +
                '<th>' + this.i18nService.translateByKey('billing.solfac.status') + '</th>' +
                '<th>' + this.i18nService.translateByKey('billing.solfac.currency') + '</th>' +
                '<th class="column-xs text-right">' + this.i18nService.translateByKey('billing.solfac.amount') + '</th>' +
            '</thead>' +
            '<tbody>' + tbody + '</tbody>' +
        '</table>';
    }

    getRowDetailForma(item) {
        return '<tr>' +
                '<td>' + item.solfacId + '</td>' +
                '<td>' + item.description + '</td>' +
                '<td>' + moment(item.updatedDate).format("DD/MM/YYYY") + '</td>' +
                '<td class="column-lg">' + this.i18nService.translateByKey(item.statusText) + '</td>' +
                '<td>' + item.currencyText + '</td>' +
                '<td class="column-xs text-right">' + item.total + '</td>' +
                '</tr>';
    }

    updateTableDetail() {
        const self = this;

        $(document).ready(function() {
            const dataTableSelector = '#purchaseOrderTable tbody';

            $(dataTableSelector).unbind('click');
            $(dataTableSelector).on('click', 'td.details-control', function () {
                const datatable = $('#purchaseOrderTable').DataTable();
                const tr = $(this).closest('tr');
                const tdi = tr.find("i.fa");
                const row = datatable.row(tr);

                if (row.child.isShown()) {
                    row.child.hide();
                    tr.removeClass('shown');
                    tdi.first().removeClass('fa-minus-square');
                    tdi.first().addClass('fa-plus-square');
                } else {
                    row.child(self.format(row.data())).show();
                    tr.addClass('shown');
                    tdi.first().removeClass('fa-plus-square');
                    tdi.first().addClass('fa-minus-square');
                }
            });
        });
    }

    customizeExcelExport(xlsx) {
        const sheet = xlsx.xl.worksheets['sheet1.xml'];
        $('row:first c', sheet).attr( 's', '7' );
        $('row:nth-child(2) c', sheet).attr( 's', '22' );
    }

    customizeExcelExportData(data) {
        const self = this;
        const idPos = 1;
        data.header.splice(0, 2);
        const dataBody = data.body;
        const result = [];
        for(let index = 0; index < dataBody.length; index++) {
            const dataBodyItem = dataBody[index];
            const itemId = dataBodyItem[idPos];
            dataBodyItem.splice(0, 2);
            result.push(dataBodyItem);
            const item = self.data.find(x => x.id == itemId);
            const details = item.details;
            const rowData = [];
            let rowItem = this.getExportSubHeader();
            result.push(rowItem);
            details.forEach(d => {
                rowItem = this.getExportSubBody(d);
                result.push(rowItem);
            });
        };
        data.body = result;
    }

    getExportSubHeader() {
        return ['',
            this.i18nService.translateByKey('report.solfacs.title'),
            this.i18nService.translateByKey('billing.hito.title'),
            this.i18nService.translateByKey('billing.solfac.date'),
            this.i18nService.translateByKey('billing.solfac.status'),
            this.i18nService.translateByKey('billing.solfac.amount')];
    }

    getExportSubBody(d) {
        return ['',
            d.solfacId,
            d.description,
            moment(d.updatedDate).format("DD/MM/YYYY"),
            this.i18nService.translateByKey(d.statusText),
            d.total];
    }

    collapse() {
        if ($("#collapseOne").hasClass('in')) {
            $("#collapseOne").removeClass('in');
        } else {
            $("#collapseOne").addClass('in');
        }
        this.changeIcon();
    }

    changeIcon() {
        if ($("#collapseOne").hasClass('in')) {
            $("#search-icon").toggleClass('fa-caret-up').toggleClass('fa-caret-down');
        } else {
            $("#search-icon").toggleClass('fa-caret-down').toggleClass('fa-caret-up');
        }
    }

    searchCriteriaChange() {
        this.customerId = this.customerId === "0" ? null : this.customerId;
    }

    customerChange() {
        
    }
}
