import { MessageService } from 'app/services/common/message.service';
import { DatatablesLocationTexts } from 'app/components/datatables/datatables.location-texts';
import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { DatatablesEditionType } from "app/components/datatables/datatables.edition-type";
import { DatatablesColumn } from "app/components/datatables/datatables.columns";
import { Subscription } from "rxjs/Subscription";
import { DatatablesOptions } from "app/components/datatables/datatables.options";
import { DatatablesDataType } from "app/components/datatables/datatables.datatype";
import { DatatablesAlignment } from "app/components/datatables/datatables.alignment";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { UserService } from "app/services/admin/user.service";

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html'
})
export class UsersComponent implements OnInit, OnDestroy {

    data;
    getAllSubscrip: Subscription;
    editionType = DatatablesEditionType.ButtonsAtTheEndOfTheRow;
    locationTexts = new DatatablesLocationTexts("Assign");

    @ViewChild('dt') dt; 

    //subscriptions
    deleteSubscrip: Subscription;
    getSubscrip: Subscription;
    habilitarSubscrip: Subscription;

    options = new DatatablesOptions( 
        false,  //edit
        false,  //delete
        true,  //view
        true,  //habInhab
        false,  //other
        false, //other2
        false, //other3
        "fa-compress",     //other1Icon
        "fa-check",      //other2Icon
        "fa-cogs",        //other3Icon
        { title: "Usuarios", columns: [0, 1, 3, 4]}
    ); 

    private dataTypeEnum = DatatablesDataType;
    private alignmentEnum = DatatablesAlignment;

    columns: DatatablesColumn[] = [
        new DatatablesColumn("id", "Id", "", 0, this.dataTypeEnum.number, this.alignmentEnum.left),
        new DatatablesColumn("name", "Nombre", "70px", 1, this.dataTypeEnum.string, this.alignmentEnum.left),
        new DatatablesColumn("email", "Mail", "70px", 1, this.dataTypeEnum.string, this.alignmentEnum.left),
        new DatatablesColumn("active", "Activo", "10px", 1, this.dataTypeEnum.boolean, this.alignmentEnum.center),
        new DatatablesColumn("startDate", "Fecha Alta", "15px", 1, this.dataTypeEnum.date, this.alignmentEnum.center),
        new DatatablesColumn("endDate", "Fecha Baja", "15px", 1, this.dataTypeEnum.date, this.alignmentEnum.center),
    ]

    constructor(
      private router: Router,
      private route: ActivatedRoute,
      private service: UserService,
      private messageService: MessageService,
      private errorHandlerService: ErrorHandlerService) {

      this.options.descripFieldName = "name";
    }

    ngOnInit() {
        this.getAll();
    }

    ngOnDestroy(){
        if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
        if(this.deleteSubscrip) this.deleteSubscrip.unsubscribe();
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
        if(this.habilitarSubscrip) this.habilitarSubscrip.unsubscribe();
    }

    getAll() {
        this.getAllSubscrip = this.service.getAll().subscribe(
            d => {
                this.data = d;
            },
            err => this.errorHandlerService.handleErrors(err));
    }

    goToDetail(id: number) {
        this.router.navigate(['/admin/users/detail/' + id]);
    }

    getEntity(id: number, callback = null){
        this.getSubscrip = this.service.get(id).subscribe(
            data => {
                if(callback != null){
                    callback(data);
                }
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
        this.deleteSubscrip = this.service.deactivate(id).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);

                this.getEntity(id, (e)=>this.dt.updateById(id, e));
            },
            err => this.errorHandlerService.handleErrors(err));
    }

    activate(id: number){
        this.habilitarSubscrip = this.service.activate(id).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);

                this.getEntity(id, (e)=>this.dt.updateById(id, e));
            },
            err => this.errorHandlerService.handleErrors(err));
    }
}