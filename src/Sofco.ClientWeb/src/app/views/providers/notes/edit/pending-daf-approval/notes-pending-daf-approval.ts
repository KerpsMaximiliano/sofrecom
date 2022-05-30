import { Component } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { ProvidersService } from "app/services/admin/providers.service";

@Component({
    selector: 'notes-pending-daf-approval',
    templateUrl: './notes-pending-daf-approval.html'
})

export class NotesPendingDAFApproval {
    productosServicios = [];
    analiticas = [];
    providersGrid = [];

    formNota: FormGroup = new FormGroup({
        descripcion: new FormControl(null),
        grillaProductosServicios: new FormControl(null),
        rubro: new FormControl(null),
        critico: new FormControl(null),
        grillaAnaliticas: new FormControl(null),
        requierePersonal: new FormControl(true),
        proveedores: new FormControl(null),
        previstoPresupuesto: new FormControl(true),
        nroEvalprop: new FormControl(null),
        observaciones: new FormControl(null),
        montoOC: new FormControl(null),
        ordenCompra: new FormControl(null)
    })

    constructor(
        private providerService: ProvidersService
    ) {}

    ngOnInit(): void {
        this.inicializar();
    }

    inicializar() {
        this.checkFormStatus()
        this.providerService.getAll().subscribe(d => {
            console.log(d.data)
            this.providersGrid = d.data;
            this.providersGrid = [...this.providersGrid]
        })
    }

    checkFormStatus() {
        this.formNota.disable();
        this.formNota.controls.observaciones.enable();
    }

    descargarArchivo() {
        
    }

    reject() {
        //Se muestra un modal para que introduzca una observación (requerida) 
        //debajo un botón de Aceptar y otro de Cancelar. 
        //Si presiona Cancelar, se cierra el modal. Si presiona Aceptar se invoca al método rechazar de la api.
        //Si rechaza se guarda la observación asociada y se cambia el estado de la nota de pedido a “Pendiente Aprobación Abastecimiento”.
    }

    approve() {
        //Si Aprueba se cambia el estado a “Aprobada”
    }
}