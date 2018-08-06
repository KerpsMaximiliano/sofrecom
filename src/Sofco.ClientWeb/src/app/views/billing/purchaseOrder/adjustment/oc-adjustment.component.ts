import { Component, OnDestroy, ViewChild } from "@angular/core";
import { MessageService } from "app/services/common/message.service";
import { PurchaseOrderService } from "app/services/billing/purchaseOrder.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";

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

    constructor(private purchaseOrderService: PurchaseOrderService,
        private messageService: MessageService,
        private errorHandlerService: ErrorHandlerService){
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
            response => {
                this.adjustmentModal.hide();
                if(response.messages) this.messageService.showMessages(response.messages);

                setTimeout(() => {
                    window.location.reload();
                }, 1000);
            },
            err => {
                this.errorHandlerService.handleErrors(err);
            });
    }
}