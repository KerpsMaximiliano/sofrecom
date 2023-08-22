import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: 'requestNoteUnit',
    pure: true
})

export class RequestNoteUnitPipe implements PipeTransform {
    private units: Array<{id: number, description: string}> = [
        {id: 1, description: "Total"},
        {id: 2, description: "Hora"},
        {id: 3, description: "Diario"},
        {id: 4, description: "Mensual"},
        {id: 5, description: "Anual"}
    ];

    transform(unitId: number): string{
        let find = this.units.find(unit => unit.id == unitId);
        if(find != undefined) {
            return find.description;
        } else {
            return ""
        }
    }
}