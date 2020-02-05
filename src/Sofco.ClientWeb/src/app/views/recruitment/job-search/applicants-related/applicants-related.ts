import { Component, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { JobSearchService } from "app/services/recruitment/jobsearch.service";
import { DataTableService } from "app/services/common/datatable.service";
import { FormGroup, FormControl, Validators } from "@angular/forms";
import { FormsService } from "app/services/forms/forms.service";
import { MessageService } from "app/services/common/message.service";
import { ReasonCauseType } from "app/models/enums/reasonCauseType";
import * as moment from "moment";

@Component({
    selector: 'job-search-applicants-related',
    templateUrl: './applicants-related.html',
    styleUrls: ['applicants-related.scss']
})
export class ApplicantsRelatedComponent implements OnDestroy {

    form: FormGroup = new FormGroup({
        comments: new FormControl(null, [Validators.required, Validators.maxLength(3000)]),
        reasonCauseId: new FormControl(null, [Validators.required]),
    });

    data: any[] = new Array();
    dataFiltered: any[] = new Array();
    reasonOptions: any[] = new Array();
    skills: any[] = new Array();
    profiles: any[] = new Array();
    jobSearchId: number;
    skillSelected: number;
    profileSelected: number;

    getSubscrip: Subscription;
    addSubscrip: Subscription;

    constructor(private jobSearchService: JobSearchService,
                public formsService: FormsService,
                private messageService: MessageService,
                private dataTableService: DataTableService){
    }

    init(jobSearchId) {
        this.jobSearchId = jobSearchId;
        this.data = new Array();
        this.dataFiltered = new Array();
        this.dataTableService.destroy("#applicantsTable");

        this.getSubscrip = this.jobSearchService.getApplicantsRelated(jobSearchId, 0).subscribe(response => {
            if(response && response.data && response.data.length > 0){
                this.data = response.data.map(item => {
                    item.selected = false;
                    return item;
                });

                this.dataFiltered = this.data;
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
        this.reasonOptions = options.filter(x => x.type == ReasonCauseType.ApplicantInProgress || x.type == ReasonCauseType.ApplicantOpen || x.type == ReasonCauseType.ApplicantContacted);
    }

    initGrid() {
        var options = {
            selector: "#applicantsTable",
            columnDefs: [ { "aTargets": [3], "sType": "date-uk" }],
        };

        setTimeout(() => {
            $("#applicantsTable_wrapper").css("float","left");
            $("#applicantsTable_wrapper").css("padding-bottom","50px");
            $("#applicantsTable_filter label").addClass('search-filter');
            $("#applicantsTable_paginate").addClass('table-pagination');
            $("#applicantsTable_length").css("margin-right","10px");
            $("#applicantsTable_info").css("padding-top","4px");
        }, 500);

        this.dataTableService.destroy(options.selector);
        this.dataTableService.initialize(options);
    }

    selectAll(){
        this.dataFiltered.forEach((item, index) => {
            item.selected = true;
        });
    }

    anySelected(){
        return this.dataFiltered.filter(x => x.selected).length > 0;
    }

    unselectAll(){
        this.dataFiltered.forEach((item, index) => {
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
            jobSearchs: [this.jobSearchId]
        };

        this.messageService.showLoading();

        this.addSubscrip = this.jobSearchService.addContacts(json).subscribe(response => {
            this.messageService.closeLoading();

            this.dataFiltered.forEach(x => {
                if(x.selected){
                    x.date = moment().toDate();

                    var itemData = this.data.find(s => s.id == s.id == x.id);
                    if(itemData){
                        itemData.date = moment().toDate();
                    }
                }
            });

            this.unselectAll();
            this.form.controls.reasonCauseId.setValue(null);
            this.form.controls.comments.setValue(null);
            this.form.reset();
        }, 
        error => this.messageService.closeLoading());
    }

    cancel(){
        this.form.controls.reasonCauseId.setValue(null);
        this.form.controls.comments.setValue(null);
        this.form.reset();
    }

    goToDetail(applicant){
        window.open("/#/recruitment/contacts/" + applicant.id, "blank");
    }

    isInCompanyReason(){
        var reason = this.reasonOptions.find(x => x.id == this.form.controls.reasonCauseId.value);

        if(reason){
            if(reason.type == ReasonCauseType.ApplicantInProgress && this.dataFiltered.filter(x => x.selected).length > 1){
                return true;
            }
        }

        return false;
    }

    filtersChange(){
        this.dataFiltered = [];

        this.data.forEach(item => {
            var mustAdd = true;

            if(this.skillSelected > 0){
                var skillSelected = this.skills.find(x => x.id == this.skillSelected);

                if(!skillSelected || !item.skills.includes(skillSelected.text)){
                    mustAdd = false;
                }
            }

            if(this.profileSelected > 0){
                var profileSelected = this.profiles.find(x => x.id == this.profileSelected);

                if(!profileSelected || !item.profiles.includes(profileSelected.text)){
                    mustAdd = false;
                }
            }

            if(mustAdd){
                this.dataFiltered.push(item);
            }
        });

        this.initGrid();
    }
}