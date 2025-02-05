import { Component, Input, OnInit, ViewChild } from "@angular/core";
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { ProvidersService } from "app/services/admin/providers.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { SettingsService } from "app/services/admin/settings.service";
import { UserService } from "app/services/admin/user.service";
import { RefundService } from "app/services/advancement-and-refund/refund.service";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { AuthService } from "app/services/common/auth.service";
import { MessageService } from "app/services/common/message.service";
import { UserInfoService } from "app/services/common/user-info.service";
import { WorkflowService } from "app/services/workflow/workflow.service";
import { environment } from "environments/environment";
import { Cookie } from "ng2-cookies";
import { FileUploader } from "ng2-file-upload";
import { forkJoin } from "rxjs";

@Component({
    selector: 'notes-draft',
    templateUrl: './notes-draft.html',
    styleUrls: ['./notes-draft.scss']
})

export class NotesDraftComponent implements OnInit{
    @Input() currentNote;
    uploadedFilesId = [];
    requestNoteId;

    @ViewChild('selectedFile') selectedFile: any;
    public uploader: FileUploader = new FileUploader({
        url: this.requestNoteService.uploadDraftFiles(),
        authToken: 'Bearer ' + Cookie.get('access_token'), 
    });
    travelFormShow: boolean = false;
    trainingFormShow: boolean = false;

    workflowId: number;
    public draftId: number;

    mode: string;

    analyticError: boolean = false;
    analyticPercentageError: boolean = false;
    productsServicesError: boolean = false;
    productsServicesQuantityError: boolean = false;
    formParticipanteCapacitacionError: boolean = false;
    formParticipanteViajeError: boolean = false;
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

