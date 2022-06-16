import { Component, OnInit } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { ProvidersService } from "app/services/admin/providers.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { SettingsService } from "app/services/admin/settings.service";
import { UserService } from "app/services/admin/user.service";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { UserInfoService } from "app/services/common/user-info.service";
import { forkJoin } from "rxjs";

@Component({
    selector: 'notes-add',
    templateUrl: './notes-add.html',
    styleUrls: ['./notes-add.scss']
})

export class NotesAddComponent implements OnInit{

    travelFormShow: boolean = false;
    trainingFormShow: boolean = false;

    providerAreas = [];
    //Combo multiselección de la tabla Providers que sean del rubro seleccionado y estén activos
    providers = ['proveedor 1', 'proveedor 2', 'proveedor 3'];
    //Participantes ficha del viaje
    //Combo agregar participantes (Texto 100 caracteres)
    participantes = ['participante 1', 'participante 2', 'participante 3'];
    participants = [];
    filteredParticipants = [];
    analiticas = [];
    analiticasTable = [];
    allProviders = [];
    providers = [];
    proveedoresTable = [];
    productosServicios = [];
    participantesViaje = [];
    participantesCapacitacion = [];
    critical: string = null;

    formNota: FormGroup = new FormGroup({
        description: new FormControl(null, [Validators.required, Validators.maxLength(1000)]),//Descripcion
        productsAndServicies: new FormControl(null),//Combo editable productos/servicios
        //2 campos
        //Productos/Servicios - texto 5000 requerido
        //Cantidad - numero mayor a 0 requerido
        area: new FormControl(null, []),//Rubro - Combo de ProvidersAreas con Active = true
        critical: new FormControl(null, []),//Se carga a partir del rubro seleccionado. Es readonly. 
        //Asociado a la columna Critical del Rubro seleccionado. Es Si o No. Va al lado del Combo de Rubro.
        analytics: new FormControl(null, []),//Grilla editable de Analíticas
        requiresPersonel: new FormControl(null, []),
        providers: new FormControl(null, []),//combo multiselección de la tabla Providers que sean del rubro seleccionado y estén activos. 
        //Es opcional, puede no elegir ninguno. 
        evaluationProposal: new FormControl(null, []),//Checkbox
        numberEvalprop: new FormControl(null, [Validators.maxLength(100)]),
        observations: new FormControl(null, []),
        travel: new FormControl(null, []),//Checkbox
        training: new FormControl(null, []),//Checkbox
    });

    formProductoServicio: FormGroup = new FormGroup({
        productService: new FormControl(null, [Validators.required, Validators.maxLength(5000)]),
        quantity: new FormControl(null, [Validators.required, Validators.min(1)])
    });

    formAnaliticas: FormGroup = new FormGroup({
        analytic: new FormControl(null, [Validators.required]),
        asigned: new FormControl(null, [Validators.required, Validators.min(1)])
    });

    formProveedores: FormGroup = new FormGroup({
        provider: new FormControl(null, [Validators.required])
    })
    
    formCapacitacion: FormGroup = new FormGroup({
        name: new FormControl(null),
        subject: new FormControl(null),
        location: new FormControl(null),
        date: new FormControl(null),
        duration: new FormControl(null),
        ammount: new FormControl(null),
        participants: new FormControl(null)
    });

    formViaje: FormGroup = new FormGroup({
        passengers: new FormControl(null),
        departureDate: new FormControl(null),
        returnDate: new FormControl(null),
        destination: new FormControl(null),
        transportation: new FormControl(null),
        accommodation: new FormControl(null),
        details: new FormControl(null)
    });

    formParticipanteViaje: FormGroup = new FormGroup({
        name: new FormControl (null, [Validators.required, Validators.maxLength(100)]),
        birth: new FormControl (null, [Validators.required]),
        cuit: new FormControl (null, [Validators.required]),
    });

    formParticipanteCapacitacion: FormGroup = new FormGroup({
        name: new FormControl (null, [Validators.required, Validators.maxLength(100)]),
        sector: new FormControl (null, [Validators.required])
    })

