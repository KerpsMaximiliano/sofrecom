import { Component, Input, ViewChild } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { ProvidersService } from "app/services/admin/providers.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { UserService } from "app/services/admin/user.service";
import { RefundService } from "app/services/advancement-and-refund/refund.service";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { AuthService } from "app/services/common/auth.service";
import { MessageService } from "app/services/common/message.service";
import { UserInfoService } from "app/services/common/user-info.service";
import { WorkflowService } from "app/services/workflow/workflow.service";
import { Cookie } from "ng2-cookies";
import { FileUploader } from "ng2-file-upload";

@Component({
    selector: 'notes-pending-purchase-approval',
    templateUrl: './notes-pending-purchase-approval.html',
    styleUrls: ['./notes-pending-purchase-approval.scss']
})

export class NotesPendingPurchaseApproval {
    @Input() currentNote;
    @Input() currentNoteStatusDescription;
    @Input() closed: boolean;
    @Input() rejected: boolean;
    uploadedFilesId = [];
    requestNoteId;

    @ViewChild('workflow') workflow;

    @ViewChild('selectedFile') selectedFile: any;
    public uploader: FileUploader = new FileUploader({
        url: this.requestNoteService.uploadDraftFiles(),
        authToken: 'Bearer ' + Cookie.get('access_token'), 
    });
    public uploaderProviders: FileUploader = new FileUploader({
        url: this.requestNoteService.uploadDraftFiles(),
        authToken: 'Bearer ' + Cookie.get('access_token'), 
    });
    travelFormShow: boolean = false;
    trainingFormShow: boolean = false;

    analyticError: boolean = false;
    analyticPercentageError: boolean = false;
    productsServicesError: boolean = false;
    productsServicesQuantityError: boolean = false;
    travelDateError: boolean = false;
    productsServicesTableError: boolean = false;
    productsServicesFormError: boolean = false;
    analyticFormError: boolean = false;
    analyticErrorSend: boolean = false;
    analyticPercentageErrorSend: boolean = false;
    descriptionError: boolean = false;
    travelFormError: boolean = false;
    trainingFormError: boolean = false;

    providerAreas = [];
    participanteViajeSeleccionado = null;
    participanteViajeSeleccionadoCuit = null;
    participanteViajeSeleccionadoFecha = null;
    participanteCapacitacionSeleccionado = null;
    participants = [];
    filteredParticipants = [];
    analiticas = [];
    analiticasTable = [];
    allProviders = [];
    providers = [];
    proveedoresTable = [];
    proveedoresSelected = [] as Array<any>;
    productosServicios = [];
    participantesViaje = [];
    participantesCapacitacion = [];
    critical: string = null;
    userInfo;

    travelBirthday: string;
    travelDepartureDate: string;
    travelReturnDate: string;
    trainingDate: string;

    filesToUpload = [] as Array<any>;
    fileIdCounter = 0;

    //FORMS
    formNota: FormGroup = new FormGroup({
        description: new FormControl(null, [Validators.required, Validators.maxLength(1000)]),
        productsAndServicies: new FormControl(null),
        providerArea: new FormControl(null, [Validators.required]),
        critical: new FormControl(null, []),
        analytics: new FormControl(null, []),
        requiresPersonel: new FormControl(null, []),
        providers: new FormControl(null, []),
        evaluationProposal: new FormControl(false, []),
        numberEvalprop: new FormControl(null, [Validators.maxLength(100)]),
        observations: new FormControl(null, []),
        travel: new FormControl(false, []),
        training: new FormControl(false, [])
    });
    formCapacitacion: FormGroup = new FormGroup({
        name: new FormControl(null, [Validators.required]),
        subject: new FormControl(null, [Validators.required]),
        location: new FormControl(null, [Validators.required]),
        date: new FormControl(null, [Validators.required]),
        duration: new FormControl(null, [Validators.required]),
        ammount: new FormControl(null, [Validators.required, Validators.min(0)]),
        participants: new FormControl(null)
    });
    formViaje: FormGroup = new FormGroup({
        passengers: new FormControl(null),
        departureDate: new FormControl(null, [Validators.required]),
        returnDate: new FormControl(null, [Validators.required]),
        destination: new FormControl(null, [Validators.required]),
        transportation: new FormControl(null, [Validators.required]),
        accommodation: new FormControl(null, [Validators.required]),
        details: new FormControl(null)
    });
    formProvidersGrid: FormGroup = new FormGroup({});

