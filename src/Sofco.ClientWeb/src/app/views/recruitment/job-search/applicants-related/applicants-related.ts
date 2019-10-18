import { Component, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { JobSearchService } from "app/services/recruitment/jobsearch.service";
import { DataTableService } from "app/services/common/datatable.service";
import { FormGroup, FormControl, Validators } from "@angular/forms";
import { FormsService } from "app/services/forms/forms.service";
import { MessageService } from "app/services/common/message.service";
import { ReasonCauseType } from "app/models/enums/reasonCauseType";

@Component({
    selector: 'job-search-applicants-related',
    templateUrl: './applicants-related.html',
    styleUrls: ['applicants-related.scss']
})
export class ApplicantsRelatedComponent implements OnDestroy {

    form: FormGroup = new FormGroup({
        comments: new FormControl(null, [Validators.required, Validators.maxLength(3000)]),
        reasonCauseId: new FormControl(null, [Validators.required]),
        documentNumber: new FormControl(null),
    });

    data: any[] = new Array();
    reasonOptions: any[] = new Array();
    jobSearchId: number;

    documentNumberVisible: boolean = false;

    getSubscrip: Subscription;
    addSubscrip: Subscription;

    constructor(private jobSearchService: JobSearchService,
                public formsService: FormsService,
                private messageService: MessageService,
                private dataTableService: DataTableService){
    }

    init(jobSearchId) {
        this.jobSearchId = jobSearchId;

        this.getSubscrip = this.jobSearchService.getApplicantsRelated(jobSearchId, 0).subscribe(response => {
            if(response && response.data && response.data.length > 0){
                this.data = response.data.map(item => {
                    item.selected = false;
                    return item;
                });
            }

            this.initGrid();
        }, 
        error => {});
    }    

    ngOnDestroy(): void {
        if (this.getSubscrip) this.getSubscrip.unsubscribe();
        if (this.addSubscrip) this.addSubscrip.unsubscribe();
    }

    setReasonOptions(options){
        this.reasonOptions = options.filter(x => x.type == ReasonCauseType.ApplicantInProgress || x.type == ReasonCauseType.ApplicantOpen || x.type == ReasonCauseType.ApplicantUnavailable);
    }

    initGrid() {
        var columns = [0, 1, 2, 3];

        var options = {
            selector: "#applicantsTable",
            columns: columns,
            columnDefs: [ { "aTargets": [3], "sType": "date-uk" }],
        };

        this.dataTableService.destroy(options.selector);
        this.dataTableService.initialize(options);
    }

    selectAll(){
        this.data.forEach((item, index) => {
            item.selected = true;
        });
    }

    unselectAll(){
        this.data.forEach((item, index) => {
            item.selected = false;
        });
    }

    save(){
        var ids = this.data.filter(item => {
            if(item.selected){
                return item;
            }

            return null;
        })
        .map(x => x.id);

        var json = {
            applicants: ids,
            reasonId: this.form.controls.reasonCauseId.value,
            comments: this.form.controls.comments.value,
            documentNumber: this.form.controls.documentNumber.value,
            jobSearchId: this.jobSearchId
        };

        this.messageService.showLoading();

        this.addSubscrip = this.jobSearchService.addContacts(json).subscribe(response => {
            this.messageService.closeLoading();
        }, 
        error => this.messageService.closeLoading());
    }

    reasonCauseChanged(){
        this.documentNumberVisible = false;
        this.form.controls.documentNumber.clearValidators();
        this.form.controls.documentNumber.setValue(null);

        var applicants = this.data.filter(x => x.selected);

        if(applicants.length == 1){
            var reasonCause = this.reasonOptions.find(x => x.id == this.form.controls.reasonCauseId.value);

            if(reasonCause && reasonCause.type == ReasonCauseType.ApplicantInProgress){
                this.form.controls.documentNumber.setValue(applicants[0].documentNumber);

                this.form.controls.documentNumber.setValidators([Validators.required, Validators.maxLength(10)]);
                this.form.controls.documentNumber.updateValueAndValidity();

                this.documentNumberVisible = true;
            }
        }
    }
}