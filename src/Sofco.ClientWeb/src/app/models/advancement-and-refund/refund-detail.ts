import { FormGroup, FormControl, Validators } from "@angular/forms";
import * as moment from 'moment';

export class RefundDetail extends FormGroup {

    private id: number;
    private refundId: number;

    constructor(isReadonly: boolean, domain?) {
        super({
            creationDate: new FormControl({value: domain && moment(domain.creationDate).toDate() || null, disabled: isReadonly}, 
                Validators.required),

            description: new FormControl({value: domain && domain.description || null, disabled: isReadonly}, [
                Validators.required,
                Validators.maxLength(300)]),

            ammount: new FormControl({value: domain && domain.ammount || null, disabled: isReadonly}, [
                Validators.required,
                Validators.max(1000000),
                Validators.min(1)
            ])
        });

        if(domain){
            this.id = domain.id || 0;
            this.refundId = domain.refundId || 0;
        }
    }

    getModel(){
        return {
            id: this.id,
            creationDate: this.controls.creationDate.value,
            description: this.controls.description.value,
            ammount: this.controls.ammount.value,
            refundId: this.refundId
        }
    }
}