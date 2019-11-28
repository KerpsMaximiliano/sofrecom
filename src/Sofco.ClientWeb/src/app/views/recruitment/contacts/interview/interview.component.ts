import { OnDestroy, Component } from "@angular/core";
import { FormGroup, FormControl, Validators } from "@angular/forms";
import { FormsService } from "app/services/forms/forms.service";
import { Subscription } from "rxjs";
import { JobSearchService } from "app/services/recruitment/jobsearch.service";
import { MessageService } from "app/services/common/message.service";

@Component({
    selector: 'interview',
    templateUrl: './interview.component.html',
})
export class InterviewComponent implements OnDestroy {

    userOptions: any[] = new Array();
    reasonOptions: any[] = new Array();

    isVisible: boolean = false;
    hasRrhhInterview: boolean = false;
    hasTechnicalInterview: boolean = false;
    hasClientInterview: boolean = false;

    jobSearchId: number;
    applicantId: number;

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

        reasonId: new FormControl(null, [Validators.required]),
    });

    addSubscrip: Subscription;

    constructor(public formsService: FormsService, 
                private messageService: MessageService,
                private jobSearchService: JobSearchService){
    }

    ngOnDestroy(): void { 
        if (this.addSubscrip) this.addSubscrip.unsubscribe();
    }

    canSave(){
        if(this.hasRrhhInterview || this.hasTechnicalInterview || this.hasClientInterview){
            return this.form.valid;
        }

        return false;
    }

    save(){
        var json = {
            reasonId: this.form.controls.reasonId.value,
            hasClientInterview: this.hasClientInterview,
            hasRrhhInterview: this.hasRrhhInterview,
            hasTechnicalInterview: this.hasTechnicalInterview,
            rrhhInterviewDate: this.form.controls.rrhhInterviewDate.value,
            rrhhInterviewPlace: this.form.controls.rrhhInterviewPlace.value,
            rrhhInterviewerId: this.form.controls.rrhhInterviewerId.value,
            technicalInterviewDate: this.form.controls.technicalInterviewDate.value,
            technicalInterviewPlace: this.form.controls.technicalInterviewPlace.value,
            technicalInterviewerId: this.form.controls.technicalInterviewerId.value,
            clientInterviewDate: this.form.controls.clientInterviewDate.value,
            clientInterviewPlace: this.form.controls.clientInterviewPlace.value,
            clientInterviewerId: this.form.controls.clientInterviewerId.value,
        };

        this.messageService.showLoading();

        this.addSubscrip = this.jobSearchService.addInterview(json, this.applicantId, this.jobSearchId).subscribe(response => {
            this.messageService.closeLoading();
        }, 
        error => {
            this.messageService.closeLoading();
        });
    }

    setData(history){
        this.applicantId = history.applicantId;
        this.jobSearchId = history.jobSearchId;
        this.isVisible = true;
        this.hasClientInterview = history.hasClientInterview;
        this.hasRrhhInterview = history.hasRrhhInterview;
        this.hasTechnicalInterview = history.hasTechnicalInterview;
        this.form.controls.reasonId.setValue(history.reasonId);
        this.form.controls.rrhhInterviewDate.setValue(history.rrhhInterviewDate);
        this.form.controls.rrhhInterviewPlace.setValue(history.rrhhInterviewPlace);
        this.form.controls.rrhhInterviewerId.setValue(history.rrhhInterviewerId);
        this.form.controls.technicalInterviewDate.setValue(history.technicalInterviewDate);
        this.form.controls.technicalInterviewPlace.setValue(history.technicalInterviewPlace);
        this.form.controls.technicalInterviewerId.setValue(history.technicalInterviewerId);
        this.form.controls.clientInterviewDate.setValue(history.clientInterviewDate);
        this.form.controls.clientInterviewPlace.setValue(history.clientInterviewPlace);
        this.form.controls.clientInterviewerId.setValue(history.clientInterviewerId);
    }

    clean(){
        this.applicantId = null;
        this.jobSearchId = null;
        this.isVisible = false;
        this.hasClientInterview = false;
        this.hasRrhhInterview = false;
        this.hasTechnicalInterview = false;
        this.form.controls.rrhhInterviewDate.setValue(null);
        this.form.controls.rrhhInterviewPlace.setValue(null);
        this.form.controls.rrhhInterviewerId.setValue(null);
        this.form.controls.technicalInterviewDate.setValue(null);
        this.form.controls.technicalInterviewPlace.setValue(null);
        this.form.controls.technicalInterviewerId.setValue(null);
        this.form.controls.clientInterviewDate.setValue(null);
        this.form.controls.clientInterviewPlace.setValue(null);
        this.form.controls.clientInterviewerId.setValue(null);
    }

    hasRrhhInterviewChanged(value){
        if(value){
            this.form.controls.rrhhInterviewDate.setValidators([Validators.required]);
            this.form.controls.rrhhInterviewDate.updateValueAndValidity();

            this.form.controls.rrhhInterviewPlace.setValidators([Validators.required, Validators.maxLength(100)]);
            this.form.controls.rrhhInterviewPlace.updateValueAndValidity();

            this.form.controls.rrhhInterviewerId.setValidators([Validators.required]);
            this.form.controls.rrhhInterviewerId.updateValueAndValidity();
        }
        else{
            this.form.controls.rrhhInterviewDate.clearValidators();
            this.form.controls.rrhhInterviewDate.setValue(null);

            this.form.controls.rrhhInterviewPlace.clearValidators();
            this.form.controls.rrhhInterviewPlace.setValue(null);

            this.form.controls.rrhhInterviewerId.clearValidators();
            this.form.controls.rrhhInterviewerId.setValue(null);
        }
    }

    hasTechnicalInterviewChanged(value){
        if(value){
            this.form.controls.technicalInterviewDate.setValidators([Validators.required]);
            this.form.controls.technicalInterviewDate.updateValueAndValidity();

            this.form.controls.technicalInterviewPlace.setValidators([Validators.required, Validators.maxLength(100)]);
            this.form.controls.technicalInterviewPlace.updateValueAndValidity();

            this.form.controls.technicalInterviewerId.setValidators([Validators.required]);
            this.form.controls.technicalInterviewerId.updateValueAndValidity();
        }
        else{
            this.form.controls.technicalInterviewDate.clearValidators();
            this.form.controls.technicalInterviewDate.setValue(null);

            this.form.controls.technicalInterviewPlace.clearValidators();
            this.form.controls.technicalInterviewPlace.setValue(null);

            this.form.controls.technicalInterviewerId.clearValidators();
            this.form.controls.technicalInterviewerId.setValue(null);
        }
    }

    hasClientInterviewChanged(value){
        if(value){
            this.form.controls.clientInterviewDate.setValidators([Validators.required]);
            this.form.controls.clientInterviewDate.updateValueAndValidity();

            this.form.controls.clientInterviewPlace.setValidators([Validators.required, Validators.maxLength(100)]);
            this.form.controls.clientInterviewPlace.updateValueAndValidity();

            this.form.controls.clientInterviewerId.setValidators([Validators.required]);
            this.form.controls.clientInterviewerId.updateValueAndValidity();
        }
        else{
            this.form.controls.clientInterviewDate.clearValidators();
            this.form.controls.clientInterviewDate.setValue(null);

            this.form.controls.clientInterviewPlace.clearValidators();
            this.form.controls.clientInterviewPlace.setValue(null);

            this.form.controls.clientInterviewerId.clearValidators();
            this.form.controls.clientInterviewerId.setValue(null);
        }
    }
}