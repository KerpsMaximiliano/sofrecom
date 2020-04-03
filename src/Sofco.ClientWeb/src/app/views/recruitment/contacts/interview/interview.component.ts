import { OnDestroy, Component, ViewChild, Output, EventEmitter, OnInit } from "@angular/core";
import { FormGroup, FormControl, Validators } from "@angular/forms";
import { FormsService } from "app/services/forms/forms.service";
import { Subscription } from "rxjs";
import { JobSearchService } from "app/services/recruitment/jobsearch.service";
import { MessageService } from "app/services/common/message.service";
import * as moment from 'moment';

@Component({
    selector: 'interview',
    templateUrl: './interview.component.html',
})
export class InterviewComponent implements OnInit, OnDestroy {
    @Output() setStatus: EventEmitter<any> = new EventEmitter();
    
    userOptions: any[] = new Array();
    reasonOptions: any[] = new Array();
    recruitersOptions: any[] = new Array();

    isVisible: boolean = false;
    hasRrhhInterview: boolean = false;
    hasTechnicalInterview: boolean = false;
    hasClientInterview: boolean = false;
    hasPhoneInterview: boolean = false;
    remoteWork: boolean = false;

    jobSearchId: number;
    applicantId: number;
    date: Date;

    history: any;

    form: FormGroup = new FormGroup({
        salary: new FormControl(null),
        phoneInterviewComments: new FormControl(null, [Validators.maxLength(1000)]),

        rrhhInterviewDate: new FormControl(null),
        rrhhInterviewPlace: new FormControl(null),
        rrhhInterviewerId: new FormControl(null),
        rrhhInterviewComments: new FormControl(null, [Validators.maxLength(1000)]),

        technicalInterviewDate: new FormControl(null),
        technicalInterviewPlace: new FormControl(null),
        technicalExternalInterviewer: new FormControl(null),
        technicalInterviewComments: new FormControl(null, [Validators.maxLength(1000)]),
 
        clientInterviewDate: new FormControl(null),
        clientInterviewPlace: new FormControl(null),
        clientExternalInterviewer: new FormControl(null),
        clientInterviewComments: new FormControl(null, [Validators.maxLength(1000)]),

        reasonId: new FormControl(null, [Validators.required]),
    });

    addSubscrip: Subscription;
    getRecruiterSubscrip: Subscription;

    constructor(public formsService: FormsService, 
                
                private messageService: MessageService,
                private jobSearchService: JobSearchService){
    }

    ngOnInit(): void {
        this.getRecruiters();
    }

    ngOnDestroy(): void { 
        if (this.addSubscrip) this.addSubscrip.unsubscribe();
        if (this.getRecruiterSubscrip) this.getRecruiterSubscrip.unsubscribe();
    }

    getRecruiters(){
        this.getRecruiterSubscrip = this.jobSearchService.getRecruiters().subscribe(response => {
            this.recruitersOptions = response.data;
        });
    }

    save(){
        var json = {
            salary: this.form.controls.salary.value,
            reasonId: this.form.controls.reasonId.value,
            hasClientInterview: this.hasClientInterview,
            hasRrhhInterview: this.hasRrhhInterview,
            hasTechnicalInterview: this.hasTechnicalInterview,
            hasPhoneInterview: this.hasPhoneInterview,
            remoteWork: this.remoteWork,

            rrhhInterviewDate: this.form.controls.rrhhInterviewDate.value,
            rrhhInterviewPlace: this.form.controls.rrhhInterviewPlace.value,
            rrhhInterviewerId: this.form.controls.rrhhInterviewerId.value,
            rrhhInterviewComments: this.form.controls.rrhhInterviewComments.value,

            phoneInterviewComments: this.form.controls.phoneInterviewComments.value,

            technicalInterviewDate: this.form.controls.technicalInterviewDate.value,
            technicalInterviewPlace: this.form.controls.technicalInterviewPlace.value,
            technicalExternalInterviewer: this.form.controls.technicalExternalInterviewer.value,
            technicalInterviewComments: this.form.controls.technicalInterviewComments.value,

            clientInterviewDate: this.form.controls.clientInterviewDate.value,
            clientInterviewPlace: this.form.controls.clientInterviewPlace.value,
            clientExternalInterviewer: this.form.controls.clientExternalInterviewer.value,
            clientInterviewComments: this.form.controls.clientInterviewComments.value,
        };
 
        this.messageService.showLoading();

        this.addSubscrip = this.jobSearchService.addInterview(json, this.applicantId, this.jobSearchId, this.date, this.history.reasonId).subscribe(response => {
            this.messageService.closeLoading();

            this.history.reasonId = this.form.controls.reasonId.value;
            var reason = this.reasonOptions.find(x => x.id == this.form.controls.reasonId.value);

            if(reason){
                this.history.reason = reason.text;
            }

            if(response.data > -1 && this.setStatus.observers.length > 0){
                this.setStatus.emit(response.data);
            }
        }, 
        error => {
            this.messageService.closeLoading();
        });
    }

