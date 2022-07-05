import { Component, Input } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { Router } from "@angular/router";
import { ProvidersService } from "app/services/admin/providers.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { MessageService } from "app/services/common/message.service";

@Component({
    selector: 'notes-pending-management-bill-approval',
    templateUrl: './notes-pending-management-bill-approval.html'
})

export class NotesPendingManagementBillApproval {
    @Input() currentNote;
    @Input() currentNoteStatusDescription;
    mode;
    productosServicios = [];
    analiticas = [];
    providersGrid = [];
    providerDocFiles = [];
    RCFiles = [];
    BillFiles = [];

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
        ordenCompra: new FormControl(null),
        documentacionProveedor: new FormControl(null),
        documentacionRecibidoConforme: new FormControl(null),
        facturas: new FormControl(null)
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
        this.mode = this.requestNoteService.getMode();
        this.providersAreaService.get(this.currentNote.providerAreaId).subscribe(d => {
            console.log(d);
            this.formNota.patchValue({
                descripcion: this.currentNote.description,
                rubro: d.data.description,
                critico: (d.data.critical) ? "Si" : "No",
                requierePersonal: this.currentNote.requiresEmployeeClient,
                previstoPresupuesto: this.currentNote.consideredInBudget,
                nroEvalprop: this.currentNote.evalpropNumber,
                observaciones: this.currentNote.comments,
                montoOC: this.currentNote.purchaseOrderAmmount,
                ordenCompra: this.currentNote.purchaseOrderNumber
            });
            this.analiticas = this.currentNote.analytics;
            this.productosServicios = this.currentNote.productsServices;
            this.providersGrid = this.currentNote.providers;
        })
        this.checkFormStatus();
        this.currentNote.attachments.forEach(att => {
            if(att.type == 3) {
                this.providerDocFiles.push(att);
                this.providerDocFiles = [...this.providerDocFiles];
            };
            if(att.type == 4) {
                this.RCFiles.push(att);
                this.RCFiles = [...this.RCFiles];
            };
            if(att.type == 5) {
                this.BillFiles.push(att);
                this.BillFiles = [...this.BillFiles];
            }
        });
    }

    checkFormStatus() {
        this.formNota.disable();
    }

    downloadOC() {
        let files = this.currentNote.attachments.find(file => file.type == 2);
        this.requestNoteService.downloadFile(files.fileId, 5, files.fileDescription);
    }

    downloadProviderDoc(item) {
        this.requestNoteService.downloadFile(item.fileId, 5, item.fileDescription);
    }

    downloadRC(item) {
        this.requestNoteService.downloadFile(item.fileId, 5, item.fileDescription);
    }

    downloadBills(item) {
        this.requestNoteService.downloadFile(item.fileId, 5, item.fileDescription);
    }

    approve() {
        let model = {
            id: this.currentNote.id,
            comments: this.formNota.controls.observaciones.value
        };
        this.requestNoteService.approvePendingManagementBillApproval(model).subscribe(d => {
            console.log(d);
            this.messageService.showMessage("El estado de sus analíticas asociadas ha sido cambiado", 0);
            this.router.navigate(['/providers/notes']);
        })
    }
}