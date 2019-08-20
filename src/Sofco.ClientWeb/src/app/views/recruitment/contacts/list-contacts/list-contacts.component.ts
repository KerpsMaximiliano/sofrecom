import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { DataTableService } from 'app/services/common/datatable.service';
import { GenericOptionService } from 'app/services/admin/generic-option.service';
import { MessageService } from 'app/services/common/message.service';

@Component({
  selector: 'app-list-contacts',
  templateUrl: './list-contacts.component.html',
  styleUrls: ['./list-contacts.component.scss']
})
export class ListContactsComponent implements OnInit, OnDestroy {

  getSubscrip: Subscription;

  public contact: any

  public searchModel = {
    name: "",
    seniority: "",
    profile: "",
    knowledge: ""
  };
  public knowledges: any[] = new Array()
  public profiles: any[] = new Array()
  public seniorities: any[] = new Array()

  public list: any[] = new Array();

  constructor(public dataTableService: DataTableService,
    public genericOptionService: GenericOptionService,
    public messageService: MessageService) { }

  ngOnInit() {
    this.getAll()
  }

  ngOnDestroy(): void {
    if (this.getSubscrip) this.getSubscrip.unsubscribe();

  }

  getAll() {
    
    this.knowledges = [
      {id: 0, text: ".Net"},
      {id: 0, text: "Scrum"},
      {id: 0, text: "Agile"},
      {id: 0, text: "Node.js"},
      {id: 0, text: "Python"},
      {id: 0, text: "Angular 7"},
    ]

    this.profiles = [
      {id: 0, text: "Desarrollador"},
      {id: 0, text: "Tester"},
      {id: 0, text: "Analista"},
      {id: 0, text: "Scrum Master"}
    ]

    this.seniorities = [
      {id: 0, text: "Junior"},
      {id: 0, text: "Semi Senior"},
      {id: 0, text: "Senior"},
      {id: 0, text: "Especialista Tecnologico"},
      {id: 0, text: "QA Senior"}
    ]
    //this.messageService.showLoading();


    // this.getSubscrip = this.genericOptionService.getAll().subscribe(response => {
    //   this.messageService.closeLoading();
    //   this.list = response.data;
    //   console.log(this.list)
    //   this.initGrid();
    // },
     // () => this.messageService.closeLoading());
  }

  initGrid() {
    var columns = [0, 1];

    var params = {
        selector: '#table',
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

}
