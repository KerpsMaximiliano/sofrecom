import { Component, Input, ViewChild } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { Router } from "@angular/router";
import { ProvidersService } from "app/services/admin/providers.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { AuthService } from "app/services/common/auth.service";
import { MessageService } from "app/services/common/message.service";
import { Cookie } from "ng2-cookies";
import { FileUploader } from "ng2-file-upload";

@Component({
    selector: 'notes-received-conformable',
    templateUrl: './notes-received-conformable.html'
})

export class NotesReceivedConformable {
    @Input() currentNote;
    @Input() currentNoteStatusDescription;
    mode;
    productosServicios = [];
    analiticas = [];
    providersGrid = [];
    providerDocFiles = [];
    RCFiles = [];
    fileSelected = false;
    uploadedFilesId = [];
    filesToUpload = [];

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
        this.checkFormStatus();
        this.currentNote.attachments.forEach(att => {
            if(att.type == 3) {
                this.providerDocFiles.push(att);
                this.providerDocFiles = [...this.providerDocFiles];
            };
            if(att.type == 4) {
                this.RCFiles.push(att);
                this.RCFiles = [...this.RCFiles];
            };
        });
    }

    checkFormStatus() {
        this.formNota.disable();
        if(this.mode == 'Edit') {
            this.formNota.controls.facturas.enable();
            this.formNota.controls.observaciones.enable();
        }
    }

    downloadOC() {
        let files = this.currentNote.attachments.find(file => file.type == 2);
        this.requestNoteService.downloadFile(files.fileId, 5, files.fileDescription);
    }

    downloadProviderDoc(item) {
        console.log(item);
        this.requestNoteService.downloadFile(item.fileId, 5, item.fileDescription);
    }

    downloadRC(item) {
        console.log(item);
        this.requestNoteService.downloadFile(item.fileId, 5, item.fileDescription);
    }

    addBill() {
        if(this.fileSelected == false) {
            this.messageService.showMessage("Debe seleccionar una factura para subir", 2);
            return;
        };
        this.uploader.uploadAll();
    }

    fileCheck(event) {
        this.filesToUpload = [];
        this.uploader.queue.forEach(file => {
            this.filesToUpload.push(file.file.name);
            this.filesToUpload = [...this.filesToUpload]
        });
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
                type: 5,
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
            this.requestNoteService.approveReceivedConformable(model).subscribe(d => {
                console.log(d);
                this.messageService.showMessage("Las facturas han sido adjuntadas a la nota de pedido", 0);
                this.router.navigate(['/providers/notes']);
            })
        };
        this.uploader.onAfterAddingFile = (file) => { file.withCredentials = false; };
    }

    clearSelectedFile(){
        if(this.uploader.queue.length > 0){
            this.uploader.queue[0].remove();
        }
  
        this.selectedFile.nativeElement.value = '';
    }
}