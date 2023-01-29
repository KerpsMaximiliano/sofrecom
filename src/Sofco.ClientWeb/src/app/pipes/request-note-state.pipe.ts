import { Pipe, PipeTransform } from "@angular/core";
import { environment } from "environments/environment";

@Pipe({
    name: 'requestNoteState',
    pure: true
})

export class RequestNoteStatePipe implements PipeTransform {
    private states = environment.REQUEST_NOTE_STATES;

    transform(stateId: number): string{
        let find = this.states.find(state => state.id == stateId);
        if(find != undefined) {
            return find.text;
        } else {
            return "Sin estado asociado"
        }
    }
}