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
    refunds: number[] = [];
	isCdg: boolean = false;

    @ViewChild('confirmModal') confirmModal;
    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "confirmModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );
    StatusID: any;
    InworkFlowProcess: number;

    @ViewChild('confirmModalReopen') confirmModalReopen;
    public confirmModalReopenConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "confirmModalReopen",
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
        this.isCdg = this.menuService.userIsCdg;

        $('input').attr('disabled', 'disabled');
        $('textarea').attr('disabled', 'disabled');
        $('select').attr('disabled', 'disabled');
        this.form.isReadOnly = true;

        if(this.menuService.hasFunctionality('CONTR', 'DAF-EDIT-ANA')){
            this.form.isEnableForDaf = true;
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

            this.getByIdSubscrip = this.analyticService.getByIdWithOnlyPendingRefunds(params['id']).subscribe(data => {
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

    
    close(){
        if (this.form.model.refund && this.form.model.refund.length != 0) {
            // Alert
            this.getRefunds();
            var msj = this.i18nService.translateByKey("allocationManagement.analytics.withRefund");
            this.messageService.showMessage(msj, 2)
            this.confirmModal.hide();
        } else {

            if(this.statusClose){
                
                this.closeSubscrip = this.analyticService.close(this.form.model.id).subscribe(response => {
                    this.confirmModal.hide();
                    this.form.model.status = 2;
                });
            }
            else 
            {
                this.closeSubscrip = this.analyticService.closeForExpenses(this.form.model.id).subscribe(response => {
                    this.confirmModal.hide();
                    this.form.model.status = 3;
                });
            }
        }
    }

    getRefunds() {
        this.form.model.refund.filter(f => f.statusId != 20)
        .forEach(element => {
            if (this.refunds.indexOf(element.analytic.id) == -1)
                this.refunds.push(element.analytic.id);
        });
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
        // debugger;
        // if(this.StatusID == 20 && this.InworkFlowProcess == 0)
        // {
        this.statusClose = true;
        this.confirmModal.show();
        //}
        //else{ console.log("Agregar alerta")}
    }

    openForCloseForExpenses(){
        this.statusClose = false;
        this.confirmModal.show();
    }
    
    canForCloseForExpenses(){
        if(this.menuService.hasFunctionality('CONTR', 'CLOSE-EXP') && this.form.model.status == 1) return true;

        return false;
    }
    openForReopen(){
        this.statusClose = true;
        this.confirmModalReopen.show();
        console.log("Se abrio el reopen")
    }

    reabrirAnalytic(){
        console.log("Llame al reopen")
        this.form.model.status = 1;
        this.analyticService.reopen(this.form.model.id).subscribe(response => {
            console.log(response)
            this.confirmModalReopen.hide();
        });
        
        console.log("Este es el objeto: ",this.paramsSubscrip);
        //this.back();
    }
}
