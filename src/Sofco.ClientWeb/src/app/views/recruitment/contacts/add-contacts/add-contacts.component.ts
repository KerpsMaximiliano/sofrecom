import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { GenericOptionService } from 'app/services/admin/generic-option.service';
import { MessageService } from 'app/services/common/message.service';
import { FormsService } from 'app/services/forms/forms.service';
import { Router } from '@angular/router';
import { CustomerService } from 'app/services/billing/customer.service';
import { GenericOptions } from 'app/models/enums/genericOptions';
import { UserService } from 'app/services/admin/user.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-add-contacts',
  templateUrl: './add-contacts.component.html',
  styleUrls: ['./add-contacts.component.scss']
})
export class AddContactsComponent implements OnInit, OnDestroy {
  
  form: FormGroup = new FormGroup({
    lastName: new FormControl(null, [Validators.required, Validators.maxLength(75)]),
    firstName: new FormControl(null, [Validators.required, Validators.maxLength(75)]),
    comments: new FormControl(null, [Validators.maxLength(3000)]),
    email: new FormControl(null, [Validators.maxLength(75)]),
    profiles: new FormControl(null),
    skills: new FormControl(null),
    clientId: new FormControl(null),
    userId: new FormControl(null),
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

  addSubscrip: Subscription;
  getUsersSubscrip: Subscription;
  getReasonsSubscrip: Subscription;
  getClientsSubscrip: Subscription;
  getProfilesSubscrip: Subscription;
  getSkilsSubscrip: Subscription;

  constructor(private genericOptionsService: GenericOptionService,
    private messageService: MessageService,
    public formsService: FormsService,
    private userService: UserService,
    private router: Router,
    private customerService: CustomerService){
  }

  ngOnInit() {
    this.messageService.showLoading();

    var promises = new Array();

    var promise1 = new Promise((resolve, reject) => this.getProfiles(resolve));
    var promise2 = new Promise((resolve, reject) => this.getSkills(resolve));
    var promise3 = new Promise((resolve, reject) => this.getReasons(resolve));
    var promise4 = new Promise((resolve, reject) => this.getCustomers(resolve));
    var promise5 = new Promise((resolve, reject) => this.getUsers(resolve));

    promises.push(promise1);
    promises.push(promise2);
    promises.push(promise3);
    promises.push(promise4);
    promises.push(promise5);

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
  }

  getProfiles(resolve){
    this.genericOptionsService.controller = GenericOptions.Profile;
    this.getProfilesSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
        resolve();
        this.profileOptions = response.data;
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

  getUsers(resolve) {
    this.getClientsSubscrip = this.userService.getOptions().subscribe(d => {
        resolve();
        this.userOptions = d;
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

  back(){
    this.router.navigate(['recruitment/contacts/']);
  }

  add(){

  }
}
