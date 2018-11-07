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
}