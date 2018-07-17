import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { Ng2ModalConfig } from "./ng2modal-config";
declare var $: any;

@Component({
  selector: 'ng2-modal',
  templateUrl: './ng2modal.component.html',
  styleUrls: ['./ng2modal.component.scss']
})
export class Ng2ModalComponent implements OnInit {

  @Input() config: Ng2ModalConfig = new Ng2ModalConfig(
    "Modal Title", //title
    "ModalId",     //id
    true,          //acceptButton
    false,         //cancelButton
    "Accept",      //acceptButtonText
    "Cancel"       //cancelButtonText
  );
  @Output() close = new EventEmitter<any>();
  @Output() accept = new EventEmitter<any>();
  @Output() delete = new EventEmitter<any>();

  @Input() size;

  @Input() isLoading: boolean = false;
  @Input() isSaveEnabled: boolean = true;

  isProcessing: boolean = false;

  @ViewChild('btnDelete') btnDelete;

  constructor() { }

  ngOnInit() {
  }

  show(){
    setTimeout(() => {
      $('#' + this.config.id).modal({
        backdrop: 'static',
        keyboard: false
      });
    }, 0);
  }

  hide(){
    this.resetDeleteButton();
    this.isProcessing = false;
    $('#' + this.config.id).modal('toggle');
  }

  resetDeleteButton(){
    if(this.btnDelete){
      this.btnDelete.reset();
    }
  }

  closeEvent(){
    this.hide();
    this.close.emit();
  }

  acceptEvent(){
    this.isProcessing = true;
    this.accept.emit();
  }

  deleteEvent(){
    this.isProcessing = true;
    this.delete.emit();
  }
}