import { AfterContentChecked, AfterContentInit, AfterViewChecked, AfterViewInit, ChangeDetectorRef, Component, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { RequestNoteService } from "app/services/admin/request-note.service";


@Component({
    selector: 'notes-edit',
    templateUrl: './notes-edit.html'
})

/*
    BOTONES:
        1 - Rechazar / Enviar
        2 - Aprobar / Rechazar
        3 - Aprobar / Rechazar
        4 - Aprobar / Rechazar(con modal)
        5 - Solicitar
        6 - Cerrar(con modal)
        7 - Adjuntar Factura
        8 - Aprobar
        9 - Cerrar

    ESTADOS:
        1 - Borrador
        2 - Pendiente Revisión Abastecimiento
        3 - Pendiente Aprobación Gerentes Analítica
        4 - Pendiente Aprobación Abastecimiento
        5 - Pendiente Aprobación DAF
        6 - Aprobada
        7 - Solicitada a Proveedor
        8 - Recibido Conforme
        9 - Factura Pendiente Aprobación Gerente
        10 - Pendiente Procesar GAF
        11 - Cerrada
        12 - Rechazada
*/

export class NotesEditComponent implements OnInit{

    estado: number;
    estadoSeleccionado: number;
    historyComments: string;
    histories = [
        {
            createdDate: "02-2022",
            userName: "usuario 1",
            statusFrom: "Borrador",
            statusTo: "Pendiente Revisión Abastecimiento",
            comment: null
        },
        {
            createdDate: "02-2022",
            userName: "usuario 2",
            statusFrom: "Pendiente Revisión Abastecimiento",
            statusTo: "Pendiente Aprobación Gerentes Analítica",
            comment: null
        },
        {
            createdDate: "02-2022",
            userName: "usuario 3",
            statusFrom: "Pendiente Aprobación Gerentes Analítica",
            statusTo: "Pendiente Aprobación Abastecimiento",
            comment: null
        },
        {
            createdDate: "02-2022",
            userName: "usuario 4",
            statusFrom: "Pendiente Aprobación Abastecimiento",
            statusTo: "Pendiente Aprobación DAF",
            comment: null
        },
        {
            createdDate: "02-2022",
            userName: "usuario 5",
            statusFrom: "Pendiente Aprobación DAF",
            statusTo: "Rechazada",
            comment: "comentario razón rechazo"
        }
    ];
    currentNote;
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
        this.requestNoteService.getById(this.route.snapshot.params['id']).subscribe(d => {
            console.log(d.data);
            this.currentNote = d.data;
            this.estado = this.currentNote.statusId;
            this.estadoSeleccionado = this.estado;
            this.showEdit = true;
        });
        //Histories
        this.requestNoteService.getHistories(this.route.snapshot.params['id']).subscribe(d => {
            console.log(d)
        })
    }

    setEstado() {
        this.estadoSeleccionado = this.estado;
    }

    showComments(history) {
        //on click en entrada de tabla mostrar modal con el comentario asociado, si es que lo hay
        this.historyComments = history.comment;
        this.commentsModal.show();
    }

    getHistories() {
        //request get traer histories de nota pedido asociada
    }
    
}