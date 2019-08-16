import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-list-contacts',
  templateUrl: './list-contacts.component.html',
  styleUrls: ['./list-contacts.component.scss']
})
export class ListContactsComponent implements OnInit {


  public searchModel = {
    name: "",
    seniority: "",
    profile: "",
    technology: "",
    percentage: null,
    analyticId: null,
    superiorId: null,
    managerId: null,
    employeeNumber: "",
    unassigned: false,
    externalOnly: false
};

  constructor() { }

  ngOnInit() {
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

}
