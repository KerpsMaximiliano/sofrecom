import { Router } from '@angular/router';
import { Component, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs";
import { MessageService } from "../../../../services/common/message.service";
import { PurchaseOrderService } from '../../../../services/billing/purchaseOrder.service';
import { MenuService } from '../../../../services/admin/menu.service';
import { PurchaseOrderViewComponent } from '../common/purchase-order-view.component';
import { PurchaseOrderViewFilterComponent } from '../common/purchase-order-view-filter.component';

@Component({
  selector: 'purchase-order-active',
  templateUrl: './purchase-order-active-view.component.html'
})
export class PurchaseOrderActiveViewComponent implements OnDestroy {

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
        this.suscription = this.purchaseOrderService.getActiveReport(parameters).subscribe(response => {
            this.messageService.closeLoading();
            this.getReportResponseHandler(response);
        },
        err => {
            this.messageService.closeLoading();
        });
    }

    getReportResponseHandler(response) {
        const data = response.data;
        if(response.messages) this.messageService.showMessages(response.messages);
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
}
