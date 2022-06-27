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
            //asignar analíticas
        })
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
        this.requestNoteService.downloadFilePendingDAFApproval(this.currentNote.id, 0).subscribe(d=>{
            if(d == null) {
                this.messageService.showMessage("No hay archivos para descargar", 2);
            }
        })
    }

    downloadProviderDoc() {
        //descargar archivos documentacion para proveedor
        //ver lista
        this.requestNoteService.downloadFileRequestedProvider().subscribe(d=>{})
    }

    downloadRC() {
        //descargar archivos documentacion recibido conforme
        //ver lista
        this.requestNoteService.downloadFileReceivedConformable().subscribe(d=>{})
    }

    downloadBills() {
        //descargar archivos facturas
        this.requestNoteService.downloadFilePendingManagementBillApproval().subscribe(d=>{})
    }

    close() {
        //No hace ninguna acción sólo cambiar de estado a “Cerrado”
        this.messageService.showMessage("La nota de pedido ha sido cerrada", 0);
        this.router.navigate(['/providers/notes']);
    }
}