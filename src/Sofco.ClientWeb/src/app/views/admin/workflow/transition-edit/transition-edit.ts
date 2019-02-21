import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { WorkflowService } from "app/services/workflow/workflow.service";
import { MessageService } from "app/services/common/message.service";
import { Subscription } from "rxjs";
import { ActivatedRoute } from "@angular/router";

@Component({
    selector: 'workflow-transition-edit',
    templateUrl: './transition-edit.html'
})
export class WorkflowTransitionEditComponent implements OnInit, OnDestroy {

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

        this.messageService.showLoading();

        this.postSubscrip = this.workflowService.getTransition(routeParams.id).subscribe(response => {
            this.messageService.closeLoading();

            this.form.model = response.data;
        }, 
        () => {
            this.messageService.closeLoading();
        });
    }    
    
    ngOnDestroy(): void {
        if(this.postSubscrip) this.postSubscrip.unsubscribe();
    }

    update(){
        var model = this.form.model;

        this.messageService.showLoading();

        this.postSubscrip = this.workflowService.putTransition(model).subscribe(() => {
            this.messageService.closeLoading();
            window.history.back();
        }, 
        () => {
            this.messageService.closeLoading();
        });
    }

}