    setData(history){
        this.form.controls.rrhhInterviewDate.setValue(null);
        this.form.controls.technicalInterviewDate.setValue(null);
        this.form.controls.clientInterviewDate.setValue(null);

        this.history = history;

        this.applicantId = history.applicantId;
        this.jobSearchId = history.jobSearchId;
        this.date = history.date;
        this.isVisible = true;

        this.hasClientInterview = history.hasClientInterview;
        this.hasRrhhInterview = history.hasRrhhInterview;
        this.hasTechnicalInterview = history.hasTechnicalInterview;
        this.hasPhoneInterview = history.hasPhoneInterview;
        this.remoteWork = history.remoteWork;

        this.form.controls.reasonId.setValue(history.reasonId);
        this.form.controls.salary.setValue(history.salary);

        this.hasRrhhInterviewChanged(this.hasRrhhInterview);
        this.hasPhoneInterviewChanged(this.hasPhoneInterview);

        // Phone
        this.form.controls.phoneInterviewComments.setValue(history.phoneInterviewComments);

        // RRHH
        if(history.rrhhInterviewDate){
            this.form.controls.rrhhInterviewDate.setValue(moment(history.rrhhInterviewDate).toDate());
        }
        if(history.rrhhInterviewerId){
            this.form.controls.rrhhInterviewerId.setValue(history.rrhhInterviewerId);
        }
        this.form.controls.rrhhInterviewPlace.setValue(history.rrhhInterviewPlace);
        this.form.controls.rrhhInterviewComments.setValue(history.rrhhInterviewComments);

        // Technical
        if(history.technicalInterviewDate){
            this.form.controls.technicalInterviewDate.setValue(moment(history.technicalInterviewDate).toDate());
        }
        this.form.controls.technicalInterviewPlace.setValue(history.technicalInterviewPlace);
        this.form.controls.technicalInterviewComments.setValue(history.technicalInterviewComments);
        this.form.controls.technicalExternalInterviewer.setValue(history.technicalExternalInterviewer);

        // Client
        if(history.clientInterviewDate){
            this.form.controls.clientInterviewDate.setValue(moment(history.clientInterviewDate).toDate());
        }
        this.form.controls.clientInterviewPlace.setValue(history.clientInterviewPlace);
        this.form.controls.clientInterviewComments.setValue(history.clientInterviewComments);
        this.form.controls.clientExternalInterviewer.setValue(history.clientExternalInterviewer);
    }

    clean(){
        this.applicantId = null;
        this.jobSearchId = null;
        this.isVisible = false;
        this.hasClientInterview = false;
        this.hasRrhhInterview = false;
        this.hasTechnicalInterview = false;
        this.remoteWork = false;

        this.form.controls.salary.setValue(null);

        this.form.controls.phoneInterviewComments.setValue(null);

        this.form.controls.rrhhInterviewDate.setValue(null);
        this.form.controls.rrhhInterviewPlace.setValue(null);
        this.form.controls.rrhhInterviewerId.setValue(null);
        this.form.controls.rrhhInterviewComments.setValue(null);

        this.form.controls.technicalInterviewDate.setValue(null);
        this.form.controls.technicalInterviewPlace.setValue(null);
        this.form.controls.technicalInterviewComments.setValue(null);
        this.form.controls.technicalExternalInterviewer.setValue(null);

        this.form.controls.clientInterviewDate.setValue(null);
        this.form.controls.clientInterviewPlace.setValue(null);
        this.form.controls.clientInterviewComments.setValue(null);
        this.form.controls.clientExternalInterviewer.setValue(null);
    }

