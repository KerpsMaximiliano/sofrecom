import { Component, OnInit, OnDestroy } from "@angular/core";
import { FormControl, Validators } from "@angular/forms";
import { Subscription } from "rxjs";
import { GenericOptionService } from "app/services/admin/generic-option.service";
import { CustomerService } from "app/services/billing/customer.service";

@Component({
    selector: 'job-search',
    templateUrl: './job-search.html'
})
export class JobSearchComponent implements OnInit, OnDestroy {

    comments = new FormControl('', [Validators.maxLength(3000)]);
    timeHiring = new FormControl('', [Validators.maxLength(100)]);
    userId = new FormControl(null, [Validators.required]);
    reasonCauseId = new FormControl(null, [Validators.required]);
    clientId = new FormControl(null, [Validators.required]);
    quantity = new FormControl(null, [Validators.required]);
    maximunSalary = new FormControl(null, [Validators.required]);

    profileOptions: any[] = new Array();
    skillOptions: any[] = new Array();
    seniorityOptions: any[] = new Array();
    reasonOptions: any[] = new Array();
    customerOptions: any[] = new Array();

    profiles: any = null;
    skills: any = null;
    seniorities: any = null;

    addSubscrip: Subscription;
    getUsersSubscrip: Subscription;
    getReasonsSubscrip: Subscription;
    getClientsSubscrip: Subscription;
    getProfilesSubscrip: Subscription;
    getSkilsSubscrip: Subscription;
    getSenioritySubscrip: Subscription;

    constructor(private genericOptionsService: GenericOptionService,
                private customerService: CustomerService){
    }

    ngOnInit(): void {
        this.getProfiles();
        this.getSeniorities();
        this.getSkills();
        this.getReasons();
    }

    getProfiles(){
        this.genericOptionsService.controller = "profile";
        this.getProfilesSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
            this.profileOptions = response.data;
        },
        () => {});
    }

    getSeniorities(){
        this.genericOptionsService.controller = "seniority";
        this.getProfilesSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
            this.seniorityOptions = response.data;
        },
        () => {});
    }

    getSkills(){
        this.genericOptionsService.controller = "skill";
        this.getProfilesSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
            this.skillOptions = response.data;
        },
        () => {});
    }

    getReasons(){
        this.genericOptionsService.controller = "reasonCause";
        this.getProfilesSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
            this.reasonOptions = response.data;
        },
        () => {});
    }

    getCustomers() {
        this.getClientsSubscrip = this.customerService.getOptions().subscribe(d => {
            this.customerOptions = d.data;
        },
        () => {});
    }

    ngOnDestroy(): void {
        if (this.addSubscrip) this.addSubscrip.unsubscribe();
        if (this.getUsersSubscrip) this.getUsersSubscrip.unsubscribe();
        if (this.getReasonsSubscrip) this.getReasonsSubscrip.unsubscribe();
        if (this.getClientsSubscrip) this.getClientsSubscrip.unsubscribe();
        if (this.getProfilesSubscrip) this.getProfilesSubscrip.unsubscribe();
        if (this.getSkilsSubscrip) this.getSkilsSubscrip.unsubscribe();
        if (this.getSenioritySubscrip) this.getSenioritySubscrip.unsubscribe();
    }
}