import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { DataTableService } from 'app/services/common/datatable.service';
import { GenericOptionService } from 'app/services/admin/generic-option.service';
import { MessageService } from 'app/services/common/message.service';
import { Router } from '@angular/router';
import { GenericOptions } from 'app/models/enums/genericOptions';
import { CustomerService } from 'app/services/billing/customer.service';
import { ApplicantService } from 'app/services/recruitment/applicant.service';
import { MenuService } from 'app/services/admin/menu.service';
import { ApplicantStatus } from 'app/models/enums/applicantStatus';

@Component({
  selector: 'app-list-contacts',
  templateUrl: './list-contacts.component.html',
  styleUrls: ['./list-contacts.component.scss']
})
export class ListContactsComponent implements OnInit, OnDestroy {

  public searchModel = {
    firstName: null,
    lastName: null,
    skills: null,
    profiles: null,
    clientCrmId: null
  };

  public skillOptions: any[] = new Array()
  public customerOptions: any[] = new Array()
  public profileOptions: any[] = new Array()

  public list: any[] = new Array();

  searchSubscrip: Subscription;
  getClientsSubscrip: Subscription;
  getProfilesSubscrip: Subscription;
  getSkillsSubscrip: Subscription;

  constructor(private dataTableService: DataTableService,
    private genericOptionsService: GenericOptionService,
    private customerService: CustomerService,
    private menuService: MenuService,
    private applicantService: ApplicantService,
    private router: Router,
    private messageService: MessageService) { }

  ngOnInit() {
    this.getCustomers();
    this.getProfiles();
    this.getSkills();
  }

  ngOnDestroy(): void {
    if (this.searchSubscrip) this.searchSubscrip.unsubscribe();
    if (this.getClientsSubscrip) this.getClientsSubscrip.unsubscribe();
    if (this.getProfilesSubscrip) this.getProfilesSubscrip.unsubscribe();
    if (this.getSkillsSubscrip) this.getSkillsSubscrip.unsubscribe();

  }

  getCustomers() {
    this.getClientsSubscrip = this.customerService.getAllOptions().subscribe(d => {
        this.customerOptions = d.data;
    });
  }

  getProfiles(){
    this.genericOptionsService.controller = GenericOptions.Profile;
    this.getProfilesSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
        this.profileOptions = response.data;
    });
  }

  getSkills(){
      this.genericOptionsService.controller = GenericOptions.Skill;
      this.getSkillsSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
          this.skillOptions = response.data;
      });
  }

  search() {
    var json = {
      firstName: this.searchModel.firstName,
      lastName: this.searchModel.lastName,
      skills: this.searchModel.skills,
      profiles: this.searchModel.profiles,
      clientCrmId: this.searchModel.clientCrmId,
    };

    this.list = [];

    this.messageService.showLoading();

    this.searchSubscrip = this.applicantService.search(json).subscribe(response => {
        this.messageService.closeLoading();

        if(response && response.data && response.data.length > 0){
            this.list = response.data;
            this.initGrid();
        }
        else{
            this.messageService.showWarning("searchEmpty");
        }
    }, 
    error => this.messageService.closeLoading())
  }

  clean(){
    this.searchModel.firstName = null;
    this.searchModel.lastName = null;
    this.searchModel.skills = null;
    this.searchModel.profiles = null;
    this.searchModel.clientCrmId = null;
  }

  initGrid() {
    var columns = [0, 1, 2, 3 ,4, 5, 6];

    var params = {
        selector: '#searchTable',
        columns: columns,
        title: 'Contactos',
        withExport: true,
    }

    this.dataTableService.destroy(params.selector);
    this.dataTableService.initialize(params);
}

  collapse() {
    if ($("#collapseOne").hasClass('in')) {
      $("#collapseOne").removeClass('in');
    }
    else {
      $("#collapseOne").addClass('in');
    }

    this.changeIcon();
  }

  changeIcon() {
    if ($("#collapseOne").hasClass('in')) {
      $("#search-icon").toggleClass('fa-caret-down').toggleClass('fa-caret-up');
    }
    else {
      $("#search-icon").toggleClass('fa-caret-up').toggleClass('fa-caret-down');
    }
  }

  canAdd(){
    return this.menuService.hasFunctionality('RECRU', 'ADD-CANDIDATE');
  }

  goToNew(){
    this.router.navigate(['recruitment/contacts/add']);
  }

  goToDetail(id){
    this.router.navigate(['recruitment/contacts/' + id]);
  }

  getStatusDesc(status){
    switch(status){
        case ApplicantStatus.Valid: return "Vigente";
        case ApplicantStatus.InProgress: return "En Curso";
        case ApplicantStatus.Close: return "Deshabilitado";
        case ApplicantStatus.InCompany: return "Ingresado";
        case ApplicantStatus.Contacted: return "Vigente/Contactado";
        default: return "";
    }
  }
}
