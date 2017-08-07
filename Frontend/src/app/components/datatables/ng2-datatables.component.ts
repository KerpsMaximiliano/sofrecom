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
  public visibleColumns: DatatablesColumn[];
  @Input() idColumnName: string = "id";
  @Input() editionType: number = DatatablesEditionType.ButtonsAtTheEndOfTheRow;
  @Input() locationTexts = new DatatablesLocationTexts();
  @Output() edit = new EventEmitter<number>();
  @Output() delete = new EventEmitter<number>();
  @Output() view = new EventEmitter<number>();
  @Output() habInhab = new EventEmitter<any>();  
  @Output() other1 = new EventEmitter<number>();
  @Output() other2 = new EventEmitter<number>();
  @Output() other3 = new EventEmitter<number>();
  @Input() callDefaultUrls: boolean = true;
  public editionTypeEnum = DatatablesEditionType;
  private actionsColumnWidth:number = 0;
  private dataTypeEnum = DatatablesDataType;
  private alignmentEnum = DatatablesAlignment;

  private tableRef: any;
  //dtInstance: Promise<DataTables.Api>;

  constructor(private router: Router) {
    this.data = [];

    
    //establecer un objeto inicial para IE no de error
    /*let obj = {};

    for(var i = 0; i< this.columns.length; i++){
      if(i == 2){
        obj[this.columns[i].name] = this.columns[i].name;
      } else{
        obj[this.columns[i].name] = "No hay datos";
      }
      
    }

    this.data.push(obj);*/

  }
    
  ngOnInit() {

    this.visibleColumns = this.columns.filter(col => col.visibility);

    this.setActionsColumnWidth();

    this.createTable();
    
  }

  createTable(){

    let arrOrder = [[this.options.orderByColumn, this.options.orderByAscDesc]];


    setTimeout(()=>{
          $( document ).ready(function() {
            this.tableRef = $('#dt-component').DataTable({
              dom: 'Bfrtp',
              order: arrOrder,//[this.options.orderByColumn, this.options.orderByAscDesc]
              oLanguage: {"sZeroRecords": "", "sEmptyTable": ""},
              buttons: [
                'excelHtml5',
                'pdfHtml5'
              ]
            });
          });
    });
  }

  refresh(data){
    //$('#dt-component').clear();
    this.data = data;
    setTimeout(()=>{
      $('#dt-component').dataTable().fnDestroy();
      this.createTable();
    });
    
    //var table = $('#dt-component').dataTable().api();
    //table.order([1, "asc"]);
    //table.draw();

  }

  updateRow(row: number, d: any){

    for(var i = 0; i< this.columns.length; i++){

      let columnName = this.columns[i].name;

      if(d.hasOwnProperty(columnName)){
        this.data[row][columnName] = d[columnName];
      }
      
    }
  }

  updateById(id: number, d: any){

    //var row = this.data.findIndex((e, i, a) => {if (e.id == id) return i});
    var row = this.getIndexById(id);

    for(var i = 0; i< this.columns.length; i++){

      let columnName = this.columns[i].name;

      if(d.hasOwnProperty(columnName)){
        this.data[row][columnName] = d[columnName];
      }
      
    }
  }

  private getIndexById(id: number){
    /*var index = this.data.findIndex((e, i, a) => {
      if (e.id == id) return i
    });*/
    var index = this.data.findIndex(x => x.id == id);
    
    return index;
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
    if(this.options.b_view){ this.actionsColumnWidth += 28 }
    if(this.options.b_habInhab){ this.actionsColumnWidth += 28 }
    if(this.options.b_other1){ this.actionsColumnWidth += 28 }
    if(this.options.b_other2){ this.actionsColumnWidth += 28 }
    if(this.options.b_other3){ this.actionsColumnWidth += 28 }
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

  habInhabClick(id:number){
    var index = this.getIndexById(id);
    this.data[index][this.options.activeFieldName] = !this.data[index][this.options.activeFieldName];
    var objRpta = {
      id: id,
      hab: this.data[index][this.options.activeFieldName]
    };
    this.habInhab.emit(objRpta);
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
