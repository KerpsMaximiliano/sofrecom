
import { Ng2ModalConfig } from './../modal/ng2modal-config';
import { DatatablesDataType } from './datatables.datatype';
import { DatatablesOptions } from './datatables.options';
import { Router } from '@angular/router';
import { DatatablesLocationTexts } from './datatables.location-texts';
import { DatatablesColumn } from './datatables.columns';
import { Component, OnInit, Input, Output, EventEmitter, Directive, ElementRef, OnChanges, ViewChild } from '@angular/core';
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
  public  deleteRowIndex: number = 0;

  public modalConfig: Ng2ModalConfig = new Ng2ModalConfig(
    "fake", //title
    "fake", //id
    true,          //Accept Button
    true,          //Cancel Button
    "fake",     //Accept Button Text
    "fake");   //Cancel Button Text

  private modalDisableConfig: Ng2ModalConfig = new Ng2ModalConfig(
    "Confirmación de baja", //title
    "ng2-datatables-delete", //id
    true,          //Accept Button
    true,          //Cancel Button
    "Deshabilitar",     //Accept Button Text
    "Cancelar");   //Cancel Button Text

  private modalEnableConfig: Ng2ModalConfig = new Ng2ModalConfig(
    "Confirmación de alta", //title
    "ng2-datatables-enable", //id
    true,          //Accept Button
    true,          //Cancel Button
    "Habilitar",     //Accept Button Text
    "Cancelar");   //Cancel Button Text

  @ViewChild("modalNg2Datatables") confirmModal;

  private tableRef: any;
  public modalMessage: string;

  constructor(private router: Router) {
    this.data = [];
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
    this.data = data;
    setTimeout(()=>{
      $('#dt-component').dataTable().fnDestroy();
      this.createTable();
    });
    
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

    var row = this.getIndexById(id);

    for(var i = 0; i< this.columns.length; i++){

      let columnName = this.columns[i].name;

      if(d.hasOwnProperty(columnName)){
        this.data[row][columnName] = d[columnName];
      }
      
    }
  }

  private getIndexById(id: number){
    var index = this.data.findIndex(x => x.id == id);
    return index;
  }

  ngOnChanges(){
  }

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
  }

  deleteClick(id:number){
    this.delete.emit(id);
  }

  viewClick(id:number){
    this.view.emit(id);
  }

  habInhabClick(id:number){
    var index = this.getIndexById(id);
    this.deleteRowIndex = index;

    let active = this.data[index][this.options.activeFieldName];

    if (active){
      this.confirm = this.disableEntity;
      this.modalConfig = this.modalDisableConfig;
      this.modalMessage = this.locationTexts.disableQuestion.replace("¶", this.data[index][this.options.descripFieldName])
      this.confirmModal.show();
    } else {
      this.confirm = this.enableEntity;
      this.modalConfig = this.modalEnableConfig;
      this.modalMessage = this.locationTexts.enableQuestion.replace("¶", this.data[index][this.options.descripFieldName])
      this.confirmModal.show();
    }
  }

  confirm(index: number) {}

  enableEntity(index: number){
    this.doHabInhab(index, false);
  }

  disableEntity(index: number){
    this.doHabInhab(index, true);
  }

  doHabInhab(index: number, active: boolean){

    let id = this.data[index][this.options.idFieldName];

    this.data[index][this.options.activeFieldName] = !active;
    var objRpta = {
      id: id,
      hab: this.data[index][this.options.activeFieldName]
    };
    this.habInhab.emit(objRpta);

    //si es eliminacion cerrar el popup
    this.confirmModal.hide();
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
