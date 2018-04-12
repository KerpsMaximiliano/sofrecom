import { Component, Input, ViewChild, OnDestroy, OnInit, Output, EventEmitter } from '@angular/core';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { Subscription } from 'rxjs';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { DataTableService } from 'app/services/common/datatable.service';
import { LicenseService } from 'app/services/human-resources/licenses.service';

@Component({
  selector: 'license-history',
  templateUrl: './license-history.component.html',
  styleUrls: ['./license-history.component.scss']
})
export class LicenseHistoryComponent implements OnInit, OnDestroy {

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

    constructor(private licenseService: LicenseService,
                private errorHandlerService: ErrorHandlerService,
                private datatableService: DataTableService) {
    }

    ngOnInit() {
    }

    getHistories(id){
        this.getHistoriesSubscrip = this.licenseService.getHistories(id).subscribe(d => {
            this.histories = d;

            this.datatableService.destroy('#historyTable');
            this.datatableService.init('#historyTable', false);
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