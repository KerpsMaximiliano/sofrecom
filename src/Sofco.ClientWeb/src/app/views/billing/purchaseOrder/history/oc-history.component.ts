import { Component, ViewChild, OnDestroy, OnInit } from '@angular/core';
import { Ng2ModalConfig } from '../../../../components/modal/ng2modal-config';
import { Subscription } from 'rxjs';
import { DataTableService } from '../../../../services/common/datatable.service';
import { PurchaseOrderService } from '../../../../services/billing/purchaseOrder.service';
import { I18nService } from '../../../../services/common/i18n.service';

@Component({
  selector: 'oc-history',
  templateUrl: './oc-history.component.html',
  styleUrls: ['./oc-history.component.scss']
})
export class PurchaseOrderHistoryComponent implements OnInit, OnDestroy {

    public histories: any[] = new Array<any>();
    public historyComments: string;
    
    @ViewChild('commentsModal') commentsModal;

    public commentsModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "comments",
        "commentsModal",
        false,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.close" 
    );
 
    getHistoriesSubscrip: Subscription;

    constructor(private purchaseOrderService: PurchaseOrderService,
                private i18nService: I18nService,
                private datatableService: DataTableService) {
    }

    ngOnInit() {
    }

    getHistories(id){
        this.getHistoriesSubscrip = this.purchaseOrderService.getHistories(id).subscribe(d => {
            this.histories = d;

            var options = { selector: "#historyTable" }
            this.datatableService.destroy(options.selector);
            this.datatableService.initialize(options);
        });
    }

    ngOnDestroy(){
        if(this.getHistoriesSubscrip) this.getHistoriesSubscrip.unsubscribe();
    }

    showComments(history){
        this.historyComments = history.comment;
        this.commentsModal.show();
    }

    getStatus(status){
        switch(status){
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