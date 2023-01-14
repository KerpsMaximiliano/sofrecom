import { OnDestroy, OnInit, Component, Input, EventEmitter, Output, ViewChild, ViewContainerRef, ComponentFactoryResolver } from "@angular/core";
import { WorkflowService } from "app/services/workflow/workflow.service";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { WorkflowStateType } from "app/models/enums/workflowStateType";
import { WfRejectComponent } from "./templates/reject/wf-reject.component";
import { Type } from "@angular/compiler/src/core";
import { WfCurrencyExchangeComponent } from "./templates/currency-exchange/currency-exchange";
import { WfCashReturnConfirmComponent } from "./templates/cash-return-confirm/cash-return-confirm";

@Component({
    selector: 'workflow',
    templateUrl: './workflow.component.html'
})
export class WorkflowComponent implements OnDestroy {

    @ViewChild('container', {read: ViewContainerRef}) container: ViewContainerRef;

    public transitions: any[] = new Array();

    private entityId: number;
    private entityController: string;

    getTransitionsSubscrip: Subscription;
    postSubscrip: Subscription;

    awaitFiles: boolean = false;
    customValidation: boolean = false;

    @Output() onSaveSuccess = new EventEmitter<any>();

    hasRejectCode: boolean = false;
    @ViewChild('wfreject') wfReject;

    private requestNoteData;

    constructor(private workflowService: WorkflowService,
                private componentFactoryResolver: ComponentFactoryResolver,
                private messageService: MessageService){}

    ngOnDestroy(): void {
        if(this.getTransitionsSubscrip) this.getTransitionsSubscrip.unsubscribe();
    }

    init(model){
        console.log(model)
        this.transitions = [];
        this.container.clear();

        this.entityId = model.entityId;
        this.entityController = model.entityController;

        this.getTransitionsSubscrip = this.workflowService.getTransitions(model).subscribe(response => {
            console.log(response)
            this.transitions = response.data.filter(x => !x.parameterCode);

            var transitionsWithParameters = response.data.filter(x => x.parameterCode);

            this.buildComponents(transitionsWithParameters);
        });
    }

    filesToUpload() {
        this.awaitFiles = true;
    }

    filesUploaded() {
        this.awaitFiles = false;
    }

    setCustomValidations(status: boolean) {
        this.customValidation = status;
    }

    buildComponents(transitionsWithParameters){
        transitionsWithParameters.forEach(item => {

            var componentClass = this.getComponentClass(item.parameterCode);

            if(!componentClass) return;

            const componentFactory = this.componentFactoryResolver.resolveComponentFactory(componentClass);
            const component = this.container.createComponent(componentFactory);
        
            var model = {
                workflowId: item.workflowId,
                nextStateId: item.nextStateId,
                entityId: this.entityId,
                entityController: this.entityController,
                status: this.getStatusClass(item.workFlowStateType),
                nextStateDescription: item.nextStateDescription
            }

            component.instance.onConfirm.subscribe(x => {
                this.onTransitionCustomConfirm();
            });
            
            component.instance.init(model);
        });
    }

    save(item){
        if(this.customValidation) {
            return;
        };
        if(this.awaitFiles) {
            setTimeout(() => {
                this.save(item)
            }, 100);
        } else {
            let model;
            if(this.entityController == "RequestNoteBorrador") {
                model = {
                    workflowId: item.workflowId,
                    nextStateId: item.nextStateId,
                    entityId: this.entityId,
                    entityController: this.entityController,
                    requestNote: this.requestNoteData
                }
            } else {
                model = {
                    workflowId: item.workflowId,
                    nextStateId: item.nextStateId,
                    entityId: this.entityId,
                    entityController: this.entityController
                }
            }
            this.messageService.showLoading();
    
            this.postSubscrip = this.workflowService.post(model).subscribe(response => {
                this.messageService.closeLoading();
                this.onSaveSuccess.emit();
            },
            error => this.messageService.closeLoading());
        }
    }

    updateRequestNote(requestNote) {
        this.requestNoteData = requestNote;
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

    getComponentClass(type) : Type{
        switch(type){
            case "REJECT": return WfRejectComponent;
            case "CURRENCY-EXCHANGE": return WfCurrencyExchangeComponent;
            case "CASH-RETURN-CONFIRM": return WfCashReturnConfirmComponent;
            default: return null;
        }
    }
}