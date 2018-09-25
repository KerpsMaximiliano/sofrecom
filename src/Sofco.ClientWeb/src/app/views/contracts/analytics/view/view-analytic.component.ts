import { Component, OnDestroy, ViewChild, OnInit } from "@angular/core";
import { AnalyticService } from "../../../../services/allocation-management/analytic.service";
import { Router, ActivatedRoute } from "@angular/router";
import { MessageService } from "../../../../services/common/message.service";
import { Subscription } from "rxjs";
import { I18nService } from "../../../../services/common/i18n.service";
import { Ng2ModalConfig } from "../../../../components/modal/ng2modal-config";
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
    private statusClose: boolean = true;

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
                private activatedRoute: ActivatedRoute){
    }

    ngOnInit(): void {
        this.getAnalytic();

        $('input').attr('disabled', 'disabled');
        $('textarea').attr('disabled', 'disabled');
        $('select').attr('disabled', 'disabled');

        if(this.menuService.hasFunctionality('CONTR', 'DAF-EDIT-ANA')){
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

                if(this.form.model.clientExternalId){
                    this.form.customerId = this.form.model.clientExternalId;
                    this.form.serviceId = this.form.model.serviceId;

                    if(data.proposal && data.proposal != ""){
                        this.form.proposals = data.proposal.split(";");
                    }
                }

                setTimeout(() => {
                    $('#userId').val(this.form.model.usersQv).trigger('change');
                }, 500);
            },
            error => {
                this.messageService.closeLoading();
            });
        });
    }

    back() {
        this.router.navigate(['/contracts/analytics']);
    }

    close() {
        if(this.statusClose){
            this.closeSubscrip = this.analyticService.close(this.form.model.id).subscribe(response => {
                this.closeSuccess(response, 2);
            });
        }
        else {
            this.closeSubscrip = this.analyticService.closeForExpenses(this.form.model.id).subscribe(response => {
                this.closeSuccess(response, 3);
            },
            err => {
                this.confirmModal.hide();
            });
        }
    }

    closeSuccess(response, status){
        this.form.model.status = status;
        this.confirmModal.hide();
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
                this.router.navigate(['contracts/analytics']);
            },
            err => {
                this.messageService.closeLoading();
            });
    }

    openForClose(){
        this.statusClose = true;
        this.confirmModal.show();
    }

    openForCloseForExpenses(){
        this.statusClose = false;
        this.confirmModal.show();
    }
}
