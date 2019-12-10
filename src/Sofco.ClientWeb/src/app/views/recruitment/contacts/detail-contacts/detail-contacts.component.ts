import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { GenericOptionService } from 'app/services/admin/generic-option.service';
import { MessageService } from 'app/services/common/message.service';
import { FormsService } from 'app/services/forms/forms.service';
import { UserService } from 'app/services/admin/user.service';
import { ApplicantService } from 'app/services/recruitment/applicant.service';
import { Router, ActivatedRoute } from '@angular/router';
import { CustomerService } from 'app/services/billing/customer.service';
import { GenericOptions } from 'app/models/enums/genericOptions';
import { DataTableService } from 'app/services/common/datatable.service';
import { AnalyticService } from 'app/services/allocation-management/analytic.service';
import { ApplicantStatus } from 'app/models/enums/applicantStatus';
import * as moment from 'moment';
import { ReasonCauseType } from 'app/models/enums/reasonCauseType';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';

@Component({
  selector: 'app-detail-contacts',
  templateUrl: './detail-contacts.component.html',
  styleUrls: ['./detail-contacts.component.scss']
})
export class DetailContactsComponent implements OnInit {

  form: FormGroup = new FormGroup({
    lastName: new FormControl(null, [Validators.required, Validators.maxLength(75)]),
    firstName: new FormControl(null, [Validators.required, Validators.maxLength(75)]),
    comments: new FormControl(null, [Validators.maxLength(3000)]),
    email: new FormControl(null, [Validators.maxLength(75)]),
    profiles: new FormControl(null),
    skills: new FormControl(null),
    clientId: new FormControl(null),
    recommendedByUserId: new FormControl(null),
    countryCode1: new FormControl(null, [Validators.min(0), Validators.max(99)]),
    areaCode1: new FormControl(null, [Validators.min(0), Validators.max(999)]),
    telephone1: new FormControl(null, [Validators.maxLength(100)]),
    countryCode2: new FormControl(null, [Validators.min(0), Validators.max(99)]),
    areaCode2: new FormControl(null, [Validators.min(0), Validators.max(999)]),
    telephone2: new FormControl(null, [Validators.maxLength(100)]),
  });

  newResourceForm: FormGroup = new FormGroup({
    nationality: new FormControl(null, [Validators.required, Validators.maxLength(50)]),
    civilStatus: new FormControl(null, [Validators.required, Validators.maxLength(50)]),
    birthDate: new FormControl(null, [Validators.required]),
    startDate: new FormControl(null, [Validators.required]),
    address: new FormControl(null, [Validators.required, Validators.maxLength(100)]),
    cuil: new FormControl(null, [Validators.required, Validators.max(99999999999)]),
    prepaid: new FormControl(null, [Validators.required, Validators.maxLength(100)]),
    profileId: new FormControl(null, [Validators.required]),
    office: new FormControl(null, [Validators.required, Validators.maxLength(100)]),
    agreements: new FormControl(null, [Validators.maxLength(3000)]),
    salary: new FormControl(null, [Validators.required]),
    managerId: new FormControl(null, [Validators.required]),
    analyticId: new FormControl(null, [Validators.required]),
    projectId: new FormControl(null),
  });

  dateModalForm: FormGroup = new FormGroup({
    reasonCauseModalId: new FormControl(null, [Validators.required]),
    comments: new FormControl(null, [Validators.required, Validators.maxLength(1000)]),
  });

  @ViewChild('interview') interview;
  @ViewChild('contactFiles') contactFiles;

