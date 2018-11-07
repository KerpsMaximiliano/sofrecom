import { FormGroup, FormControl } from "@angular/forms";
import { Validators } from '@angular/forms';

export class Advancement extends FormGroup {

    constructor() {
        super({
            userApplicantId: new FormControl(null, Validators.required),
            paymentForm: new FormControl('1', Validators.required),
            type: new FormControl('1', Validators.required),
            advancementReturnFormId: new FormControl(null, Validators.required),
            startDateReturn: new FormControl(null, Validators.required),
            analyticId: new FormControl(null, Validators.required),
            currencyId: new FormControl(null, Validators.required),
        });
    }

    getModel(){
        return {
            userApplicantId: this.controls.userApplicantId.value,
            paymentForm: this.controls.paymentForm.value,
            type: this.controls.type.value,
            advancementReturnFormId: this.controls.advancementReturnFormId.value,
            startDateReturn: this.controls.startDateReturn.value,
            analyticId: this.controls.analyticId.value,
            currencyId: this.controls.currencyId.value,
            details: []
        }
    }
}

export class AdvancementDetail extends FormGroup {

    constructor(){
        super({
            date: new FormControl(null, Validators.required),
            description: new FormControl(null, [
                Validators.required, 
                Validators.maxLength(400)
            ]),
            ammount: new FormControl(null, [
                Validators.required,
                Validators.max(1000000),
                Validators.min(1)
            ])
        });
    }

    getModel(){
        return {
            date: this.controls.date.value,
            description: this.controls.description.value,
            ammount: this.controls.ammount.value,
        }
    }
}