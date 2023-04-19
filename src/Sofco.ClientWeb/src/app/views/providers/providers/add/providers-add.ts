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
        { id: 4, description: "Excento" },
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
        CUIT: new FormControl(null, [Validators.maxLength(11), Validators.minLength(11), Validators.required, Validators.pattern("^[0-9]*$")/*, Validators.pattern("^(20|23|27|30|33)([0-9]{8})([0-9]{1})$")*/]),
        ingresosBrutos: new FormControl(null, [Validators.required]),
        condicionIVA: new FormControl(null, [Validators.required]),
        address: new FormControl(null, [Validators.maxLength(1000), Validators.required]),
        city: new FormControl(null, [Validators.maxLength(1000), Validators.required]),
        ZIPCode: new FormControl(null, [Validators.maxLength(10), Validators.required]),
        province: new FormControl(null, [Validators.maxLength(20), Validators.required]),
        country: new FormControl(null, [Validators.maxLength(20), Validators.required]),
        contactName: new FormControl(null, [Validators.maxLength(100), Validators.required]),
        phone: new FormControl(null, [Validators.maxLength(50), Validators.required, Validators.pattern("^[0-9]*$")]),
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
        //let model = this.form.value;
        let model = {
            id: 0,
            name: this.form.get('name').value,
            //providerAreaId: [],
            providersAreaProviders: [],
            CUIT: this.form.get('CUIT').value,
            ingresosBrutos:this.form.get('ingresosBrutos').value,
            condicionIVA:this.form.get('condicionIVA').value,
            address:this.form.get('address').value,
            city:this.form.get('city').value,
            ZIPCode: this.form.get('ZIPCode').value,
            province: this.form.get('province').value,
            country: this.form.get('country').value,
            contactName: this.form.get('contactName').value,
            phone: this.form.get('phone').value,
            email: this.form.get('email').value,
            website: this.form.get('website').value,
            comments: this.form.get('comments').value
        };
        this.form.get('providerAreaId').value.forEach(pr => {
            model.providersAreaProviders.push({
                id: 0,
                providerAreaId: pr,
                providerId: 0
            })
        });
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
        if(event.find(r => r.critical == true) == undefined) {
            this.updateFormValidators(false);
            this.critical = "No"
        } else {
            this.updateFormValidators(true);
            this.critical = "Si"
        }
    }

    updateFormValidators(critical: boolean) {
        if(critical) {
            //CUIT
            this.form.get('CUIT').clearValidators();
            this.form.get('CUIT').setValidators([Validators.maxLength(11), Validators.minLength(11), Validators.required, Validators.pattern("^[0-9]*$")]);
            this.form.get('CUIT').updateValueAndValidity();
            //Ing. Brutos
            this.form.get('ingresosBrutos').setValidators([Validators.required]);
            this.form.get('ingresosBrutos').updateValueAndValidity();
            //Condición IVA
            this.form.get('condicionIVA').setValidators([Validators.required]);
            this.form.get('condicionIVA').updateValueAndValidity();
            //Dirección
            this.form.get('address').clearValidators();
            this.form.get('address').setValidators([Validators.maxLength(1000), Validators.required]);
            this.form.get('address').updateValueAndValidity();
            //Localidad
            this.form.get('city').clearValidators();
            this.form.get('city').setValidators([Validators.maxLength(1000), Validators.required]);
            this.form.get('city').updateValueAndValidity();
            //Código Postal
            this.form.get('ZIPCode').clearValidators();
            this.form.get('ZIPCode').setValidators([Validators.maxLength(10), Validators.required]);
            this.form.get('ZIPCode').updateValueAndValidity();
            //Provincia
            this.form.get('province').clearValidators();
            this.form.get('province').setValidators([Validators.maxLength(20), Validators.required]);
            this.form.get('province').updateValueAndValidity();
            //País
            this.form.get('country').clearValidators();
            this.form.get('country').setValidators([Validators.maxLength(20), Validators.required]);
            this.form.get('country').updateValueAndValidity();
            //Contacto
            this.form.get('contactName').clearValidators();
            this.form.get('contactName').setValidators([Validators.maxLength(100), Validators.required]);
            this.form.get('contactName').updateValueAndValidity();
            //Teléfono
            this.form.get('phone').clearValidators();
            this.form.get('phone').setValidators([Validators.maxLength(50), Validators.required, Validators.pattern("^[0-9]*$")]);
            this.form.get('phone').updateValueAndValidity();
            //Email
            this.form.get('email').clearValidators();
            this.form.get('email').setValidators([Validators.maxLength(100), Validators.required, Validators.pattern("^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$")]);
            this.form.get('email').updateValueAndValidity();
        } else {
            //CUIT
            this.form.get('CUIT').clearValidators();
            this.form.get('CUIT').setValidators([Validators.maxLength(11), Validators.minLength(11), Validators.pattern("^[0-9]*$")]);
            this.form.get('CUIT').updateValueAndValidity();
            //Ing. Brutos
            this.form.get('ingresosBrutos').clearValidators();
            this.form.get('ingresosBrutos').updateValueAndValidity();
            //Condición IVA
            this.form.get('condicionIVA').clearValidators();
            this.form.get('condicionIVA').updateValueAndValidity();
            //Dirección
            this.form.get('address').clearValidators();
            this.form.get('address').setValidators([Validators.maxLength(1000)]);
            this.form.get('address').updateValueAndValidity();
            //Localidad
            this.form.get('city').clearValidators();
            this.form.get('city').setValidators([Validators.maxLength(1000)]);
            this.form.get('city').updateValueAndValidity();
            //Código Postal
            this.form.get('ZIPCode').clearValidators();
            this.form.get('ZIPCode').setValidators([Validators.maxLength(10)]);
            this.form.get('ZIPCode').updateValueAndValidity();
            //Provincia
            this.form.get('province').clearValidators();
            this.form.get('province').setValidators([Validators.maxLength(20)]);
            this.form.get('province').updateValueAndValidity();
            //País
            this.form.get('country').clearValidators();
            this.form.get('country').setValidators([Validators.maxLength(20)]);
            this.form.get('country').updateValueAndValidity();
            //Contacto
            this.form.get('contactName').clearValidators();
            this.form.get('contactName').setValidators([Validators.maxLength(100)]);
            this.form.get('contactName').updateValueAndValidity();
            //Teléfono
            this.form.get('phone').clearValidators();
            this.form.get('phone').setValidators([Validators.maxLength(50), Validators.pattern("^[0-9]*$")]);
            this.form.get('phone').updateValueAndValidity();
            //Email
            this.form.get('email').clearValidators();
            this.form.get('email').setValidators([Validators.maxLength(100), Validators.pattern("^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$")]);
            this.form.get('email').updateValueAndValidity();
        }
    }
}