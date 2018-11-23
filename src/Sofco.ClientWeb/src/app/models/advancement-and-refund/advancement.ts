import { FormGroup, FormControl } from "@angular/forms";
import { Validators } from '@angular/forms';
import * as moment from 'moment';
import { WorkflowStateType } from "../enums/workflowStateType";

export class Advancement extends FormGroup {

    private id: number;
    public workflowStateType: WorkflowStateType;

    constructor(domain?) {
        super({
            userApplicantId: new FormControl(domain && domain.userApplicantId || null, 
                Validators.required),

            paymentForm: new FormControl(domain && domain.paymentForm.toString() || '1', 
                Validators.required),

            type: new FormControl(domain && domain.type.toString() || '1', 
                Validators.required),

            advancementReturnFormId: new FormControl(domain && domain.advancementReturnFormId || null, 
                Validators.required),

            startDateReturn: new FormControl(domain && moment(domain.startDateReturn).toDate() || null, 
                Validators.required),

            currencyId: new FormControl(domain && domain.currencyId || null, 
                Validators.required),

            description: new FormControl(domain && domain.description || null, [
                Validators.required, 
                Validators.maxLength(400)
            ]),
            
            ammount: new FormControl(domain && domain.ammount || null, [
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