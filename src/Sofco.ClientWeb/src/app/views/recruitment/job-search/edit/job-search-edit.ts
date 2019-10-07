import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { FormControl, Validators, FormGroup } from "@angular/forms";
import { Subscription } from "rxjs";
import { GenericOptionService } from "app/services/admin/generic-option.service";
import { CustomerService } from "app/services/billing/customer.service";
import { JobSearchService } from "app/services/recruitment/jobsearch.service";
import { FormsService } from "app/services/forms/forms.service";
import { MessageService } from "app/services/common/message.service";
import { ActivatedRoute, Router } from "@angular/router";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { JobSearchStatus } from "app/models/enums/jobSearchStatus";
import { GenericOptions } from "app/models/enums/genericOptions";

@Component({
    selector: 'job-search',
    templateUrl: './job-search-edit.html',
    styleUrls: ['job-search-edit.scss']
})
export class JobSearchEditComponent implements OnInit, OnDestroy {

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
        jobType: new FormControl(null),
        resourceAssignment: new FormControl(null),
        language: new FormControl(null, [Validators.maxLength(100)]),
        study: new FormControl(null, [Validators.maxLength(100)]),
        jobTime: new FormControl(null, [Validators.maxLength(100)]),
        location: new FormControl(null, [Validators.maxLength(200)]),
        benefits: new FormControl(null, [Validators.maxLength(3000)]),
        observations: new FormControl(null, [Validators.maxLength(3000)]),
        tasksToDo: new FormControl(null, [Validators.maxLength(3000)]),
    });

    dateModalForm: FormGroup = new FormGroup({
        date: new FormControl(null, [Validators.required]),
        comments: new FormControl(null, [Validators.maxLength(1000)]),
    });

    hasExtraHours: boolean;
    extraHoursPaid: boolean;
    hasGuards: boolean;
    guardsPaid: boolean;
    languageRequired: boolean;
    studyRequired: boolean;

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
    getSubscrip: Subscription;
    changeStatusSubscrip: Subscription;
    getTimeHiringSubscrip: Subscription;

    entityId: number;
    status: number;
    statusSelected: JobSearchStatus;

    @ViewChild('applicantsRelated') applicantsRelated;
    
    @ViewChild('dateModal') dateModal;
    public dateModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "dateModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    constructor(private genericOptionsService: GenericOptionService,
                private jobSearchService: JobSearchService,
                private messageService: MessageService,
                private activateRoute: ActivatedRoute,
                private router: Router,
                public formsService: FormsService,
                private customerService: CustomerService){
    }

    ngOnInit(): void {
        this.getProfiles();
        this.getSeniorities();
        this.getSkills();
        this.getReasons();
        this.getCustomers();
        this.getRecruiters();
        this.getApplicants();
        this.getTimeHirings();

        this.activateRoute.params.subscribe(routeParams => {
            this.entityId = routeParams.id;
            this.getData(routeParams.id);
            this.applicantsRelated.init(this.entityId);
        });

        setTimeout(() => {
            $('#tab-1 .ng-select .ng-select-container').css('min-height', '30px');
            $('#tab-1 .ng-select .ng-select-container').css('height', '30px');
        }, 200);
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
        if (this.getSubscrip) this.getSubscrip.unsubscribe();
        if (this.changeStatusSubscrip) this.changeStatusSubscrip.unsubscribe();
        if (this.getTimeHiringSubscrip) this.getTimeHiringSubscrip.unsubscribe();
    }

    getStatusDesc(){
        switch(this.status){
            case JobSearchStatus.Open: return "Abierta";
            case JobSearchStatus.Reopen: return "Re-Abierta";
            case JobSearchStatus.Suspended: return "Suspendida";
            case JobSearchStatus.Close: return "Cerrada";
            default: return "";
        }
    }

    getData(id){
        this.messageService.showLoading();

        this.getSubscrip = this.jobSearchService.get(id).subscribe(response => {
            this.messageService.closeLoading();

            this.entityId = id;

            this.form.controls.userId.setValue(response.data.userId);
            this.form.controls.recruiterId.setValue(response.data.recruiterId);
            this.form.controls.reasonCauseId.setValue(response.data.reasonCauseId);
            this.form.controls.clientId.setValue(response.data.clientCrmId);
            this.form.controls.profiles.setValue(response.data.profiles);
            this.form.controls.skillsRequired.setValue(response.data.skillsRequired);
            this.form.controls.skillsNotRequired.setValue(response.data.skillsNotRequired);
            this.form.controls.seniorities.setValue(response.data.seniorities);
            this.form.controls.quantity.setValue(response.data.quantity);
            this.form.controls.timeHiringId.setValue(response.data.timeHiringId);
            this.form.controls.maximunSalary.setValue(response.data.maximunSalary);
            this.form.controls.comments.setValue(response.data.comments);
            this.form.controls.yearsExperience.setValue(response.data.yearsExperience);
            this.form.controls.email.setValue(response.data.email);
            this.form.controls.area.setValue(response.data.area);
            this.form.controls.telephone.setValue(response.data.telephone);
            this.form.controls.clientContact.setValue(response.data.clientContact);
            this.form.controls.jobType.setValue(response.data.jobType.toString());
            this.form.controls.resourceAssignment.setValue(response.data.resourceAssignment.toString());
            this.form.controls.language.setValue(response.data.language);
            this.form.controls.study.setValue(response.data.study);
            this.form.controls.jobTime.setValue(response.data.jobTime);
            this.form.controls.location.setValue(response.data.location);
            this.form.controls.benefits.setValue(response.data.benefits);
            this.form.controls.observations.setValue(response.data.observations);
            this.form.controls.tasksToDo.setValue(response.data.tasksToDo);

            this.hasExtraHours = response.data.hasExtraHours;
            this.extraHoursPaid = response.data.extraHoursPaid;
            this.hasGuards = response.data.hasGuards;
            this.guardsPaid = response.data.guardsPaid;
            this.languageRequired = response.data.languageRequired;
            this.studyRequired = response.data.studyRequired;

            this.status = response.data.status;
        }, 
        error => this.messageService.closeLoading());
    }

    getTimeHirings(){
        this.genericOptionsService.controller = GenericOptions.TimeHiring;
        this.getTimeHiringSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
            this.timeHiringOptions = response.data;
        },
        () => {});
    }

    getApplicants(){
        this.getUsersSubscrip = this.jobSearchService.getApplicants().subscribe(response => {
            this.applicantOptions = response.data;
        },
        () => {});
    }

    getRecruiters(){
        this.getRecruiterSubscrip = this.jobSearchService.getRecruiters().subscribe(response => {
            this.recruitersOptions = response.data;
        },
        () => {});
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
            this.applicantsRelated.setReasonOptions(this.reasonOptions);
        },
        () => {});
    }

    getCustomers() {
        this.getClientsSubscrip = this.customerService.getAllOptions().subscribe(d => {
            this.customerOptions = d.data;
        },
        () => {});
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
            languageRequired: this.languageRequired,
            studyRequired: this.studyRequired,
            guardsPaid: this.guardsPaid,
            clientId: 0
        }

        this.messageService.showLoading();

        this.addSubscrip = this.jobSearchService.put(this.entityId, json).subscribe(response => {
            this.messageService.closeLoading();
        }, 
        error => {
            this.messageService.closeLoading();
        });
    }

    canSave(){
        return this.status != JobSearchStatus.Close;
    }

    canClose(){
        return this.status == JobSearchStatus.Open || this.status == JobSearchStatus.Reopen;
    }

    canReopen(){
        return this.status == JobSearchStatus.Suspended;
    }

    canSuspend(){
        return this.status == JobSearchStatus.Open || this.status == JobSearchStatus.Reopen;
    }

    close(){
        this.statusSelected = JobSearchStatus.Close;
        this.dateModal.show();
    }

    reopen(){
        this.statusSelected = JobSearchStatus.Reopen;
        this.dateModal.show();
    }

    suspend(){
        this.statusSelected = JobSearchStatus.Suspended;
        this.dateModal.show();
    }

    changeStatus(){
        var json = {
            status: this.statusSelected,
            date: this.dateModalForm.controls.date.value,
            reason: this.dateModalForm.controls.comments.value
        }

        this.changeStatusSubscrip = this.jobSearchService.changeStatus(this.entityId, json).subscribe(response => {
            this.dateModal.hide();
            this.status = this.statusSelected;

            this.dateModalForm.controls.date.setValue(null);
            this.dateModalForm.controls.comments.setValue(null);
        }, 
        error => {
            this.dateModal.resetButtons();
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

    studyRequiredChanged(value){
        if(value == true){
            this.form.controls.study.setValidators([Validators.required, Validators.maxLength(100)]);
            this.form.controls.study.updateValueAndValidity();
        }
        else {
            this.form.controls.study.setValidators([Validators.maxLength(100)]);
            this.form.controls.study.updateValueAndValidity();
        }
    }

    languageRequiredChanged(value){
        if(value == true){
            this.form.controls.language.setValidators([Validators.required, Validators.maxLength(100)]);
            this.form.controls.language.updateValueAndValidity();
        }
        else {
            this.form.controls.language.setValidators([Validators.maxLength(100)]);
            this.form.controls.language.updateValueAndValidity();
        }
    }
}