    constructor(
        private providersService: ProvidersService,
        private providersAreaService: ProvidersAreaService,
        private employeeService: EmployeeService,
        private analyticService: AnalyticService,
        private userService: UserService
    ) {}

    ngOnInit(): void {
        this.inicializar();
        const userInfo = UserInfoService.getUserInfo();
        console.log(userInfo);
    }

    inicializar() {
        this.providersAreaService.getAll().subscribe(d => {
            console.log(d)
            d.data.forEach(providerArea => {
                if(providerArea.active) {
                    this.providerAreas.push(providerArea);
                    this.providerAreas = [...this.providerAreas]
                }
            });
        });
        this.employeeService.getEveryone().subscribe(d => {
            console.log(d);
            this.participants = d;
            d.forEach(user => {
                if(user.isExternal == 0 && user.endDate == null) {
                    this.filteredParticipants.push(user);
                    this.filteredParticipants = [...this.filteredParticipants]
                }
            });
        });
        forkJoin([this.analyticService.getAll(), this.userService.getManagers(), this.userService.getManagersAndDirectors()]).subscribe(results => {
            console.log(results[0]);
            this.analiticas = results[0];
            console.log(results[1]);
            console.log(results[2]);
        });
        this.providersService.getAll().subscribe(d => {
            this.allProviders = d.data;
        })
    }

    change(event) {
        if(event != undefined) {
            this.critical = (event.critical) ? "Si" : "No";
            this.providers = [];
            this.allProviders.forEach(prov => {
                if(prov.providerAreaId == event.id) {
                    this.providers.push(prov);
                    this.providers = [...this.providers];
                }
            })
        } else {
            this.critical = null
        }
    }

    travelChange(event) {
        console.log(event)
    }

    openTravelModal() {
        this.travelFormShow = !this.travelFormShow;
    }

    openTrainingModal() {
        this.trainingFormShow = !this.trainingFormShow;
    }

    agregarParticipanteViaje() {
        if(this.formParticipanteViaje.invalid) {
            return;
        }
        let participante = {
            name: this.formParticipanteViaje.controls.name.value,
            birth: this.formParticipanteViaje.controls.birth.value,
            cuit: this.formParticipanteViaje.controls.cuit.value,
        }
        this.participantesViaje.push(participante)
    }

    eliminarParticipanteViaje(index: number) {
        this.participantesViaje.splice(index, 1);
    }

    agregarParticipanteCapacitacion() {
        if(this.formParticipanteCapacitacion.invalid) {
            return;
        }
        let participante = {
            name: this.formParticipanteCapacitacion.controls.name.value,
            sector: this.formParticipanteCapacitacion.controls.sector.value
        }
        this.participantesCapacitacion.push(participante)
    }

    eliminarParticipanteCapacitacion(index: number) {
        this.participantesCapacitacion.splice(index, 1);
    }

    agregarProductoServicio() {
        if(this.formProductoServicio.invalid) {
            return;
        }
        let productoServicio = {
            productService: this.formProductoServicio.controls.productService.value,
            quantity: this.formProductoServicio.controls.quantity.value
        }
        this.productosServicios.push(productoServicio)
    }

    eliminarProductoServicio(index: number) {
        this.productosServicios.splice(index, 1);
    }

    agregarAnalitica() {
        if(this.formAnaliticas.invalid) {
            return;
        }
        let busqueda = this.analiticas.find(analytic => analytic.id == this.formAnaliticas.controls.analytic.value)
        let analitica = {
            analytic: busqueda,
            asigned: this.formAnaliticas.controls.asigned.value
        }
        this.analiticasTable.push(analitica)

    }

    eliminarAnalitica(index: number) {
        this.analiticasTable.splice(index, 1);
    }

    agregarProveedor() {
        if(this.formProveedores.invalid) {
            return;
        }
        let busqueda = this.allProviders.find(prov => prov.id == this.formProveedores.controls.provider.value);
        this.proveedoresTable.push(busqueda)
    }

    eliminarProveedor(index: number) {
        this.proveedoresTable.splice(index, 1);
    }

    saveNote() {
        console.log(this.formNota.value),
        console.log(this.formViaje.value),
        console.log(this.formCapacitacion.value)
    }

}