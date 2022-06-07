import { Component } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { ProvidersService } from "app/services/admin/providers.service";

@Component({
    selector: 'notes-received-conformable',
    templateUrl: './notes-received-conformable.html'
})

export class NotesReceivedConformable {
    productosServicios = [];
    analiticas = [];
    providersGrid = [];//proveedor seleccionado etapas anteriores

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
        ordenCompra: new FormControl(null),
        documentacionProveedor: new FormControl(null),
        documentacionRecibidoConforme: new FormControl(null),
        facturas: new FormControl(null)
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
        this.formNota.controls.facturas.enable();
    }

    downloadOC() {
        //descargar archivo orden de compra
    }

    downloadProviderDoc() {
        //descargar archivos documentacion para proveedor
        //ver lista
    }

    downloadRC() {
        //descargar archivos documentacion recibido conforme
        //ver lista
    }

    uploadBills() {
        //subir archivos facturas
    }

    addBill() {
        //Se valida que se hayan adjuntado archivos y se hayan completados los campos en el formulario de la factura. 
        //Se cambia el estado a “Factura Pendiente Aprobación Gerente”
    }
}