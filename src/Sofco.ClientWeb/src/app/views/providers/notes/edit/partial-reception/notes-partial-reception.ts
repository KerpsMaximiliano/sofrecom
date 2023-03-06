import { Component, Input, ViewChild } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { ProvidersService } from "app/services/admin/providers.service";
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { MessageService } from "app/services/common/message.service";
import { Router } from "@angular/router";
import * as FileSaver from "file-saver";
import { FileUploader } from "ng2-file-upload";
import { Cookie } from "ng2-cookies";
import { AuthService } from "app/services/common/auth.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { UserService } from "app/services/admin/user.service";
import { RefundService } from "app/services/advancement-and-refund/refund.service";
import { WorkflowService } from "app/services/workflow/workflow.service";
import { UserInfoService } from "app/services/common/user-info.service";

@Component({
    selector: 'notes-partial-reception',
    templateUrl: './notes-partial-reception.html',
    styleUrls: ['./notes-partial-reception.scss']
})

export class NotesPartialReception {

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
    productosServicios = [];
    participantesViaje = [];
    participantesCapacitacion = [];
    critical: string = null;
    userInfo;

    travelBirthday: string;
    travelDepartureDate: string;
    travelReturnDate: string;
    trainingDate: string;

    mode: string;

    formNota: FormGroup = new FormGroup({
        id: new FormControl(null),
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
        training: new FormControl(false, []),
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

    private workflowModel: any;

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
        this.mode = this.requestNoteService.getMode();
        this.userInfo = UserInfoService.getUserInfo();
        if(this.requestNoteService.getMode() == "Edit") {
            this.workflowModel = {
                workflowId: this.currentNote.workflowId,
                entityController: "RequestNoteBorrador",
                entityId: this.currentNote.id,
                actualStateId: this.currentNote.statusId
            };
            this.workflow.init(this.workflowModel);
        }
    }

    workflowClick() {
        this.workflow.updateRequestNote(this.currentNote);
    }


    inicializar() {
        this.uploaderConfig();
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
        if(this.currentNote.attachments != null) {
            this.uploadedFilesId = this.currentNote.attachments;
        }
        this.providersAreaService.get(this.currentNote.providerAreaId).subscribe(d => {
            this.formNota.patchValue({
                id: this.currentNote.id,
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
            this.formNota.get('id').disable();
        });
        this.providersService.getAll().subscribe(d => {
            d.data.forEach(prov => {
                if(prov.providerAreaId == this.currentNote.providerAreaId) {
                    this.providers.push(prov);
                    this.providers = [...this.providers]
                }
            });
        });
        this.analiticasTable = this.currentNote.analytics;
        this.productosServicios = this.currentNote.productsServices;
        this.proveedoresTable = this.currentNote.providersSelected;
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
        this.formViaje.disable()
    }

    change(event) {
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
            this.clearSelectedFile();
        };
        this.uploader.onAfterAddingFile = (file) => { file.withCredentials = false; };
    }

    downloadFiles() {
        this.currentNote.attachments.forEach(file => {
            if(file.fileDescription) {
                this.requestNoteService.downloadFile(file.fileId, 5, file.fileDescription);
            }
        })
    }

    downloadProvFile(item: any) {
        this.requestNoteService.downloadFile(item.fileId, 5, item.fileDescription);
    }
}