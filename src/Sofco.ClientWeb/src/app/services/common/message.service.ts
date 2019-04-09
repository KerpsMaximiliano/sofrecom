import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Message } from "../../models/message";
import { I18nService } from './i18n.service';

declare var swal: any;

@Injectable()
export class MessageService {
    private successType = 0;
    private errorType = 1;
    private warningType = 2;
    private ErrorTimeOut = 150000;

    constructor(private toastrService: ToastrService,
                private i18nService: I18nService) {}


    showMessages(messages: Message[]) {
        messages.forEach((value, index) => {
            this.showMessage(this.i18nService.translate(value.folder, value.code), value.type);
        });
    }

    showError(key){
        this.showMessage(this.i18nService.translateByKey(key), this.errorType);
    }

    showWarning(key){
        this.showMessage(this.i18nService.translateByKey(key), this.warningType);
    }

    succes(key, config?: any){
        this.showMessage(this.i18nService.translateByKey(key), this.successType, config);
    }

    showErrorByFolder(folder: any, code: string) {
        this.showMessage(this.i18nService.translate(folder, code), this.errorType);
    }

    showWarningByFolder(folder, code){
        this.showMessage(this.i18nService.translate(folder, code), this.warningType);
    }

    showSuccessByFolder(folder, code){
        this.showMessage(this.i18nService.translate(folder, code), this.successType);
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

    removeMessages(){
        this.toastrService.clear();
    }

    getDefaultConfig(config?: any):any {
        const option = {
            closeButton: true
        };
        for(const key in config)
        {
            option[key] = config[key];
        }
        return option;
    }

    showMessage(msg, type:number, config?: any) {
        const defaultConfig = this.getDefaultConfig(config);
        switch (type) {
            case this.successType: this.toastrService.success(msg, null, defaultConfig); break;
            case this.errorType:
                defaultConfig.timeOut = this.ErrorTimeOut;
                this.toastrService.error(msg, 'Error', defaultConfig);
                break;
            case this.warningType: 
                defaultConfig.timeOut = this.ErrorTimeOut;
                this.toastrService.warning(msg, null, defaultConfig); 
                break;
        }
    }

    showCustomConfirm(msj, callback){
        swal({
            title: `<h2> ${ this.i18nService.translateByKey('ACTIONS.confirmTitle')} </h2>`,
            html: `<h3> ${ msj } </h3>`,
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            allowOutsideClick: false,
            confirmButtonText: this.i18nService.translateByKey('ACTIONS.confirm'),
        })
        .then((result) => {
            if (result.value) {
                callback();
            }
        });
    }

    showConfirm(callback){
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
                callback();
            }
        });
    }
}
