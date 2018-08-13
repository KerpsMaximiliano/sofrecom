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
import { ModuleService } from "../../../../services/admin/module.service";
import { I18nService } from '../../../../services/common/i18n.service';

@Component({
  selector: 'app-modules',
  templateUrl: './modules.component.html'
})
export class ModulesComponent implements OnInit, OnDestroy {

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
      { title: this.i18nService.translateByKey("ADMIN.module.title"), columns: [0]},
      0,     //orderByColumn
      "asc"
      ); 

    private dataTypeEnum = DatatablesDataType;
    private alignmentEnum = DatatablesAlignment;

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
        "",   //width
        1,     //visibility
        this.dataTypeEnum.boolean,  //dataType
        this.alignmentEnum.center
      )
    ];

    constructor(
        private router: Router,
        private service: ModuleService,
        private messageService: MessageService,
        private i18nService: I18nService) { }

    ngOnInit() {
      this.getAll();
    }

    ngOnDestroy(){
      if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
      if(this.activateSubscrip) this.activateSubscrip.unsubscribe();
      if(this.deactivateSubscrip) this.deactivateSubscrip.unsubscribe();
      if(this.getSubscrip) this.getSubscrip.unsubscribe();
    }

    getAll(callback = null){
      this.getAllSubscrip = this.service.getAll().subscribe(
        d => {
          this.data = d;
          if(callback != null){
            callback();
          }
      });
    }

    getEntity(id: number, callback = null){
      this.messageService.showLoading();

      this.getSubscrip = this.service.get(id).subscribe(
        data => {
          this.messageService.closeLoading();
          if(callback != null){
            callback(data);
          }
        },
        () => this.messageService.closeLoading());
    }

    editClick(id: number){
      this.router.navigate(['/admin/entities/edit/'+id]);
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