    travelBirthday;
    travelDepartureDate;
    travelReturnDate;
    trainingDate;

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
        training: new FormControl(false, []),
    });

    formProductoServicio: FormGroup = new FormGroup({
        productService: new FormControl(null, [Validators.required, Validators.maxLength(500)]),
        quantity: new FormControl(null, [Validators.required, Validators.min(1)])
    });

    formProductoServicioTable: FormGroup;

    formAnaliticas: FormGroup = new FormGroup({
        analytic: new FormControl(null, [Validators.required]),
        asigned: new FormControl(null, [Validators.required, Validators.min(1)])
    });

    formAnaliticasTable: FormGroup;

    formProveedores: FormGroup = new FormGroup({
        provider: new FormControl(null, [Validators.required])
    })
    
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

    formParticipanteViaje: FormGroup = new FormGroup({
        name: new FormControl (null, [Validators.required, Validators.maxLength(100)]),
        //birth: new FormControl (null, [Validators.required]),
        //cuit: new FormControl (null, [Validators.required]),
    });

    formParticipanteCapacitacion: FormGroup = new FormGroup({
        name: new FormControl (null, [Validators.required, Validators.maxLength(100)]),
        sector: new FormControl (null, [Validators.required])
    });

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
        private builder: FormBuilder,
        private workflowService: WorkflowService,
        private activatedRoute: ActivatedRoute
    ) {
        this.formAnaliticasTable = this.builder.group({
            analiticas: this.builder.array([])
        });
        this.formProductoServicioTable = this.builder.group({
            productoServicio: this.builder.array([])
        })
    }

    ngOnInit(): void {
        this.mode = this.requestNoteService.getMode();
        this.workflowId = this.currentNote.workflowId;
        this.inicializar();
        this.userInfo = UserInfoService.getUserInfo();
        this.draftId = this.activatedRoute.snapshot.params.id;
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
            d.forEach(emp => {
                if(emp.birthday != null) {
                    let year = emp.birthday.split('-')[0];
                    let month = emp.birthday.split('-')[1];
                    let day = emp.birthday.split('-')[2];
                    emp.birthday = `${day}/${month}/${year}`
                }
            });
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
                description: this.currentNote.description,
                providerArea: this.currentNote.providerAreaId,
                requiresPersonel: this.currentNote.requiresEmployeeClient,
                evaluationProposal: this.currentNote.consideredInBudget,
                numberEvalprop: this.currentNote.evalpropNumber,
                observations: this.currentNote.comments,
                travel: this.currentNote.travelSection,
                training: this.currentNote.trainingSection,
                providers: this.currentNote.providerSuggested
            });
            this.critical = (d.data.critical) ? "Si" : "No";
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
        this.analiticasTable.forEach(analytic => {
            this.getAnaliticas().push(this.builder.group({
                analyticName: {value: analytic.description, disabled: true },
                analyticId: analytic.analyticId,
                asigned: analytic.asigned,
            }));
        })
        this.productosServicios = this.currentNote.productsServices;
        this.productosServicios.forEach(ps => {
            this.getProductoServicio().push(this.builder.group({
                productService: ps.productService,
                quantity: ps.quantity,
            }));
        })
        this.currentNote.providers.forEach(prov => {
            this.proveedoresTable.push({
                id: prov.providerId,
                name: prov.providerDescription
            })
        });
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
            this.currentNote.training.participants.forEach(participant => {
                this.participantesCapacitacion.push({
                    data: {
                        id: participant.employeeId,
                        name: participant.name
                    },
                    sector: participant.sector
                })
            });
        }
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

    travelChange(event) {
        if(event == undefined) {
            this.participanteViajeSeleccionado = null;
            this.participanteViajeSeleccionadoCuit = null;
            this.participanteViajeSeleccionadoFecha = null;
        } else {
            this.participanteViajeSeleccionado = event;
            this.participanteViajeSeleccionadoCuit = event.cuil;
            this.participanteViajeSeleccionadoFecha = event.birthday;
        }
    }

    trainingChange(event) {
        if(event == undefined) {
            this.participanteCapacitacionSeleccionado = null;
        } else {
            this.participanteCapacitacionSeleccionado = event;
            this.employeeService.getSectorName(event.id).subscribe(d => {
                this.formParticipanteCapacitacion.controls.sector.setValue(d.data);
            })
        }
    }

    openTravelModal() {
        this.travelFormShow = !this.travelFormShow;
    }

    openTrainingModal() {
        this.trainingFormShow = !this.trainingFormShow;
    }

    agregarParticipanteViaje() {
        if(this.formParticipanteViaje.invalid) {
            return;
        };
        if(this.participanteViajeSeleccionado == null) {
            return;
        }
        
        this.participantesViaje.push(this.participanteViajeSeleccionado);
        this.participanteViajeSeleccionado = null;
        this.participanteViajeSeleccionadoCuit = null;
        this.participanteViajeSeleccionadoFecha = null;
        this.formParticipanteViajeError = false;
        this.formParticipanteViaje.get('name').setValue(null);
    }

    eliminarParticipanteViaje(index: number) {
        this.participantesViaje.splice(index, 1);
    }

    agregarParticipanteCapacitacion() {
        this.markFormGroupTouched(this.formParticipanteCapacitacion);
        if(this.formParticipanteCapacitacion.invalid) {
            return;
        };
        if(this.participanteCapacitacionSeleccionado == null) {
            return;
        };
        let search = this.participantesCapacitacion.find(part => part.data.id == this.participanteCapacitacionSeleccionado.id);
        if (search != undefined) {
            this.messageService.showMessage("Ya existe este participante en la grilla", 2);
            return;
        };
        let participante = {
            data: this.participanteCapacitacionSeleccionado,
            sector: this.formParticipanteCapacitacion.controls.sector.value
        };
        this.participantesCapacitacion.push(participante);
        this.formParticipanteCapacitacionError = false;
        this.participanteCapacitacionSeleccionado = null;
        this.formParticipanteCapacitacion.reset();
    }

    eliminarParticipanteCapacitacion(index: number) {
        this.participantesCapacitacion.splice(index, 1);
        if (this.participantesCapacitacion.length < 1) {
            this.formParticipanteCapacitacionError = true;
        };
    }

    agregarProductoServicio() {
        this.productsServicesTableError = false;
        if(this.formProductoServicio.invalid) {
            this.productsServicesTableError = true;
            this.markFormGroupTouched(this.formProductoServicio);
            return;
        };
        let productoServicio = {
            productService: this.formProductoServicio.controls.productService.value,
            quantity: this.formProductoServicio.controls.quantity.value
        };
        let search = this.getProductoServicio().value.find(ps => ps.productService == productoServicio.productService);
        if (search != undefined) {
            this.messageService.showMessage("Ya existe este producto o servicio en la grilla", 2);
            return;
        };
        this.getProductoServicio().push(this.builder.group({
            productService: productoServicio.productService,
            quantity: productoServicio.quantity,
        }));
        this.productosServicios.push(productoServicio);
        this.productsServicesError = false;
        let totalQuantity = 0;
        this.getProductoServicio().value.forEach(product => {
            totalQuantity = totalQuantity + product.quantity;
        });
        if(totalQuantity <= 0) {
            this.productsServicesQuantityError = true;
        } else {
            this.productsServicesQuantityError = false;
        };
        this.formProductoServicio.get('productService').setValue(null);
        this.formProductoServicio.get('quantity').setValue(null);
    }

    eliminarProductoServicio(index: number) {
        this.productosServicios.splice(index, 1);
        this.getProductoServicio().removeAt(index);
        if(this.productosServicios.length <= 0) {
            this.productsServicesError = true;
        }
        let totalQuantity = 0;
        this.getProductoServicio().value.forEach(product => {
            totalQuantity = totalQuantity + product.quantity;
        });
        if(totalQuantity <= 0) {
            this.productsServicesQuantityError = true;
        } else {
            this.productsServicesQuantityError = false;
        }
    }

    getProductoServicio(): FormArray {
        return this.formProductoServicioTable.get("productoServicio") as FormArray;
    }

    productoServicioChange() {
        let totalQuantity = 0;
        this.getProductoServicio().value.forEach(ps => {
            totalQuantity = totalQuantity + ps.quantity;
        });
        if(totalQuantity <= 0) {
            this.productsServicesQuantityError = true;
        } else {
            this.productsServicesQuantityError = false;
        }
    }

    agregarAnalitica() {
        this.analyticFormError = false;
        this.analyticErrorSend = false;
        this.analyticPercentageErrorSend = false;
        if(this.formAnaliticas.invalid) {
            this.analyticFormError = true;
            this.markFormGroupTouched(this.formAnaliticas);
            return;
        };
        let busqueda = this.analiticas.find(analytic => analytic.id == this.formAnaliticas.controls.analytic.value)
        let analitica = {
            analytic: busqueda,
            asigned: this.formAnaliticas.controls.asigned.value
        };
        let search = this.getAnaliticas().value.find(ps => ps.analyticId == analitica.analytic.id);
        if (search != undefined) {
            this.messageService.showMessage("Ya existe esta analítica en la grilla", 2);
            return;
        };
        this.getAnaliticas().push(this.builder.group({
            analyticName: {value: busqueda.text, disabled: true },
            analyticId: busqueda.id,
            asigned: this.formAnaliticas.controls.asigned.value,
        }));
        this.analiticasTable.push(analitica)
        this.analyticError = false;
        let totalPercentage = 0;
        this.getAnaliticas().value.forEach(analytic => {
            totalPercentage = totalPercentage + analytic.asigned;
        });
        if(totalPercentage != 100) {
            this.analyticPercentageError = true;
        } else {
            this.analyticPercentageError = false;
        };
        this.formAnaliticas.get('analytic').setValue(null);
        this.formAnaliticas.get('asigned').setValue(null);
    }

    eliminarAnalitica(index: number) {
        this.analyticErrorSend = false;
        this.analyticPercentageErrorSend = false;
        this.analiticasTable.splice(index, 1);
        this.getAnaliticas().removeAt(index);
        if(this.analiticasTable.length <= 0) {
            this.analyticError = true;
        };
        let totalPercentage = 0;
        this.getAnaliticas().value.forEach(analytic => {
            totalPercentage = totalPercentage + analytic.asigned;
        });
        if(totalPercentage != 100) {
            this.analyticPercentageError = true;
        } else {
            this.analyticPercentageError = false;
        }
    }

    getAnaliticas(): FormArray {
        return this.formAnaliticasTable.get("analiticas") as FormArray;
    }

    analyticChange() {
        this.analyticPercentageErrorSend = false;
        let totalPercentage = 0;
        this.getAnaliticas().value.forEach(analytic => {
            totalPercentage = totalPercentage + analytic.asigned;
        });
        if(totalPercentage != 100) {
            this.analyticPercentageError = true;
        } else {
            this.analyticPercentageError = false;
        }
    }

    // agregarProveedor() {
    //     if(this.formProveedores.invalid) {
    //         return;
    //     };
    //     let busqueda = this.allProviders.find(prov => prov.id == this.formProveedores.controls.provider.value);
    //     this.proveedoresTable.push(busqueda);
    //     this.formProveedores.get('provider').setValue(null);
    // }

    // eliminarProveedor(index: number) {
    //     this.proveedoresTable.splice(index, 1);
    // }

    saveNote(send: boolean) {
        this.markFormGroupTouched(this.formNota);
        if(this.formNota.controls.training.value == true) {
            this.markFormGroupTouched(this.formCapacitacion);
        };
        if(this.formNota.controls.training.value == false) {
            this.formParticipanteCapacitacionError = false;
        } else {
            this.markFormGroupTouched(this.formViaje);
        };
        this.productsServicesTableError = false;
        this.getProductoServicio().value.forEach(ps => {
            if(ps.productService == null || ps.productService == '') {
                this.messageService.showMessage("El nombre de un prodcuto/servicio no puede estar vacío", 2);
                this.productsServicesTableError = true;
            };
        });
        if(this.formNota.controls.providerArea.errors != null) {
            return;
        };
        if(this.formNota.controls.travel.value == true) {
            if(!this.formViaje.valid || this.travelDateError) {
                return;
            };
        };
        if(this.formNota.controls.training.value == true) {
            if(!this.formCapacitacion.valid) {
                return;
            };
        };
        let finalProductsAndServices = this.getProductoServicio().value;
        let analytics = [];
        if(this.getAnaliticas().value.length > 0) {
            this.getAnaliticas().value.forEach(analytic => {
                let push = {
                    analyticId: analytic.analyticId,
                    asigned: analytic.asigned
                }
                analytics.push(push)
            });
        };
        // let finalProviders = [];
        // this.proveedoresTable.forEach(prov => {
        //     let mock = {
        //         providerId: prov.id,
        //         fileId: null
        //     };
        //     finalProviders.push(mock);
        // })
        let finalTrainingPassengers = [];
        this.participantesCapacitacion.forEach(employee => {
            finalTrainingPassengers.push({
                employeeId: employee.data.id,
                sector: employee.sector
            });
        });
        let finalTravelPassengers = [];
        this.participantesViaje.forEach(employee => {
            finalTravelPassengers.push({
                employeeId: employee.id,
                cuit: employee.cuil,
                birth: employee.birthday
            });
        });
        let model = {
            id: this.currentNote.id,
            description: this.formNota.controls.description.value,
            productsServices: finalProductsAndServices,
            providerAreaId: this.formNota.controls.providerArea.value,
            analytics: analytics,
            requiresEmployeeClient: this.formNota.controls.requiresPersonel.value,
            providerSuggested: this.formNota.controls.providers.value,
            consideredInBudget: this.formNota.controls.evaluationProposal.value,
            evalpropNumber: this.formNota.controls.numberEvalprop.value,
            comments: this.formNota.controls.observations.value,
            travelSection: this.formNota.controls.travel.value,
            trainingSection: this.formNota.controls.training.value,
            training: {
                name: this.formCapacitacion.controls.name.value,
                subject: this.formCapacitacion.controls.subject.value,
                location: this.formCapacitacion.controls.location.value,
                date: this.formCapacitacion.controls.date.value,
                duration: this.formCapacitacion.controls.duration.value,
                ammount: this.formCapacitacion.controls.ammount.value,
                participants: finalTrainingPassengers
            },
            travel: {
                passengers: finalTravelPassengers,
                departureDate: this.formViaje.controls.departureDate.value,
                returnDate: this.formViaje.controls.returnDate.value,
                destination: this.formViaje.controls.destination.value,
                transportation: this.formViaje.controls.transportation.value,
                accommodation: this.formViaje.controls.accommodation.value,
                details: this.formViaje.controls.details.value
            },
            userApplicantId: this.userInfo.id,
            workflowId: this.workflowId,
            attachments: this.uploadedFilesId
        };
        if(!model.travelSection) {
            model.travel = null;
        };
        if(!model.trainingSection) {
            model.training = null;
        }
        this.requestNoteService.saveDraft(model).subscribe(d=>{
            this.requestNoteId = d.data;
            if(send) {
                var modelWorkflow = {
                    workflowId: this.workflowId,
                    nextStateId: environment.PENDIENTE_APROBACION_GERENTE_ANALITICA_STATE_ID,
                    entityId: this.requestNoteId,
                    entityController: "RequestNoteBorrador",
                    requestNote: model
                };
                this.workflowService.post(modelWorkflow).subscribe(response => {
                    setTimeout(() => {
                        this.router.navigate(['/providers/notes']);
                    }, 500);
                });
            } else {
                setTimeout(() => {
                    this.router.navigate(['/providers/notes']);
                }, 500);
            }
        })
    }

    markFormGroupTouched(formGroup: FormGroup) {
        (<any>Object).values(formGroup.controls).forEach(control => {
            control.markAsTouched();
            if (control.controls) {
                this.markFormGroupTouched(control);
            }
        });
    }

    dateChange(number, event) {
        if(number == 1) {
            this.formParticipanteViaje.controls.birth.setValue(this.travelBirthday);
        }
        if(number == 2) {
            this.formViaje.controls.departureDate.setValue(this.travelDepartureDate);
            this.formViaje.controls.departureDate.markAsDirty();
            this.formViaje.controls.departureDate.markAsTouched();
            if(this.travelDepartureDate != null && this.travelReturnDate != null) {
                if (this.travelDepartureDate > this.travelReturnDate) {
                    this.travelDateError = true;
                } else {
                    this.travelDateError = false;
                }
            } else {
                this.travelDateError = false;
            }
        }
        if(number == 3) {
            this.formViaje.controls.returnDate.setValue(this.travelReturnDate);
            this.formViaje.controls.returnDate.markAsDirty();
            this.formViaje.controls.returnDate.markAsTouched();
            if(this.travelDepartureDate != null && this.travelReturnDate != null) {
                if (this.travelDepartureDate > this.travelReturnDate) {
                    this.travelDateError = true;
                } else {
                    this.travelDateError = false;
                }
            } else {
                this.travelDateError = false;
            }
        }
        if(number == 4) {
            this.formCapacitacion.controls.date.setValue(this.trainingDate);
            this.formCapacitacion.controls.date.markAsDirty();
            this.formCapacitacion.controls.date.markAsTouched();
        }        
    }

    sendDraft() {
        this.markFormGroupTouched(this.formNota);
        this.travelFormError = true;
        this.trainingFormError = true;
        this.descriptionError = true;
        if(this.formNota.controls.training.value == true) {
            this.markFormGroupTouched(this.formCapacitacion);
            if(this.participantesCapacitacion.length <= 0) {
                this.formParticipanteCapacitacionError = true;
            };
        };
        if(this.formNota.controls.travel.value == true) {
            this.markFormGroupTouched(this.formViaje);
            if(this.participantesViaje.length <= 0) {
                this.formParticipanteViajeError = true;
            };
        };
        if(this.formNota.controls.training.value == false) {
            this.formParticipanteCapacitacionError = false;
        } else {
            this.markFormGroupTouched(this.formViaje);
        };
        if(this.formNota.controls.travel.value == false) {
            this.formParticipanteViajeError = false;
        } else {
            this.markFormGroupTouched(this.formCapacitacion);
        };
        this.productsServicesTableError = false;
        this.getProductoServicio().value.forEach(ps => {
            if(ps.productService == null || ps.productService == '') {
                this.messageService.showMessage("El nombre de un prodcuto/servicio no puede estar vacío", 2);
                this.productsServicesTableError = true;
            };
        });
        let totalPercentage = 0;
        this.getAnaliticas().value.forEach(analytic => {
            totalPercentage = totalPercentage + analytic.asigned;
        });
        if(totalPercentage != 100) {
            this.analyticPercentageErrorSend = true;
            this.analyticPercentageError = true;
        } else {
            this.analyticPercentageErrorSend = false;
            this.analyticPercentageError = false;
        };
        if(!this.formNota.valid || this.productosServicios.length <= 0 || this.analiticasTable.length <= 0 || this.analyticPercentageError || this.productsServicesQuantityError || this.formParticipanteCapacitacionError || this.productsServicesTableError) {
            if(this.productosServicios.length <= 0) {
                this.productsServicesError = true;
            }
            if(this.analiticasTable.length <= 0) {
                this.analyticError = true;
                this.analyticErrorSend = true;
            }
            return;
        };
        if(this.formNota.controls.travel.value == true) {
            if(!this.formViaje.valid || this.participantesViaje.length <= 0 || this.travelDateError) {
                return;
            }
        };
        if(this.formNota.controls.training.value == true) {
            if(!this.formCapacitacion.valid) {
                return;
            }
        };
        this.saveNote(true);
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
}