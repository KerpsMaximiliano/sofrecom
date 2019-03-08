import { Component, OnInit, OnDestroy } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { Subscription } from "rxjs";
import { MenuService } from "app/services/admin/menu.service";
import { WorkflowService } from "app/services/workflow/workflow.service";
import { MessageService } from "app/services/common/message.service";

@Component({
    selector: 'workflow-state-add',
    templateUrl: './state-edit.component.html'
})

export class WorkflowStateEditComponent implements OnInit, OnDestroy {

    private id: number;

    public Name: string;
    public ActionName: string;
    public TypesId: number;
    public types: any[] = new Array();

    public EditSubscription: Subscription;
    public TypeSubscrip: Subscription;
    public ParamsSubscrip: Subscription;
    public GetSubscrip : Subscription;

    constructor(private router: Router, 
                private activatedRoute: ActivatedRoute,
                private workflowService: WorkflowService,
                private messageService: MessageService){}

    ngOnInit(): void {
        //Called after the constructor, initializing input properties, and the first call to ngOnChanges.
        //Add 'implements OnInit' to the class.
        this.messageService.showLoading();

        this.ParamsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.id = params['id'];
            this.getDetails();
        });

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
        if(this.EditSubscription) this.EditSubscription.unsubscribe();
        if(this.TypeSubscrip) this.TypeSubscrip.unsubscribe();
        if(this.ParamsSubscrip) this.ParamsSubscrip.unsubscribe();
        if(this.GetSubscrip) this.GetSubscrip.unsubscribe();        
    }

    goBack(){
        this.router.navigate(["admin/states"])
    }

    getDetails(){
        this.messageService.showLoading();

        this.GetSubscrip = this.workflowService.getWorkflowState(this.id).subscribe(response => {
            this.messageService.closeLoading();
            this.Name = response.data.name;
            this.ActionName = response.data.actionName;
            this.TypesId = response.data.idType;
        }, 
        error => this.messageService.closeLoading());
    }

    update(){
        this.messageService.showLoading();

        this.EditSubscription = this.workflowService.editWorkflowState({ id: this.id, Name: this.Name, ActionName: this.ActionName, IdType: this.TypesId }).subscribe(response => {
            this.messageService.closeLoading();
            this.router.navigate(["/admin/states"]);    
        }, 
        error => this.messageService.closeLoading());
    }

}