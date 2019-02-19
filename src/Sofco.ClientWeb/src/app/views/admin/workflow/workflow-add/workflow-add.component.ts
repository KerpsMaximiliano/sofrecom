import { Component, OnInit, OnDestroy, ViewChild, EventEmitter, Output } from "@angular/core";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { WorkflowService } from "app/services/workflow/workflow.service";
import { MessageService } from "app/services/common/message.service";
import { Subscription } from "rxjs";

@Component({
    selector: 'workflow-add',
    templateUrl: './workflow-add.component.html'
})
export class WorkflowAddComponent implements OnInit, OnDestroy {

    @ViewChild('addModal') addModal;
    public addModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "addModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    getSubscrip: Subscription;
    postSubscrip: Subscription;

    @Output() onSuccess: EventEmitter<any> = new EventEmitter();

    public types: any[] = new Array();

    public model: any = {
        description: "",
        type: null,
        version: "",
        id: null
    }

    constructor(private messageService: MessageService,
                private workflowService: WorkflowService){
    }

    ngOnInit(): void {
        this.getTypes();
    }    
    
    ngOnDestroy(): void {
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
        if(this.postSubscrip) this.postSubscrip.unsubscribe();
    }

    getTypes() {
        this.getSubscrip = this.workflowService.getTypes().subscribe(res => {
            this.types = res.data;
        });
    }

    add(){
        this.postSubscrip = this.workflowService.add(this.model).subscribe(res => {
            this.addModal.hide();

            if(this.onSuccess.observers.length > 0){
                this.onSuccess.emit(res.data);
            }

            this.model = {
                description: "",
                type: null,
                version: "",
                id: null
            };
        }, 
        error => {
            this.addModal.resetButtons();
        });
    }

    show(){
        this.addModal.show();
    }
}