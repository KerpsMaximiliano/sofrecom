import { Component, OnInit } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { ProvidersService } from "app/services/admin/providers.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { forkJoin } from "rxjs";
import { MessageService } from "app/services/common/message.service"

@Component({
    selector: 'providers-edit',
    templateUrl: './providers-edit.html'
})

export class ProvidersEditComponent implements OnInit{

    mode: string;
    id: number;
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
    ];
    providerAreas = [];

    form: FormGroup = new FormGroup({
        id: new FormControl(null),
        active: new FormControl(null),
        name: new FormControl(null, [Validators.required, Validators.maxLength(200)]),
        providerAreaId: new FormControl(null, [Validators.required]),
        cuit: new FormControl(null, [Validators.maxLength(11), Validators.minLength(11), Validators.required]),
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
        phone: new FormControl(null, [Validators.maxLength(50), Validators.required]),
        email: new FormControl(null, [Validators.maxLength(100), Validators.required]),
        website: new FormControl(null, [Validators.maxLength(1000)]),
        comments: new FormControl(null, [Validators.maxLength(5000)])
    });

    constructor(
        private activatedRoute: ActivatedRoute,
        private providersService: ProvidersService,
        private providersAreaService: ProvidersAreaService,
        private messageService: MessageService
    ) {}

    ngOnInit(): void {
        this.inicializar();
    }

    inicializar() {
        this.id = this.activatedRoute.snapshot.params.id;
        forkJoin([
            this.providersService.get(this.id),
            this.providersAreaService.getAll()
        ]).subscribe(results => {
            this.providerAreas = results[1].data;
            this.form.patchValue({
                id: results[0].data.id,
                active: results[0].data.active,
                name: results[0].data.name,
                providerAreaId: results[0].data.providerAreaId,
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
            })
        })
        this.mode = this.providersService.getMode();
        if(this.mode == undefined) {
            this.mode = "View"
        }
        if (this.mode == "View") {
            this.form.disable();
        }
        this.form.controls.id.disable();
        this.form.controls.active.disable();
        this.form.controls.startDate.disable();
        this.form.controls.endDate.disable();
    }

    save() {
        if(!this.form.valid) {
            return;
        }
        this.form.enable();
        let model = this.form.value;
        this.form.controls.id.disable();
        this.form.controls.active.disable();
        this.form.controls.startDate.disable();
        this.form.controls.endDate.disable();
        this.providersService.edit(this.id, model).subscribe(response => {
            if (response.status == 200) {
                this.messageService.showMessage("Proveedor guardado", 0);
            }
        })
    }
}