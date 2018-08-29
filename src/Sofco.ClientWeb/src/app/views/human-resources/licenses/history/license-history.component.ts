import { Component, ViewChild, OnDestroy, OnInit } from '@angular/core';
import { Ng2ModalConfig } from '../../../../components/modal/ng2modal-config';
import { Subscription } from 'rxjs';
import { DataTableService } from '../../../../services/common/datatable.service';
import { LicenseService } from '../../../../services/human-resources/licenses.service';

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
                private datatableService: DataTableService) {
    }

    ngOnInit() {
    }

    getHistories(id){
        this.getHistoriesSubscrip = this.licenseService.getHistories(id).subscribe(d => {
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
}