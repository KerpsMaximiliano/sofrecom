import { Component, OnDestroy, ViewChild, Input, EventEmitter, Output } from '@angular/core';
import { Subscription } from "rxjs";
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { WorkflowService } from 'app/services/workflow/workflow.service';

@Component({
  selector: 'wf-reject',
  templateUrl: './wf-reject.component.html'
})
export class WfRejectComponent implements OnDestroy  {

    @ViewChild('rejectModal') modal;
    public rejectModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "rejectModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    postSubscrip: Subscription;

    @Output() onConfirm = new EventEmitter<any>();

    public model: any = {
        workflowId: 0,
        nextStateId: 0,
        entityId: 0,
        entityController: 0,
        parameters: []
    }

    rejectComments: string;
    status: string;
    text: string;

    constructor(private workflowService: WorkflowService){}

    ngOnDestroy(): void {
        if(this.postSubscrip) this.postSubscrip.unsubscribe();
    }

    showModal(){
        this.modal.show();
    }

    init(model){
        this.model.workflowId = model.workflowId;
        this.model.nextStateId = model.nextStateId;
        this.model.entityId = model.entityId;
        this.model.entityController = model.entityController;

        this.status = model.status;
        this.text = model.nextStateDescription;
    }

    save(){
        this.model.parameters = {
            'comments': this.rejectComments
        };

        this.postSubscrip = this.workflowService.post(this.model).subscribe(response => {
            this.modal.hide();
            this.onConfirm.emit();
        },
        error => {
            this.modal.resetButtons();
        });
    }
}