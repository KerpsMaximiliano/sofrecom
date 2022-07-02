import { Component, Input, ViewChild } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { Router } from "@angular/router";
import { ProvidersService } from "app/services/admin/providers.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { AuthService } from "app/services/common/auth.service";
import { FileService } from "app/services/common/file.service";
import { MessageService } from "app/services/common/message.service";
import { Cookie } from "ng2-cookies";
import { FileUploader } from "ng2-file-upload";
import * as FileSaver from "file-saver";

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
    uploadedFilesId = [];

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
        private authService: AuthService,
        private router: Router,
        private fileService: FileService
    ) {}

    ngOnInit(): void {
        this.inicializar();
    }

    inicializar() {
        this.mode = this.requestNoteService.getMode();
        this.uploaderConfig();
        this.providersAreaService.get(this.currentNote.providerAreaId).subscribe(d => {
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
        let files = this.currentNote.attachments.find(file => file.type == 2);
        //this.fileService.getFile(files.fileId, 5).subscribe(d => {
        //    console.log(d)
        //})
        this.requestNoteService.downloadProviderFile(files.fileId, 5).subscribe(d => {
            console.log(d);
            FileSaver.saveAs(d, "file.txt");
        })
    }

    request() {
        //Validar que se haya adjuntado al menos un archivo adjunto para el proveedor y notificar al mismo seleccionado en la nota de pedido. 
        //Se cambia el estado a “Solicitada a Proveedor”.
        if(this.fileSelected == false) {
            this.messageService.showMessage("Debe seleccionar la documentación para el proveedor para subir", 2);
            return;
        };
        this.uploader.uploadAll();
    }

    fileCheck(event) {
        console.log(event)
        if(event.length >= 1) {
            this.fileSelected = true;
        } else {
            this.fileSelected = false;
        }
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
                type: 3,
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
            this.requestNoteService.applyApproved(model).subscribe(d => {
                console.log(d);
                this.messageService.showMessage("La nota de pedido ha sido solicitada", 0);
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