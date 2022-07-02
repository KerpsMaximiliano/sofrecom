import { Component, Input, OnInit, ViewChild } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { ProvidersService } from "app/services/admin/providers.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { AuthService } from "app/services/common/auth.service";
import { MessageService } from "app/services/common/message.service";
import { Cookie } from "ng2-cookies";
import { FileUploader } from "ng2-file-upload";

@Component({
    selector: 'notes-pending-supply-approval',
    templateUrl: './notes-pending-supply-approval.html'
})

export class NotesPendingSupplyApproval implements OnInit{

    @Input() currentNote;
    productosServicios = [];
    analiticas = [];
    providersGrid = [];
    mode;
    show = false;
    fileSelected = false;
    providerSelected = null;
    fileUploaded;

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
        ordenCompra: new FormControl(null, Validators.required)
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
        console.log(this.currentNote);
        this.checkFormStatus();
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
                montoOC: this.currentNote.purchaseOrderAmmount
            });
            this.analiticas = this.currentNote.analytics;
            this.productosServicios = this.currentNote.productsServices;
            this.providersGrid = this.currentNote.providers;
            this.show = true;
        })
    }

    checkFormStatus() {
        this.formNota.disable();
        if(this.mode == 'Edit') {
            this.formNota.controls.proveedores.enable();
            this.formNota.controls.observaciones.enable();
            this.formNota.controls.ordenCompra.enable();
        }
    }

    reject() {
        let model = {
            id: this.currentNote.id
        };
        this.requestNoteService.rejectPendingSupplyApproval(model).subscribe(d => {
            console.log(d);
            this.messageService.showMessage("La nota de pedido ha sido rechazada", 0);
            this.router.navigate(['/providers/notes']);
        })
    }

    approve() {
        //se debe validar que haya elegido un proveedor del listado de proveedores.
        //Se cambia el estado de la nota de pedido a “Pendiente Aprobación DAF”
        let fileError = false;
        let providerError = false;
        this.markFormGroupTouched(this.formNota);
        if(this.fileSelected == false) {
            this.messageService.showMessage("Debe seleccionar un archivo orden de compra para subir", 2);
            fileError = true;
        };
        if(this.providerSelected == null) {
            this.messageService.showMessage("Debe seleccionar un proveedor", 2);
            providerError = true;
        };
        if(this.formNota.invalid || providerError || fileError) {
            return;
        };
        this.uploader.uploadAll();
    }

    fileCheck(event) {
        if(event.length == 1) {
            this.fileSelected = true;
        } else {
            this.fileSelected = false;
        }
    }

    selectProvider(item) {
        this.providerSelected = item;
    }

    markFormGroupTouched(formGroup: FormGroup) {
        (<any>Object).values(formGroup.controls).forEach(control => {
            control.markAsTouched();

            if (control.controls) {
                this.markFormGroupTouched(control);
            }
        });
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
            let model = {
                fileId: jsonResponse.data[0].id,
                purchaseOrderNumber: this.formNota.controls.ordenCompra.value
            };
            this.fileUploaded = model;
            this.clearSelectedFile();
        };

        this.uploader.onCompleteAll = () => {
            this.providerSelected.isSelected
            let model = {
                id: this.currentNote.id,
                purchaseOrderNumber: this.fileUploaded.purchaseOrderNumber,
                attachments: [{
                    fileId: this.fileUploaded.fileId,
                    type: 2
                }],
                providerSelectedId:  this.providerSelected.providerId,
                comments: this.formNota.controls.observaciones.value
            };
            console.log(model);
            this.requestNoteService.approvePendingSupplyApproval(model).subscribe(d => {
                console.log(d);
                this.messageService.showMessage("La nota de pedido ha sido aprobada", 0);
                this.router.navigate(['/providers/notes']);
            });
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