    private workflowModel: any;
    finalProviders = [] as Array<any>;

    constructor(
        private providersService: ProvidersService,
        private providersAreaService: ProvidersAreaService,
        private employeeService: EmployeeService,
        private analyticService: AnalyticService,
        private userService: UserService,
        private refundService: RefundService,
        private requestNoteService: RequestNoteService,
        private messageService: MessageService,
        private authService: AuthService,
        private router: Router,
        private workflowService: WorkflowService
    ) {}

    ngOnInit(): void {
        this.inicializar();
        this.userInfo = UserInfoService.getUserInfo();
        this.analyticService.getByCurrentUser().subscribe(d=>console.log(d));
        this.refundService.getAnalytics().subscribe(d=>console.log(d));
        this.workflowService.getTransitions({
            workflowId: this.currentNote.workflowId,
            entityController: "RequestNoteBorrador",
            entityId: this.currentNote.id,
            actualStateId: this.currentNote.statusId
        }).subscribe(response => {
            console.log(response)
        });
        this.workflowModel = {
            //workflowId: response.data.workflowId,
            //entityController: "refund",
            //entityId: response.data.id,
            //actualStateId: response.data.statusId
            workflowId: this.currentNote.workflowId,
            entityController: "RequestNoteBorrador",
            entityId: this.currentNote.id,
            actualStateId: this.currentNote.statusId
        };
        this.workflow.filesToUpload();
        this.workflow.setCustomValidations(true);
        this.workflow.init(this.workflowModel);
    }

    workflowClick(event: any) {
        console.log(event.target.innerText);
        if(event.target.innerText == "RECHAZAR ") {
            this.workflow.setCustomValidations(false);
        } else {
            this.markFormGroupTouched(this.formProvidersGrid);
            if(this.proveedoresSelected.length == 0) {
                this.messageService.showMessage("Debe seleccionar al menos un proveedor", 2);
                this.workflow.setCustomValidations(true);
                return;
            }
            if(!this.formProvidersGrid.valid) {
                this.messageService.showMessage("Cada proveedor debe tener un monto", 2);
                this.workflow.setCustomValidations(true);
                return;
            };
            if(this.proveedoresSelected.length != this.filesToUpload.length) {
                this.messageService.showMessage("Cada proveedor debe tener un presupuesto cargado", 2);
                this.workflow.setCustomValidations(true);
                return;
            };
            this.workflow.setCustomValidations(false);
            this.uploaderProviders.uploadAll();
        }
    }


    inicializar() {
        console.log(this.currentNote);
        this.uploaderConfig();
        this.uploaderProviderConfig();
        this.travelFormShow = this.currentNote.travelSection;
        this.trainingFormShow = this.currentNote.trainingSection;
        this.providersAreaService.getAll().subscribe(d => {
            d.data.forEach(providerArea => {
                if(providerArea.active) {
                    this.providerAreas.push(providerArea);
                    this.providerAreas = [...this.providerAreas]
                }
            });
        });
        this.existingData();
        this.employeeService.getEveryone().subscribe(d => {
            this.participants = d;
            d.forEach(user => {
                if(user.isExternal == 0 && user.endDate == null) {
                    this.filteredParticipants.push(user);
                    this.filteredParticipants = [...this.filteredParticipants]
                }
            });
        });

        this.analyticService.getByCurrentUserRequestNote().subscribe(d => {
            this.analiticas = d.data;
        });
        
        this.providersService.getAll().subscribe(d => {
            this.allProviders = d.data;
        })
    }

