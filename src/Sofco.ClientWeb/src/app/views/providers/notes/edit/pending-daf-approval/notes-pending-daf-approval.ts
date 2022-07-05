import { Component, Input, ViewChild } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { ProvidersService } from "app/services/admin/providers.service";
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { MessageService } from "app/services/common/message.service";
import { Router } from "@angular/router";
import * as FileSaver from "file-saver";

@Component({
    selector: 'notes-pending-daf-approval',
    templateUrl: './notes-pending-daf-approval.html'
})

export class NotesPendingDAFApproval {

    @ViewChild('rejectModal') modal;
    public rejectModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "rejectModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );
    @ViewChild('pdfViewer') pdfViewer;
    @Input() currentNote;
    @Input() currentNoteStatusDescription;
    rejectComments = null;
    mode;

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
        private providerService: ProvidersService,
        private providersAreaService: ProvidersAreaService,
        private requestNoteService: RequestNoteService,
        private messageService: MessageService,
        private router: Router
    ) {}

    ngOnInit(): void {
        console.log(this.providersGrid)
        this.inicializar();
    }

    inicializar() {
        this.mode = this.requestNoteService.getMode();
        this.analiticas = this.currentNote.analytics;
        this.productosServicios = this.currentNote.productsServices;
        this.providersGrid = this.currentNote.providers;
        console.log(this.currentNote)
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
        })
        this.checkFormStatus()
    }

    checkFormStatus() {
        this.formNota.disable();
        if(this.mode == "Edit") {
            this.formNota.controls.observaciones.enable();
        }
    }

    downloadOC() {
        let files = this.currentNote.attachments.find(file => file.type == 2);
        this.requestNoteService.downloadFile(files.fileId, 5, files.fileDescription);
    }

    rejectM() {
        this.modal.show()
    }

    reject() {
        if(this.rejectComments == null || this.rejectComments.length == 0) {
            this.messageService.showMessage("Debe dejar una observación si desea rechazar la nota de pedido", 2);
            this.modal.hide();
            return;
        };
        let model = {
            id: this.currentNote.id,
            remarks: this.rejectComments
        }
        this.requestNoteService.rejectPendingDAFApproval(model).subscribe(d => {
            console.log(d);
            this.modal.hide();
            this.messageService.showMessage("La nota de pedido ha sido rechazada", 0);
            this.router.navigate(['/providers/notes']);
        })
    }

    approve() {
        let model = {
            id: this.currentNote.id,
            comments: this.formNota.controls.observaciones.value
        }
        this.requestNoteService.approvePendingDAFApproval(model).subscribe(d => {
            console.log(d);
            this.messageService.showMessage("La nota de pedido ha sido aprobada", 0);
            this.router.navigate(['/providers/notes']);
        })
    }
}