import { Component, OnInit } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { ProvidersService } from "app/services/admin/providers.service";

@Component({
    selector: 'notes-pending-supply-revision',
    templateUrl: './notes-pending-supply-revision.html'
})

export class NotesPendingSupplyRevision implements OnInit {

    productosServicios = [];
    analiticas = [];
    providers = [];
    selectedProviderId: number;
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
        montoOC: new FormControl(null, [Validators.required]),
    })

    constructor(
        private providerService: ProvidersService,
    ) { }

    ngOnInit(): void {
        this.inicializar();
    }

    inicializar() {
        this.checkFormStatus()
        this.providerService.getAll().subscribe(d => {
            console.log(d.data)
            //check providerAreaId que sea igual al rubro
            this.providers = d.data;
            this.providers = [...this.providers]
        })
    }

    checkFormStatus() {
        this.formNota.disable();
        this.formNota.controls.proveedores.enable();
        this.formNota.controls.observaciones.enable();
        this.formNota.controls.montoOC.enable();
    }

    agregarProveedor() {
        if (this.formNota.controls.proveedores.value == null) {
            return;
        }
        let busqueda = this.providers.find(proveedor => proveedor.id == this.formNota.controls.proveedores.value);
        this.providersGrid.push(busqueda);
        this.providersGrid = [...this.providersGrid]
    }

    eliminarProveedor(index: number) {
        this.providersGrid.splice(index, 1);
    }

    reject() {
        //Si presiona rechazar se cambia al estado “Rechazado”
    }

    send() {
        //Se debe validar que haya al menos un proveedor con un archivo adjunto.
        //Se debe validar que haya ingresado un valor para el monto final de la OC.
        //Se cambia al estado “Pendiente Aprobación Gerente Analíticas”
        //Se asignan a todas las analíticas asociadas el estado “Pendiente Aprobación”
    }
}