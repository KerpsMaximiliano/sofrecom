import { FormGroup, FormControl } from "@angular/forms";
import { Validators } from '@angular/forms';
import * as moment from 'moment';
import { WorkflowStateType } from "../enums/workflowStateType";

export class Advancement extends FormGroup {

    private id: number;
    public workflowStateType: WorkflowStateType;

    constructor(isReadonly: boolean, domain?) {
        super({
            userApplicantId: new FormControl(domain && domain.userApplicantId || null, 
                Validators.required),

            paymentForm: new FormControl({value: domain && domain.paymentForm.toString() || '1', disabled: isReadonly}, 
                Validators.required),

            type: new FormControl({value: domain && domain.type.toString() || '1', disabled: isReadonly}, 
                Validators.required),

            advancementReturnFormId: new FormControl({value: domain && domain.advancementReturnFormId || null, disabled: isReadonly}, 
                Validators.required),

            startDateReturn: new FormControl({value: domain && moment(domain.startDateReturn).toDate() || null, disabled: isReadonly}, 
                Validators.required),

            currencyId: new FormControl({value: domain && domain.currencyId || null, disabled: isReadonly}, 
                Validators.required),

            description: new FormControl({value: domain && domain.description || null, disabled: isReadonly}, 
                Validators.maxLength(1000)),
            
            ammount: new FormControl({value: domain && domain.ammount || null, disabled: isReadonly}, [
                Validators.required,
                Validators.max(1000000),
                Validators.min(1)
            ])
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
            paymentForm: this.controls.paymentForm.value,
            type: this.controls.type.value,
            advancementReturnFormId: this.controls.advancementReturnFormId.value,
            startDateReturn: this.controls.startDateReturn.value,
            currencyId: this.controls.currencyId.value,
            description: this.controls.description.value,
            ammount: this.controls.ammount.value
        }
    }
}