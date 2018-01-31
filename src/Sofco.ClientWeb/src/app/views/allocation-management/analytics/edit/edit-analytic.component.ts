import { Component, OnDestroy, ViewChild, OnInit } from "@angular/core";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { Router, ActivatedRoute } from "@angular/router";
import { MessageService } from "app/services/common/message.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs/Subscription";

declare var $: any;

@Component({
    selector: 'edit-analytic',
    templateUrl: './edit-analytic.component.html'
})
export class EditAnalyticComponent implements OnInit, OnDestroy {

    @ViewChild('form') form;
    addSubscrip: Subscription;
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
    }

    ngOnDestroy(): void {
        if(this.addSubscrip) this.addSubscrip.unsubscribe();
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

    edit() {
        this.form.model.title = $('#title').val();

        this.messageService.showLoading();

        this.addSubscrip = this.analyticService.update(this.form.model).subscribe(
            data => {
                this.messageService.closeLoading();
                if(data.messages) this.messageService.showMessages(data.messages);
                this.router.navigate(['contracts/analytics']);
            },
            err => {
                this.messageService.closeLoading();
                this.errorHandlerService.handleErrors(err);
            });
    }
}