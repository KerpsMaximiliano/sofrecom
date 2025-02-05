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
  @Input() otherTitle: string = ""

  // @Input() isLoading: boolean = false;
  @Input() isSaveEnabled = true;

  isProcessing = false;

  @ViewChild('btnDelete') btnDelete;
  @ViewChild('btnSuccess') btnSuccess;

  constructor() { }

  ngOnInit() {
  }

  show(){
    if(this.config.isDraggable){
      setTimeout(() => {
        $('#' + this.config.id).modal({
          backdrop: false,
          keyboard: false
        });

        $('#' + this.config.id).draggable({ handle: ".modal-header" });
      }, 0);
    }
    else{
      setTimeout(() => {
        $('#' + this.config.id).modal({
          backdrop: 'static',
          keyboard: false
        });
      }, 0);
    }
  }

  hide(){
    this.resetButtons();
    $('#' + this.config.id).modal('toggle');
    $('body').css('padding-right', '0px')
  }

  resetButtons(){
    this.isProcessing = false;

    if(this.btnDelete){
      this.btnDelete.reset();
    }

    if(this.btnSuccess){
      this.btnSuccess.reset();
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