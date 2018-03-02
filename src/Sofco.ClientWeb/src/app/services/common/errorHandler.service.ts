import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Message } from 'app/models/message';
import { MessageService } from 'app/services/common/message.service';
import { AuthenticationService } from 'app/services/common/authentication.service';

@Injectable()
export class ErrorHandlerService {
    constructor(private messageService: MessageService,
                private authenticationService: AuthenticationService,
                private router: Router){}

    public handleErrors(response){
        this.messageService.closeLoading();

        switch (response.status){
            case 400: this.handle400(response); break;
            case 401: this.handle401(response); break;
            case 500: this.handle500(); break;
            default: break;
        }
    }
 
    private handle401(response) {
        this.authenticationService.logout();
        this.router.navigate(['/login']);
    }

    private handle400(response) {
        const json = JSON.parse(response._body)
        if (json.messages) this.messageService.showMessages(json.messages);
    }

    private handle500(){
        const err = new Message('common', 'generalError', 1);
        this.messageService.showMessages([err]);
    }
}