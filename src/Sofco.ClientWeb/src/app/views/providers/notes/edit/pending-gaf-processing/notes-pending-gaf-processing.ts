import { Component, Input } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { Router } from "@angular/router";
import { ProvidersService } from "app/services/admin/providers.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { MessageService } from "app/services/common/message.service";

@Component({
    selector: 'notes-pending-gaf-processing',
    templateUrl: './notes-pending-gaf-processing.html'
})

export class NotesPendingGAFProcessing {
    @Input() currentNote;
    @Input() currentNoteStatusDescription;
    mode;
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
        private providerService: ProvidersService,
        private providersAreaService: ProvidersAreaService,
        private messageService: MessageService,
        private requestNoteService: RequestNoteService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.inicializar();
    }

    inicializar() {
        this.mode = this.requestNoteService.getMode();
        this.providersAreaService.get(this.currentNote.providerAreaId).subscribe(d => {
            console.log(d);
            this.formNota.patchValue({
                descripcion: this.currentNote.description,
                rubro: d.data.description,
                critico: (d.data.critical) ? "Si" : "No",
                requierePersonal: this.currentNote.requiresEmployeeClient,
                previstoPresupuesto: this.currentNote.consideredInBudget,
                nroEvalprop: this.currentNote.evalpropNumber,
                observaciones: this.currentNote.comments,
                montoOC: this.currentNote.purchaseOrderAmmount,
                ordenCompra: this.currentNote.purchaseOrderNumber
            });
            this.analiticas = this.currentNote.analytics;
            this.productosServicios = this.currentNote.productsServices;
            this.providersGrid = this.currentNote.providers;
        })
        this.checkFormStatus()
    }

    checkFormStatus() {
        this.formNota.disable();
    }

    downloadOC() {
        let files = this.currentNote.attachments.find(file => file.type == 2);
        this.requestNoteService.downloadProviderFile(files.fileId, 5);
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
        this.messageService.showMessage("La nota de pedido ha sido cerrada", 0);
        this.router.navigate(['/providers/notes']);
    }
}