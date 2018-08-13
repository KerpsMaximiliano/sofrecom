import { Component, OnDestroy } from "@angular/core";
import { OnInit } from "@angular/core/src/metadata/lifecycle_hooks";
import { Subscription } from "rxjs";
import { PurchaseOrderService } from "../../../../services/billing/purchaseOrder.service";
import { Router } from "@angular/router";
import { DataTableService } from "../../../../services/common/datatable.service";
import { I18nService } from "../../../../services/common/i18n.service";
import { MessageService } from "../../../../services/common/message.service";

@Component({
    selector: 'purchase-orders-pendings',
    templateUrl: './oc-pendings.component.html'
})
export class PurchaseOrderPendingsComponent implements OnInit, OnDestroy {

    public model: any[] = new Array();

    getSubscrip: Subscription;

    constructor(private purchaseOrderService: PurchaseOrderService,
                private router: Router,
                private messageService: MessageService,
                private i18nService: I18nService,
                private datatableService: DataTableService){}

    ngOnInit(): void {
        this.getPendings();
    }

    ngOnDestroy(): void {
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
    }

    getPendings(){
        this.messageService.showLoading();

        this.purchaseOrderService.getPendings().subscribe(res => {
            this.messageService.closeLoading();

            this.model = res.data;

            this.initGrid();
        },
        err => {
            this.messageService.closeLoading();
        });
    }

    initGrid(){
        var options = { selector: "#purchaseOrderTable" }
        this.datatableService.initialize(options);
    }

    goToDetail(item){
        this.router.navigate([`/billing/purchaseOrders/${item.id}`]);
    }

    getStatus(item){
        switch(item.status){
            case 1: return this.i18nService.translateByKey("Valid");
            case 2: return this.i18nService.translateByKey("Consumed");
            case 3: return this.i18nService.translateByKey("Closed");
            case 4: return this.i18nService.translateByKey("Draft");
            case 5: return this.i18nService.translateByKey("ComercialPending");
            case 6: return this.i18nService.translateByKey("OperativePending");
            case 7: return this.i18nService.translateByKey("DafPending");
            case 8: return this.i18nService.translateByKey("Reject");
            case 9: return this.i18nService.translateByKey("CompliancePending");
        }
    }
} 