  @ViewChild('dateModal') dateModal;
  public dateModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      "ACTIONS.confirmTitle",
      "dateModal",
      true,
      true,
      "ACTIONS.ACCEPT",
      "ACTIONS.cancel"
  );

  profileOptions: any[] = new Array();
  skillOptions: any[] = new Array();
  reasonOptions: any[] = new Array();
  applicantCloseReasons: any[] = new Array();
  customerOptions: any[] = new Array();
  userOptions: any[] = new Array();
  history: any[] = new Array();
  analytics: any[] = new Array();
  projects: any[] = new Array();

  entityId: number;
  status: number;
  
  registerVisible: boolean = false;

  addSubscrip: Subscription;
  getUsersSubscrip: Subscription;
  getReasonsSubscrip: Subscription;
  getClientsSubscrip: Subscription;
  getProfilesSubscrip: Subscription;
  getSkilsSubscrip: Subscription;
  getSubscrip: Subscription;
  getHistorySubscrip: Subscription;
  getAnalyticSubscrip: Subscription;
  getProjectsSubscrip: Subscription;
  changeStatusSubscrip: Subscription;

  constructor(private genericOptionsService: GenericOptionService,
    private messageService: MessageService,
    public formsService: FormsService,
    private activateRoute: ActivatedRoute,
    private userService: UserService,
    private analyticService: AnalyticService,
    private applicantService: ApplicantService,
    private dataTableService: DataTableService,
    private router: Router,
    private customerService: CustomerService){
  }

  ngOnInit() {
    this.getProfiles();
    this.getSkills();
    this.getReasons();
    this.getCustomers();
    this.getUsers();
    this.getAnalytics();

    this.activateRoute.params.subscribe(routeParams => {
        this.entityId = routeParams.id;
        this.getData(routeParams.id);
        this.contactFiles.getFiles(this.entityId);
    });
  }

  ngOnDestroy(): void {
    if (this.addSubscrip) this.addSubscrip.unsubscribe();
    if (this.getUsersSubscrip) this.getUsersSubscrip.unsubscribe();
    if (this.getReasonsSubscrip) this.getReasonsSubscrip.unsubscribe();
    if (this.getClientsSubscrip) this.getClientsSubscrip.unsubscribe();
    if (this.getProfilesSubscrip) this.getProfilesSubscrip.unsubscribe();
    if (this.getSkilsSubscrip) this.getSkilsSubscrip.unsubscribe();
    if (this.getSubscrip) this.getSubscrip.unsubscribe();
    if (this.getHistorySubscrip) this.getHistorySubscrip.unsubscribe();
  }

  getProfiles(){
    this.genericOptionsService.controller = GenericOptions.Profile;
    this.getProfilesSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
        this.profileOptions = response.data;
    });
  }

  getSkills(){
      this.genericOptionsService.controller = GenericOptions.Skill;
      this.getProfilesSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
          this.skillOptions = response.data;
      });
  }

  getReasons(){
      this.genericOptionsService.controller =  GenericOptions.ReasonCause;
      this.getProfilesSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
          this.reasonOptions = response.data.filter(x => x.type == ReasonCauseType.ApplicantUnavailable || 
                                                        x.type == ReasonCauseType.ApplicantInProgress ||
                                                        x.type == ReasonCauseType.ApplicantInCompany ||
                                                        x.type == ReasonCauseType.ApplicantOpen);

          this.applicantCloseReasons = response.data.filter(x => x.type == ReasonCauseType.ApplicantInCompany ||
                                                                x.type == ReasonCauseType.ApplicantOpen);

          this.interview.reasonOptions = this.reasonOptions;                              
      });
  }

  getAnalytics() {
    this.getAnalyticSubscrip = this.analyticService.getOptionsActives().subscribe(d => {
        this.analytics = d;
    });
  }

  analyticChange(){
    this.getProjectsSubscrip = this.analyticService.getOpportunities(this.newResourceForm.controls.analyticId.value).subscribe(response => {
      this.projects = response.data;
    });
  }

  getUsers() {
    this.getClientsSubscrip = this.userService.getOptions().subscribe(d => {
        this.userOptions = d;
        this.interview.userOptions = d;
    });
  }

  getCustomers() {
      this.getClientsSubscrip = this.customerService.getAllOptions().subscribe(d => {
          this.customerOptions = d.data;
      });
  }

  back(){
    this.router.navigate(['recruitment/contacts/']);
  }

  getData(id){
    this.messageService.showLoading();

    this.getSubscrip = this.applicantService.get(id).subscribe(response => {
        this.messageService.closeLoading();

        this.entityId = id;

        this.getHistory();

        this.form.controls.lastName.setValue(response.data.lastName);
        this.form.controls.firstName.setValue(response.data.firstName);
        this.form.controls.comments.setValue(response.data.comments);
        this.form.controls.email.setValue(response.data.email);
        this.form.controls.profiles.setValue(response.data.profiles);
        this.form.controls.skills.setValue(response.data.skills);
        this.form.controls.clientId.setValue(response.data.clientId);
        this.form.controls.recommendedByUserId.setValue(response.data.recommendedByUserId);
        this.form.controls.countryCode1.setValue(response.data.countryCode1);
        this.form.controls.areaCode1.setValue(response.data.areaCode1);
        this.form.controls.telephone1.setValue(response.data.telephone1);
        this.form.controls.countryCode2.setValue(response.data.countryCode2);
        this.form.controls.areaCode2.setValue(response.data.areaCode2);
        this.form.controls.telephone2.setValue(response.data.telephone2);

        this.newResourceForm.controls.nationality.setValue(response.data.nationality);
        this.newResourceForm.controls.civilStatus.setValue(response.data.civilStatus);
        if(response.data.birthDate) this.newResourceForm.controls.birthDate.setValue(moment(response.data.birthDate).toDate());
        if(response.data.startDate) this.newResourceForm.controls.startDate.setValue(moment(response.data.startDate).toDate());
        this.newResourceForm.controls.address.setValue(response.data.address);
        this.newResourceForm.controls.cuil.setValue(response.data.cuil);
        this.newResourceForm.controls.prepaid.setValue(response.data.prepaid);
        this.newResourceForm.controls.profileId.setValue(response.data.profileId);
        this.newResourceForm.controls.office.setValue(response.data.office);
        this.newResourceForm.controls.agreements.setValue(response.data.agreements);
        this.newResourceForm.controls.salary.setValue(response.data.salary);
        if(response.data.managerId) this.newResourceForm.controls.managerId.setValue(response.data.managerId.toString());
        this.newResourceForm.controls.analyticId.setValue(response.data.analyticId);
        if(response.data.projectId) this.newResourceForm.controls.projectId.setValue(response.data.projectId.toString());

        if(response.data.analyticId && response.data.projectId){
          this.analyticChange();
        }

        this.status = response.data.status;

        if(this.status == ApplicantStatus.Close || this.status == ApplicantStatus.InCompany){
          this.form.disable();
          this.newResourceForm.disable();
          this.registerVisible = true;
        }
    },
    error => this.messageService.closeLoading());
  }

  canMakeRegister(){
    if(this.status == ApplicantStatus.InCompany || this.status == ApplicantStatus.InProgress) return true;

    return false;
  }

  save(){
    if(!this.form.valid) return;

    var json = this.getGeneralData()

    this.messageService.showLoading();

    this.addSubscrip = this.applicantService.put(this.entityId, json).subscribe(response => {
        this.messageService.closeLoading();
        this.back();
    }, 
    error => {
        this.messageService.closeLoading();
    });
  }

  private getGeneralData() {
    return {
      lastName: this.form.controls.lastName.value,
      firstName: this.form.controls.firstName.value,
      email: this.form.controls.email.value,
      comments: this.form.controls.comments.value,
      clientCrmId: this.form.controls.clientId.value,
      recommendedByUserId: this.form.controls.recommendedByUserId.value,
      countryCode1: this.form.controls.countryCode1.value,
      countryCode2: this.form.controls.countryCode2.value,
      areaCode1: this.form.controls.areaCode1.value,
      areaCode2: this.form.controls.areaCode2.value,
      telephone1: this.form.controls.telephone1.value,
      telephone2: this.form.controls.telephone2.value,
      skills: this.form.controls.skills.value,
      profiles: this.form.controls.profiles.value,
      clientId: 0
    };
  }

  register(){
    if(!this.form.valid || !this.newResourceForm.valid) return;

    var generalData = this.getGeneralData();

    var registerData = this.getRegisterData();

    var json = { generalData, registerData };

    this.messageService.showLoading();

    this.addSubscrip = this.applicantService.register(this.entityId, json).subscribe(response => {
        this.messageService.closeLoading();
        this.back();
    }, 
    error => {
        this.messageService.closeLoading();
    });
  }

  private getRegisterData() {
    return {
      nationality: this.newResourceForm.controls.nationality.value,
      civilStatus: this.newResourceForm.controls.civilStatus.value,
      birthDate: this.newResourceForm.controls.birthDate.value,
      startDate: this.newResourceForm.controls.startDate.value,
      address: this.newResourceForm.controls.address.value,
      cuil: this.newResourceForm.controls.cuil.value,
      prepaid: this.newResourceForm.controls.prepaid.value,
      profileId: this.newResourceForm.controls.profileId.value,
      office: this.newResourceForm.controls.office.value,
      aggreements: this.newResourceForm.controls.agreements.value,
      salary: this.newResourceForm.controls.salary.value,
      managerId: this.newResourceForm.controls.managerId.value,
      analyticId: this.newResourceForm.controls.analyticId.value,
      projectId: this.newResourceForm.controls.projectId.value
    };
  }

  getHistory(){
    this.addSubscrip = this.applicantService.getHistory(this.entityId).subscribe(response => {
      this.history = response.data;

      this.initGrid();
    });
  }

  initGrid() {
    var columns = [0, 1, 2, 3, 4, 5, 6, 7];

    var options = {
        selector: "#historyTable",
        columns: columns,
        order: [[ 0, "desc" ]],
        columnDefs: [ { "aTargets": [0], "sType": "date-uk" }],
    };

    this.dataTableService.destroy(options.selector);
    this.dataTableService.initialize(options);
  }

  makeRegister(){
    this.registerVisible = true;
  }

  cancelRegister() {
    this.registerVisible = false;
  }

  getStatusDesc(){
    switch(this.status){
        case ApplicantStatus.Valid: return "Vigente";
        case ApplicantStatus.InProgress: return "En Curso";
        case ApplicantStatus.Close: return "Deshabilitado";
        case ApplicantStatus.InCompany: return "Ingresado";
        default: return "";
    }
  }

  closeApplicant(){
    var json = {
      status: ApplicantStatus.Close,
      reasonCauseId: this.dateModalForm.controls.reasonCauseModalId.value,
      comments: this.dateModalForm.controls.comments.value
    }

    this.changeStatusSubscrip = this.applicantService.changeStatus(this.entityId, json).subscribe(response => {
        this.dateModal.hide();
        this.status = ApplicantStatus.Close;

        this.dateModalForm.controls.comments.setValue(null);
        this.dateModalForm.controls.reasonCauseModalId.setValue(null);
    }, 
    error => {
        this.dateModal.resetButtons();
    });
  }

  canClose(){
    if(this.status != ApplicantStatus.Close) return true;

    return false;
  }

  openCloseModal(){
    this.dateModal.show();
  }

  onHistoryClick(history){
    if(history.hasRrhhInterview || history.hasTechnicalInterview || history.hasClientInterview){
      this.interview.setData(history);
    }
    else{
      if(history.reasonType == ReasonCauseType.ApplicantInProgress){
        this.interview.setData(history);
      }
      else{
        this.interview.clean();
      }
    }
  }

  refreshFiles(){
    this.contactFiles.getFiles(this.entityId);
  }
}
