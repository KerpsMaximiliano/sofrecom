import { DatatablesLocationTexts } from './../../../components/datatables/datatables.location-texts';
import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { DatatablesEditionType } from "app/components/datatables/datatables.edition-type";
import { DatatablesColumn } from "app/components/datatables/datatables.columns";
import { Subscription } from "rxjs/Subscription";
import { DatatablesOptions } from "app/components/datatables/datatables.options";
import { DatatablesDataType } from "app/components/datatables/datatables.datatype";
import { DatatablesAlignment } from "app/components/datatables/datatables.alignment";
import { Functionality } from 'models/functionality';
import { FunctionalityService } from './../../../services/functionality.service';

@Component({
  selector: 'app-functionalities',
  templateUrl: './functionalities.component.html'
})
export class FunctionalitiesComponent implements OnInit, OnDestroy {

    data;
    getAllSubscrip: Subscription;
    editionType = DatatablesEditionType.ButtonsAtTheEndOfTheRow;
    locationTexts = new DatatablesLocationTexts("Assign");

    options = new DatatablesOptions(
        true,  //edit
        true,  //delete
        false,  //view
        true,  //other
        false, //other2
        false, //other3
        "fa-compress",     //other1Icon
        "fa-check",      //other2Icon
        "fa-cogs"        //other3Icon
    ); 

    private dataTypeEnum = DatatablesDataType;
    private alignmentEnum = DatatablesAlignment;

    columns: DatatablesColumn[] = [
        new DatatablesColumn("id", "Id", "", 1, this.dataTypeEnum.number, this.alignmentEnum.left),
        new DatatablesColumn("description", "Descripción", "60px", 1, this.dataTypeEnum.string, this.alignmentEnum.left),
        new DatatablesColumn("active", "Activo", "10px", 1, this.dataTypeEnum.boolean, this.alignmentEnum.center),
        new DatatablesColumn("startDate", "Fecha Alta", "15px", 1, this.dataTypeEnum.date, this.alignmentEnum.right),
        new DatatablesColumn("endDate", "Fecha Baja", "15px", 1, this.dataTypeEnum.date, this.alignmentEnum.right),
    ]

    constructor(
      private router: Router,
      private route: ActivatedRoute,
      private service: FunctionalityService) { }

    ngOnInit() {
        this.getAll();
    }

    ngOnDestroy(){
        this.getAllSubscrip.unsubscribe();
    }

    getAll() {
        this.getAllSubscrip = this.service.getAll().subscribe(d => {
            this.data = d;
        });
    }
}