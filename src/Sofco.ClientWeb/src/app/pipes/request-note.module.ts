import { NgModule } from "@angular/core";
import { RequestNoteStatePipe } from "./request-note-state.pipe";
import { RequestNoteDatePipe } from "./request-note-histories-date.pipe";

@NgModule({
    imports: [
    ],
    declarations: [ 
        RequestNoteStatePipe,
        RequestNoteDatePipe
    ],
    providers: [
    ],
    exports: [
        RequestNoteStatePipe,
        RequestNoteDatePipe
    ]
})
export class RequestNotePipeModule { }