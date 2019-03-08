import { Component, OnInit, OnDestroy } from "@angular/core";
import { Router } from "@angular/router";
import { Subscription } from "rxjs";
import { MenuService } from "app/services/admin/menu.service";
import { WorkflowService } from "app/services/workflow/workflow.service";
import { MessageService } from "app/services/common/message.service";


@Component({
    selector: 'workflow-state-add',
    templateUrl: './state-add.component.html'
})

export class WorkflowStateAddComponent implements OnInit, OnDestroy {

    public Name: string;
    public ActionName: string;
    public TypesId: number;
    public types: any[] = new Array();

    public AddSubscription: Subscription;
    public TypeSubscrip: Subscription;

    constructor(private router: Router,
        private workflowService: WorkflowService,
        private messageService: MessageService) { }

    ngOnInit(): void {
        //Called after the constructor, initializing input properties, and the first call to ngOnChanges.
        //Add 'implements OnInit' to the class.
        this.messageService.showLoading();

        this.TypeSubscrip = this.workflowService.getWorkflowStateType().subscribe(
            response => {
                this.messageService.closeLoading();
                this.types = response;
            },
            error => this.messageService.closeLoading());
    }

    ngOnDestroy(): void {
        //Called once, before the instance is destroyed.
        //Add 'implements OnDestroy' to the class.
        if(this.AddSubscription) this.AddSubscription.unsubscribe();
        if(this.TypeSubscrip) this.TypeSubscrip.unsubscribe();
    }

    goBack() {
        this.router.navigate(["admin/states"])
    }

    save(){
        this.messageService.showLoading();

        this.AddSubscription = this.workflowService.AddWorkflowState({ Name: this.Name, ActionName: this.ActionName, IdType: this.TypesId }).subscribe(response => {
            this.messageService.closeLoading();
            this.router.navigate(["/admin/states"]);    
        }, 
        error => this.messageService.closeLoading());
    }
}
