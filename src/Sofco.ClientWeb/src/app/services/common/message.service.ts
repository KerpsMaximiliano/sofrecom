import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Message } from "../../models/message";
import { I18nService } from './i18n.service';

declare var swal: any;

@Injectable()
export class MessageService {
    constructor(private toastrService: ToastrService,
                private i18nService: I18nService) {}

    showMessages(messages: Message[]) {
        messages.forEach((value, index) => {
            const msg = this.i18nService.translate(value.folder, value.code);

            switch (value.type) {
                case 0: this.toastrService.success(msg, null, this.getDefaultConfig()); break;
                case 1: this.toastrService.error(msg, 'Error', this.getDefaultConfig()); break;
                case 2: this.toastrService.warning(msg, null, this.getDefaultConfig()); break;
            }
        })
    }

    showError(message){
        this.toastrService.error(this.i18nService.translateByKey(message), null, this.getDefaultConfig());
    }

    showWarning(message){
        this.toastrService.warning(this.i18nService.translateByKey(message), null, this.getDefaultConfig());
    }

    succes(message, config?: any){
        this.toastrService.success(this.i18nService.translateByKey(message), null, this.getDefaultConfig(config));
    }

    showErrorByFolder(folder: any, code: string) {
        this.toastrService.error(this.i18nService.translate(folder, code), null, this.getDefaultConfig());
    }

    showWarningByFolder(folder, code){
        this.toastrService.warning(this.i18nService.translate(folder, code), null, this.getDefaultConfig());
    }

    showSuccessByFolder(folder, code){
        this.toastrService.success(this.i18nService.translate(folder, code), null, this.getDefaultConfig());
    }

    showLoading(){
        swal({
            title: 'Procesando !',
            allowOutsideClick: false,
            onOpen: () => {
              swal.showLoading()
            }
          });
    }

    showLoginLoading(){
        swal({
            onOpen: () => {
              swal.showLoading()
            }
          });
    }

    closeLoading(){
        swal.close();
    }

    getDefaultConfig(config?: any):any {
        const option = {
            timeOut: 10000,
            closeButton: true
        };
        for(const key in config)
        {
            option[key] = config[key];
        }
        return option;
    }
}