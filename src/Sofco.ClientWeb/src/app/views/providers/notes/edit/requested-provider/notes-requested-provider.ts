import { Component, Input, ViewChild } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { ProvidersService } from "app/services/admin/providers.service";
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { FileUploader } from "ng2-file-upload";
import { MessageService } from "app/services/common/message.service";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { Router } from "@angular/router";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";

@Component({
    selector: 'notes-requested-provider',
    templateUrl: './notes-requested-provider.html'
})

export class NotesRequestedProvider {

    @ViewChild('closeModal') modal;
    public closeModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "rejectModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    @ViewChild('selectedFile') selectedFile: any;
    public uploader: FileUploader = new FileUploader({url:""});
    
    rejectComments;
    @Input() currentNote;
    mode;

    productosServicios = [];
    analiticas = [
        {analytic: "Analítica 1", asigned: 10},
        {analytic: "Analítica 2", asigned: 30},
        {analytic: "Analítica 3", asigned: 5}
    ];
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
        documentacionRecibidoConforme: new FormControl(null)
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
        if(this.mode == 'Edit') {
            this.formNota.controls.documentacionRecibidoConforme.enable();
        }
    }

    downloadOC() {
        this.requestNoteService.downloadFilePendingDAFApproval(this.currentNote.id, 1).subscribe(d=>{
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

    closeM() {
        //abrir modal
        this.modal.show()
    }

    close() {
        //Cerrar: si cierra se muestra un modal con un input de texto para que cargue una observación. 
        //Debajo los botones “Aceptar” y “Cancelar”. 
        //Si cancela se cierra el modal, si acepta se invoca a la API.
        //Si cierra se carga el comentario y pasa a estado Cerrada. 
        //Fin del workflow.
        if(this.rejectComments == null || this.rejectComments.length == 0) {
            this.messageService.showMessage("Debe dejar una observación si desea cerrar la nota de pedido", 2);
            this.modal.hide();
            return;
        }
        this.messageService.showMessage("La nota de pedido ha sido cerrada", 0);
        this.router.navigate(['/providers/notes']);
    }

    confirm() {
        //Confirmar Recepción
        //Si Confirma Recepción se cambia a estado “Recibido Conforme” y se adjuntan archivos sobre la recepción (si es que se adjuntaron)
        this.messageService.showMessage("La recepción de la nota de pedido ha sido confirmada", 0);
        this.router.navigate(['/providers/notes']);
    }
}