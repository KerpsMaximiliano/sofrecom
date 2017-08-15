import { MessageService } from 'app/services/message.service';
import { DatatablesLocationTexts } from 'app/components/datatables/datatables.location-texts';
import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { DatatablesEditionType } from "app/components/datatables/datatables.edition-type";
import { DatatablesColumn } from "app/components/datatables/datatables.columns";
import { Subscription } from "rxjs/Subscription";
import { DatatablesOptions } from "app/components/datatables/datatables.options";
import { DatatablesDataType } from "app/components/datatables/datatables.datatype";
import { DatatablesAlignment } from "app/components/datatables/datatables.alignment";
import { ProjectService } from "app/services/project.service";

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html'
})
export class ProjectsComponent implements OnInit, OnDestroy {

    data;
    @ViewChild('dt') dt;
    getAllSubscrip: Subscription;
    getSubscrip: Subscription;
    activateSubscrip: Subscription;
    deactivateSubscrip: Subscription;
    paramsSubscrip: Subscription;
    editionType = DatatablesEditionType.ButtonsAtTheEndOfTheRow;
    locationTexts = new DatatablesLocationTexts("Details");
    customerId: number;
    serviceId: number;

    options = new DatatablesOptions(
      false,  //edit
      false,  //delete
      true,  //view
      true,  //habInhab
      false,  //other1
      false, //other2
      false, //other3
      "fa-eye",     //other1Icon
      "fa-check",      //other2Icon
      "fa-cogs",        //other3Icon
      1,     //orderByColumn
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
        "Description",  //title
        "",    //width
        1,     //visibility
        this.dataTypeEnum.string,  //dataType
        this.alignmentEnum.left
      ),
      new DatatablesColumn(
        "active",  //name
        "Active",  //title
        "100px",   //width
        1,     //visibility
        this.dataTypeEnum.boolean,  //dataType
        this.alignmentEnum.center
      ),
      new DatatablesColumn(
        "startDate",  //name
        "Start Date",  //title
        "100px",    //width
        1,     //visibility
        this.dataTypeEnum.date,  //dataType
        this.alignmentEnum.right
      ),
      new DatatablesColumn(
        "endDate",  //name
        "End Date",  //title
        "100px",    //width
        1,     //visibility
        this.dataTypeEnum.date,  //dataType
        this.alignmentEnum.right
      ),
    ];

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private service: ProjectService,
        private messageService: MessageService) { }

    ngOnInit() {
      this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
        this.customerId = params['customerId'];
        this.serviceId = params['serviceId'];
        this.getAll();
      });
      
    }

    ngOnDestroy(){
      if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
      if(this.activateSubscrip) this.activateSubscrip.unsubscribe();
      if(this.deactivateSubscrip) this.deactivateSubscrip.unsubscribe();
      if(this.getSubscrip) this.getSubscrip.unsubscribe();
      if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
    }

    getAll(callback = null){
      this.getAllSubscrip = this.service.getAll(this.customerId).subscribe(d => {
        //d[1].active = true;
        //d[1].startDate = new Date();
        this.data = d;
        if(callback != null){
          callback();
        }
      },
      err => {
        console.log(err);
      });
    }

    getEntity(id: number, callback = null){
      this.getSubscrip = this.service.get(id).subscribe((data) => {
        
        if(callback != null){
          callback(data);
        }
      });
    }

/*
    deleteClick(id: number){
      console.log("Delete ID: " + id);
      this.deleteSubscrip = this.service.deactivate(id).subscribe(
        data => {
          if(data.messages) this.messageService.showMessages(data.messages);

          this.getEntity(id, (e)=>this.dt.updateById(id, e));
        },
        err => {
          var json = JSON.parse(err._body)
          if(json.messages) this.messageService.showMessages(json.messages);
        }
      );
      //this.router.navigate(['/admin/roles/delete/'+id]);
    }*/

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
            err => {
                var json = JSON.parse(err._body)
                if(json.messages) this.messageService.showMessages(json.messages);
            }
        );
    }

    activate(id: number){
        this.activateSubscrip = this.service.activate(id).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);

                this.getEntity(id, (e)=>this.dt.updateById(id, e));
            },
            err => {
                var json = JSON.parse(err._body)
                if(json.messages) this.messageService.showMessages(json.messages);
            }
        );
    }

}
