import { OnDestroy, Component } from "@angular/core";
import { FormGroup, FormControl, Validators } from "@angular/forms";
import { FormsService } from "app/services/forms/forms.service";

@Component({
    selector: 'interview',
    templateUrl: './interview.component.html',
})
export class InterviewComponent implements OnDestroy {

    isVisible: boolean = false;
    hasRrhhInterview: boolean = false;
    hasTechnicalInterview: boolean = false;
    hasClientInterview: boolean = false;

    form: FormGroup = new FormGroup({
        rrhhInterviewDate: new FormControl(null),
        rrhhInterviewPlace: new FormControl(null,),
        rrhhInterviewerId: new FormControl(null),

        technicalInterviewDate: new FormControl(null),
        technicalInterviewPlace: new FormControl(null,),
        technicalInterviewerId: new FormControl(null),

        clientInterviewDate: new FormControl(null),
        clientInterviewPlace: new FormControl(null,),
        clientInterviewerId: new FormControl(null),
    });

    constructor(public formsService: FormsService){
    }

    ngOnDestroy(): void {
        
    }

    hasRrhhInterviewChanged(value){

    }

    hasTechnicalInterviewChanged(value){

    }
}