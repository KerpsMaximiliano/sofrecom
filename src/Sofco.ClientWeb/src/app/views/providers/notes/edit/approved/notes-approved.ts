import { Component, Input, ViewChild } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { Router } from "@angular/router";
import { ProvidersService } from "app/services/admin/providers.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { MessageService } from "app/services/common/message.service";
import { FileUploader } from "ng2-file-upload";

@Component({
    selector: 'notes-approved',
    templateUrl: './notes-approved.html'
})

export class NotesApproved {
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
        documentacionProveedor: new FormControl(null)
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
        if(this.mode == "Edit") {
            this.formNota.controls.documentacionProveedor.enable();
        }
    }

    downloadOC() {
        this.requestNoteService.downloadFilePendingDAFApproval(this.currentNote.id, 1).subscribe(d=>{
            if(d == null) {
                this.messageService.showMessage("No hay archivos para descargar", 2);
            }
        })
    }

    request() {
        //Validar que se haya adjuntado al menos un archivo adjunto para el proveedor y notificar al mismo seleccionado en la nota de pedido. 
        //Se cambia el estado a “Solicitada a Proveedor”.
        if(this.fileSelected == false) {
            this.messageService.showMessage("Debe seleccionar la documentación para el proveedor para subir", 2);
            return;
        };
        this.messageService.showMessage("La nota de pedido ha sido solicitada", 0);
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