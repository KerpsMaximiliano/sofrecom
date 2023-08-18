import { Component, OnInit } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { ProvidersService } from "app/services/admin/providers.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { forkJoin } from "rxjs";
import { MessageService } from "app/services/common/message.service"

@Component({
    selector: 'providers-edit',
    templateUrl: './providers-edit.html',
    styleUrls: [
        './providers-edit.scss',
    ]
})

export class ProvidersEditComponent implements OnInit{

    mode: string;
    id: number;
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
    ];
    providerAreas = [];
    providerAreaProviders = [];

    form: FormGroup = new FormGroup({
        id: new FormControl(null),
        active: new FormControl(null),
        name: new FormControl(null, [Validators.required, Validators.maxLength(200)]),
        providerAreaId: new FormControl([], [Validators.required]),
        cuit: new FormControl(null, [Validators.maxLength(11), Validators.minLength(11), Validators.required, Validators.pattern("^[0-9]*$")/*, Validators.pattern("^(20|23|27|30|33)([0-9]{8})([0-9]{1})$")*/]),
        startDate: new FormControl(null),
        endDate: new FormControl(null),
        ingresosBrutos: new FormControl(null, [Validators.required]),
        condicionIVA: new FormControl(null, [Validators.required]),
        address: new FormControl(null, [Validators.maxLength(1000), Validators.required]),
        city: new FormControl(null, [Validators.maxLength(1000), Validators.required]),
        ZIPCode: new FormControl(null, [Validators.maxLength(10), Validators.required]),
        province: new FormControl(null, [Validators.maxLength(20), Validators.required]),
        country: new FormControl(null, [Validators.maxLength(20), Validators.required]),
        contactName: new FormControl(null, [Validators.maxLength(100), Validators.required]),
        phone: new FormControl(null, [Validators.maxLength(50), Validators.required, Validators.pattern("^[0-9]*$")]),
        email: new FormControl(null, [Validators.maxLength(100), Validators.required]),//Validators.pattern("^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$")
        website: new FormControl(null, [Validators.maxLength(1000)]),
        comments: new FormControl(null, [Validators.maxLength(5000)])
    });
    public pathParam: string;
    public isEdit: boolean = false;

    constructor(
        private activatedRoute: ActivatedRoute,
        private providersService: ProvidersService,
        private providersAreaService: ProvidersAreaService,
        private messageService: MessageService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.pathParam = this.activatedRoute.snapshot.routeConfig.path;
        this.inicializar();
    }

    inicializar() {
        this.messageService.showLoading();
        this.id = this.activatedRoute.snapshot.params.id;
        forkJoin([
            this.providersService.get(this.id),
            this.providersAreaService.getAll()
          ]).subscribe(
            (results) => {
              this.providerAreas = results[1].data;
              this.providerAreaProviders = results[0].data.providersAreaProviders;
              this.form.patchValue({
                id: results[0].data.id,
                active: results[0].data.active ? "Activo" : "Inactivo",
                name: results[0].data.name,
                cuit: results[0].data.cuit,
                startDate: results[0].data.startDate,
                endDate: results[0].data.endDate,
                ingresosBrutos: results[0].data.ingresosBrutos,
                condicionIVA: results[0].data.condicionIVA,
                address: results[0].data.address,
                city: results[0].data.city,
                ZIPCode: results[0].data.zipCode,
                province: results[0].data.province,
                country: results[0].data.country,
                contactName: results[0].data.contactName,
                phone: results[0].data.phone,
                email: results[0].data.email,
                website: results[0].data.webSite,
                comments: results[0].data.comments,
              });
              let array = [];
              let critical = false;
              results[0].data.providersAreaProviders.forEach((pr) => {
                array.push(pr.providerAreaId);
                if (
                  results[1].data.find(
                    (p) => p.id == pr.providerAreaId && p.critical == true
                  ) !== undefined
                ) {
                  critical = true;
                }
              });
              this.updateFormValidators(critical);
              this.form.get("providerAreaId").setValue(array);
              this.messageService.closeLoading();
            },
            (error) => {
                this.messageService.closeLoading();
            },
            () => {
                this.messageService.closeLoading();
            }
          );

        if(this.pathParam === 'edit/:id'){
            this.isEdit = true;
            this.form.controls.id.disable();
            this.form.controls.active.disable();
            this.form.controls.startDate.disable();
            this.form.controls.endDate.disable();
        } else{
            this.form.disable();
        }
}

    save() {
        this.messageService.showLoading();
        if(!this.form.valid) {
            this.markFormGroupTouched(this.form)
            this.messageService.closeLoading();
            this.messageService.showMessage('Por favor, verifica el formulario.', 1);
            return;
        }
        this.form.enable();
        //let model = this.form.value;
        let model = {
            id: this.form.get('id').value,
            active: this.form.get('active').value,
            name: this.form.get('name').value,
            //providerAreaId: [],
            providersAreaProviders: [],
            cuit: this.form.get('cuit').value,
            startDate: this.form.get('startDate').value,
            endDate: this.form.get('endDate').value,
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
            let find = this.providerAreaProviders.find(prov => prov.providerAreaId == pr);
            if (find != undefined) {
                model.providersAreaProviders.push({
                    id: find.id,
                    providerAreaId: pr,
                    providerId: model.id
                })
            } else {
                model.providersAreaProviders.push({
                    id: 0,
                    providerAreaId: pr,
                    providerId: model.id
                })
            }
        });
        model.active = (model.active == "Activo") ? true : false;
        this.form.controls.id.disable();
        this.form.controls.active.disable();
        this.form.controls.startDate.disable();
        this.form.controls.endDate.disable();
        console.log(model);
        this.providersService.edit(this.id, model).subscribe(response => {
            if (response.status == 200) {
                this.messageService.closeLoading();
                this.messageService.showMessage("El proveedor fue editado exitosamente.", 0);
                this.router.navigate([`providers/providers`]);

            } else{
                this.messageService.closeLoading();
            }
        })
    }

    change(event) {
        console.log(event)
        if(event.find(r => r.critical == true) == undefined) {
            this.updateFormValidators(false);
        } else {
            this.updateFormValidators(true);
        }
    }

    updateFormValidators(critical: boolean) {
        if(critical) {
            //CUIT
            this.form.get('cuit').clearValidators();
            this.form.get('cuit').setValidators([Validators.maxLength(11), Validators.minLength(11), Validators.required, Validators.pattern("^[0-9]*$")]);
            this.form.get('cuit').updateValueAndValidity();
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
            this.form.get('email').setValidators([Validators.maxLength(100), Validators.required]);
            this.form.get('email').updateValueAndValidity();
        } else {
            //CUIT
            this.form.get('cuit').clearValidators();
            this.form.get('cuit').setValidators([Validators.maxLength(11), Validators.minLength(11), Validators.pattern("^[0-9]*$")]);//Validators.pattern("^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$")
            this.form.get('cuit').updateValueAndValidity();
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
            this.form.get('email').setValidators([Validators.maxLength(100)]);//Validators.pattern("^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$")
            this.form.get('email').updateValueAndValidity();
        }
    }

    markFormGroupTouched(formGroup: FormGroup) {
        (<any>Object).values(formGroup.controls).forEach(control => {
            control.markAsTouched();

            if (control.controls) {
                this.markFormGroupTouched(control);
            }
        });
    };
}