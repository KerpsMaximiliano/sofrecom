import { AfterViewInit, Component, Input, OnInit, ViewChild } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { ProvidersService } from "app/services/admin/providers.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { MessageService } from "app/services/common/message.service";
import { FileUploader } from "ng2-file-upload";

@Component({
    selector: 'notes-pending-supply-revision',
    templateUrl: './notes-pending-supply-revision.html'
})

export class NotesPendingSupplyRevision implements OnInit {

    @Input() currentNote;
    mode;
    show = false;
    productosServicios = [];
    analiticas = [
        {analytic: "Analítica 1", asigned: 10},
        {analytic: "Analítica 2", asigned: 30},
        {analytic: "Analítica 3", asigned: 5}
    ];
    providers = [];
    selectedProviderId: number;
    providersGrid = [];

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
        private router: Router
    ) { }

    ngOnInit(): void{
        this.inicializar();
    }

    inicializar() {
        this.mode = this.requestNoteService.getMode();
        console.log(this.currentNote)
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
            //asignar analíticas
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
        })
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
        this.providersGrid.push(busqueda);
        this.providersGrid = [...this.providersGrid]
    }

    eliminarProveedor(index: number) {
        this.providersGrid.splice(index, 1);
    }

    reject() {
        //this.requestNoteService.rejectPendingSupplyRevision(this.currentNote.id).subscribe(d=>console.log(d))
        this.messageService.showMessage("La nota de pedido ha sido rechazada", 0);
        this.router.navigate(['/providers/notes']);
    }

    send() {
        //Se debe validar que haya al menos un proveedor con un archivo adjunto.
        //Se debe validar que haya ingresado un valor para el monto final de la OC.
        //Se cambia al estado “Pendiente Aprobación Gerente Analíticas”
        //Se asignan a todas las analíticas asociadas el estado “Pendiente Aprobación”
        //this.requestNoteService.approvePendingSupplyRevision()
        this.markFormGroupTouched(this.formNota);
        if(this.formNota.invalid) {
            return;
        }
        this.messageService.showMessage("La nota de pedido ha sido enviada", 0);
        this.router.navigate(['/providers/notes']);
        //subir archivos al guardar
    }

    addFile(provider: any) {
        //Agregar archivo al proveedor
        console.log(provider)
        let model = {
            requestNoteId: this.currentNote.id,
            requestNote: this.currentNote,
            providerId: provider.id,
            provider: provider,
            productService: "",//Ver
            quantity: 0,//Ver
            fileId: 0,//Ver
            file: {}//Ver
        }
        //upload file
    }

    markFormGroupTouched(formGroup: FormGroup) {
        (<any>Object).values(formGroup.controls).forEach(control => {
            control.markAsTouched();

            if (control.controls) {
                this.markFormGroupTouched(control);
            }
        });
    }

    //Debe ser llamado al iniciar para configurar el file uploader
    /*
    uploaderConfig(){
        this.uploader = new FileUploader({url: this.salaryAdvancementService.getUrlForImportFile(),
            authToken: 'Bearer ' + Cookie.get('access_token') ,
            maxFileSize: 50*1024*1024,
            allowedMimeType: ['application/vnd.ms-excel','application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'],
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

            var dataJson = JSON.parse(response);

            this.getData();

            if(dataJson.messages){
                this.messageService.showMessages(dataJson.messages);
            }

            this.clearSelectedFile();
        };

        this.uploader.onAfterAddingFile = (file) => { file.withCredentials = false; };
    }

    clearSelectedFile(){
        if(this.uploader.queue.length > 0){
            this.uploader.queue[0].remove();
        }
  
        this.selectedFile.nativeElement.value = '';
    } 
    */
}