import { Component, OnInit } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { ProvidersService } from "../../providers.service";

@Component({
    selector: 'providers-edit',
    templateUrl: './providers-edit.html'
})

export class ProvidersEditComponent implements OnInit{

    mode: string;
    id: number;

    form: FormGroup = new FormGroup({
        id: new FormControl(1),
        active: new FormControl(true),
        businessName: new FormControl("Razón social", [Validators.required, Validators.maxLength(200)]),
        area: new FormControl("Rubro (select)"),
        initialDate: new FormControl("Fecha Alta"),
        finalDate: new FormControl("Fecha Baja"),
        cuit: new FormControl("xx-xxxxxxxx-x"),
        income: new FormControl(1231241231),
        iva: new FormControl("21%"),
        addressStreet: new FormControl("Calle", [Validators.maxLength(1000)]),
        addressCity: new FormControl("Ciudad", [Validators.maxLength(1000)]),
        addressPC: new FormControl("2000", [Validators.maxLength(10)]),
        addressProvince: new FormControl("Provincia", [Validators.maxLength(20)]),
        addressCountry: new FormControl("País", [Validators.maxLength(20)]),
        contactName: new FormControl("Nombre y Apellido", [Validators.maxLength(100)]),
        contactPhone: new FormControl("1212121212", [Validators.maxLength(50)]),
        contactMail: new FormControl("E-Mail", [Validators.maxLength(100)]),
        web: new FormControl("www.asdasdawd.com", [Validators.maxLength(1000)]),
        comments: new FormControl("Comentarios", [Validators.maxLength(5000)])
    });

    constructor(
        private activatedRoute: ActivatedRoute,
        private providersService: ProvidersService
    ) {}

    ngOnInit(): void {
        this.id = this.activatedRoute.snapshot.params.id;
        console.log(this.id)
        this.mode = this.providersService.getMode();
        if(this.mode == undefined) {
            this.mode = "View"
        }
        if (this.mode == "View") {
            this.form.disable();
        }
        this.form.controls.id.disable();
        this.form.controls.active.disable();
        this.form.controls.initialDate.disable();
        this.form.controls.finalDate.disable();
    }

    save() {
        console.log(this.form.value)
    }
}