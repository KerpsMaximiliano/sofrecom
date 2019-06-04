import { Component, OnDestroy, ViewChild, Input, EventEmitter, Output } from '@angular/core';
import { Subscription } from "rxjs";
import { WorkflowService } from 'app/services/workflow/workflow.service';
import { MessageService } from 'app/services/common/message.service';

@Component({
  selector: 'wf-cash-return-confirm',
  templateUrl: './cash-return-confirm.html'
})
export class WfCashReturnConfirmComponent implements OnDestroy  {
    postSubscrip: Subscription;

    @Output() onConfirm = new EventEmitter<any>();

    public model: any = {
        workflowId: 0,
        nextStateId: 0,
        entityId: 0,
        entityController: 0,
        parameters: []
    }

    status: string;
    text: string;

    constructor(private workflowService: WorkflowService, private messageService: MessageService){

    }

    ngOnDestroy(): void {
        if(this.postSubscrip) this.postSubscrip.unsubscribe();
    }

    init(model){
        this.model.workflowId = model.workflowId;
        this.model.nextStateId = model.nextStateId;
        this.model.entityId = model.entityId;
        this.model.entityController = model.entityController;

        this.status = model.status;
        this.text = model.nextStateDescription;
    }

    showConfirm(){
        this.model.parameters = {
            'empty': ""
        };

        var self = this;
        this.messageService.showCustomConfirm("¿ Confirma que esta recibiendo el dinero en efectivo ?", () => {

            self.postSubscrip = self.workflowService.post(self.model).subscribe(response => {
                self.onConfirm.emit();
            });
        });
    }
}