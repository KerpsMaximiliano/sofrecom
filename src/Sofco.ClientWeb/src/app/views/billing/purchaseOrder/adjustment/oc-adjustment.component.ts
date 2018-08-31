import { Component, OnDestroy, ViewChild } from "@angular/core";
import { PurchaseOrderService } from "../../../../services/billing/purchaseOrder.service";
import { Subscription } from "rxjs";
import { Ng2ModalConfig } from "../../../../components/modal/ng2modal-config";

@Component({
    selector: 'purchase-order-adjustment',
    templateUrl: './oc-adjustment.component.html',
    styleUrls: ['./oc-adjustment.component.scss']
})
export class PurchaseOrderAdjustmentComponent implements OnDestroy {

    @ViewChild('adjustmentModal') adjustmentModal;
    public adjustmentConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "billing.purchaseOrder.adjustmentModelTitle",
        "adjustmentModal", 
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    public details: any[] = new Array();
    public id: number;

    addSubscrip: Subscription;

    constructor(private purchaseOrderService: PurchaseOrderService){
    }

    ngOnDestroy(): void {
        if(this.addSubscrip) this.addSubscrip.unsubscribe();
    }

    show(settings){
        this.id = settings.id;
        this.details = settings.details;
        this.adjustmentModal.show();
    }

    create(){
        this.addSubscrip = this.purchaseOrderService.makeAdjustment(this.id, this.details).subscribe(
            () => {
                this.adjustmentModal.hide();
                setTimeout(() => {
                    window.location.reload();
                }, 1000);
            },
            error => this.adjustmentModal.resetButtons());
    }
}