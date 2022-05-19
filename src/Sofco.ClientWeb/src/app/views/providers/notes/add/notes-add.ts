import { Component } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";

@Component({
    selector: 'notes-add',
    templateUrl: './notes-add.html',
    styleUrls: ['./notes-add.scss']
})

export class NotesAddComponent {

    travelFormShow: boolean = false;
    trainingFormShow: boolean = false;

    providerAreas = ['area 1', 'area 2', 'area 3'];
    //Combo multiselección de la tabla Providers que sean del rubro seleccionado y estén activos
    providers = ['proveedor 1', 'proveedor 2', 'proveedor 3'];
    //Participantes ficha del viaje
    //Combo agregar participantes (Texto 100 caracteres)
    participantes = ['participante 1', 'participante 2', 'participante 3'];
    analiticas = ['Uno', 'Dos', 'Tres', 'Cuatro'];
    participantesViaje = [];
    participantesCapacitacion = [];
    productosServicios = [];
    analiticasTable = [];

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

    constructor() {}

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
        let analitica = {
            analytic: this.formAnaliticas.controls.analytic.value,
            asigned: this.formAnaliticas.controls.asigned.value
        }
        this.analiticasTable.push(analitica)

    }

    eliminarAnalitica(index: number) {
        this.analiticasTable.splice(index, 1);
    }

    saveNote() {
        console.log(this.formNota.value),
        console.log(this.formViaje.value),
        console.log(this.formCapacitacion.value)
    }

}