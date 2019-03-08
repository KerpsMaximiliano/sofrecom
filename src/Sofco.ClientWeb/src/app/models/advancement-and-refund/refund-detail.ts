import { FormGroup, FormControl, Validators } from "@angular/forms";
import * as moment from 'moment';
import { FormValidationService } from "app/services/common/form-validation.service";

export class RefundDetail extends FormGroup {

    private id: number;
    private refundId: number;

    constructor(domain?) {
        super({
            creationDate: new FormControl(domain && moment(domain.creationDate).toDate() || null,
                [Validators.required, FormValidationService.dateRangeValidator]
                ),

            description: new FormControl(domain && domain.description || null, [
                Validators.required,
                Validators.maxLength(300)]),

            ammount: new FormControl(domain && domain.ammount || null, [
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
