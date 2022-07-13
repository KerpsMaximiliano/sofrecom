import { Component, OnInit, ViewChild } from "@angular/core";
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from "@angular/forms";
import { ProvidersService } from "app/services/admin/providers.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { SettingsService } from "app/services/admin/settings.service";
import { UserService } from "app/services/admin/user.service";
import { RefundService } from "app/services/advancement-and-refund/refund.service";
import { SalaryAdvancementService } from "app/services/advancement-and-refund/salary-advancement";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { AuthService } from "app/services/common/auth.service";
import { MessageService } from "app/services/common/message.service";
import { UserInfoService } from "app/services/common/user-info.service";
import { FileUploader } from "ng2-file-upload";
import { forkJoin } from "rxjs";
import { Cookie } from "ng2-cookies/ng2-cookies";
import { Router } from "@angular/router";

@Component({
    selector: 'notes-add',
    templateUrl: './notes-add.html',
    styleUrls: ['./notes-add.scss']
})

export class NotesAddComponent implements OnInit{

    @ViewChild('selectedFile') selectedFile: any;
    public uploader: FileUploader = new FileUploader({
        url: this.requestNoteService.uploadDraftFiles(),
        authToken: 'Bearer ' + Cookie.get('access_token'), 
    });
    requestNoteId: number = null;
    uploadedFilesId = [];

    travelFormShow: boolean = false;
    trainingFormShow: boolean = false;

    analyticError: boolean = false;
    analyticPercentageError: boolean = false;
    productsServicesError: boolean = false;
    productsServicesQuantityError: boolean = false;
    formParticipanteCapacitacionError: boolean = false;
    travelDateError: boolean = false;
    productsServicesTableError: boolean = false;
    productsServicesFormError: boolean = false;
    analyticFormError: boolean = false;
    analyticErrorSend: boolean = false;
    analyticPercentageErrorSend: boolean = false;

