import { Injectable } from '@angular/core';
import { ToastrService } from 'toastr-ng2';
import { Message } from "app/models/message";
import { I18nService } from 'app/services/common/i18n.service';

@Injectable()
export class MessageService {
    
    constructor(private toastrService: ToastrService,
                private i18nService: I18nService) { }

    showMessages(messages: Message[]) {
        messages.forEach((value, index) => {

            var msg = this.i18nService.translate(value.folder, value.code);

            switch(value.type){
                case 0: this.toastrService.success(msg); break;
                case 1: this.toastrService.error(msg); break;
                case 2: this.toastrService.warning(msg); break;
            }
        })
    }

    showError(message){
        this.toastrService.error(this.i18nService.translateByKey(message));
    }

    showWarning(message){
        this.toastrService.warning(this.i18nService.translateByKey(message));
    }

    succes(message){
        this.toastrService.success(this.i18nService.translateByKey(message));
    }

    showErrorByFolder(folder: any, code: string) {
        this.toastrService.error(this.i18nService.translate(folder, code));
    }

    showWarningByFolder(folder, code){
        this.toastrService.warning(this.i18nService.translate(folder, code));
    }

    showSuccessByFolder(folder, code){
        this.toastrService.success(this.i18nService.translate(folder, code));
    }
}