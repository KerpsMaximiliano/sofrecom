import { Component, OnInit, ViewChild } from "@angular/core";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";


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
*/

export class NotesEditComponent{

    estado: number;
    estadoSeleccionado: number = 1;
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

    @ViewChild('commentsModal') commentsModal;

    public commentsModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "comments",
        "commentsModal",
        false,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.close" 
    );

    constructor(){}

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