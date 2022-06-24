import { Component, Input, OnInit } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { ProvidersService } from "app/services/admin/providers.service";

@Component({
    selector: 'notes-pending-supply-approval',
    templateUrl: './notes-pending-supply-approval.html'
})

export class NotesPendingSupplyApproval implements OnInit{

    @Input() currentNote;
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
        this.formNota.controls.proveedores.enable();
        this.formNota.controls.observaciones.enable();
        this.formNota.controls.montoOC.enable();
        this.formNota.controls.ordenCompra.enable()
    }

    uploadOC() {
        //subir archivo orden de compra
    }

    reject() {
        //Si Rechaza, se cambia el estado de la nota de pedido a Rechazada y finaliza el workflow.
    }

    approve() {
        //se debe validar que haya elegido un proveedor del listado de proveedores.
        //se debe validar que haya un archivo de orden de compra adjunto
        //se debe validar que haya ingresado un número de orden de compra.
        //Se cambia el estado de la nota de pedido a “Pendiente Aprobación DAF”
    }
}