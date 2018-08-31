import { DatatablesLocationTexts } from '../../../components/datatables/datatables.location-texts';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { DatatablesEditionType } from "../../../components/datatables/datatables.edition-type";
import { DatatablesColumn } from "../../../components/datatables/datatables.columns";
import { Subscription } from "rxjs";
import { DatatablesOptions } from "../../../components/datatables/datatables.options";
import { DatatablesDataType } from "../../../components/datatables/datatables.datatype";
import { DatatablesAlignment } from "../../../components/datatables/datatables.alignment";
import { MessageService } from "../../../services/common/message.service";
import { FunctionalityService } from "../../../services/admin/functionality.service";
import { I18nService } from '../../../services/common/i18n.service';

@Component({
  selector: 'app-functionalities',
  templateUrl: './functionalities.component.html'
})
export class FunctionalitiesComponent implements OnInit, OnDestroy {

    data;
    @ViewChild('dt') dt;
    getAllSubscrip: Subscription;
    getSubscrip: Subscription;
    activateSubscrip: Subscription;
    deactivateSubscrip: Subscription;
    editionType = DatatablesEditionType.ButtonsAtTheEndOfTheRow;
    locationTexts = new DatatablesLocationTexts("Details");

    options = new DatatablesOptions(
      false,  //edit
      false,  //delete
      false,  //view
      true,  //habInhab
      false,  //other1
      false, //other2
      false, //other3
      "fa-eye",     //other1Icon
      "fa-check",      //other2Icon
      "fa-cogs",        //other3Icon
      { title: this.i18nService.translateByKey("ADMIN.FUNCTIONALITIES.TITLE"), columns: [0]},
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
        private service: FunctionalityService,
        private i18nService: I18nService,
        private messageService: MessageService) { }

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
      this.messageService.showLoading();

      this.getAllSubscrip = this.service.getAll().subscribe(
        d => {
          this.messageService.closeLoading();
          this.data = d;
          
          if(callback != null){
            callback();
          }
        },
        err => this.messageService.closeLoading());
    }

    getEntity(id: number, callback = null){
      this.getSubscrip = this.service.get(id).subscribe(
        data => {
          if(callback != null){
            callback(data);
          }
        });
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
                this.getEntity(id, (e)=>this.dt.updateById(id, e));
            });
    }

    activate(id: number){
        this.activateSubscrip = this.service.activate(id).subscribe(
            data => {
                this.getEntity(id, (e)=>this.dt.updateById(id, e));
            });
    }

}
