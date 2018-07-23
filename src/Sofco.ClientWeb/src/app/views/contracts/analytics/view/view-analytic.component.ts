import { Component, OnDestroy, ViewChild, OnInit } from "@angular/core";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { Router, ActivatedRoute } from "@angular/router";
import { MessageService } from "app/services/common/message.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs/Subscription";
import { I18nService } from "../../../../services/common/i18n.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { MenuService } from "../../../../services/admin/menu.service";

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
    editSubscrip: Subscription;
    public showClientButton = true;

    @ViewChild('confirmModal') confirmModal;
    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "confirmModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    constructor(private analyticService: AnalyticService,
                private router: Router,
                private i18nService: I18nService,
                public menuService: MenuService,
                private messageService: MessageService,
                private activatedRoute: ActivatedRoute,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void {
        this.getAnalytic();

        $('input').attr('disabled', 'disabled');
        $('textarea').attr('disabled', 'disabled');
        $('select').attr('disabled', 'disabled');

        if(this.menuService.hasFunctionality('ALLOC', 'DAF-EDIT-ANA')){
            $('#softwareLaw select').removeAttr('disabled');
            $('#activity select').removeAttr('disabled');
        }
    }

    ngOnDestroy(): void {
        if (this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if (this.getByIdSubscrip) this.getByIdSubscrip.unsubscribe();
        if (this.closeSubscrip) this.closeSubscrip.unsubscribe();
        if (this.editSubscrip) this.editSubscrip.unsubscribe();
    }

    getAnalytic() {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.messageService.showLoading();

            this.getByIdSubscrip = this.analyticService.getById(params['id']).subscribe(data => {
                this.messageService.closeLoading();
                this.form.model = data;
                this.showClientButton = this.form.model.clientExternalId != null;

                this.form.customerId = this.form.model.clientExternalId;
                this.form.serviceId = this.form.model.serviceId;

                this.form.getServices();

                setTimeout(() => {
                    $('#userId').val(this.form.model.usersQv).trigger('change');
                }, 1000);
            },
            error => {
                this.messageService.closeLoading();
                this.errorHandlerService.handleErrors(error);
            });
        });
    }

    back() {
        this.router.navigate(['/contracts/analytics']);
    }

    close() {
        this.closeSubscrip = this.analyticService.close(this.form.model.id).subscribe(response => {
            this.confirmModal.hide();
            if(response.messages) this.messageService.showMessages(response.messages);
            this.form.model.status = 2;
        },
        err => {
            this.errorHandlerService.handleErrors(err);
        });
    }

    getStatus() {
        switch (this.form.model.status){
            case 1: return this.i18nService.translateByKey("allocationManagement.analytics.status.open");
            case 2: return this.i18nService.translateByKey("allocationManagement.analytics.status.close");
            case 3: return this.i18nService.translateByKey("allocationManagement.analytics.status.closeForExpenses");
        }
    }

    goToProjects() {
        sessionStorage.setItem('customerName', this.form.model.clientExternalName);
        sessionStorage.setItem('serviceName', this.form.model.service);
        this.router.navigate([`/billing/customers/${this.form.model.clientExternalId}/services/${this.form.model.serviceId}/projects`]);
    }

    goToResources(){
        sessionStorage.setItem('analyticName', this.form.model.title + ' - ' + this.form.model.name);
        this.router.navigate([`/contracts/analytics/${this.form.model.id}/resources`]);
    }

    edit() {
        this.messageService.showLoading();

        this.editSubscrip = this.analyticService.updateDaf(this.form.model).subscribe(
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
