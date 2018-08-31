import { MessageService } from '../../../../services/common/message.service';
import { DatatablesLocationTexts } from '../../../../components/datatables/datatables.location-texts';
import { Router } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { DatatablesEditionType } from "../../../../components/datatables/datatables.edition-type";
import { DatatablesColumn } from "../../../../components/datatables/datatables.columns";
import { Subscription } from "rxjs";
import { DatatablesOptions } from "../../../../components/datatables/datatables.options";
import { DatatablesDataType } from "../../../../components/datatables/datatables.datatype";
import { DatatablesAlignment } from "../../../../components/datatables/datatables.alignment";
import { RoleService } from "../../../../services/admin/role.service";
import { MenuService } from "../../../../services/admin/menu.service";
import { I18nService } from '../../../../services/common/i18n.service';

@Component({
  selector: 'app-roles',
  templateUrl: './roles.component.html',
  styleUrls: ['./roles.component.css']
})
export class RolesComponent implements OnInit, OnDestroy {

  data;
  @ViewChild('dt') dt;
  getAllSubscrip: Subscription;
  getSubscrip: Subscription;
  activateSubscrip: Subscription;
  deactivateSubscrip: Subscription;
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
    { title: this.i18nService.translateByKey("ADMIN.ROLES.TITLE"), columns: [0, 2, 3]},
    0,     //orderByColumn
    "asc",
    ); 

  private dataTypeEnum = DatatablesDataType;
  private alignmentEnum = DatatablesAlignment;

  //private activeTitle$;

  columns: DatatablesColumn[] = 
  [
    new DatatablesColumn(
      "id",  //name
      "Id",  //title
      "",    //width
      0,     //visibility
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
      private service: RoleService,
      public menuService: MenuService,
      private messageService: MessageService,
      private i18nService: I18nService) {
  }

  ngOnInit() {
    this.getAll();
  }

  ngOnDestroy(){
    if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
    if(this.activateSubscrip) this.activateSubscrip.unsubscribe();
    if(this.deactivateSubscrip) this.deactivateSubscrip.unsubscribe();
    if(this.getSubscrip) this.getSubscrip.unsubscribe();
  }

  getAll(){
    this.messageService.showLoading();

    this.getAllSubscrip = this.service.getAll().subscribe(
      res => { 
        this.messageService.closeLoading();
        this.data = res;
      },
      () => this.messageService.closeLoading());
  }

  getEntity(id: number, callback = null){
    this.getSubscrip = this.service.get(id).subscribe(
      data => {
        if(callback != null){
          callback(data);
        }
      });
  }

  editClick(id: number){
    this.router.navigate(['/admin/roles/edit/'+id]);
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
          () => {
          this.getEntity(id, (e) => this.dt.updateById(id, e));
        });
  }

  activate(id: number){
      this.activateSubscrip = this.service.activate(id).subscribe(
          () => {
          this.getEntity(id, (e) => this.dt.updateById(id, e));
        });
  }
}
