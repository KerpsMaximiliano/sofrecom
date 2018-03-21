import { Component, OnDestroy, ViewChild } from "@angular/core";
import { OnInit } from "@angular/core/src/metadata/lifecycle_hooks";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { Router } from "@angular/router";
import { MessageService } from "app/services/common/message.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs/Subscription";

declare var $: any;

@Component({
    selector: 'new-analytic',
    templateUrl: './new-analytic.component.html'
})
export class NewAnalyticComponent implements OnInit, OnDestroy {

    @ViewChild('form') form;
    addSubscrip: Subscription;

    constructor(private analyticService: AnalyticService,
                private router: Router,
                private messageService: MessageService,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void {
    }

    ngOnDestroy(): void {
        if(this.addSubscrip) this.addSubscrip.unsubscribe();
    }

    add() {
        this.form.model.title = $('#title').val();

        this.messageService.showLoading();

        this.addSubscrip = this.analyticService.add(this.form.model).subscribe(
            data => {
                this.messageService.closeLoading();
              if(data.messages) this.messageService.showMessages(data.messages);
              
              this.back();
            },
            err => {
                this.messageService.closeLoading();
                this.errorHandlerService.handleErrors(err);
            });
    }

    back(){
        window.history.back();
    }
}