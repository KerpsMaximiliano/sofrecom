import { Component, Input, Output, EventEmitter } from "@angular/core";

@Component({
    selector: 'btn-confirm-inline',
    templateUrl: './btn-confirm-inline.html',
  })
  export class ButtonConfirmInlineComponent {

    @Input() class: string;
    @Input() text: string;

    @Output() onConfirm = new EventEmitter<any>();

    btnConfirmVisible: boolean = false;
    isLoading: boolean = false;

    constructor(){}

    showConfirm(){
        this.btnConfirmVisible = true;
    }
    
    confirm(){
        this.isLoading = true;
        this.onConfirm.emit();
    }

    reset(){
        this.isLoading = false;
        this.btnConfirmVisible = false;
    }
  }