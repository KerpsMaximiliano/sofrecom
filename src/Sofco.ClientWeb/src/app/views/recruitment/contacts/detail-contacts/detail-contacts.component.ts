import { Component, OnInit } from '@angular/core';
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
    reasonCauseId: new FormControl(null),
    countryCode1: new FormControl(null, [Validators.min(0), Validators.max(99)]),
    areaCode1: new FormControl(null, [Validators.min(0), Validators.max(999)]),
    telephone1: new FormControl(null, [Validators.min(0), Validators.max(9999999999)]),
    countryCode2: new FormControl(null, [Validators.min(0), Validators.max(99)]),
    areaCode2: new FormControl(null, [Validators.min(0), Validators.max(999)]),
    telephone2: new FormControl(null, [Validators.min(0), Validators.max(9999999999)]),
  });

  profileOptions: any[] = new Array();
  skillOptions: any[] = new Array();
  reasonOptions: any[] = new Array();
  customerOptions: any[] = new Array();
  userOptions: any[] = new Array();
  history: any[] = new Array();

  entityId: number;

  addSubscrip: Subscription;
  getUsersSubscrip: Subscription;
  getReasonsSubscrip: Subscription;
  getClientsSubscrip: Subscription;
  getProfilesSubscrip: Subscription;
  getSkilsSubscrip: Subscription;
  getSubscrip: Subscription;
  getHistorySubscrip: Subscription;

  constructor(private genericOptionsService: GenericOptionService,
    private messageService: MessageService,
    public formsService: FormsService,
    private activateRoute: ActivatedRoute,
    private userService: UserService,
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

    this.activateRoute.params.subscribe(routeParams => {
        this.entityId = routeParams.id;
        this.getData(routeParams.id);
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
          this.reasonOptions = response.data;
      });
  }

  getUsers() {
    this.getClientsSubscrip = this.userService.getOptions().subscribe(d => {
        this.userOptions = d;
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
        this.form.controls.reasonCauseId.setValue(response.data.reasonCauseId);
        this.form.controls.countryCode1.setValue(response.data.countryCode1);
        this.form.controls.areaCode1.setValue(response.data.areaCode1);
        this.form.controls.telephone1.setValue(response.data.telephone1);
        this.form.controls.countryCode2.setValue(response.data.countryCode2);
        this.form.controls.areaCode2.setValue(response.data.areaCode2);
        this.form.controls.telephone2.setValue(response.data.telephone2);
    },
    error => this.messageService.closeLoading());
  }

  save(){
    if(!this.form.valid) return;

    var json = {
        lastName: this.form.controls.lastName.value,
        firstName: this.form.controls.firstName.value,
        email: this.form.controls.email.value,
        comments: this.form.controls.comments.value,
        clientCrmId: this.form.controls.clientId.value,
        reasonCauseId: this.form.controls.reasonCauseId.value,
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
    }

    this.messageService.showLoading();

    this.addSubscrip = this.applicantService.put(this.entityId, json).subscribe(response => {
        this.messageService.closeLoading();
        this.back();
    }, 
    error => {
        this.messageService.closeLoading();
    });
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
}
