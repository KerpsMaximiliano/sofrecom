import { Injectable } from '@angular/core';
import { ToastrService } from 'toastr-ng2';
import { Message } from "app/models/message";

@Injectable()
export class MessageService {
    
    constructor(private toastrService: ToastrService) { }

    showMessages(messages: Message[]) {
        messages.forEach((value, index) => {

            switch(value.type){
                case 0: this.toastrService.success(value.description); break;
                case 1: this.toastrService.error(value.description); break;
                case 2: this.toastrService.warning(value.description); break;
            }
        })
    }

    showError(message: string){
        this.toastrService.error(message);
    }

    showWarning(message: string){
        this.toastrService.warning(message);
    }

    succes(message){
        this.toastrService.success(message);
    }
}