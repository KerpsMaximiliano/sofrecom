import { Component, OnDestroy } from "@angular/core";

@Component({
    selector: 'interview',
    templateUrl: './interview.component.html',
})
export class ContactFileComponent implements OnDestroy {   


    
    ngOnDestroy(): void {
        throw new Error("Method not implemented.");
    }

}