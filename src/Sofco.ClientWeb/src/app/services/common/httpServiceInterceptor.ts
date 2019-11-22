import { Injectable } from "@angular/core";
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpResponse, HttpErrorResponse } from "@angular/common/http";
import { Observable } from "rxjs/Observable";
import "rxjs/add/operator/do";
import { ErrorHandlerService } from "./errorHandler.service";
import { MessageService } from "./message.service";

@Injectable()
export class HttpServiceInterceptor implements HttpInterceptor {

    constructor(private errorHandlerService: ErrorHandlerService, private messageService: MessageService) { 
    }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        this.messageService.removeMessages();

        return next.handle(req).do(
            response => {
                if (response instanceof HttpResponse) {
                    if(response.body && response.body.messages && response.body.messages.length > 0){
                        this.messageService.showMessages(response.body.messages);
                    }
                }
            },
            responseError => {
                if (responseError instanceof HttpErrorResponse) {
                    if(responseError.error && responseError.error.messages){
                        this.errorHandlerService.handleErrors(responseError);
                    }
                }
            });
    }
}