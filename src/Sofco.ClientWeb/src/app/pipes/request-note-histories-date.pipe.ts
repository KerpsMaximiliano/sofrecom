import { Pipe, PipeTransform } from "@angular/core";
import { environment } from "environments/environment";

@Pipe({
    name: 'requestNoteDatePipe',
    pure: true
})

export class RequestNoteDatePipe implements PipeTransform {
    private states = environment.REQUEST_NOTE_STATES;

    transform(date: string): string{
        let dateArray = date.split('-');
        return `${dateArray[2]}-${dateArray[1]}-${dateArray[0]}`
    }
}