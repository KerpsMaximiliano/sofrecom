import { Component, Input, OnInit } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { Router } from "@angular/router";
import { ProvidersService } from "app/services/admin/providers.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { MessageService } from "app/services/common/message.service";

@Component({
    selector: 'notes-pending-approval-management-analytic',
    templateUrl: './notes-pending-approval-management-analytic.html'
})

export class NotesPendingApprovalManagementAnalytic implements OnInit{

    @Input() currentNote;
    @Input() currentNoteStatusDescription;
    mode;
    show = false;
    productosServicios = [];
    analiticas = [];
    gerenteLogueado;

    formNota: FormGroup = new FormGroup({
        descripcion: new FormControl(null),
        grillaProductosServicios: new FormControl(null),
        rubro: new FormControl(null),
        critico: new FormControl(null),
        grillaAnaliticas: new FormControl(null),
        requierePersonal: new FormControl(true),
        previstoPresupuesto: new FormControl(true),
        nroEvalprop: new FormControl(null),
        observaciones: new FormControl(null),
        montoOC: new FormControl(null)
    })

    constructor(
        private providersAreaService: ProvidersAreaService,
        private requestNoteService: RequestNoteService,
        private messageService: MessageService,
        private router: Router
    ) {}

    ngOnInit(): void {
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
                observaciones: this.currentNote.comments,
                montoOC: this.currentNote.purchaseOrderAmmount
            });
            this.analiticas = this.currentNote.analytics;
            this.productosServicios = this.currentNote.productsServices;
            this.show = true;
        })
        this.checkFormStatus()
    }

    checkFormStatus() {
        this.formNota.disable();
        if(this.mode == "Edit") {
            this.formNota.controls.observaciones.enable();
        }
    }

    approve() {
        let model = {
            id: this.currentNote.id,
            comments: this.formNota.controls.observaciones.value
        };
        this.requestNoteService.approvePendingApprovalManagementAnalytic(model).subscribe(d => {
            console.log(d);
            this.messageService.showMessage("Las analíticas asociadas han sido aprobadas", 0);
            this.router.navigate(['/providers/notes']);
        })
        
    }

    reject() {
        let model = {
            id: this.currentNote.id
        };
        this.requestNoteService.rejectPendingApprovalManagementAnalytic(model).subscribe(d => {
            console.log(d);
            this.messageService.showMessage("Las analíticas asociadas han sido aprobadas", 0);
            this.router.navigate(['/providers/notes']);
        })
    }
}