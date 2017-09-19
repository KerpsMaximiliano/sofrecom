import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
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

  constructor() { }

  ngOnInit() {
  }

  show(){
    setTimeout(() => {
      $('#' + this.config.id).modal('toggle');
    });
  }

  hide(){
    setTimeout(() => {
      $('#' + this.config.id).modal('toggle');
    });
  }

  closeEvent($event){
    $event.stopPropagation();
    setTimeout(() => {
      $('#' + this.config.id).modal('toggle');
    });
    this.close.emit();
  }

  acceptEvent($event){
    $event.stopPropagation();
    this.accept.emit();
  }

}