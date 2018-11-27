import { Component, ViewChild, OnDestroy } from '@angular/core';
import { Ng2ModalConfig } from '../../../../components/modal/ng2modal-config';
import { Subscription } from 'rxjs';
import { DataTableService } from '../../../../services/common/datatable.service';
import { WorkflowStateType } from 'app/models/enums/workflowStateType';
import { AdvancementService } from 'app/services/advancement-and-refund/advancement.service';

@Component({
  selector: 'advancement-history',
  templateUrl: './advancement-histories.component.html',
  styleUrls: ['./advancement-histories.component.scss']
})
export class AdvancementHistoryComponent implements OnDestroy {

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

    constructor(private advancementService: AdvancementService,
                private datatableService: DataTableService) {
    }

    getHistories(id){
        this.getHistoriesSubscrip = this.advancementService.getHistories(id).subscribe(response => {
            this.histories = response.data;

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

    getStatusClass(type){
        switch(type){
            case WorkflowStateType.Info: return "label-success";
            case WorkflowStateType.Warning: return "label-warning";
            case WorkflowStateType.Success: return "label-primary";
            case WorkflowStateType.Danger: return "label-danger";
        }
    }
}