    existingData() {
        console.log(this.currentNote);
        if(this.currentNote.attachments != null) {
            this.uploadedFilesId = this.currentNote.attachments;
        }
        this.providersAreaService.get(this.currentNote.providerAreaId).subscribe(d => {
            this.formNota.patchValue({
                description: this.currentNote.description,
                providerArea: this.currentNote.providerAreaId,
                requiresPersonel: this.currentNote.requiresEmployeeClient,
                evaluationProposal: this.currentNote.consideredInBudget,
                numberEvalprop: this.currentNote.evalpropNumber,
                observations: this.currentNote.comments,
                travel: this.currentNote.travelSection,
                training: this.currentNote.trainingSection
            });
            this.critical = (d.data.critical) ? "Si" : "No";
        });
        this.providersService.getAll().subscribe(d => {
            console.log(d.data)
            d.data.forEach(prov => {
                if(prov.providerAreaId == this.currentNote.providerAreaId) {
                    this.providers.push(prov);
                    this.providers = [...this.providers]
                }
            });
        });
        this.analiticasTable = this.currentNote.analytics;
        this.productosServicios = this.currentNote.productsServices;
        this.proveedoresTable = this.currentNote.providers;
        this.proveedoresTable.forEach(prov => {
            this.formProvidersGrid.addControl(`control${prov.providerId}`, new FormControl(null, Validators.required));
        });
        this.proveedoresSelected = this.proveedoresTable;
        if(this.currentNote.travelSection) {
            this.formViaje.patchValue({
                destination: this.currentNote.travel.destination,
                transportation: this.currentNote.travel.transportation,
                accommodation: this.currentNote.travel.accommodation,
                details: this.currentNote.travel.details,
                departureDate: this.currentNote.travel.departureDate,
                returnDate: this.currentNote.travel.returnDate
            });
            this.travelDepartureDate = this.currentNote.travel.departureDate;
            this.travelReturnDate = this.currentNote.travel.returnDate;
            this.employeeService.getEveryone().subscribe(d => {
                this.currentNote.travel.passengers.forEach(ps => {
                    let findPs = d.find(passenger => passenger.id == ps.employeeId);
                    console.log(findPs)
                    if(findPs != undefined) {
                        this.participantesViaje.push(findPs);
                        this.participantesViaje = [...this.participantesViaje]
                    }
                });
            });
        }
        if(this.currentNote.trainingSection) {
            this.formCapacitacion.patchValue({
                name: this.currentNote.training.name,
                subject: this.currentNote.training.subject,
                location: this.currentNote.training.location,
                date: this.currentNote.training.date,
                duration: this.currentNote.training.duration,
                ammount: this.currentNote.training.ammount
            });
            this.trainingDate = this.currentNote.training.date;
            this.participantesCapacitacion = this.currentNote.training.participants;
        }
        //Disablear formularios
        this.formNota.disable();
        this.formCapacitacion.disable();
        this.formViaje.disable();
        if(!this.closed && !this.rejected) {
            this.formNota.get('providers').enable();
        }
    }

    change(event) {
        console.log(event)
        if(event != undefined) {
            this.critical = (event.critical) ? "Si" : "No";
            this.providers = [];
            this.allProviders.forEach(prov => {
                if(prov.providerAreaId == event.id) {
                    this.providers.push(prov);
                    this.providers = [...this.providers];
                }
            })
        } else {
            this.critical = null
        }
    }

    openTravelModal() {
        this.travelFormShow = !this.travelFormShow;
    }

    openTrainingModal() {
        this.trainingFormShow = !this.trainingFormShow;
    }

    markFormGroupTouched(formGroup: FormGroup) {
        (<any>Object).values(formGroup.controls).forEach(control => {
            control.markAsTouched();

            if (control.controls) {
                this.markFormGroupTouched(control);
            }
        });
    }

    onTransitionSuccess() {
        this.router.navigate(["/providers/notes"]);
    }

