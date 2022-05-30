import { Component, OnInit } from "@angular/core";


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

    constructor(){}

    setEstado() {
        this.estadoSeleccionado = this.estado;
    }
    
}