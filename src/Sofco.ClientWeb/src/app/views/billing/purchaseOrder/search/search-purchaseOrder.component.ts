import { Router } from '@angular/router';
import { Component, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { MessageService } from "app/services/common/message.service";
import { PurchaseOrderService } from 'app/services/billing/purchaseOrder.service';
import { MenuService } from 'app/services/admin/menu.service';
import { PurchaseOrderViewComponent } from '../common/purchase-order-view.component';
import { PurchaseOrderViewFilterComponent } from '../common/purchase-order-view-filter.component';

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
        public menuService: MenuService,
        private errorHandlerService: ErrorHandlerService) {}

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
            this.errorHandlerService.handleErrors(err);
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
