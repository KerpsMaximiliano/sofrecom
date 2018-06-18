import { Component, Input, ViewChild, OnDestroy, OnInit, Output, EventEmitter } from '@angular/core';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { Subscription } from 'rxjs';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { DataTableService } from 'app/services/common/datatable.service';
import { InvoiceService } from 'app/services/billing/invoice.service';

@Component({
  selector: 'invoice-history',
  templateUrl: './invoice-history.component.html'
})
export class InvoiceHistoryComponent implements OnInit, OnDestroy {

    @Input() invoiceId: number;
    
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

    constructor(private invoiceService: InvoiceService,
                private errorHandlerService: ErrorHandlerService,
                private datatableService: DataTableService) {

    }

    ngOnInit() {
        this.getHistories();
    }

    getHistories(){
        this.getHistoriesSubscrip = this.invoiceService.getHistories(this.invoiceId).subscribe(d => {
            this.histories = d;

            var options = { selector: "#historyTable" }
            this.datatableService.destroy(options.selector);
            this.datatableService.initialize(options);
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    ngOnDestroy(){
        if(this.getHistoriesSubscrip) this.getHistoriesSubscrip.unsubscribe();
    }

    showComments(history){
        this.historyComments = history.comment;
        this.commentsModal.show();
    }
}