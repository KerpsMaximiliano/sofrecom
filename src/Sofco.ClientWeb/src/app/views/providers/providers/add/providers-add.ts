import { Component, OnInit } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { GrossIncomeTypes } from "app/models/enums/GrossIncomeTypes";
import { ProvidersService } from "app/services/admin/providers.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";

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
        active: new FormControl(true),
        businessName: new FormControl(null, [Validators.required, Validators.maxLength(200)]),
        area: new FormControl(null),
        initialDate: new FormControl(Date()),//fecha y hora actual
        finalDate: new FormControl(null),//vacio
        cuit: new FormControl(null, [Validators.maxLength(11), Validators.minLength(11)]),
        income: new FormControl(null),
        iva: new FormControl(null),
        addressStreet: new FormControl(null, [Validators.maxLength(1000)]),
        addressCity: new FormControl(null, [Validators.maxLength(1000)]),
        addressPC: new FormControl(null, [Validators.maxLength(10)]),
        addressProvince: new FormControl(null, [Validators.maxLength(20)]),
        addressCountry: new FormControl(null, [Validators.maxLength(20)]),
        contactName: new FormControl(null, [Validators.maxLength(100)]),
        contactPhone: new FormControl(null, [Validators.maxLength(50)]),
        contactMail: new FormControl(null, [Validators.maxLength(100)]),
        web: new FormControl(null, [Validators.maxLength(1000)]),
        comments: new FormControl(null, [Validators.maxLength(5000)])
    });

    constructor(
        private providersAreaService: ProvidersAreaService,
        private providersService: ProvidersService
    ) {
        
    }

    ngOnInit(): void {
        this.form.controls.id.disable();
        this.form.controls.active.disable();
        this.providersAreaService.getAll().subscribe(d => {
            d.data.forEach(area => {
                if(area.active) {
                    this.activeProvidersArea.push(area);
                    this.activeProvidersArea = [...this.activeProvidersArea]
                }
            });
        });
        //this.providersService.getAll().subscribe(d=>console.log(d))
    }

    save() {
        console.log(this.form.value)
        let model = this.form.value;
        model.id = 0;
        model.active = true;
        console.log(model);
        this.providersService.post(model).subscribe(d=>console.log(d));
    }

    change(event) {
        if(event != undefined) {
            this.critical = (event.critical) ? "Si" : "No"
        } else {
            this.critical = null
        }
    }
}