    hasPhoneInterviewChanged(value){
        if(value){
            this.form.controls.salary.setValidators([Validators.required]);
            this.form.controls.salary.updateValueAndValidity();
        }
        else{
            this.form.controls.salary.clearValidators();

            this.form.controls.phoneInterviewComments.setValue(null);
        }
    }

    hasRrhhInterviewChanged(value){
        if(value){
            this.form.controls.salary.setValidators([Validators.required]);
            this.form.controls.salary.updateValueAndValidity();

            this.form.controls.rrhhInterviewDate.setValidators([Validators.required]);
            this.form.controls.rrhhInterviewDate.updateValueAndValidity();

            this.form.controls.rrhhInterviewPlace.setValidators([Validators.required, Validators.maxLength(100)]);
            this.form.controls.rrhhInterviewPlace.updateValueAndValidity();

            this.form.controls.rrhhInterviewerId.setValidators([Validators.required]);
            this.form.controls.rrhhInterviewerId.updateValueAndValidity();
        }
        else{
            this.form.controls.salary.clearValidators();
            
            this.form.controls.rrhhInterviewDate.clearValidators();
            this.form.controls.rrhhInterviewDate.setValue(null);

            this.form.controls.rrhhInterviewPlace.clearValidators();
            this.form.controls.rrhhInterviewPlace.setValue(null);

            this.form.controls.rrhhInterviewerId.clearValidators();
            this.form.controls.rrhhInterviewerId.setValue(null);

            this.form.controls.rrhhInterviewComments.setValue(null);
        }
    }

    hasTechnicalInterviewChanged(value){
        if(value){
            this.form.controls.technicalInterviewDate.setValidators([Validators.required]);
            this.form.controls.technicalInterviewDate.updateValueAndValidity();

            this.form.controls.technicalInterviewPlace.setValidators([Validators.required, Validators.maxLength(100)]);
            this.form.controls.technicalInterviewPlace.updateValueAndValidity();

            this.form.controls.technicalExternalInterviewer.setValidators([Validators.required, Validators.maxLength(100)]);
            this.form.controls.technicalExternalInterviewer.updateValueAndValidity();
        }
        else{
            this.form.controls.technicalInterviewDate.clearValidators();
            this.form.controls.technicalInterviewDate.setValue(null);

            this.form.controls.technicalInterviewPlace.clearValidators();
            this.form.controls.technicalInterviewPlace.setValue(null);

            this.form.controls.technicalInterviewComments.setValue(null);

            this.form.controls.technicalExternalInterviewer.clearValidators();
            this.form.controls.technicalExternalInterviewer.setValue(null);
        }
    }

    hasClientInterviewChanged(value){
        if(value){
            this.form.controls.clientInterviewDate.setValidators([Validators.required]);
            this.form.controls.clientInterviewDate.updateValueAndValidity();

            this.form.controls.clientInterviewPlace.setValidators([Validators.required, Validators.maxLength(100)]);
            this.form.controls.clientInterviewPlace.updateValueAndValidity();

            this.form.controls.clientExternalInterviewer.setValidators([Validators.required, Validators.maxLength(100)]);
            this.form.controls.clientExternalInterviewer.updateValueAndValidity();
        }
        else{
            this.form.controls.clientInterviewDate.clearValidators();
            this.form.controls.clientInterviewDate.setValue(null);

            this.form.controls.clientInterviewPlace.clearValidators();
            this.form.controls.clientInterviewPlace.setValue(null);

            this.form.controls.clientInterviewComments.setValue(null);

            this.form.controls.clientExternalInterviewer.clearValidators();
            this.form.controls.clientExternalInterviewer.setValue(null);
        }
    }

    canSave(){
        if(this.form.valid && (this.hasPhoneInterview || this.hasRrhhInterview || this.hasTechnicalInterview || this.hasClientInterview)) return true;

        return false;
    }
}