    providerAreas = [];
    participants = [];
    participanteViajeSeleccionado = null;
    participanteViajeSeleccionadoCuit = null;
    participanteViajeSeleccionadoFecha = null;
    participanteCapacitacionSeleccionado = null;
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
        requiresPersonel: new FormControl(false, []),
        providers: new FormControl(null, []),
        evaluationProposal: new FormControl(false, []),
        numberEvalprop: new FormControl(null, [Validators.maxLength(100)]),
        observations: new FormControl(null, []),
        travel: new FormControl(false, []),
        training: new FormControl(false, []),
    });

    formProductoServicio: FormGroup = new FormGroup({
        productService: new FormControl(null, [Validators.required, Validators.maxLength(5000)]),
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
    })

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
        private builder: FormBuilder
    ) {
        this.formAnaliticasTable = this.builder.group({
            analiticas: this.builder.array([])
        });
        this.formProductoServicioTable = this.builder.group({
            productoServicio: this.builder.array([])
        })
    }

    ngOnInit(): void {
        this.inicializar();
        this.userInfo = UserInfoService.getUserInfo();
        console.log(this.userInfo);
        this.analyticService.getByCurrentUser().subscribe(d=>console.log(d))
        this.refundService.getAnalytics().subscribe(d=>console.log(d))
    }

    inicializar() {
        this.uploaderConfig();
        this.providersAreaService.getAll().subscribe(d => {
            d.data.forEach(providerArea => {
                if(providerArea.active) {
                    this.providerAreas.push(providerArea);
                    this.providerAreas = [...this.providerAreas]
                }
            });
        });
        this.employeeService.getEveryone().subscribe(d => {
            console.log(d)
            this.participants = d;
            d.forEach(user => {
                if(user.isExternal == 0 && user.endDate == null) {
                    this.filteredParticipants.push(user);
                    this.filteredParticipants = [...this.filteredParticipants]
                }
            });
        });
        forkJoin([this.analyticService.getByCurrentUser(), this.userService.getManagers(), this.userService.getManagersAndDirectors()]).subscribe(results => {
            console.log(results[0]);//Analiticas
            this.analiticas = results[0].data;
            let managers = results[1];
            let managersAndDirectors = results[2];
            let directors = [];
            /*
            managersAndDirectors.forEach(item => {
                let directorSearch = managers.find(x => x.id == item.id);
                if(directorSearch == undefined) {
                    directors.push(item)
                }
            });
            let isManager = managers.find(employee => Number(employee.id) == this.userInfo.id);
            if(isManager != undefined) {
                results[0].forEach(analytic => {
                    if(analytic.managerId == this.userInfo.id) {
                        this.analiticas.push(analytic);
                        this.analiticas = [...this.analiticas]
                    }
                })
            }
            let isDirector = directors.find(employee => Number(employee.id) == this.userInfo.id);
            if(isDirector != undefined) {
                console.log("Es director")
            };
            */
        });
        
        this.providersService.getAll().subscribe(d => {
            this.allProviders = d.data;
        })
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
        let search = this.participantesViaje.find(part => part.id == this.participanteViajeSeleccionado.id);
        if (search != undefined) {
            this.messageService.showMessage("Ya existe este participante en la grilla", 2);
            return;
        };
        this.participantesViaje.push(this.participanteViajeSeleccionado);
        this.participanteViajeSeleccionado = null;
        this.participanteViajeSeleccionadoCuit = null;
        this.participanteViajeSeleccionadoFecha = null;
        this.formParticipanteViaje.reset();
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
        }
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
        }
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

    agregarProveedor() {
        if(this.formProveedores.invalid) {
            return;
        }
        let busqueda = this.allProviders.find(prov => prov.id == this.formProveedores.controls.provider.value);
        this.proveedoresTable.push(busqueda)
    }

    eliminarProveedor(index: number) {
        this.proveedoresTable.splice(index, 1);
    }

    saveNote(send: boolean) {
        console.log(this.formNota.value);
        console.log(this.formViaje.value);
        console.log(this.formCapacitacion.value);
        this.markFormGroupTouched(this.formNota);
        if(this.formNota.controls.training.value == true) {
            this.markFormGroupTouched(this.formCapacitacion);
            if(this.participantesCapacitacion.length <= 0) {
                this.formParticipanteCapacitacionError = true;
            };
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
        if(!this.formNota.valid || this.productosServicios.length <= 0 || this.productsServicesQuantityError || this.formParticipanteCapacitacionError || this.productsServicesTableError) {
            if(this.productosServicios.length <= 0) {
                this.productsServicesError = true;
            };
            console.log("Invalid nota");
            return;
        };
        if(this.formNota.controls.travel.value == true) {
            if(!this.formViaje.valid || this.participantesViaje.length <= 0 || this.travelDateError) {
                console.log("Invalid viaje");
                return;
            };
        };
        if(this.formNota.controls.training.value == true) {
            if(!this.formCapacitacion.valid) {
                console.log("Invalid capacitación");
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
        let finalProviders = [];
        this.proveedoresTable.forEach(prov => {
            let mock = {
                providerId: prov.id,
                fileId: null
            };
            finalProviders.push(mock);
        })
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
        let finalAttachments = [];
        if(this.uploadedFilesId.length > 0) {
            this.uploadedFilesId.forEach(id => {
                finalAttachments.push({
                    fileId: id,
                    type: 1,
                })
            })
        };
        let provAreaSearch = this.providerAreas.find(prov => prov.id == this.formNota.controls.providerArea.value);
        let model = {
            id: null,
            description: this.formNota.controls.description.value,
            productsServices: finalProductsAndServices,
            providerAreaId: this.formNota.controls.providerArea.value,
            providerAreaDescription: provAreaSearch.description,
            analytics: analytics,
            requiresEmployeeClient: this.formNota.controls.requiresPersonel.value,
            providers: finalProviders,
            consideredInBudget: this.formNota.controls.evaluationProposal.value,
            evalpropNumber: Number(this.formNota.controls.numberEvalprop.value),
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
            workflowId: 2,
            attachments: finalAttachments
        };
        if(!model.travelSection) {
            model.travel = null;
        };
        if(!model.trainingSection) {
            model.training = null;
        }
        console.log(model);
        if(this.requestNoteId != null) {
            model.id = this.requestNoteId;
        }
        this.requestNoteService.saveDraft(model).subscribe(d=>{
            this.requestNoteId = d.data;
            this.messageService.showMessage("La nota de pedido ha sido guardada", 0);
            if(send) {
                this.requestNoteService.approveDraft(this.requestNoteId).subscribe(d=>{
                    console.log(d);
                    this.messageService.showMessage("La nota de pedido ha sido enviada", 0);
                    setTimeout(() => {
                        this.router.navigate(['/providers/notes']);
                    }, 500);
                });
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
    };

    dateChange(number, event) {
        console.log(event)
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
        if(this.formNota.controls.training.value == true) {
            this.markFormGroupTouched(this.formCapacitacion);
            if(this.participantesCapacitacion.length <= 0) {
                this.formParticipanteCapacitacionError = true;
            };
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
        let totalPercentage = 0;
        this.getAnaliticas().value.forEach(analytic => {
            totalPercentage = totalPercentage + analytic.asigned;
        });
        if(totalPercentage != 100) {
            this.analyticPercentageErrorSend = true;
        } else {
            this.analyticPercentageErrorSend = false;
        };
        if(!this.formNota.valid || this.productosServicios.length <= 0 || this.analiticasTable.length <= 0 || this.analyticPercentageError || this.productsServicesQuantityError || this.formParticipanteCapacitacionError || this.productsServicesTableError) {
            if(this.productosServicios.length <= 0) {
                this.productsServicesError = true;
            }
            if(this.analiticasTable.length <= 0) {
                this.analyticError = true;
                this.analyticErrorSend = true;
            }
            console.log("Invalid nota");
            return;
        }
        if(this.formNota.controls.travel.value == true) {
            if(!this.formViaje.valid || this.participantesViaje.length <= 0 || this.travelDateError) {
                console.log("Invalid viaje");
                return;
            };
        }
        if(this.formNota.controls.training.value == true) {
            if(!this.formCapacitacion.valid) {
                console.log("Invalid capacitación");
                return;
            };
        }
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
            this.uploadedFilesId.push(jsonResponse.data[0].id);
            console.log(this.uploadedFilesId)
            this.clearSelectedFile();
        };
        this.uploader.onAfterAddingFile = (file) => { file.withCredentials = false; };
    }

}