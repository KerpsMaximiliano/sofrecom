import { Component, Input, ViewChild } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { ProvidersService } from "app/services/admin/providers.service";
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';

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
    rejectComments;
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
        documentacionRecibidoConforme: new FormControl(null)
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
        this.formNota.controls.documentacionRecibidoConforme.enable();
    }

    downloadOC() {
        //descargar archivo orden de compra
    }

    downloadProviderDoc() {
        //descargar archivos documentacion para proveedor
        //ver lista
    }

    uploadRC() {
        //subir archivos documentacion recibido conforme
    }

    closeM() {
        //abrir modal
        this.modal.show()
    }

    close() {
        //Cerrar: si cierra se muestra un modal con un input de texto para que cargue una observación. 
        //Debajo los botones “Aceptar” y “Cancelar”. 
        //Si cancela se cierra el modal, si acepta se invoca a la API.
        //Confirmar Recepción

        //Si cierra se carga el comentario y pasa a estado Cerrada. 
        //Fin del workflow.
        //Si Confirma Recepción se cambia a estado “Recibido Conforme” y se adjuntan archivos sobre la recepción (si es que se adjuntaron)
    }
}