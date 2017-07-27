import { Injectable } from '@angular/core';
import { Message } from '../../models/message';
import { ToastrService } from 'toastr-ng2';

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
}