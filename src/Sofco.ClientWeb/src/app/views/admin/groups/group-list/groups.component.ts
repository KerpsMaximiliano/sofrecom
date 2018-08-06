import { TranslateService } from '@ngx-translate/core';
import { MessageService } from 'app/services/common/message.service';
import { DatatablesLocationTexts } from 'app/components/datatables/datatables.location-texts';
import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { DatatablesEditionType } from "app/components/datatables/datatables.edition-type";
import { DatatablesColumn } from "app/components/datatables/datatables.columns";
import { Subscription } from "rxjs";
import { DatatablesOptions } from "app/components/datatables/datatables.options";
import { DatatablesDataType } from "app/components/datatables/datatables.datatype";
import { DatatablesAlignment } from "app/components/datatables/datatables.alignment";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { GroupService } from "app/services/admin/group.service";
import { MenuService } from "app/services/admin/menu.service";
import { I18nService } from 'app/services/common/i18n.service';

@Component({
  selector: 'app-groups',
  templateUrl: './groups.component.html'
})
export class GroupsComponent implements OnInit, OnDestroy {

  data;
  @ViewChild('dt') dt;
  getAllSubscrip: Subscription;
  getSubscrip: Subscription;
  deleteSubscrip: Subscription;
  deactivateSubscrip: Subscription;
  activateSubscrip: Subscription;
  editionType = DatatablesEditionType.ButtonsAtTheEndOfTheRow;
  locationTexts = new DatatablesLocationTexts("Details");

  options = new DatatablesOptions(
    true,  //edit
    false,  //delete
    false,  //view
    true,  //habInhab
    false,  //other1
    false, //other2
    false, //other3
    "fa-eye",     //other1Icon
    "fa-check",      //other2Icon
    "fa-cogs",        //other3Icon
    { title: this.i18nService.translateByKey("ADMIN.GROUPS.TITLE"), columns: [0, 1, 3, 4]},
    0,     //orderByColumn
    "asc"
    ); 

  private dataTypeEnum = DatatablesDataType;
  private alignmentEnum = DatatablesAlignment;

  columns: DatatablesColumn[] = 
  [
    new DatatablesColumn(
      "id",      //name
      "Id",      //title
      "",    //width
      0,         //visibility
      this.dataTypeEnum.number,  //dataType
      this.alignmentEnum.left
    ),
    new DatatablesColumn(
      "description",  //name
      "ADMIN.description",  //title
      "",    //width
      1,     //visibility
      this.dataTypeEnum.string,  //dataType
      this.alignmentEnum.left
    ),
    new DatatablesColumn(
      "email",  //name
      "ADMIN.GROUPS.mail",  //title
      "",    //width
      1,     //visibility
      this.dataTypeEnum.string,  //dataType
      this.alignmentEnum.left
    ),
    new DatatablesColumn(
      "active",  //name
      "ADMIN.active",  //title
      "100px",    //width
      1,     //visibility
      this.dataTypeEnum.boolean,  //dataType
      this.alignmentEnum.center
    ),
    new DatatablesColumn(
      "startDate",  //name
      "ADMIN.startDate",  //title
      "100px",    //width
      1,     //visibility
      this.dataTypeEnum.date,  //dataType
      this.alignmentEnum.right
    ),
    new DatatablesColumn(
      "endDate",  //name
      "ADMIN.endDate",  //title
      "100px",    //width
      1,     //visibility
      this.dataTypeEnum.date,  //dataType
      this.alignmentEnum.right
    ),
  ];

  constructor(
      private router: Router,
      private route: ActivatedRoute,
      private service: GroupService,
      public menuService: MenuService,
      private i18nService: I18nService,
      private messageService: MessageService,
      private errorHandlerService: ErrorHandlerService) {
  }

  ngOnInit() {
    this.getAll();
  }

  ngOnDestroy(){
    if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
    if(this.deleteSubscrip) this.deleteSubscrip.unsubscribe();
    if(this.getSubscrip) this.getSubscrip.unsubscribe();
  }

  getAll(callback = null){
    this.messageService.showLoading();

    this.getAllSubscrip = this.service.getAll().subscribe(
      d => {
        this.messageService.closeLoading();
        this.data = d;
        if(callback != null){ callback(); }
      },
      err => {
        this.messageService.closeLoading();
        this.errorHandlerService.handleErrors(err);
      });
  }

  getEntity(id: number, callback = null){
    this.getSubscrip = this.service.get(id).subscribe(
      data => {
        if(callback != null){ callback(data); }
      },
      err => this.errorHandlerService.handleErrors(err));
  }

  editClick(id: number){
    this.router.navigate(['/admin/groups/edit/'+id]);
  }

  deleteClick(id: number){
    this.deleteSubscrip = this.service.delete(id).subscribe(
      data => {
        if(data.messages) this.messageService.showMessages(data.messages);

        this.getEntity(id, (e)=>this.dt.updateById(id, e));
      },
      err => this.errorHandlerService.handleErrors(err));
  }

  habInhab(obj: any){
      if (!obj.hab){
          this.deactivate(obj.id);
      } else {
          this.activate(obj.id);
      }
  }

  deactivate(id: number){
      this.deactivateSubscrip = this.service.deactivate(id).subscribe(
          data => {
              if(data.messages) this.messageService.showMessages(data.messages);

              this.getEntity(id, (e)=>this.dt.updateById(id, e));
          },
          err => this.errorHandlerService.handleErrors(err));
  }

  activate(id: number){
      this.activateSubscrip = this.service.activate(id).subscribe(
          data => {
              if(data.messages) this.messageService.showMessages(data.messages);

              this.getEntity(id, (e)=>this.dt.updateById(id, e));
          },
          err => this.errorHandlerService.handleErrors(err));
  }
}