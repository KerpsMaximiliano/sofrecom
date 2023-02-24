import { Component, Input } from "@angular/core";

@Component({
    selector: 'purchase-orders-pending-bill-reception',
    templateUrl: './purchase-orders-pending-bill-reception.html'
})

export class PurchaseOrdersPendingBillReception {

    @Input() purchaseOrder: any;

    constructor() {}
}