    clearSelectedFile(){
        if(this.uploader.queue.length > 0){
            this.uploader.queue[0].remove();
        }
  
        this.selectedFile.nativeElement.value = '';
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
                type: 1,
                fileId: jsonResponse.data[0].id
            });
            console.log(this.uploadedFilesId)
            this.clearSelectedFile();
        };
        this.uploader.onAfterAddingFile = (file) => { file.withCredentials = false; };
    }

    uploaderProviderConfig(){
        this.uploaderProviders = new FileUploader({url: this.requestNoteService.uploadDraftFiles(),
            authToken: 'Bearer ' + Cookie.get('access_token') ,
        });

        this.uploaderProviders.onCompleteItem = (item:any, response:any, status:any, headers:any) => {
            if(status == 401){
                this.authService.refreshToken().subscribe(token => {
                    this.messageService.closeLoading();

                    if(token){
                        this.clearSelectedFile();
                        this.messageService.showErrorByFolder('common', 'fileMustReupload');
                        this.uploaderProviderConfig();
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
        this.uploaderProviders.onCompleteAll = () => {
            this.proveedoresSelected.forEach(prov => {
                let search = this.filesToUpload.find(provFile => provFile.providerId == prov.providerId);
                    this.finalProviders.push({
                        fileDescription: null,
                        fileId: search.fileId,
                        providerDescription: prov.providerDescription,
                        providerId: prov.providerId,
                        ammount: this.formProvidersGrid.get(`control${prov.providerId}`).value
                    });
            });
            this.currentNote.providersSelected = this.finalProviders;
            this.workflow.updateRequestNote(this.currentNote);
            this.workflow.filesUploaded();
        };
        this.uploaderProviders.onAfterAddingFile = (file) => { file.withCredentials = false; };
    }

    downloadFiles() {
        this.currentNote.attachments.forEach(file => {
            if(file.fileDescription) {
                this.requestNoteService.downloadFile(file.fileId, 5, file.fileDescription);
            }
        })
    }

    //Proveedores
    agregarProveedor() {
        if (this.formNota.controls.providers.value == null) {
            return;
        };
        if(this.proveedoresSelected.find(prov => prov.providerId == this.formNota.controls.providers.value) != undefined) {
            return;
        }
        let busqueda = this.providers.find(proveedor => proveedor.id == this.formNota.controls.providers.value);
        this.proveedoresSelected.push({
            providerId: busqueda.id,
            providerDescription: busqueda.name,
            fileId: null,
            fileDescription: null,
            ammount: null
        });
        this.proveedoresSelected = [...this.proveedoresSelected];
        this.formProvidersGrid.addControl(`control${busqueda.id}`, new FormControl(null, Validators.required));
        this.formNota.get('providers').setValue(null);
    }

    selectedFileProvider(providerId: number, event: any, i: number) {
        console.log(providerId);
        console.log(event);
        console.log(i);
        let search = this.filesToUpload.find(file => file.providerId == providerId);
        if(search == undefined) {
            if(event.length == 1) {
                let provData = {
                    providerId: providerId,
                    tableIndex: innerHeight,
                    queueIndex: this.uploaderProviders.queue.length - 1,
                    fileId: null
                };
                this.filesToUpload.push(provData);
            }
        } else {
            if(event.length == 1) {
                this.filesToUpload[search.queueIndex].tableIndex = i;
                this.filesToUpload[search.queueIndex].queueIndex = this.uploaderProviders.queue.length - 1;
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

    downloadProvFile(item: any) {
        console.log(item)
    }

    eliminarProveedor(i: number, item: any) {
        this.proveedoresSelected.splice(i, 1);
        let search =  this.filesToUpload.find(file => file.tableIndex == i);
        if (search != undefined) {
            this.uploaderProviders.queue[search.queueIndex].remove();
            this.filesToUpload.splice(search.queueIndex ,1);
            this.filesToUpload.forEach(file => {
                if(file.queueIndex > search.queueIndex) {
                    file.queueIndex--;
                }
            });
        };
        this.filesToUpload.forEach(file => {
            if(file.tableIndex > i) {
                file.tableIndex--;
            }
        });
        this.formProvidersGrid.removeControl(`control${item.providerId}`);
        this.proveedoresSelected = [...this.proveedoresSelected];
    }

}