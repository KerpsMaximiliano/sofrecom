import { FormGroup, FormControl } from "@angular/forms";
import { Validators } from '@angular/forms';

export class Setting extends FormGroup {

    constructor(domain) {
        super({
            ammountAPesos: new FormControl(domain.ammountAPesos, [
                Validators.required,
                Validators.min(1)
            ]),

            ammountADolares: new FormControl(domain.ammountADolares, [
                Validators.required,
                Validators.min(1)
            ]),

            ammountAEuros: new FormControl(domain.ammountAEuros, [
                Validators.required,
                Validators.min(1)
            ]),

            ammountBPesos: new FormControl(domain.ammountBPesos, [
                Validators.required,
                Validators.min(1)
            ]),

            ammountBDolares: new FormControl(domain.ammountBDolares, [
                Validators.required,
                Validators.min(1)
            ]),

            ammountBEuros: new FormControl(domain.ammountBEuros, [
                Validators.required,
                Validators.min(1)
            ])
        });
    }

    getModel(){
        return {
            ammountAPesos: this.controls.ammountAPesos.value,
            ammountADolares: this.controls.ammountADolares.value,
            ammountAEuros: this.controls.ammountAEuros.value,
            ammountBPesos: this.controls.ammountBPesos.value,
            ammountBDolares: this.controls.ammountBDolares.value,
            ammountBEuros: this.controls.ammountBEuros.value,
        }
    }
}