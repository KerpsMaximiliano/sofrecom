import { Component, OnDestroy, ViewChild, OnInit } from "@angular/core";
import { AnalyticService } from "../../../../services/allocation-management/analytic.service";
import { Router, ActivatedRoute } from "@angular/router";
import { MessageService } from "../../../../services/common/message.service";
import { Subscription } from "rxjs";
import { I18nService } from "../../../../services/common/i18n.service";
import { Ng2ModalConfig } from "../../../../components/modal/ng2modal-config";
import { MenuService } from "../../../../services/admin/menu.service";
import { Message } from "app/models/message";
import { getNumberOfCurrencyDigits } from "@angular/common";

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
    refunds: any[] = [];

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
                private activatedRoute: ActivatedRoute){
    }

    ngOnInit(): void {
        this.getAnalytic();
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

                    if(data.proposal && data.proposal != ""){
                        this.form.proposals = data.proposal.split(";");
                    }
                }
            },
            () => {
                    this.messageService.closeLoading();
                });
        });
    }

    edit() {
        this.form.model.title = $('#title').val();

        this.messageService.showLoading();

        this.addSubscrip = this.analyticService.update(this.form.model).subscribe(
            () => {
                this.messageService.closeLoading();
                this.router.navigate(['contracts/analytics']);
            },
            () => {
                this.messageService.closeLoading();
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

    canClose(){
        if(this.menuService.hasFunctionality('CONTR', 'CLOSE') && this.form.model.status != 2) return true;

        return false;
    }

    canForCloseForExpenses(){
        if(this.menuService.hasFunctionality('CONTR', 'CLOSE-EXP') && this.form.model.status == 1) return true;

        return false;
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
            if (this.form.model.refund && this.form.model.refund.filter(f => f.statusId != 20).length != 0) {
                // Alert
                this.getRefunds();
                var msj = `${this.i18nService.translateByKey("allocationManagement.analytics.withRefund")} ${this.refunds.join(',')}`
                this.messageService.showMessage(msj, 2)

                this.confirmModal.hide();
            } else {
                this.closeSubscrip = this.analyticService.close(this.form.model.id).subscribe(response => {
                    this.confirmModal.hide();
                    this.form.model.status = 2;
                });
            }
        }
        else {
            this.closeSubscrip = this.analyticService.closeForExpenses(this.form.model.id).subscribe(response => {
                this.confirmModal.hide();
                this.form.model.status = 3;
            });
        }
    }

    getRefunds() {
        this.form.model.refund.filter(f => f.statusId != 20)
        .forEach(element => {
            if (this.refunds.indexOf(element.analytic.id) == -1)
                this.refunds.push(element.analytic.id);
        });
    }

    goToProjects(){
        sessionStorage.setItem('customerName', this.form.model.clientExternalName);
        sessionStorage.setItem('serviceName', this.form.model.service);
        this.router.navigate([`/billing/customers/${this.form.model.clientExternalId}/services/${this.form.model.serviceId}/projects`]);
    }
} 