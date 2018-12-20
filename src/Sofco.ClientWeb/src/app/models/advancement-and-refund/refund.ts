import { FormGroup, FormControl, Validators } from "@angular/forms";
import { WorkflowStateType } from "../enums/workflowStateType";

export class Refund extends FormGroup {

    private id: number;
    public workflowStateType: WorkflowStateType;

    constructor(isReadonly: boolean, domain?) {
        super({
            userApplicantId: new FormControl(domain && domain.userApplicantId || null, 
                Validators.required),
                
            currencyId: new FormControl({value: domain && domain.currencyId || null, disabled: isReadonly}),

            advancements: new FormControl({value: domain && domain.advancementIds || null, disabled: isReadonly}, 
                Validators.required),

            analyticId: new FormControl({value: domain && domain.analyticId || null, disabled: isReadonly},
                Validators.required),
        });

        if(domain){
            this.id = domain.id || 0;
            this.workflowStateType = domain.workflowStateType;
        }
    } 

    getModel(){
        return {
            id: this.id,
            userApplicantId: this.controls.userApplicantId.value,
            currencyId: this.controls.currencyId.value,
            analyticId: this.controls.analyticId.value,
            advancements: this.controls.advancements.value,
            details: new Array()
        }
    }
}