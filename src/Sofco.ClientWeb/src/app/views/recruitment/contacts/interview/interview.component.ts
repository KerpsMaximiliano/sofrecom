import { OnDestroy, Component, ViewChild, Output, EventEmitter } from "@angular/core";
import { FormGroup, FormControl, Validators } from "@angular/forms";
import { FormsService } from "app/services/forms/forms.service";
import { Subscription } from "rxjs";
import { JobSearchService } from "app/services/recruitment/jobsearch.service";
import { MessageService } from "app/services/common/message.service";
import * as moment from 'moment';
import { AuthService } from "app/services/common/auth.service";
import { FileUploader } from "ng2-file-upload";
import { Cookie } from "ng2-cookies/ng2-cookies";

@Component({
    selector: 'interview',
    templateUrl: './interview.component.html',
})
export class InterviewComponent implements OnDestroy {

    @ViewChild('selectedFile') selectedFile: any;
    public uploader: FileUploader = new FileUploader({url:""});
    @Output() getFiles: EventEmitter<any> = new EventEmitter();
    
    userOptions: any[] = new Array();
    reasonOptions: any[] = new Array();

    isVisible: boolean = false;
    hasRrhhInterview: boolean = false;
    hasTechnicalInterview: boolean = false;
    hasClientInterview: boolean = false;

    isTechnicalExternal: boolean = false;
    isClientExternal: boolean = false;

    jobSearchId: number;
    applicantId: number;
    date: Date;

    history: any;

    form: FormGroup = new FormGroup({
        rrhhInterviewDate: new FormControl(null),
        rrhhInterviewPlace: new FormControl(null),
        rrhhInterviewerId: new FormControl(null),
        rrhhInterviewComments: new FormControl(null, [Validators.maxLength(1000)]),

        technicalInterviewDate: new FormControl(null),
        technicalInterviewPlace: new FormControl(null),
        technicalInterviewerId: new FormControl(null),
        technicalExternalInterviewer: new FormControl(null),
        technicalInterviewComments: new FormControl(null, [Validators.maxLength(1000)]),
 
        clientInterviewDate: new FormControl(null),
        clientInterviewPlace: new FormControl(null),
        clientInterviewerId: new FormControl(null),
        clientExternalInterviewer: new FormControl(null),
        clientInterviewComments: new FormControl(null, [Validators.maxLength(1000)]),

        reasonId: new FormControl(null, [Validators.required]),
    });

    addSubscrip: Subscription;

    constructor(public formsService: FormsService, 
                private authService: AuthService,
                private messageService: MessageService,
                private jobSearchService: JobSearchService){
    }

    ngOnDestroy(): void { 
        if (this.addSubscrip) this.addSubscrip.unsubscribe();
    }

    save(){
        var json = {
            reasonId: this.form.controls.reasonId.value,
            hasClientInterview: this.hasClientInterview,
            hasRrhhInterview: this.hasRrhhInterview,
            hasTechnicalInterview: this.hasTechnicalInterview,
            isTechnicalExternal: this.isTechnicalExternal,
            isClientExternal: this.isClientExternal,

            rrhhInterviewDate: this.form.controls.rrhhInterviewDate.value,
            rrhhInterviewPlace: this.form.controls.rrhhInterviewPlace.value,
            rrhhInterviewerId: this.form.controls.rrhhInterviewerId.value,
            rrhhInterviewComments: this.form.controls.rrhhInterviewComments.value,

            technicalInterviewDate: this.form.controls.technicalInterviewDate.value,
            technicalInterviewPlace: this.form.controls.technicalInterviewPlace.value,
            technicalInterviewerId: this.form.controls.technicalInterviewerId.value,
            technicalExternalInterviewer: this.form.controls.technicalExternalInterviewer.value,
            technicalInterviewComments: this.form.controls.technicalInterviewComments.value,

            clientInterviewDate: this.form.controls.clientInterviewDate.value,
            clientInterviewPlace: this.form.controls.clientInterviewPlace.value,
            clientInterviewerId: this.form.controls.clientInterviewerId.value,
            clientExternalInterviewer: this.form.controls.clientExternalInterviewer.value,
            clientInterviewComments: this.form.controls.clientInterviewComments.value,
        };
 
        this.messageService.showLoading();

        this.addSubscrip = this.jobSearchService.addInterview(json, this.applicantId, this.jobSearchId, this.date).subscribe(response => {
            this.messageService.closeLoading();

            this.history.reasonId = this.form.controls.reasonId.value;
            var reason = this.reasonOptions.find(x => x.id == this.form.controls.reasonId.value);

            if(reason){
                this.history.reason = reason.text;
            }
        }, 
        error => {
            this.messageService.closeLoading();
        });
    }

