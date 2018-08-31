import { Component, OnInit,  OnDestroy, ViewChild, Output, EventEmitter } from '@angular/core';
import { Subscription } from "rxjs";
import { DataTableService } from "../../../../services/common/datatable.service";
import { PurchaseOrderService } from '../../../../services/billing/purchaseOrder.service';
import * as FileSaver from "file-saver";
import { I18nService } from '../../../../services/common/i18n.service';
import { AmountFormatPipe } from '../../../../pipes/amount-format.pipe';
import { MenuService } from '../../../../services/admin/menu.service';
import { PurchaseOrderBalanceModel } from '../../../../models/billing/purchase-order/purchase-order-balance-model';
declare var $: any;
declare var moment: any;

@Component({
  selector: 'purchase-order-view',
  templateUrl: './purchase-order-view.component.html',
  styleUrls: ['./purchase-order-view.component.css']
})
export class PurchaseOrderViewComponent implements OnInit, OnDestroy {

    @Output() clickView:EventEmitter<any> = new EventEmitter();

    public data: any[] = new Array();

    public analytics: any[] = new Array();
    public purchaseOrders: any[] = new Array();
    public projectManagers: any[] = new Array();
    public commercialManagers: any[] = new Array();

    private adjustmentSuffix = "";

    suscription: Subscription;

    @ViewChild('pdfViewer') pdfViewer;

    constructor(
        private purchaseOrderService: PurchaseOrderService,
        public menuService: MenuService,
        private datatableService: DataTableService,
        private i18nService: I18nService) {}

    ngOnInit() {
        this.adjustmentSuffix = " - " + this.i18nService.translateByKey('billing.purchaseOrder.adjustment');
    }

    ngOnDestroy(){
        if(this.suscription) this.suscription.unsubscribe();
    }

    public setData(data:any){
        this.data = this.addPurchaseOrderAdjust(data);
        this.initGrid();
    }

    initGrid(){
        const excelColumns = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11];

        const title = `OrdenesDeCompra-${moment(new Date()).format("YYYYMMDD")}`;

        const self = this;

        const params = {
            selector: '#purchaseOrderTable',
            columns: excelColumns,
            title: title,
            withExport: true,
            customizeExcelExport: this.customizeExcelExport,
            customizeExcelExportData: function(data) {
                self.customizeExcelExportData(data);
            },
            columnDefs: [
                { "targets": [ 1,10,11 ], "visible": false, "searchable": false },
                { "aTargets": [5], "sType": "date-uk"},
            ]
        }

        this.datatableService.destroy('#purchaseOrderTable');

        this.datatableService.initialize(params);

        this.updateTableDetail();
    }

    export(purchaseOrder){
        this.suscription = this.purchaseOrderService.exportFile(purchaseOrder.fileId).subscribe(file => {
            FileSaver.saveAs(file, purchaseOrder.fileName);
        });
    }

    viewFile(purchaseOrder){
        if(purchaseOrder.fileName.endsWith('.pdf')){
            this.suscription = this.purchaseOrderService.getFile(purchaseOrder.fileId).subscribe(response => {
                this.pdfViewer.renderFile(response.data);
            });
        }
    }

    formatDetail( data ) {
        const id = $(data[0]).data("id");
        const item = <any>this.data.find(x => x.id == id);

        const details = item != null ? item.details : [];

        let tbody = "";

        details.forEach(x => {
            tbody += this.getRowDetailForma(x);
        });

        return '<div style="margin-left:20px;margin-right:100px"><table class="table table-striped">' +
            '<thead>' +
                '<th>' + this.i18nService.translateByKey('billing.solfac.analytic') + '</th>' +
                '<th>' + this.i18nService.translateByKey('billing.solfac.hito') + '</th>' +
                '<th>' + this.i18nService.translateByKey('billing.solfac.date') + '</th>' +
                '<th>' + this.i18nService.translateByKey('billing.solfac.status') + '</th>' +
                '<th>' + this.i18nService.translateByKey('billing.solfac.currency') + '</th>' +
                '<th class="column-xs text-right">' + this.i18nService.translateByKey('billing.solfac.amount') + '</th>' +
            '</thead>' +
            '<tbody>' + tbody + '</tbody>' +
        '</table></div>';
    }

    getRowDetailForma(item) {
        return '<tr>' +
                '<td>' + item.analytic + '</td>' +
                '<td>' + item.description + '</td>' +
                '<td>' + moment(item.updatedDate).format("DD/MM/YYYY") + '</td>' +
                '<td class="column-lg">' + this.i18nService.translateByKey(item.statusText) + '</td>' +
                '<td>' + item.currencyText + '</td>' +
                '<td class="column-xs text-right">' + new AmountFormatPipe().transform(item.total) + '</td>' +
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
                    row.child(self.formatDetail(row.data())).show();
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
        const receptionDatePos = 3;
        const ammountNumberPos = 6;
        const balanceNumberPos = 7;
        data.header.splice(0, 2);
        const dataBody = data.body;
        const result = [];
        for(let index = 0; index < dataBody.length; index++) {
            const dataBodyItem = dataBody[index];
            const itemId = dataBodyItem[idPos];
            dataBodyItem.splice(0, 2);
            const item = self.data.find(x => x.id == itemId);
            if(item === undefined) continue;
            dataBodyItem[receptionDatePos] = item.receptionDate;
            dataBodyItem[ammountNumberPos] = item.ammount;
            dataBodyItem[balanceNumberPos] = item.balance;
            result.push(dataBodyItem);
            const details = item.details;
            if(details.length == 0) continue;
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
            this.i18nService.translateByKey('billing.solfac.analytic'),
            this.i18nService.translateByKey('billing.solfac.hito'),
            this.i18nService.translateByKey('billing.solfac.date'),
            this.i18nService.translateByKey('billing.solfac.status'),
            this.i18nService.translateByKey('billing.solfac.amount')];
    }

    getExportSubBody(d) {
        return ['',
            d.analytic,
            d.description,
            moment(d.updatedDate).format("DD/MM/YYYY"),
            this.i18nService.translateByKey(d.statusText),
            d.total];
    }

    addPurchaseOrderAdjust(data): any[] {
        data.forEach(item => {
            if(item.adjustment != 0){
                const newItem = this.createPurchaseOrderAdjustment(item);
                data.push(newItem);
            }
        });
        return data.sort(function (a, b) {
            return a.number.localeCompare(b.number);
        });
    }

    createPurchaseOrderAdjustment(data): PurchaseOrderBalanceModel {
        const item = new PurchaseOrderBalanceModel();
        item.number = data.number + this.adjustmentSuffix;
        item.clientExternalName = data.clientExternalName;
        item.currencyText = data.currencyText;
        item.ammount = data.adjustment;
        item.statusId = data.statusId;
        item.statusText = this.i18nService.translateByKey(data.statusText) + this.adjustmentSuffix;
        return item;
    }

    clickViewHandler(data) {
        this.clickView.emit(data);
    }
}
