import { Component, OnInit } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";

@Component({
    selector: 'notes-pending-approval-management-analytic',
    templateUrl: './notes-pending-approval-management-analytic.html'
})

export class NotesPendingApprovalManagementAnalytic implements OnInit{

    productosServicios = [];
    analiticas = [];
    gerenteLogueado;//Solo mostrar las analiticas asociadas al gerente logueado

    formNota: FormGroup = new FormGroup({
        descripcion: new FormControl(null),
        grillaProductosServicios: new FormControl(null),
        rubro: new FormControl(null),
        critico: new FormControl(null),
        grillaAnaliticas: new FormControl(null),
        requierePersonal: new FormControl(true),
        previstoPresupuesto: new FormControl(true),
        nroEvalprop: new FormControl(null),
        observaciones: new FormControl(null),
        montoOC: new FormControl(null)
    })

    constructor() {}

    ngOnInit(): void {
        this.inicializar();
    }

    inicializar() {
        this.checkFormStatus()
    }

    checkFormStatus() {
        this.formNota.disable();
        this.formNota.controls.observaciones.enable();
    }

    approve() {
        //Marca las analíticas del gerente logueado como “Aprobada”. 
        //Luego se hace un barrido de todas las analíticas asociadas a la nota de pedido; 
        //si todas están aprobadas, entonces la nota de pedido pasa a estado “Pendiente Aprobación Abastecimiento”. 
        //Si hay al menos una que esté en estado “Pendiente de Aprobación”, se mantiene el estado actual.
    }

    reject() {
        //Se marcan las analíticas del gerente logueado como “Rechazada”. 
        //Se cambia el estado de la nota de pedido a estado “Pendiente Revisión Abastecimiento” sin importar el estado del resto de las analíticas.
    }
}