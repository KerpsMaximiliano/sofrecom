import { Component, Input, ViewChild } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { Router } from "@angular/router";
import { ProvidersService } from "app/services/admin/providers.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { MessageService } from "app/services/common/message.service";
import { FileUploader } from "ng2-file-upload";

@Component({
    selector: 'notes-received-conformable',
    templateUrl: './notes-received-conformable.html'
})

export class NotesReceivedConformable {
    @Input() currentNote;
    mode;
    productosServicios = [];
    analiticas = [];
    providersGrid = [];//proveedor seleccionado etapas anteriores
    fileSelected = false;

    @ViewChild('selectedFile') selectedFile: any;
    public uploader: FileUploader = new FileUploader({url:""});

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
        facturas: new FormControl(null),
        campoFactura: new FormControl(null)
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
        if(this.mode == 'Edit') {
            this.formNota.controls.facturas.enable();
        }
    }

    downloadOC() {
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

    addBill() {
        //Se valida que se hayan adjuntado archivos y se hayan completados los campos en el formulario de la factura. 
        //Se cambia el estado a “Factura Pendiente Aprobación Gerente”
        if(this.fileSelected == false) {
            this.messageService.showMessage("Debe seleccionar una factura para subir", 2);
            return;
        };
        this.messageService.showMessage("La factura ha sido adjuntada a la nota de pedido", 0);
        this.router.navigate(['/providers/notes']);
    }

    fileCheck(event) {
        if(event.length >= 1) {
            this.fileSelected = true;
        } else {
            this.fileSelected = false;
        }
    }
}