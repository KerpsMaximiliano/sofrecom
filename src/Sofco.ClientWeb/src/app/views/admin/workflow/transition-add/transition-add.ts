import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { WorkflowService } from "app/services/workflow/workflow.service";
import { MessageService } from "app/services/common/message.service";
import { Subscription } from "rxjs";
import { ActivatedRoute } from "@angular/router";

@Component({
    selector: 'workflow-transition-add',
    templateUrl: './transition-add.html'
})
export class WorkflowTransitionAddComponent implements OnInit, OnDestroy {

    @ViewChild('form') form;

    postSubscrip: Subscription;

    workflowId: number;

    constructor(private messageService: MessageService,
                private activateRoute: ActivatedRoute,
                private workflowService: WorkflowService){
    }

    ngOnInit(): void {
        const routeParams = this.activateRoute.snapshot.params;
        this.workflowId = routeParams.workflowId;
    }    
    
    ngOnDestroy(): void {
        if(this.postSubscrip) this.postSubscrip.unsubscribe();
    }

    add(){
        var model = this.form.model;
        model.workflowId = this.workflowId;

        this.messageService.showLoading();

        this.postSubscrip = this.workflowService.addTransition(model).subscribe(() => {
            this.messageService.closeLoading();
            window.history.back();
        }, 
        () => {
            this.messageService.closeLoading();
        });
    }

}