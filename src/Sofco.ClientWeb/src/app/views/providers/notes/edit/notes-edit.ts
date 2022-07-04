import { AfterContentChecked, AfterContentInit, AfterViewChecked, AfterViewInit, ChangeDetectorRef, Component, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { forkJoin } from "rxjs";


@Component({
    selector: 'notes-edit',
    templateUrl: './notes-edit.html'
})

export class NotesEditComponent implements OnInit{

    states = [
        "", 
        "Borrador",
        "Pendiente Revisión Abastecimiento",
        "Pendiente Aprobación Gerentes Analítica",
        "Pendiente Aprobación Abastecimiento",
        "Pendiente Aprobación DAF",
        "Aprobada",
        "Solicitada a Proveedor",
        "Recibido Conforme",
        "Factura Pendiente Aprobación Gerente",
        "Pendiente Procesar GAF",
        "Rechazada",
        "Cerrada"
    ]

    estado: number;
    estadoSeleccionado: number;
    historyComments: string;
    histories = [
    ];
    currentNote;
    currentNoteStatusDescription;
    showEdit = false;

    @ViewChild('commentsModal') commentsModal;

    public commentsModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "comments",
        "commentsModal",
        false,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.close" 
    );

    constructor(
        private requestNoteService: RequestNoteService,
        private route: ActivatedRoute
    ){}

    ngOnInit(): void {
        forkJoin([this.requestNoteService.getById(this.route.snapshot.params['id']), this.requestNoteService.getHistories(this.route.snapshot.params['id'])]).subscribe(results => {
            //results[0] - RN
            //results[1] - Histories
            this.currentNote = results[0].data;
            this.estado = this.currentNote.statusId;
            if(this.estado > 10) {
                this.requestNoteService.setMode("View");
                this.currentNoteStatusDescription = (this.estado == 11) ? "Rechazada" : "Cerrada"
                this.estadoSeleccionado = results[1][results[1].length - 1].statusFromId
            } else {
                this.estadoSeleccionado = this.estado;
                this.currentNoteStatusDescription = null;
            }
            this.showEdit = true;
            results[1].forEach(history => {
                let model = {
                    createdDate: history.createdDate,
                    userName: history.userName,
                    statusFrom: this.states[history.statusFromId],
                    statusTo: this.states[history.statusToId],
                    comment: history.comment
                }
                this.histories.push(model);
                this.histories = [...this.histories]
            });
        })
    }

    setEstado() {
        this.estadoSeleccionado = this.estado;
    }

    showComments(history) {
        this.historyComments = history.comment;
        this.commentsModal.show();
    }
    
}