import { FormGroup, FormControl, Validators } from "@angular/forms";
import { WorkflowStateType } from "../enums/workflowStateType";

export class Refund extends FormGroup {

    private id: number;
    public workflowStateType: WorkflowStateType;

    constructor(isReadonly: boolean, domain?) {
        super({
            userApplicantId: new FormControl(domain && domain.userApplicantId || null, 
                Validators.required),
                
            currencyId: new FormControl({value: domain && domain.currencyId || null, disabled: isReadonly}, 
                Validators.required),

            advancements: new FormControl({value: domain && domain.advancements || null, disabled: isReadonly}, 
                Validators.required),

            contract: new FormControl({value: domain && domain.contract || null, disabled: isReadonly}, 
                Validators.maxLength(300)),
        });

        if(domain){
            this.id = domain.id || 0;
            this.workflowStateType = domain.workflowStateType;
        }
    }
}