import { Component, OnInit } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { GrossIncomeTypes } from "app/models/enums/GrossIncomeTypes";
import { ProvidersService } from "app/services/admin/providers.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { MessageService } from "app/services/common/message.service"

@Component({
    selector: 'providers-add',
    templateUrl: './providers-add.html'
})

export class ProvidersAddComponent implements OnInit{

    activeProvidersArea = [];
    critical: string = null;
    incomeTypes = [
        { id: 1, description: "Inscripto en régimen local General" },
        { id: 2, description: "Inscripto en régimen local simplificado" },
        { id: 3, description: "Inscripto Convenio Multilateral" },
        { id: 4, description: "Extento" },
        { id: 5, description: "No aplica" }
    ];
    IVAConditions = [
        { id: 1, description: "Resp. Inscripto" },
        { id: 2, description: "Resp. No Inscripto" },
        { id: 3, description: "Monotributo" },
        { id: 4, description: "Exento/No Resp." },
    ]

    form: FormGroup = new FormGroup({
        id: new FormControl(0),
        name: new FormControl(null, [Validators.required, Validators.maxLength(200)]),
        providerAreaId: new FormControl(null, [Validators.required]),
        CUIT: new FormControl(null, [Validators.maxLength(11), Validators.minLength(11), Validators.required, Validators.pattern("^(20|23|27|30|33)([0-9]{8})([0-9]{1})$")]),
        ingresosBrutos: new FormControl(null, [Validators.required]),
        condicionIVA: new FormControl(null, [Validators.required]),
        address: new FormControl(null, [Validators.maxLength(1000), Validators.required]),
        city: new FormControl(null, [Validators.maxLength(1000), Validators.required]),
        ZIPCode: new FormControl(null, [Validators.maxLength(10), Validators.required]),
        province: new FormControl(null, [Validators.maxLength(20), Validators.required]),
        country: new FormControl(null, [Validators.maxLength(20), Validators.required]),
        contactName: new FormControl(null, [Validators.maxLength(100), Validators.required]),
        phone: new FormControl(null, [Validators.maxLength(15), Validators.required]),
        email: new FormControl(null, [Validators.maxLength(100), Validators.required, Validators.pattern("^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$")]),
        website: new FormControl(null, [Validators.maxLength(1000)]),
        comments: new FormControl(null, [Validators.maxLength(5000)])
    });

    constructor(
        private providersAreaService: ProvidersAreaService,
        private providersService: ProvidersService,
        private messageService: MessageService,
        private router: Router
    ) {
        
    }

    ngOnInit(): void {
        this.providersAreaService.getAll().subscribe(d => {
            d.data.forEach(area => {
                if(area.active) {
                    this.activeProvidersArea.push(area);
                    this.activeProvidersArea = [...this.activeProvidersArea]
                }
            });
        });
    }

    save() {
        this.markFormGroupTouched(this.form)
        if(!this.form.valid) {
            return;
        }
        let model = this.form.value;
        this.providersService.post(model).subscribe(response=>{
            if(response.status == 200) {
                this.messageService.showMessage("Proveedor guardado", 0);
                this.router.navigate([`providers/providers`]);
            }
        });
    }

    markFormGroupTouched(formGroup: FormGroup) {
        (<any>Object).values(formGroup.controls).forEach(control => {
            control.markAsTouched();

            if (control.controls) {
                this.markFormGroupTouched(control);
            }
        });
    }

    change(event) {
        if(event != undefined) {
            this.critical = (event.critical) ? "Si" : "No"
        } else {
            this.critical = null
        }
    }
}