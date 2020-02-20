import { FormGroup, FormControl, Validators } from "@angular/forms";
import * as moment from 'moment';
import { FormValidationService } from "app/services/common/form-validation.service";

export class RefundDetail extends FormGroup {

    private id: number;
    private refundId: number;
    public order: number;
    public costTypeDesc: string;

    constructor(domain?) {
        super({
            creationDate: new FormControl(domain && domain.creationDate && moment(domain.creationDate).toDate() || null,
                [Validators.required, FormValidationService.dateRangeValidator]
                ),

            description: new FormControl(domain && domain.description || null, [
                Validators.required,
                Validators.maxLength(300)]),

            ammount: new FormControl(domain && domain.ammount || null, [
                Validators.required,
                Validators.max(1000000),
                Validators.min(1)
            ]),

            costTypeId: new FormControl(domain && domain.costTypeId || null)
        });

        if(domain){
            this.id = domain.id || 0;
            this.refundId = domain.refundId || 0;
            this.order = domain.order || 0;
            this.costTypeDesc = domain.costTypeDesc || "";
        }
    }

    getModel(){
        return {
            id: this.id,
            creationDate: this.controls.creationDate.value,
            description: this.controls.description.value,
            ammount: this.controls.ammount.value,
            costTypeId: this.controls.costTypeId.value,
            refundId: this.refundId,
            order: this.order
        }
    }
}
