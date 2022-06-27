import { Component, OnInit } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { ProvidersService } from "app/services/admin/providers.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { SettingsService } from "app/services/admin/settings.service";
import { UserService } from "app/services/admin/user.service";
import { RefundService } from "app/services/advancement-and-refund/refund.service";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { UserInfoService } from "app/services/common/user-info.service";
import { forkJoin } from "rxjs";

@Component({
    selector: 'notes-add',
    templateUrl: './notes-add.html',
    styleUrls: ['./notes-add.scss']
})

export class NotesAddComponent implements OnInit{

    travelFormShow: boolean = false;
    trainingFormShow: boolean = false;

    analyticError: boolean = false;
    productsServicesError: boolean = false;

    providerAreas = [];
    //Combo multiselección de la tabla Providers que sean del rubro seleccionado y estén activos
    //Participantes ficha del viaje
    //Combo agregar participantes (Texto 100 caracteres)
    participantes = ['participante 1', 'participante 2', 'participante 3'];
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
        description: new FormControl(null, [Validators.required, Validators.maxLength(1000)]),//Descripcion
        productsAndServicies: new FormControl(null),//Combo editable productos/servicios
        //2 campos
        //Productos/Servicios - texto 5000 requerido
        //Cantidad - numero mayor a 0 requerido
        providerArea: new FormControl(null, [Validators.required]),//Rubro - Combo de ProvidersAreas con Active = true
        critical: new FormControl(null, []),//Se carga a partir del rubro seleccionado. Es readonly. 
        //Asociado a la columna Critical del Rubro seleccionado. Es Si o No. Va al lado del Combo de Rubro.
        analytics: new FormControl(null, []),//Grilla editable de Analíticas
        requiresPersonel: new FormControl(null, []),
        providers: new FormControl(null, []),//combo multiselección de la tabla Providers que sean del rubro seleccionado y estén activos. 
        //Es opcional, puede no elegir ninguno. 
        evaluationProposal: new FormControl(false, []),//Checkbox
        numberEvalprop: new FormControl(null, [Validators.maxLength(100)]),
        observations: new FormControl(null, []),
        travel: new FormControl(false, []),//Checkbox
        training: new FormControl(false, []),//Checkbox
    });

    formProductoServicio: FormGroup = new FormGroup({
        productService: new FormControl(null, [Validators.required, Validators.maxLength(5000)]),
        quantity: new FormControl(null, [Validators.required, Validators.min(1)])
    });

    formAnaliticas: FormGroup = new FormGroup({
        analytic: new FormControl(null, [Validators.required]),
        asigned: new FormControl(null, [Validators.required, Validators.min(1)])
    });

    formProveedores: FormGroup = new FormGroup({
        provider: new FormControl(null, [Validators.required])
    })
    
    formCapacitacion: FormGroup = new FormGroup({
        name: new FormControl(null, [Validators.required]),
        subject: new FormControl(null, [Validators.required]),
        location: new FormControl(null, [Validators.required]),
        date: new FormControl(null, [Validators.required]),
        duration: new FormControl(null, [Validators.required]),
        ammount: new FormControl(null, [Validators.required]),
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
        birth: new FormControl (null, [Validators.required]),
        cuit: new FormControl (null, [Validators.required]),
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
        private requestNoteService: RequestNoteService
    ) {}

    ngOnInit(): void {
        this.inicializar();
        this.userInfo = UserInfoService.getUserInfo();
        console.log(this.userInfo);
        this.analyticService.getByCurrentUser().subscribe(d=>console.log(d))
        this.refundService.getAnalytics().subscribe(d=>console.log(d))
        //this.requestNoteService.getById(1).subscribe(d=>console.log(d))
    }

    inicializar() {
        this.providersAreaService.getAll().subscribe(d => {
            //console.log(d)
            d.data.forEach(providerArea => {
                if(providerArea.active) {
                    this.providerAreas.push(providerArea);
                    this.providerAreas = [...this.providerAreas]
                }
            });
        });
        this.employeeService.getEveryone().subscribe(d => {
            //console.log(d);
            this.participants = d;
            d.forEach(user => {
                if(user.isExternal == 0 && user.endDate == null) {
                    this.filteredParticipants.push(user);
                    this.filteredParticipants = [...this.filteredParticipants]
                }
            });
        });
        forkJoin([this.analyticService.getAll(), this.userService.getManagers(), this.userService.getManagersAndDirectors()]).subscribe(results => {
            console.log(results[0]);//Analiticas
            let managers = results[1];
            let managersAndDirectors = results[2];
            let directors = [];
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
        //console.log(event)
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
        }
        let participante = {
            name: this.formParticipanteViaje.controls.name.value,
            birth: this.formParticipanteViaje.controls.birth.value,
            cuit: this.formParticipanteViaje.controls.cuit.value,
        }
        this.participantesViaje.push(participante)
    }

    eliminarParticipanteViaje(index: number) {
        this.participantesViaje.splice(index, 1);
    }

    agregarParticipanteCapacitacion() {
        if(this.formParticipanteCapacitacion.invalid) {
            return;
        }
        let participante = {
            name: this.formParticipanteCapacitacion.controls.name.value,
            sector: this.formParticipanteCapacitacion.controls.sector.value
        }
        this.participantesCapacitacion.push(participante)
    }

    eliminarParticipanteCapacitacion(index: number) {
        this.participantesCapacitacion.splice(index, 1);
    }

    agregarProductoServicio() {
        if(this.formProductoServicio.invalid) {
            return;
        }
        let productoServicio = {
            productService: this.formProductoServicio.controls.productService.value,
            quantity: this.formProductoServicio.controls.quantity.value
        }
        this.productosServicios.push(productoServicio);
        this.productsServicesError = false;
    }

    eliminarProductoServicio(index: number) {
        this.productosServicios.splice(index, 1);
        if(this.productosServicios.length <= 0) {
            this.productsServicesError = true;
        }
    }

    agregarAnalitica() {
        if(this.formAnaliticas.invalid) {
            return;
        }
        let busqueda = this.analiticas.find(analytic => analytic.id == this.formAnaliticas.controls.analytic.value)
        let analitica = {
            analytic: busqueda,
            asigned: this.formAnaliticas.controls.asigned.value
        }
        this.analiticasTable.push(analitica)
        this.analyticError = false;
    }

    eliminarAnalitica(index: number) {
        this.analiticasTable.splice(index, 1);
        if(this.analiticasTable.length <= 0) {
            this.analyticError = true;
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

    saveNote() {
        console.log(this.formNota.value);
        console.log(this.formViaje.value);
        console.log(this.formCapacitacion.value);
        this.markFormGroupTouched(this.formNota);
        this.markFormGroupTouched(this.formViaje);
        this.markFormGroupTouched(this.formCapacitacion);
        if(!this.formNota.valid || this.productosServicios.length <= 0 || this.analiticasTable.length <= 0) {
            if(this.productosServicios.length <= 0) {
                this.productsServicesError = true;
            }
            if(this.analiticasTable.length <= 0) {
                this.analyticError = true;
            }
            console.log("Invalid nota");
            return;
        }
        if(this.formNota.controls.travel.value == true) {
            if(!this.formViaje.valid || this.participantesViaje.length <= 0) {
                console.log("Invalid viaje");
                return;
            }
        }
        if(this.formNota.controls.training.value == true) {
            if(!this.formCapacitacion.valid || this.participantesCapacitacion.length <= 0) {
                console.log("Invalid capacitacion");
                return;
            }
            
        }
        let finalProductsAndServices = this.productosServicios;
        let analytics = [];
        this.analiticasTable.forEach(analytic => {
            let push = {
                analyticId: analytic.analytic.id,
                asigned: analytic.asigned
            }
            analytics.push(push)
        });
        let finalAnalytics = this.analiticasTable;
        //let finalProviders = this.proveedoresTable;
        let finalProviders = [];
        this.proveedoresTable.forEach(prov => {
            let mock = {
                providerId: prov.id,
                name: prov.name,
                providerAreaId: prov.providerAreaId,
                userApplicantId: prov.userApplicantId,
                score: prov.score,
                startDate: prov.startDate,
                endDate: prov.endDate,
                active: prov.active,
                cuit: prov.cuit,
                ingresosBrutos: prov.ingresosBrutos,
                condicionIVA: prov.condicionIVA,
                address: prov.address,
                city: prov.city,
                zipCode: prov.zipCode,
                province: prov.province,
                contactName: prov.contactName,
                phone: prov.phone,
                email: prov.email,
                webSite: prov.webSite,
                comments: prov.comments,
                country: prov.country,
                fileId: 18
            };
            finalProviders.push(mock);
        })
        let finalTrainingPassengers = this.participantesCapacitacion;
        let finalTravelPassengers = this.participantesViaje;
        let model = {
            description: this.formNota.controls.description.value,
            productsServices: finalProductsAndServices,
            providerAreaId: this.formNota.controls.providerArea.value,
            analytics: analytics,
            requiresEmployeeClient: this.formNota.controls.requiresPersonel.value,
            providers: finalProviders,
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
            creationUserId: 5,
            creationUserApplicantId: 3,
            workflowId: 2,
        };
        
        let model2 = {
            description: this.formNota.controls.description.value,
            productsAndServicies: finalProductsAndServices,
            providerAreaId: this.formNota.controls.providerArea.value,
            analytics: finalAnalytics,
            requiresEmployeeClient: this.formNota.controls.requiresPersonel.value,
            providers: finalProviders,
            consideredInBudget: this.formNota.controls.evaluationProposal.value,
            evalpropNumber: this.formNota.controls.numberEvalprop.value,
            comments: this.formNota.controls.observations.value,
            travelSection: this.formNota.controls.travel.value,
            trainingSection: this.formNota.controls.training.value,
            creationUserId: 0,
            creationUser: "X",
            workflowId: 0,
            workflow: "X",
            attachments: "X"
        };
        console.log(model);
        this.requestNoteService.saveDraft(model).subscribe(d=>console.log(d))
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
        console.log(event)
        if(number == 1) {
            this.formParticipanteViaje.controls.birth.setValue(this.travelBirthday);
        }
        if(number == 2) {
            this.formViaje.controls.departureDate.setValue(this.travelDepartureDate);
            this.formViaje.controls.departureDate.markAsDirty();
            this.formViaje.controls.departureDate.markAsTouched();
        }
        if(number == 3) {
            this.formViaje.controls.returnDate.setValue(this.travelReturnDate);
            this.formViaje.controls.returnDate.markAsDirty();
            this.formViaje.controls.returnDate.markAsTouched();
        }
        if(number == 4) {
            this.formCapacitacion.controls.date.setValue(this.trainingDate);
            this.formCapacitacion.controls.date.markAsDirty();
            this.formCapacitacion.controls.date.markAsTouched();
        }        
    }

    sendDraft() {
        //guardar como borrador
        //pasar de estado
        //this.requestNoteService.approveDraft(id)
    }

}