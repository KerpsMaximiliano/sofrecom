import { Component, Input, ViewChild } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { ProvidersService } from "app/services/admin/providers.service";
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { FileUploader } from "ng2-file-upload";
import { MessageService } from "app/services/common/message.service";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { Router } from "@angular/router";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { Cookie } from "ng2-cookies";
import { AuthService } from "app/services/common/auth.service";

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
    @Input() currentNoteStatusDescription;
    mode;

    productosServicios = [];
    analiticas = [];
    providersGrid = [];//proveedor seleccionado etapas anteriores
    fileSelected = false;
    uploadedFilesId = [];

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
        private authService: AuthService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.inicializar();
    }

    inicializar() {
        this.mode = this.requestNoteService.getMode();
        this.uploaderConfig();
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
            this.formNota.controls.documentacionRecibidoConforme.enable();
        }
    }

    downloadOC() {
        
    }

    downloadProviderDoc() {
        //descargar archivos documentacion para proveedor
        //ver lista
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
        };
        let model = {
            id: this.currentNote.id,
            remarks: this.rejectComments
        };
        this.requestNoteService.closeRequestedProvider(model).subscribe(d => {{
            console.log(d);
            this.modal.hide();
            this.messageService.showMessage("La nota de pedido ha sido cerrada", 0);
            this.router.navigate(['/providers/notes']);
        }})
    }

    confirm() {
        //Confirmar Recepción
        //Si Confirma Recepción se cambia a estado “Recibido Conforme” y se adjuntan archivos sobre la recepción (si es que se adjuntaron)
        this.uploader.uploadAll();
    }

    uploaderConfig(){
        this.uploader = new FileUploader({url: this.requestNoteService.uploadDraftFiles(),
            authToken: 'Bearer ' + Cookie.get('access_token') ,
        });

        this.uploader.onCompleteItem = (item:any, response:any, status:any, headers:any) => {
            if(status == 401){
                this.authService.refreshToken().subscribe(token => {
                    this.messageService.closeLoading();

                    if(token){
                        this.clearSelectedFile();
                        this.messageService.showErrorByFolder('common', 'fileMustReupload');
                        this.uploaderConfig();
                    }
                });
                return;
            }
            let jsonResponse = JSON.parse(response);
            this.uploadedFilesId.push({
                type: 4,
                fileId: jsonResponse.data[0].id
            });
            console.log(this.uploadedFilesId)
            this.clearSelectedFile();
        };
        this.uploader.onCompleteAll = () => {
            let model = {
                id: this.currentNote.id,
                attachments: this.uploadedFilesId,
                comments: this.formNota.controls.observaciones.value
            };
            this.requestNoteService.approveRequestedProvider(model).subscribe(d => {
                console.log(d);
                this.messageService.showMessage("La recepción de la nota de pedido ha sido confirmada", 0);
                this.router.navigate(['/providers/notes']);
            })
        }
        this.uploader.onAfterAddingFile = (file) => { file.withCredentials = false; };
    }

    clearSelectedFile(){
        if(this.uploader.queue.length > 0){
            this.uploader.queue[0].remove();
        }
  
        this.selectedFile.nativeElement.value = '';
    }
}