import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { HitoDetail } from "app/models/billing/solfac/hitoDetail";
import { SolfacService } from "app/services/billing/solfac.service";
import { Router, ActivatedRoute } from '@angular/router';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { Cookie } from "ng2-cookies/ng2-cookies";
import * as FileSaver from "file-saver";
import { InvoiceService } from "app/services/billing/invoice.service";
import { SolfacStatus } from "app/models/enums/solfacStatus";
import { MessageService } from "app/services/common/message.service";
import { MenuService } from "app/services/admin/menu.service";
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';

@Component({
  selector: 'app-solfac-detail',
  templateUrl: './solfac-detail.component.html',
  styleUrls: ['./solfac-detail.component.scss']
})
export class SolfacDetailComponent implements OnInit, OnDestroy {

    @ViewChild('confirmModal') confirmModal;
    @ViewChild('rejectModal') rejectModal;
    @ViewChild('commentsModal') commentsModal;

    public rejectModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "billing.solfac.addComments",
        "rejectModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "confirmModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    public commentsModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "comments",
        "commentsModal",
        false,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.close"
    );

    public model: any = {};
    private solfacId: any;
    public currencySymbol: string = "$";
    public histories: any[] = new Array<any>();
    public rejectComments: string;
    public historyComments: string;

    paramsSubscrip: Subscription;
    getDetailSubscrip: Subscription;
    getHistoriesSubscrip: Subscription;
    changeStatusSubscrip: Subscription;

    constructor(private solfacService: SolfacService,
                private activatedRoute: ActivatedRoute,
                private invoiceService: InvoiceService,
                private messageService: MessageService,
                private menuService: MenuService,
                private errorHandlerService: ErrorHandlerService,
                private router: Router) { }

    ngOnInit() {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.solfacId = params['solfacId'];
            this.getSolfac();
            this.getHistories();
        });
    }

    ngOnDestroy(){
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.getDetailSubscrip) this.getDetailSubscrip.unsubscribe();
        if(this.changeStatusSubscrip) this.changeStatusSubscrip.unsubscribe();
        if(this.getHistoriesSubscrip) this.getHistoriesSubscrip.unsubscribe();
    }

    getSolfac(){
        this.getDetailSubscrip = this.solfacService.get(this.solfacId).subscribe(d => {
            this.model = d;
            this.setCurrencySymbol(this.model.currencyId);
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    getHistories(){
        this.getDetailSubscrip = this.solfacService.getHistories(this.solfacId).subscribe(d => {
            this.histories = d;
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    setCurrencySymbol(currencyId){
      switch(currencyId){
        case 1: { this.currencySymbol = "$"; break; }
        case 2: { this.currencySymbol = "U$D"; break; }
        case 3: { this.currencySymbol = "€"; break; }
      }
    }

    goToProject(){
      this.router.navigate([`/billing/customers/${this.model.customerId}/services/${this.model.serviceId}/projects/${this.model.projectId}`]);
    }

    goToSearch(){
      this.router.navigate([`/billing/solfac/search`]);
    }

    exportPdf(){
        this.invoiceService.getPdf(this.model.invoiceId).subscribe(file => {
            FileSaver.saveAs(file, this.model.pdfFileName);
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    canSendToCDG(){
        if((this.model.statusName == SolfacStatus[SolfacStatus.SendPending] || 
           this.model.statusName == SolfacStatus[SolfacStatus.ManagementControlRejected])
           && this.menuService.hasFunctionality("SOLFA", "SCDG")){

            return true;
        }

        return false;
    }

    canRejectByCDG(){
        if(this.model.statusName == SolfacStatus[SolfacStatus.PendingByManagementControl] &&
           this.menuService.hasFunctionality("SOLFA", "REJEC")){
            return true;
        }

        return false;
    }

    canSendToDAF(){
        if(this.model.statusName == SolfacStatus[SolfacStatus.PendingByManagementControl] &&
           this.menuService.hasFunctionality("SOLFA", "SDAF")){
            return true;
        }

        return false;
    }

    canSendToBill(){
        if(this.model.statusName == SolfacStatus[SolfacStatus.InvoicePending] && 
           this.menuService.hasFunctionality("SOLFA", "BILL")){
            return true;
        }

        return false;
    }

    canSendToCash(){
        if(this.model.statusName == SolfacStatus[SolfacStatus.Invoiced] && 
           this.menuService.hasFunctionality("SOLFA", "CASH")){
            return true;
        }

        return false;
    }

    canDelete(){
        if(this.model.statusName == SolfacStatus[SolfacStatus.SendPending] || 
           this.model.statusName == SolfacStatus[SolfacStatus.ManagementControlRejected]){
            return true;
        }

        return false;
    }

    sendToCDG(){
        this.changeStatusSubscrip = this.solfacService.changeStatus(this.model.id, SolfacStatus.PendingByManagementControl, "").subscribe(
            data => {
                this.confirmModal.hide();
                if(data.messages) this.messageService.showMessages(data.messages);
                this.model.statusName = SolfacStatus[SolfacStatus.PendingByManagementControl];
                this.getHistories();
            },
            error => {
                this.confirmModal.hide();
                this.errorHandlerService.handleErrors(error);
            });
    }

    rejectByCDG(){
        if(!this.rejectComments || this.rejectComments == ""){
            this.messageService.showError("Se debe agregar un motivo de rechazo");
            return;
        }

        this.changeStatusSubscrip = this.solfacService.changeStatus(this.model.id, SolfacStatus.ManagementControlRejected, this.rejectComments).subscribe(
            data => {
                this.rejectModal.hide();
                if(data.messages) this.messageService.showMessages(data.messages);
                this.model.statusName = SolfacStatus[SolfacStatus.ManagementControlRejected];
                this.getHistories();
            },
            error => {
                this.rejectModal.hide();
                this.errorHandlerService.handleErrors(error);
            });
    }

    sendToDAF(){
        this.changeStatusSubscrip = this.solfacService.changeStatus(this.model.id, SolfacStatus.InvoicePending, "").subscribe(
            data => {
                this.confirmModal.hide();
                if(data.messages) this.messageService.showMessages(data.messages);
                this.model.statusName = SolfacStatus[SolfacStatus.InvoicePending];
                this.getHistories();
            },
            error => {
                this.confirmModal.hide();
                this.errorHandlerService.handleErrors(error);
            });
    }

    sendToBill(){
        this.changeStatusSubscrip = this.solfacService.changeStatus(this.model.id, SolfacStatus.Invoiced, "").subscribe(
            data => {
                this.confirmModal.hide();
                if(data.messages) this.messageService.showMessages(data.messages);
                this.model.statusName = SolfacStatus[SolfacStatus.Invoiced];
                this.getHistories();
            },
            error => {
                this.confirmModal.hide();
                this.errorHandlerService.handleErrors(error);
            });
    }

    sendToCash(){
        this.changeStatusSubscrip = this.solfacService.changeStatus(this.model.id, SolfacStatus.AmountCashed, "").subscribe(
            data => {
                this.confirmModal.hide();
                if(data.messages) this.messageService.showMessages(data.messages);
                this.model.statusName = SolfacStatus[SolfacStatus.AmountCashed];
                this.getHistories();
            },
            error => {
                this.confirmModal.hide();
                this.errorHandlerService.handleErrors(error);
            });
    }

    delete(){
        this.solfacService.delete(this.model.id).subscribe(data => {
            this.confirmModal.hide();
            if(data.messages) this.messageService.showMessages(data.messages);

            setTimeout(() => { this.goToProject(); }, 1500)
        },
        err => {
            this.confirmModal.hide();
            this.errorHandlerService.handleErrors(err);
        });
    }

    confirm() {}
    
    showConfirmSendToCDG(){
        this.confirm = this.sendToCDG;
        this.confirmModal.show();
    }

    showConfirmRejectByCDG(){
        this.rejectModal.show();
    }

    showConfirmSendToDaf(){
        this.confirm = this.sendToDAF;
        this.confirmModal.show();
    }

    showConfirmSendToBill(){
        this.confirm = this.sendToBill;
        this.confirmModal.show();
    }

    showConfirmSendToCash(){
        this.confirm = this.sendToCash;
        this.confirmModal.show();
    }

    showConfirmDelete(){
        this.confirm = this.delete;
        this.confirmModal.show();
    }

    showComments(history){
        this.historyComments = history.comment;
        this.commentsModal.show();
    }
}