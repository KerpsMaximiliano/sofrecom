import { Component, OnInit, OnDestroy } from "@angular/core";
import { FormControl, Validators, FormGroup } from "@angular/forms";
import { Subscription } from "rxjs";
import { GenericOptionService } from "app/services/admin/generic-option.service";
import { CustomerService } from "app/services/billing/customer.service";
import { JobSearchService } from "app/services/recruitment/jobsearch.service";
import { FormsService } from "app/services/forms/forms.service";
import { MessageService } from "app/services/common/message.service";
import { Router } from "@angular/router";
import { GenericOptions } from "app/models/enums/genericOptions";

@Component({
    selector: 'job-search',
    templateUrl: './job-search-add.html',
    styleUrls: ['job-search-add.scss']
})
export class JobSearchComponent implements OnInit, OnDestroy {

    form: FormGroup = new FormGroup({
        comments: new FormControl(null, [Validators.maxLength(3000)]),
        timeHiringId: new FormControl(null, [Validators.required]),
        userId: new FormControl(null, [Validators.required]),
        reasonCauseId: new FormControl(null, [Validators.required]),
        clientId: new FormControl(null, [Validators.required]),
        quantity: new FormControl(null, [Validators.required, Validators.min(1)]),
        yearsExperience: new FormControl(null, [Validators.max(99), Validators.min(0)]),
        maximunSalary: new FormControl(null, [Validators.required]),
        recruiterId: new FormControl(null, [Validators.required]),
        profiles: new FormControl(null),
        skillsNotRequired: new FormControl(null),
        skillsRequired: new FormControl(null),
        seniorities: new FormControl(null),
        email: new FormControl(null),
        area: new FormControl(null, [Validators.maxLength(100)]),
        telephone: new FormControl(null),
        clientContact: new FormControl(null),
        jobType: new FormControl('1'),
        resourceAssignment: new FormControl('1'),
        language: new FormControl(null, [Validators.maxLength(100)]),
        study: new FormControl(null, [Validators.maxLength(100)]),
        jobTime: new FormControl(null, [Validators.maxLength(100)]),
        location: new FormControl(null, [Validators.maxLength(200)]),
        benefits: new FormControl(null, [Validators.maxLength(3000)]),
        observations: new FormControl(null, [Validators.maxLength(3000)]),
        tasksToDo: new FormControl(null, [Validators.maxLength(3000)]),
    });

    hasExtraHours: boolean;
    extraHoursPaid: boolean;
    hasGuards: boolean;
    guardsPaid: boolean;

    profileOptions: any[] = new Array();
    skillOptions: any[] = new Array();
    seniorityOptions: any[] = new Array();
    reasonOptions: any[] = new Array();
    customerOptions: any[] = new Array();
    applicantOptions: any[] = new Array();
    recruitersOptions: any[] = new Array();
    timeHiringOptions: any[] = new Array();

    addSubscrip: Subscription;
    getUsersSubscrip: Subscription;
    getReasonsSubscrip: Subscription;
    getClientsSubscrip: Subscription;
    getProfilesSubscrip: Subscription;
    getSkilsSubscrip: Subscription;
    getSenioritySubscrip: Subscription;
    getRecruiterSubscrip: Subscription;
    getTimeHiringSubscrip: Subscription;

    constructor(private genericOptionsService: GenericOptionService,
                private jobSearchService: JobSearchService,
                private messageService: MessageService,
                public formsService: FormsService,
                private router: Router,
                private customerService: CustomerService){
    }

    ngOnInit(): void {
        this.messageService.showLoading();

        var promises = new Array();

        var promise1 = new Promise((resolve, reject) => this.getProfiles(resolve));
        var promise2 = new Promise((resolve, reject) => this.getSeniorities(resolve));
        var promise3 = new Promise((resolve, reject) => this.getSkills(resolve));
        var promise4 = new Promise((resolve, reject) => this.getReasons(resolve));
        var promise5 = new Promise((resolve, reject) => this.getCustomers(resolve));
        var promise6 = new Promise((resolve, reject) => this.getRecruiters(resolve));
        var promise7 = new Promise((resolve, reject) => this.getApplicants(resolve));
        var promise8 = new Promise((resolve, reject) => this.getTimeHirings(resolve));

        promises.push(promise1);
        promises.push(promise2);
        promises.push(promise3);
        promises.push(promise4);
        promises.push(promise5);
        promises.push(promise6);
        promises.push(promise7);
        promises.push(promise8);

        Promise.all(promises).then(data => { 
            this.messageService.closeLoading();
        });
    }