    setData(history){
        this.history = history;

        this.applicantId = history.applicantId;
        this.jobSearchId = history.jobSearchId;
        this.date = history.date;
        this.isVisible = true;

        this.uploaderConfig();

        this.hasClientInterview = history.hasClientInterview;
        this.hasRrhhInterview = history.hasRrhhInterview;
        this.hasTechnicalInterview = history.hasTechnicalInterview;
        this.isTechnicalExternal = history.isTechnicalExternal;
        this.isClientExternal = history.isClientExternal;

        this.form.controls.reasonId.setValue(history.reasonId);

        // RRHH
        if(history.rrhhInterviewDate){
            this.form.controls.rrhhInterviewDate.setValue(moment(history.rrhhInterviewDate).toDate());
        }
        if(history.rrhhInterviewerId){
            this.form.controls.rrhhInterviewerId.setValue(history.rrhhInterviewerId.toString());
        }
        this.form.controls.rrhhInterviewPlace.setValue(history.rrhhInterviewPlace);
        this.form.controls.rrhhInterviewComments.setValue(history.rrhhInterviewComments);

        // Technical
        if(history.technicalInterviewDate){
            this.form.controls.technicalInterviewDate.setValue(moment(history.technicalInterviewDate).toDate());
        }
        if(history.technicalInterviewerId){
            this.form.controls.technicalInterviewerId.setValue(history.technicalInterviewerId.toString());
        }
        this.form.controls.technicalInterviewPlace.setValue(history.technicalInterviewPlace);
        this.form.controls.technicalInterviewComments.setValue(history.technicalInterviewComments);
        this.form.controls.technicalExternalInterviewer.setValue(history.technicalExternalInterviewer);

        // Client
        if(history.clientInterviewerId){
            this.form.controls.clientInterviewerId.setValue(history.clientInterviewerId.toString());
        }
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
        this.isTechnicalExternal = false;
        this.isClientExternal = false;

        this.form.controls.rrhhInterviewDate.setValue(null);
        this.form.controls.rrhhInterviewPlace.setValue(null);
        this.form.controls.rrhhInterviewerId.setValue(null);
        this.form.controls.rrhhInterviewComments.setValue(null);

        this.form.controls.technicalInterviewDate.setValue(null);
        this.form.controls.technicalInterviewPlace.setValue(null);
        this.form.controls.technicalInterviewerId.setValue(null);
        this.form.controls.technicalInterviewComments.setValue(null);
        this.form.controls.technicalExternalInterviewer.setValue(null);

        this.form.controls.clientInterviewDate.setValue(null);
        this.form.controls.clientInterviewPlace.setValue(null);
        this.form.controls.clientInterviewerId.setValue(null);
        this.form.controls.clientInterviewComments.setValue(null);
        this.form.controls.clientExternalInterviewer.setValue(null);
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

            this.form.controls.rrhhInterviewComments.setValue(null);
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

            this.form.controls.technicalInterviewComments.setValue(null);

            this.form.controls.technicalExternalInterviewer.clearValidators();
            this.form.controls.technicalExternalInterviewer.setValue(null);

            this.isTechnicalExternal = false;
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

            this.form.controls.clientInterviewComments.setValue(null);

            this.form.controls.clientExternalInterviewer.clearValidators();
            this.form.controls.clientExternalInterviewer.setValue(null);

            this.isClientExternal = false;
        }
    }

    isTechnicalExternalChanged(value){
        if(value){
            this.form.controls.technicalExternalInterviewer.setValidators([Validators.required, Validators.maxLength(100)]);
            this.form.controls.technicalExternalInterviewer.updateValueAndValidity();

            this.form.controls.technicalInterviewerId.clearValidators();
            this.form.controls.technicalInterviewerId.setValue(null);
        }
        else{
            this.form.controls.technicalExternalInterviewer.clearValidators();
            this.form.controls.technicalExternalInterviewer.setValue(null);

            this.form.controls.technicalInterviewerId.setValidators([Validators.required]);
            this.form.controls.technicalInterviewerId.updateValueAndValidity();
        }
    }

    isClientExternalChanged(value){
        if(value){
            this.form.controls.clientExternalInterviewer.setValidators([Validators.required, Validators.maxLength(100)]);
            this.form.controls.clientExternalInterviewer.updateValueAndValidity();

            this.form.controls.clientInterviewerId.clearValidators();
            this.form.controls.clientInterviewerId.setValue(null);
        }
        else{
            this.form.controls.clientExternalInterviewer.clearValidators();
            this.form.controls.clientExternalInterviewer.setValue(null);

            this.form.controls.clientInterviewerId.setValidators([Validators.required]);
            this.form.controls.clientInterviewerId.updateValueAndValidity();
        }
    }

    uploaderConfig(){
        this.uploader = new FileUploader({url: this.jobSearchService.getUrlForImportExcel(this.applicantId, this.jobSearchId, this.date),
                                          authToken: 'Bearer ' + Cookie.get('access_token') ,
                                          maxFileSize: 50*1024*1024
                                        });

        this.uploader.onCompleteItem = (item:any, response:any, status:any, headers:any) => {
            if(status == 401){
                this.authService.refreshToken().subscribe(token => {
                    this.messageService.closeLoading();

                    if(token){
                        this.clearSelectedFile();
                        this.messageService.showErrorByFolder('common', 'fileMustReupload');
                        this.uploaderConfig();
                    }
                });

                return;
            }

            this.messageService.closeLoading();

            var dataJson = JSON.parse(response);
            
            if(dataJson){
                if(dataJson.messages) this.messageService.showMessages(dataJson.messages);
                
                if (this.getFiles.observers.length > 0) {
                    this.getFiles.emit();
                }
            }

            this.clearSelectedFile();
        };

        this.uploader.onAfterAddingFile = (file)=> { file.withCredentials = false; };
    }

    clearSelectedFile(){
        if(this.uploader.queue.length > 0){
            this.uploader.queue[0].remove();
        }
  
        this.selectedFile.nativeElement.value = '';
    }
}