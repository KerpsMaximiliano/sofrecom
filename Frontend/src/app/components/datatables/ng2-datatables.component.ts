import { DatatablesDataType } from './datatables.datatype';
import { DatatablesOptions } from './datatables.options';
import { Router } from '@angular/router';
import { DatatablesLocationTexts } from './datatables.location-texts';
import { DatatablesColumn } from './datatables.columns';
import { Component, OnInit, Input, Output, EventEmitter, Directive, ElementRef, OnChanges } from '@angular/core';
import { DatatablesEditionType } from "./datatables.edition-type";
import { DatatablesAlignment } from "./datatables.alignment";
import { Subject } from 'rxjs/Rx';
import { BehaviorSubject } from "rxjs/BehaviorSubject";
declare var $:any;

@Component({
  selector: 'datatables',
  templateUrl: './datatables.component.html',
  styleUrls: ['./datatables.component.scss']
})
export class Ng2DatatablesComponent implements OnInit, OnChanges {

  @Input() data;
  @Input() options = new DatatablesOptions();
  @Input() columns: DatatablesColumn[];
  @Input() idColumnName: string = "id";
  @Input() editionType: number = DatatablesEditionType.ButtonsAtTheEndOfTheRow;
  @Input() locationTexts = new DatatablesLocationTexts();
  @Output() edit = new EventEmitter<number>();
  @Output() delete = new EventEmitter<number>();
  @Output() view = new EventEmitter<number>();
  @Output() other1 = new EventEmitter<number>();
  @Output() other2 = new EventEmitter<number>();
  @Output() other3 = new EventEmitter<number>();
  @Input() callDefaultUrls: boolean = true;
  private editionTypeEnum = DatatablesEditionType;
  private actionsColumnWidth:number = 0;
  private dataTypeEnum = DatatablesDataType;
  private alignmentEnum = DatatablesAlignment;
  //dtInstance: Promise<DataTables.Api>;

  constructor(private router: Router) {
  }
    
  ngOnInit() {

    this.setActionsColumnWidth();

    setTimeout(()=>{
        $( document ).ready(function() {
          $('#dt-component').DataTable({
            dom: 'Bfrtp',
            //"ajax": 'http://localhost:49963/api/role',
            oLanguage: {"sZeroRecords": "", "sEmptyTable": ""},
            buttons: [
              'copy', 'excel', 'pdf'
            ]
          });
        });
    });
    
  }

  ngOnChanges(){
    //$('#dt-component').DataTable().ajax.reload();
  }

  /*private displayTable(): void {
    this.dtInstance = new Promise((resolve, reject) => {
      Promise.resolve(this.dtOptions).then(dtOptions => {
        // Using setTimeout as a "hack" to be "part" of NgZone
        setTimeout(() => {
          const dt = $(this.el.nativeElement).DataTable(dtOptions);
          resolve(dt);
        });
      });
    });
  }*/

  private setActionsColumnWidth(){
    this.actionsColumnWidth = 0;
    if(this.options.b_delete){ this.actionsColumnWidth += 28 }
    if(this.options.b_edit){ this.actionsColumnWidth += 28 }
    if(this.options.b_other1){ this.actionsColumnWidth += 28 }
    if(this.options.b_other2){ this.actionsColumnWidth += 28 }
    if(this.options.b_other3){ this.actionsColumnWidth += 28 }
    if(this.options.b_view){ this.actionsColumnWidth += 28 }
    this.actionsColumnWidth += 10;
  }

  editClick(id:number){
    this.edit.emit(id);
    /*if(this.callDefaultUrls){
      this.router.navigate(['edit', id]);
    }*/
    
  }

  deleteClick(id:number){
    this.delete.emit(id);
    /*if(this.callDefaultUrls){
      this.router.navigate(['delete', id]);
    }*/
  }

  viewClick(id:number){
    this.view.emit(id);
    /*if(this.callDefaultUrls){
      this.router.navigate(['view', id]);
    }*/
  }

  other1Click(id:number){
    this.other1.emit(id);
  }

  other2Click(id:number){
    this.other2.emit(id);
  }

  other3Click(id:number){
    this.other3.emit(id);
  }

  onSearch(event){

  }

}