    ngOnDestroy(): void {
        if (this.addSubscrip) this.addSubscrip.unsubscribe();
        if (this.getUsersSubscrip) this.getUsersSubscrip.unsubscribe();
        if (this.getReasonsSubscrip) this.getReasonsSubscrip.unsubscribe();
        if (this.getClientsSubscrip) this.getClientsSubscrip.unsubscribe();
        if (this.getProfilesSubscrip) this.getProfilesSubscrip.unsubscribe();
        if (this.getSkilsSubscrip) this.getSkilsSubscrip.unsubscribe();
        if (this.getSenioritySubscrip) this.getSenioritySubscrip.unsubscribe();
        if (this.getRecruiterSubscrip) this.getRecruiterSubscrip.unsubscribe();
        if (this.getTimeHiringSubscrip) this.getTimeHiringSubscrip.unsubscribe();
    }

    getApplicants(resolve){
        this.getUsersSubscrip = this.jobSearchService.getApplicants().subscribe(response => {
            resolve();
            this.applicantOptions = response.data;
        },
        () => resolve());
    }

    getRecruiters(resolve){
        this.getRecruiterSubscrip = this.jobSearchService.getRecruiters().subscribe(response => {
            resolve();
            this.recruitersOptions = response.data;
        },
        () => resolve());
    }

    getTimeHirings(resolve){
        this.genericOptionsService.controller = GenericOptions.TimeHiring;
        this.getTimeHiringSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
            resolve();
            this.timeHiringOptions = response.data;
        },
        () => resolve());
    }

    getProfiles(resolve){
        this.genericOptionsService.controller = GenericOptions.Profile;
        this.getProfilesSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
            resolve();
            this.profileOptions = response.data;
        },
        () => resolve());
    }

    getSeniorities(resolve){
        this.genericOptionsService.controller = GenericOptions.Seniority;
        this.getProfilesSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
            resolve();
            this.seniorityOptions = response.data;
        },
        () => resolve());
    }

    getSkills(resolve){
        this.genericOptionsService.controller = GenericOptions.Skill;
        this.getProfilesSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
            resolve();
            this.skillOptions = response.data;
        },
        () => resolve());
    }

    getReasons(resolve){
        this.genericOptionsService.controller =  GenericOptions.ReasonCause;
        this.getProfilesSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
            resolve();
            this.reasonOptions = response.data;
        },
        () => resolve());
    }

    getCustomers(resolve) {
        this.getClientsSubscrip = this.customerService.getAllOptions().subscribe(d => {
            resolve();
            this.customerOptions = d.data;
        },
        () => resolve());
    }

    add(){
        if(!this.form.valid) return;

        var json = {
            userId: this.form.controls.userId.value,
            recruiterId: this.form.controls.recruiterId.value,
            reasonCauseId: this.form.controls.reasonCauseId.value,
            clientCrmId: this.form.controls.clientId.value,
            profiles: this.form.controls.profiles.value,
            skillsNotRequired: this.form.controls.skillsNotRequired.value,
            skillsRequired: this.form.controls.skillsRequired.value,
            seniorities: this.form.controls.seniorities.value,
            quantity: this.form.controls.quantity.value,
            timeHiringId: this.form.controls.timeHiringId.value,
            maximunSalary: this.form.controls.maximunSalary.value,
            comments: this.form.controls.comments.value,
            yearsExperience: this.form.controls.yearsExperience.value,
            email: this.form.controls.email.value,
            area: this.form.controls.area.value,
            telephone: this.form.controls.telephone.value,
            clientContact: this.form.controls.clientContact.value,
            jobType: this.form.controls.jobType.value,
            resourceAssignment: this.form.controls.resourceAssignment.value,
            language: this.form.controls.language.value,
            study: this.form.controls.study.value,
            jobTime: this.form.controls.jobTime.value,
            location: this.form.controls.location.value,
            benefits: this.form.controls.benefits.value,
            observations: this.form.controls.observations.value,
            tasksToDo: this.form.controls.observations.value,
            hasExtraHours: this.hasExtraHours,
            extraHoursPaid: this.extraHoursPaid,
            hasGuards: this.hasGuards,
            guardsPaid: this.guardsPaid,
            clientId: 0
        }

        this.messageService.showLoading();

        this.addSubscrip = this.jobSearchService.post(json).subscribe(response => {
            this.messageService.closeLoading();
            this.back();
        }, 
        error => {
            this.messageService.closeLoading();
        });
    }

    back(){
        this.router.navigate(['recruitment/jobSearch/']);
    }

    hasExtraHoursChanged(){
        this.extraHoursPaid = false;
    }

    hasGuardsChanged(){
        this.guardsPaid = false;
    }
}