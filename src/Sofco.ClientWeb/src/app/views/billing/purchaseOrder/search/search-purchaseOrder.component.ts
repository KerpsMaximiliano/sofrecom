import { Router } from '@angular/router';
import { Component, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs";
import { MessageService } from "../../../../services/common/message.service";
import { PurchaseOrderService } from '../../../../services/billing/purchaseOrder.service';
import { MenuService } from '../../../../services/admin/menu.service';
import { PurchaseOrderViewComponent } from '../common/purchase-order-view.component';
import { PurchaseOrderViewFilterComponent } from '../common/purchase-order-view-filter.component';
import * as FileSaver from "file-saver";

@Component({
  selector: 'purchase-order-search',
  templateUrl: './search-purchaseOrder.component.html'
})
export class PurchaseOrderSearchComponent implements OnDestroy {

    suscription: Subscription;

    @ViewChild('purchaseOrderViewFilter') purchaseOrderViewFilter:PurchaseOrderViewFilterComponent;
    @ViewChild('purchaseOrderView') purchaseOrderView:PurchaseOrderViewComponent;

    constructor(
        private router: Router,
        private messageService: MessageService,
        private purchaseOrderService: PurchaseOrderService,
        public menuService: MenuService) {}

    ngOnDestroy(){
      if(this.suscription) this.suscription.unsubscribe();
    }

    goToEdit(data) {
        this.router.navigate([`/billing/purchaseOrders/${data.purchaseOrderId}`]);
    }

    goToAdd(){
        this.router.navigate([`/billing/purchaseOrders/new`]);
    }

    getReport(parameters) {
        this.messageService.showLoading();
        this.suscription = this.purchaseOrderService.getReport(parameters).subscribe(response => {
            this.messageService.closeLoading();
            this.getReportResponseHandler(response);
        },
        err => {
            this.messageService.closeLoading();
        });
    }

    getReportResponseHandler(response) {
        const data = response.data;
        if(data.length == 0) {
            this.showEmptyData();
        }
        this.purchaseOrderView.setData(data);
    }

    showEmptyData() {
        this.messageService.showWarning("report.resultNotFound");
    }

    viewFilterChange(data) {
        this.getReport(data);
    }

    export(){
        this.messageService.showLoading();

        this.purchaseOrderService.createReport().subscribe(file => {
            this.messageService.closeLoading();
            FileSaver.saveAs(file, `Reporte Ordenes de compra.xlsx`);
        },
        err => {
            this.messageService.closeLoading();
        });
    }
}
