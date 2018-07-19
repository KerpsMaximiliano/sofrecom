import { Component, Input, Output, EventEmitter } from "@angular/core";
import { I18nService } from "../../../services/common/i18n.service";
import { MessageService } from "../../../services/common/message.service";
declare var swal: any;

@Component({
    selector: 'btn-confirm',
    templateUrl: './btn-confirm.html',
  })
  export class ButtonConfirmComponent {

    @Input() class: string;
    @Input() text: string;

    @Output() onConfirm = new EventEmitter<any>();

    constructor(private i18nService: I18nService, private messageService: MessageService){}

    showConfirm(){
        swal({
            title: `<h2> ${ this.i18nService.translateByKey('ACTIONS.confirmTitle')} </h2>`,
            html: `<h3> ${ this.i18nService.translateByKey('ACTIONS.confirmBody')} </h3>`,
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            allowOutsideClick: false,
            confirmButtonText: this.i18nService.translateByKey('ACTIONS.confirm'),
        })
        .then((result) => {
            if (result.value) {
                this.messageService.showLoading();
                this.onConfirm.emit();
            }
        });
    }
  }