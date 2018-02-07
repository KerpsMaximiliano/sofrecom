import { Component, OnDestroy, ViewChild, OnInit } from "@angular/core";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { Router, ActivatedRoute } from "@angular/router";
import { MessageService } from "app/services/common/message.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs/Subscription";

declare var $: any;

@Component({
    selector: 'view-analytic',
    templateUrl: './view-analytic.component.html'
})
export class ViewAnalyticComponent implements OnInit, OnDestroy {

    @ViewChild('form') form;
    paramsSubscrip: Subscription;
    getByIdSubscrip: Subscription;

    constructor(private analyticService: AnalyticService,
                private router: Router,
                private messageService: MessageService,
                private activatedRoute: ActivatedRoute,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void {
        this.getAnalytic();

        $('input').attr('disabled', 'disabled');
        $('textarea').attr('disabled', 'disabled');
        $('select').attr('disabled', 'disabled');
    }

    ngOnDestroy(): void {
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.getByIdSubscrip) this.getByIdSubscrip.unsubscribe();
    }

    getAnalytic(){
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.messageService.showLoading();

            this.getByIdSubscrip = this.analyticService.getById(params['id']).subscribe(data => {
                this.messageService.closeLoading();
                this.form.model = data;
            },
            error => {
                this.messageService.closeLoading();
                this.errorHandlerService.handleErrors(error);
            });
        });
    }
}