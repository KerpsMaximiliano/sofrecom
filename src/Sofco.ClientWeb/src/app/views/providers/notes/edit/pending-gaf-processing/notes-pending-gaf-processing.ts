import { Component, Input } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { ProvidersService } from "app/services/admin/providers.service";

@Component({
    selector: 'notes-pending-gaf-processing',
    templateUrl: './notes-pending-gaf-processing.html'
})

export class NotesPendingGAFProcessing {
    @Input() currentNote;
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

    downloadBills() {
        //descargar archivos facturas
    }

    close() {
        //No hace ninguna acción sólo cambiar de estado a “Cerrado”
    }
}