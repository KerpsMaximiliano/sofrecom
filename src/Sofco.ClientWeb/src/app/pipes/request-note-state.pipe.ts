import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: 'requestNoteState',
    pure: true
})

export class RequestNoteStatePipe implements PipeTransform {
    private states = [
        { id: 0, text: "Todos" },
        { id: 8, text: "Borrador" },
        { id: 29, text: "Pendiente Aprobación Gerentes Analítica" },
        { id: 31, text: "Pendiente Aprobación Compras" },
        { id: 4, text: "Pendiente Aprobación DAF" },
        { id: 33, text: "Pendiente Generación SAP" },
        { id: 34, text: "Pendiente Recepción Mercadería" },
        { id: 35, text: "Recepción Parcial" },
        { id: 36, text: "Cerrada" },
        { id: 30, text: "Rechazada" }
    ];

    transform(stateId: number): string{
        let find = this.states.find(state => state.id == stateId);
        if(find != undefined) {
            return find.text;
        } else {
            return "Sin estado asociado"
        }
    }
}