import { Component, Input, OnInit, ViewChild } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { ProvidersService } from "app/services/admin/providers.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { MessageService } from "app/services/common/message.service";
import { FileUploader } from "ng2-file-upload";

@Component({
    selector: 'notes-pending-supply-approval',
    templateUrl: './notes-pending-supply-approval.html'
})

export class NotesPendingSupplyApproval implements OnInit{

    @Input() currentNote;
    productosServicios = [];
    analiticas = [
        {analytic: "Analítica 1", asigned: 10},
        {analytic: "Analítica 2", asigned: 30},
        {analytic: "Analítica 3", asigned: 5}
    ];
    providersGrid = [];
    show = false;
    fileSelected = false;
    providerSelected = null;

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
        private router: Router
    ) {}

    ngOnInit(): void {
        this.inicializar();
    }

    inicializar() {
        this.checkFormStatus()
        this.providerService.getAll().subscribe(d => {
            console.log(d.data)
            this.providersGrid = d.data;
            this.providersGrid = [...this.providersGrid]
        });
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
            //asignar analíticas
            this.show = true;
        })
    }

    checkFormStatus() {
        this.formNota.disable();
        this.formNota.controls.proveedores.enable();
        this.formNota.controls.observaciones.enable();
        this.formNota.controls.ordenCompra.enable()
    }

    reject() {
        //Si Rechaza, se cambia el estado de la nota de pedido a Rechazada y finaliza el workflow.
        this.messageService.showMessage("La nota de pedido ha sido rechazada", 0);
        this.router.navigate(['/providers/notes']);
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
        }
        if(this.providerSelected == null) {
            this.messageService.showMessage("Debe seleccionar un proveedor", 2);
            providerError = true;
        }
        if(this.formNota.invalid || providerError || fileError) {
            return;
        }
        this.messageService.showMessage("La nota de pedido ha sido aprobada", 0);
        this.router.navigate(['/providers/notes']);
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
}