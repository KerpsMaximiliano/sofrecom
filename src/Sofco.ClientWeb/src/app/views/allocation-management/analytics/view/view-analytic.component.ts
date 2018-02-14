import { Component, OnDestroy, ViewChild, OnInit } from "@angular/core";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { Router, ActivatedRoute } from "@angular/router";
import { MessageService } from "app/services/common/message.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs/Subscription";
import { I18nService } from "../../../../services/common/i18n.service";

declare var $: any;

@Component({
    selector: 'view-analytic',
    templateUrl: './view-analytic.component.html'
})
export class ViewAnalyticComponent implements OnInit, OnDestroy {

    @ViewChild('form') form;
    paramsSubscrip: Subscription;
    getByIdSubscrip: Subscription;
    closeSubscrip: Subscription;

    constructor(private analyticService: AnalyticService,
                private router: Router,
                private i18nService: I18nService,
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
        if(this.closeSubscrip) this.closeSubscrip.unsubscribe();
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

    back(){
        this.router.navigate(['/contracts/analytics']);
    }

    close(){
        this.closeSubscrip = this.analyticService.close(this.form.model.id).subscribe(response => {
            if(response.messages) this.messageService.showMessages(response.messages);
            this.form.model.status = 2;
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    getStatus(){
        switch(this.form.model.status){
            case 1: return this.i18nService.translateByKey("allocationManagement.analytics.status.open");
            case 2: return this.i18nService.translateByKey("allocationManagement.analytics.status.close");
            case 3: return this.i18nService.translateByKey("allocationManagement.analytics.status.closeForExpenses");
        }
    }
}