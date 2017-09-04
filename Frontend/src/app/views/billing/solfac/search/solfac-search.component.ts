import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { SolfacService } from "app/services/billing/solfac.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";

import { DatatablesOptions } from "app/components/datatables/datatables.options";
import { DatatablesDataType } from "app/components/datatables/datatables.datatype";
import { DatatablesAlignment } from "app/components/datatables/datatables.alignment";
import { DatatablesEditionType } from "app/components/datatables/datatables.edition-type";
import { DatatablesColumn } from "app/components/datatables/datatables.columns";
import { DatatablesLocationTexts } from 'app/components/datatables/datatables.location-texts';

@Component({
  selector: 'app-solfacSearch',
  templateUrl: './solfac-search.component.html'
})
export class SolfacSearchComponent implements OnInit, OnDestroy {
    getAllSubscrip: Subscription;
    data;

    editionType = DatatablesEditionType.ButtonsAtTheEndOfTheRow;
    locationTexts = new DatatablesLocationTexts("Assign");

    @ViewChild('dt') dt; 
  
    options = new DatatablesOptions( 
        false,  //edit
        false,  //delete
        true,  //view
        false,  //habInhab
        false,  //other
        false, //other2
        false, //other3
        "fa-compress",     //other1Icon
        "fa-check",      //other2Icon
        "fa-cogs",        //other3Icon
        { title: "Solicitudes", columns: [0, 1, 2, 3, 4, 5]}
    ); 

    private dataTypeEnum = DatatablesDataType;
    private alignmentEnum = DatatablesAlignment;

    columns: DatatablesColumn[] = [
        new DatatablesColumn("id", "Id", "", 0, this.dataTypeEnum.number, this.alignmentEnum.left),
        new DatatablesColumn("project", "Proyecto", "", 1, this.dataTypeEnum.string, this.alignmentEnum.left),
        new DatatablesColumn("businessName", "Razón Social", "", 1, this.dataTypeEnum.string, this.alignmentEnum.left),
        new DatatablesColumn("documentTypeName", "Tipo Doc.", "", 1, this.dataTypeEnum.string, this.alignmentEnum.left),
        new DatatablesColumn("startDate", "Fecha", "", 1, this.dataTypeEnum.date, this.alignmentEnum.center),
        new DatatablesColumn("totalAmount", "Total", "", 1, this.dataTypeEnum.currency, this.alignmentEnum.center),
        new DatatablesColumn("statusName", "Estado", "", 1, this.dataTypeEnum.labelWarning, this.alignmentEnum.center),
    ]

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private service: SolfacService,
        private errorHandlerService: ErrorHandlerService) { }

    ngOnInit() {
        this.getAllSubscrip = this.service.getAll().subscribe(data => {
            this.data = data;
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    ngOnDestroy(){
      if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
    }

    setCurrencySymbol(currencyId){
      switch(currencyId){
        case 1: { return "$";  }
        case 2: { return "U$D"; }
        case 3: { return "€"; }
      }
    }

    goToDetail(id: number) {
        this.router.navigate(["/billing/solfac/" + id]);
    }
}
