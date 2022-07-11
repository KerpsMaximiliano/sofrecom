import { AfterViewInit, Component, Input, OnInit, ViewChild } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { ProvidersService } from "app/services/admin/providers.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { AuthService } from "app/services/common/auth.service";
import { MessageService } from "app/services/common/message.service";
import { Cookie } from "ng2-cookies";
import { FileUploader } from "ng2-file-upload";

@Component({
    selector: 'notes-pending-supply-revision',
    templateUrl: './notes-pending-supply-revision.html'
})

export class NotesPendingSupplyRevision implements OnInit {

    @Input() currentNote;
    @Input() currentNoteStatusDescription;
    mode;
    show = false;
    productosServicios = [];
    analiticas = [];
    providers = [];
    selectedProviderId: number;
    providersGrid = [];
    filesToUpload = [];
    fileIdCounter = 0;
    finalProviders = [];

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
        montoOC: new FormControl(null, [Validators.required, Validators.min(1)]),
    })

    constructor(
        private providerService: ProvidersService,
        private providersAreaService: ProvidersAreaService,
        private requestNoteService: RequestNoteService,
        private messageService: MessageService,
        private authService: AuthService,
        private router: Router
    ) { }

    ngOnInit(): void{
        this.inicializar();
    }

    inicializar() {
        this.mode = this.requestNoteService.getMode();
        console.log(this.currentNote);
        this.uploaderConfig();
        let providerArea;
        this.providersAreaService.get(this.currentNote.providerAreaId).subscribe(d => {
            console.log(d);
            providerArea = d.data;
            this.formNota.patchValue({
                descripcion: this.currentNote.description,
                rubro: providerArea.description,
                critico: (providerArea.critical) ? "Si" : "No",
                requierePersonal: this.currentNote.requiresEmployeeClient,
                previstoPresupuesto: this.currentNote.consideredInBudget,
                nroEvalprop: this.currentNote.evalpropNumber,
                observaciones: this.currentNote.comments
            });
            this.analiticas = this.currentNote.analytics;
            this.productosServicios = this.currentNote.productsServices;
            this.providersGrid = this.currentNote.providers;
            this.show = true;
        })
        this.checkFormStatus();
        this.providerService.getAll().subscribe(d => {
            console.log(d.data)
            d.data.forEach(prov => {
                if(prov.providerAreaId == this.currentNote.providerAreaId) {
                    this.providers.push(prov);
                    this.providers = [...this.providers]
                }
            });
        });
    }

    checkFormStatus() {
        this.formNota.disable();
        if (this.mode == "Edit") {
            this.formNota.controls.proveedores.enable();
            this.formNota.controls.observaciones.enable();
            this.formNota.controls.montoOC.enable();
        }
    }

    agregarProveedor() {
        if (this.formNota.controls.proveedores.value == null) {
            return;
        }
        let busqueda = this.providers.find(proveedor => proveedor.id == this.formNota.controls.proveedores.value);
        this.providersGrid.push({
            providerId: busqueda.id,
            providerDescription: busqueda.name,
            fileId: null,
            fileDescription: null
        });
        this.providersGrid = [...this.providersGrid]
    }

    eliminarProveedor(index: number) {
        let search =  this.filesToUpload.find(file => file.tableIndex == index);
        if (search != undefined) {
            this.uploader.queue[search.queueIndex].remove();
            this.filesToUpload.splice(search.queueIndex ,1);
            this.filesToUpload.forEach(file => {
                if(file.queueIndex > search.queueIndex) {
                    file.queueIndex--;
                }
            });
        }
        this.filesToUpload.forEach(file => {
            if(file.tableIndex > index) {
                file.tableIndex--;
            }
        })
        this.providersGrid.splice(index, 1);
    }

    reject() {
        let model = {
            id: this.currentNote.id
        }
        this.requestNoteService.rejectPendingSupplyRevision(model).subscribe(d=>{
            console.log(d);
            this.messageService.showMessage("La nota de pedido ha sido rechazada", 0);
            this.router.navigate(['/providers/notes']);
        })
    }

    send() {
        this.markFormGroupTouched(this.formNota);
        if(this.formNota.invalid) {
            return;
        };
        if(this.filesToUpload.length < 1) {
            this.messageService.showMessage("Al menos un proveedor debe tener un archivo adjunto", 2);
            return;
        };
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
            this.filesToUpload[this.fileIdCounter].fileId = jsonResponse.data[0].id;
            this.fileIdCounter++;
            console.log(jsonResponse);
            this.clearSelectedFile();
        };

        this.uploader.onCompleteAll = () => {
            this.providersGrid.forEach(prov => {
                let search = this.filesToUpload.find(provFile => provFile.providerId == prov.providerId);
                if (search == undefined) {
                    this.finalProviders.push(prov);
                } else {
                    this.finalProviders.push({
                        fileDescription: null,
                        fileId: search.fileId,
                        providerDescription: prov.providerDescription,
                        providerId: prov.providerId,
                    });
                };
            });
            let model = {
                id: this.currentNote.id,
                purchaseOrderAmmount: this.formNota.controls.montoOC.value,
                providers: this.finalProviders,
                comments: this.formNota.controls.observaciones.value
            }
            this.requestNoteService.sendPendingSupplyRevision(model).subscribe(d=>{
                if(d == null) {
                    this.messageService.showMessage("La nota de pedido ha sido enviada", 0);
                    this.router.navigate(['/providers/notes']);
                }
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

    selectedFileProvider(providerId: number, event: any, index: number) {
        let search = this.filesToUpload.find(file => file.providerId == providerId);
        if(search == undefined) {
            if(event.length == 1) {
                let provData = {
                    providerId: providerId,
                    tableIndex: index,
                    queueIndex: this.uploader.queue.length - 1,
                    fileId: null
                };
                this.filesToUpload.push(provData);
            }
        } else {
            if(event.length == 1) {
                this.filesToUpload[search.queueIndex].tableIndex = index;
                this.filesToUpload[search.queueIndex].queueIndex = this.uploader.queue.length - 1;
            } else {
                this.filesToUpload.splice(search.queueIndex ,1);
                this.filesToUpload.forEach(file => {
                    if(file.queueIndex > search.queueIndex) {
                        file.queueIndex--;
                    }
                })
            }
        }
        
    }

    markFormGroupTouched(formGroup: FormGroup) {
        (<any>Object).values(formGroup.controls).forEach(control => {
            control.markAsTouched();

            if (control.controls) {
                this.markFormGroupTouched(control);
            }
        });
    }

    downloadProvFile(item) {
        this.requestNoteService.downloadFile(item.fileId, 5, item.fileDescription);
    }
}