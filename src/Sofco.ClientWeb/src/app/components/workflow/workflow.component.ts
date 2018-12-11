import { OnDestroy, OnInit, Component, Input, EventEmitter, Output, ViewChild } from "@angular/core";
import { WorkflowService } from "app/services/workflow/workflow.service";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { WorkflowStateType } from "app/models/enums/workflowStateType";

@Component({
    selector: 'workflow',
    templateUrl: './workflow.component.html'
})
export class WorkflowComponent implements OnDestroy {

    public transitions: any[] = new Array();
    public transitionsWithParameters: any[] = new Array();

    private entityId: number;
    private entityController: string;

    getTransitionsSubscrip: Subscription;
    postSubscrip: Subscription;

    @Output() onSaveSuccess = new EventEmitter<any>();

    hasRejectCode: boolean = false;
    @ViewChild('wfreject') wfReject;

    constructor(private workflowService: WorkflowService,
                private messageService: MessageService){}

    ngOnDestroy(): void {
        if(this.getTransitionsSubscrip) this.getTransitionsSubscrip.unsubscribe();
    }

    init(model){
        this.entityId = model.entityId;
        this.entityController = model.entityController;

        this.getTransitionsSubscrip = this.workflowService.getTransitions(model).subscribe(response => {
            this.transitions = response.data.filter(x => x.parameterCode == null);

            var transitionsWithParameters = response.data.filter(x => x.parameterCode != null);

            transitionsWithParameters.forEach(item => {
                if(item.parameterCode == 'REJECT'){
                    this.hasRejectCode = true;

                    var model = {
                        workflowId: item.workflowId,
                        nextStateId: item.nextStateId,
                        entityId: this.entityId,
                        entityController: this.entityController,
                        status: this.getStatusClass(item.workFlowStateType),
                        nextStateDescription: item.nextStateDescription
                    }

                    setTimeout(() => {
                        this.wfReject.init(model);
                    }, 0);
                }
            });
        });
    } 

    save(item){
        var model = {
            workflowId: item.workflowId,
            nextStateId: item.nextStateId,
            entityId: this.entityId,
            entityController: this.entityController
        }

        this.messageService.showLoading();

        this.postSubscrip = this.workflowService.post(model).subscribe(response => {
            this.messageService.closeLoading();
            this.onSaveSuccess.emit();
        });
    }

    onTransitionCustomConfirm(){
        this.onSaveSuccess.emit();
    }

    getStatusClass(type){
        switch(type){
            case WorkflowStateType.Info: return "btn-success btn-outline dim btn-md";
            case WorkflowStateType.Warning: return "btn-warning btn-outline dim btn-md";
            case WorkflowStateType.Success: return "btn-primary btn-outline dim btn-md";
            case WorkflowStateType.Danger: return "btn-danger btn-outline dim btn-md";
            default: return ""
        }
    }
}