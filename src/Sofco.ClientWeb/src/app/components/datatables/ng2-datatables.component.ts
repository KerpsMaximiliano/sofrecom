
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
import { BehaviorSubject } from "rxjs";
import { MenuService } from 'app/services/admin/menu.service';
import { Configuration } from 'app/services/common/configuration';
declare var $: any;

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
  @Input() module: string = "";

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
    "ACTIONS.confirmBody", //title
    "modalConfirm", //id
    true,          //Accept Button
    true,          //Cancel Button
    "ACTIONS.DEACTIVATE",     //Accept Button Text
    "ACTIONS.cancel");   //Cancel Button Text

  @ViewChild("modalNg2Datatables") confirmModal;

  private tableRef: any;
  public modalMessage: string;

  constructor(private router: Router, private menuService: MenuService, private config: Configuration) {
    this.data = [];
  }
    
  ngOnInit() {

    this.visibleColumns = this.columns.filter(col => col.visibility);

    this.setActionsColumnWidth();

    this.createTable();
  }

  createTable(){

    let arrOrder = [[this.options.orderByColumn, this.options.orderByAscDesc]];
    let title = this.options.exportOptions.title + "_" + this.getDateForFile();
    let columns = this.options.exportOptions.columns;
 
    var lang = {};
    if(this.config.currLang == "es" || this.config.currLang == "fr"){
      lang = this.getLanguageEs();
    }

    setTimeout(()=>{
          $( document ).ready(function() {
            this.tableRef = $('#dt-component').DataTable({
              dom: '<"html5buttons"B>lTfgitp',
              order: arrOrder,//[this.options.orderByColumn, this.options.orderByAscDesc]
              buttons: [
                {
                    extend: 'excelHtml5', title: title,
                    exportOptions: {
                        columns: columns
                    }
                },
                {
                  extend: 'pdfHtml5', title: title,
                    exportOptions: {
                        columns: columns
                    }
                }
              ],
              scrollX: true,
              oSearch: { "bSmart": false, "bRegex": true },
              language: lang
            });
          });
    });
  }

  getLanguageEs(){
    var language =
     {
         sProcessing: "Procesando...",
         sLengthMenu: "Mostrar _MENU_ registros",
         sZeroRecords: "No se encontraron resultados",
         sEmptyTable: "Sin información disponible para esta tabla",
         sInfo: "Mostrando del registro _START_ al _END_ de un total de _TOTAL_",
         sInfoEmpty: "Mostrando del registro 0 al 0 de un total de 0",
         sInfoFiltered: "(filtrado de un total de _MAX_ registros)",
         sInfoPostFix: "",
         sSearch: "Buscar:",
         sUrl: "",
         sInfoThousands: ",",
         sLoadingRecords: "Cargando...",
         oPaginate: {
             sFirst: "Primero",
             sLast: "Último",
             sNext: "Siguiente",
             sPrevious: "Anterior"
         },
         oAria: {
             sSortAscending: ": Activar para ordenar la columna de manera ascendente",
             sSortDescending: ": Activar para ordenar la columna de manera descendente"
         }
     };

     return language;
 }

  getDateForFile(){
      var date = new Date();

      var yyyy = date.getFullYear().toString();
      var mm = (date.getMonth()+1).toString();
      var dd  = date.getDate().toString();

      var mmChars = mm.split('');
      var ddChars = dd.split('');

      return yyyy + (mmChars[1]?mm:"0"+mmChars[0]) + (ddChars[1]?dd:"0"+ddChars[0]);
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

  canEdit(){
    return this.menuService.hasFunctionality(this.module, "UPDAT");
  }

  canHabInhab(){
    return this.menuService.hasFunctionality(this.module, "HABIN");
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
      this.modalConfig.acceptButtonText = "ACTIONS.DEACTIVATE";
      this.modalMessage = this.locationTexts.disableQuestion.replace("¶", this.data[index][this.options.descripFieldName])
      this.confirmModal.show();
    } else {
      this.confirm = this.enableEntity;
      this.modalConfig.acceptButtonText = "ACTIONS.ACTIVATE"
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
