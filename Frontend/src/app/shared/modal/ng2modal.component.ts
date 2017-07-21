import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Ng2ModalConfig } from "app/shared/modal/ng2modal-config";
declare var $: any;

@Component({
  selector: 'ng2-modal',
  templateUrl: './ng2modal.component.html',
  styleUrls: ['./ng2modal.component.scss']
})
export class Ng2ModalComponent implements OnInit {

/*  @Input() title: string = "Modal Title";
  @Input() id: string = "MyModal";
  @Input() acceptButton: boolean = true;
  @Input() cancelButton: boolean = true;
  @Input() acceptButtonText: string = "Accept";
  @Input() cancelButtonText: string = "Cancel";*/

  @Input() config: Ng2ModalConfig;
  @Output() close = new EventEmitter<any>();
  @Output() accept = new EventEmitter<any>();

  

  constructor() { 
  }

  ngOnInit() {
  }

  show(){
    setTimeout(() => {
      $('#' + this.config.id).modal('toggle');
    });
  }

  closeEvent(){
    this.close.emit();
  }

  acceptEvent(){
    this.accept.emit();
  }

}