import { Component, OnInit } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
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
        providerAreaId: new FormControl(null),
        CUIT: new FormControl(null, [Validators.maxLength(11), Validators.minLength(11)]),
        ingresosBrutos: new FormControl(null),
        condicionIVA: new FormControl(null),
        address: new FormControl(null, [Validators.maxLength(1000)]),
        city: new FormControl(null, [Validators.maxLength(1000)]),
        ZIPCode: new FormControl(null, [Validators.maxLength(10)]),
        province: new FormControl(null, [Validators.maxLength(20)]),
        country: new FormControl(null, [Validators.maxLength(20)]),
        contactName: new FormControl(null, [Validators.maxLength(100)]),
        phone: new FormControl(null, [Validators.maxLength(50)]),
        email: new FormControl(null, [Validators.maxLength(100)]),
        website: new FormControl(null, [Validators.maxLength(1000)]),
        comments: new FormControl(null, [Validators.maxLength(5000)])
    });

    constructor(
        private providersAreaService: ProvidersAreaService,
        private providersService: ProvidersService,
        private messageService: MessageService
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
        if(!this.form.valid) {
            return;
        }
        let model = this.form.value;
        this.providersService.post(model).subscribe(response=>{
            if(response.status == 200) {
                this.messageService.showMessage("Proveedor guardado", 0)
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