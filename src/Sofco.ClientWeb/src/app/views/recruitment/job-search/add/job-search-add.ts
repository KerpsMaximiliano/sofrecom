import { Component, OnInit, OnDestroy } from "@angular/core";
import { FormControl, Validators, FormGroup } from "@angular/forms";
import { Subscription } from "rxjs";
import { GenericOptionService } from "app/services/admin/generic-option.service";
import { CustomerService } from "app/services/billing/customer.service";
import { JobSearchService } from "app/services/recruitment/jobsearch.service";
import { FormsService } from "app/services/forms/forms.service";
import { MessageService } from "app/services/common/message.service";

@Component({
    selector: 'job-search',
    templateUrl: './job-search-add.html'
})
export class JobSearchComponent implements OnInit, OnDestroy {

    form: FormGroup = new FormGroup({
        comments: new FormControl('', [Validators.maxLength(3000)]),
        timeHiring: new FormControl('', [Validators.maxLength(100)]),
        userId: new FormControl(null, [Validators.required]),
        reasonCauseId: new FormControl(null, [Validators.required]),
        clientId: new FormControl(null, [Validators.required]),
        quantity: new FormControl(null, [Validators.required, Validators.min(1)]),
        maximunSalary: new FormControl(null, [Validators.required]),
        recruiterId: new FormControl(null, [Validators.required]),
        profiles: new FormControl(null),
        skills: new FormControl(null),
        seniorities: new FormControl(null),
    });

    profileOptions: any[] = new Array();
    skillOptions: any[] = new Array();
    seniorityOptions: any[] = new Array();
    reasonOptions: any[] = new Array();
    customerOptions: any[] = new Array();
    applicantOptions: any[] = new Array();
    recruitersOptions: any[] = new Array();

    addSubscrip: Subscription;
    getUsersSubscrip: Subscription;
    getReasonsSubscrip: Subscription;
    getClientsSubscrip: Subscription;
    getProfilesSubscrip: Subscription;
    getSkilsSubscrip: Subscription;
    getSenioritySubscrip: Subscription;
    getRecruiterSubscrip: Subscription;

    constructor(private genericOptionsService: GenericOptionService,
                private jobSearchService: JobSearchService,
                private messageService: MessageService,
                public formsService: FormsService,
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

        promises.push(promise1);
        promises.push(promise2);
        promises.push(promise3);
        promises.push(promise4);
        promises.push(promise5);
        promises.push(promise6);
        promises.push(promise7);

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

    getProfiles(resolve){
        this.genericOptionsService.controller = "profile";
        this.getProfilesSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
            resolve();
            this.profileOptions = response.data;
        },
        () => resolve());
    }

    getSeniorities(resolve){
        this.genericOptionsService.controller = "seniority";
        this.getProfilesSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
            resolve();
            this.seniorityOptions = response.data;
        },
        () => resolve());
    }

    getSkills(resolve){
        this.genericOptionsService.controller = "skill";
        this.getProfilesSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
            resolve();
            this.skillOptions = response.data;
        },
        () => resolve());
    }

    getReasons(resolve){
        this.genericOptionsService.controller = "reasonCause";
        this.getProfilesSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
            resolve();
            this.reasonOptions = response.data;
        },
        () => resolve());
    }

    getCustomers(resolve) {
        this.getClientsSubscrip = this.customerService.getOptions().subscribe(d => {
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
            skills: this.form.controls.skills.value,
            seniorities: this.form.controls.seniorities.value,
            quantity: this.form.controls.quantity.value,
            timeHiring: this.form.controls.timeHiring.value,
            maximunSalary: this.form.controls.maximunSalary.value,
            comments: this.form.controls.comments.value,
            clientId: 0
        }

        this.messageService.showLoading();

        this.addSubscrip = this.jobSearchService.post(json).subscribe(response => {
            this.messageService.closeLoading();
        }, 
        error => {
            this.messageService.closeLoading();
        });
    }
}