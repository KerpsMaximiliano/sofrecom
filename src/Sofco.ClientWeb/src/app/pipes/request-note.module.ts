import { NgModule } from "@angular/core";
import { RequestNoteStatePipe } from "./request-note-state.pipe";
import { RequestNoteDatePipe } from "./request-note-histories-date.pipe";
import { RequestNoteUnitPipe } from "./request-note-unit.pipe";
import { RequestNoteCurrencyPipe } from "./request-note-currency.pipe";

@NgModule({
    imports: [
    ],
    declarations: [ 
        RequestNoteStatePipe,
        RequestNoteDatePipe,
        RequestNoteUnitPipe,
        RequestNoteCurrencyPipe
    ],
    providers: [
    ],
    exports: [
        RequestNoteStatePipe,
        RequestNoteDatePipe,
        RequestNoteUnitPipe,
        RequestNoteCurrencyPipe
    ]
})
export class RequestNotePipeModule { }