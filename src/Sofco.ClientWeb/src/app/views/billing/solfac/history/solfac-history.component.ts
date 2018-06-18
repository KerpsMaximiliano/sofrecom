import { Component, Input, ViewChild, OnDestroy, OnInit, Output, EventEmitter } from '@angular/core';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { Subscription } from 'rxjs';
import { SolfacService } from 'app/services/billing/solfac.service';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { DataTableService } from 'app/services/common/datatable.service';

@Component({
  selector: 'solfac-history',
  templateUrl: './solfac-history.component.html',
  styleUrls: ['./solfac-history.component.scss']
})
export class SolfacHistoryComponent implements OnInit, OnDestroy {

    @Input() solfacId: number;
    
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

    constructor(private solfacService: SolfacService,
                private errorHandlerService: ErrorHandlerService,
                private datatableService: DataTableService) {

    }

    ngOnInit() {
        this.getHistories();
    }

    getHistories(){
        this.getHistoriesSubscrip = this.solfacService.getHistories(this.solfacId).subscribe(d => {
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