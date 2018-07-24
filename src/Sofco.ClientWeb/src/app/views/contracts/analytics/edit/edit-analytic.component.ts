import { Component, OnDestroy, ViewChild, OnInit } from "@angular/core";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { Router, ActivatedRoute } from "@angular/router";
import { MessageService } from "app/services/common/message.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs";
import { I18nService } from "app/services/common/i18n.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { MenuService } from "../../../../services/admin/menu.service";

declare var $: any;

@Component({
    selector: 'edit-analytic',
    templateUrl: './edit-analytic.component.html',
    styleUrls: ['./edit-analytic.component.scss']
})
export class EditAnalyticComponent implements OnInit, OnDestroy {

    @ViewChild('form') form;
    addSubscrip: Subscription;
    paramsSubscrip: Subscription;
    getByIdSubscrip: Subscription;
    closeSubscrip: Subscription;

    @ViewChild('confirmModal') confirmModal;
    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "confirmModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    private statusClose: boolean = true;

    constructor(private analyticService: AnalyticService,
                private router: Router,
                private messageService: MessageService,
                public menuService: MenuService,
                private i18nService: I18nService,
                private activatedRoute: ActivatedRoute,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void {
        this.getAnalytic();

        $('#customer-select select').attr('disabled', 'disabled');
        $('#service-select select').attr('disabled', 'disabled');
    }

    ngOnDestroy(): void {
        if(this.addSubscrip) this.addSubscrip.unsubscribe();
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

                if(this.form.model.clientExternalId){
                    
                    this.form.customerId = this.form.model.clientExternalId;
                    this.form.serviceId = this.form.model.serviceId;

                    this.form.getServices();
                }

                setTimeout(() => {
                    $('#userId').val(this.form.model.usersQv).trigger('change');
                }, 500);
            },
            error => {
                this.messageService.closeLoading();
                this.errorHandlerService.handleErrors(error);
            });
        });
    }

    edit() {
        this.form.model.title = $('#title').val();
        this.form.model.usersQv = $('#userId').val();

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

    back(){
        window.history.back();
    }

    getStatus(){
        switch(this.form.model.status){
            case 1: return this.i18nService.translateByKey("allocationManagement.analytics.status.open");
            case 2: return this.i18nService.translateByKey("allocationManagement.analytics.status.close");
            case 3: return this.i18nService.translateByKey("allocationManagement.analytics.status.closeForExpenses");
        }
    }

    openForClose(){
        this.statusClose = true;
        this.confirmModal.show();
    }

    openForCloseForExpenses(){
        this.statusClose = false;
        this.confirmModal.show();
    }

    close(){
        if(this.statusClose){
            this.closeSubscrip = this.analyticService.close(this.form.model.id).subscribe(response => {
                this.closeSuccess(response);
                this.form.model.status = 2;
            },
            err => {
                this.errorHandlerService.handleErrors(err);
            });
        }
        else {
            this.closeSubscrip = this.analyticService.closeForExpenses(this.form.model.id).subscribe(response => {
                this.closeSuccess(response);
                this.form.model.status = 3;
            },
            err => {
                this.errorHandlerService.handleErrors(err);
            });
        }
    }

    closeSuccess(response){
        this.confirmModal.hide();
        if(response.messages) this.messageService.showMessages(response.messages);
    }

    goToProjects(){
        sessionStorage.setItem('customerName', this.form.model.clientExternalName);
        sessionStorage.setItem('serviceName', this.form.model.service);
        this.router.navigate([`/billing/customers/${this.form.model.clientExternalId}/services/${this.form.model.serviceId}/projects`]);
    }
} 