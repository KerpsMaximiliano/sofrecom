import { Component, OnInit, OnDestroy} from '@angular/core';
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

@Component({
  selector: 'app-solfac-detail',
  templateUrl: './solfac-detail.component.html',
  styleUrls: ['./solfac-detail.component.scss']
})
export class SolfacDetailComponent implements OnInit, OnDestroy {

    public model: any = {};
    private solfacId: any;
    public currencySymbol: string = "$";

    paramsSubscrip: Subscription;
    getDetailSubscrip: Subscription;
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
        });
    }

    ngOnDestroy(){
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.getDetailSubscrip) this.getDetailSubscrip.unsubscribe();
        if(this.changeStatusSubscrip) this.changeStatusSubscrip.unsubscribe();
    }

    getSolfac(){
        this.getDetailSubscrip = this.solfacService.get(this.solfacId).subscribe(d => {
            this.model = d;
            this.setCurrencySymbol(this.model.currencyId);
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
        this.changeStatusSubscrip = this.solfacService.changeStatus(this.model.id, SolfacStatus.PendingByManagementControl).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                this.model.statusName = SolfacStatus[SolfacStatus.PendingByManagementControl];
            },
            error => this.errorHandlerService.handleErrors(error));
    }

    rejectByCDG(){
        this.changeStatusSubscrip = this.solfacService.changeStatus(this.model.id, SolfacStatus.ManagementControlRejected).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                this.model.statusName = SolfacStatus[SolfacStatus.ManagementControlRejected];
            },
            error => this.errorHandlerService.handleErrors(error));
    }

    sendToDAF(){
        this.changeStatusSubscrip = this.solfacService.changeStatus(this.model.id, SolfacStatus.InvoicePending).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                this.model.statusName = SolfacStatus[SolfacStatus.InvoicePending];
            },
            error => this.errorHandlerService.handleErrors(error));
    }

    sendToBill(){
        this.changeStatusSubscrip = this.solfacService.changeStatus(this.model.id, SolfacStatus.Invoiced).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                this.model.statusName = SolfacStatus[SolfacStatus.Invoiced];
            },
            error => this.errorHandlerService.handleErrors(error));
    }

    sendToCash(){
        this.changeStatusSubscrip = this.solfacService.changeStatus(this.model.id, SolfacStatus.AmountCashed).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                this.model.statusName = SolfacStatus[SolfacStatus.AmountCashed];
            },
            error => this.errorHandlerService.handleErrors(error));
    }

    delete(){
        this.solfacService.delete(this.model.id).subscribe(data => {
            if(data.messages) this.messageService.showMessages(data.messages);

            setTimeout(() => { this.goToProject(); }, 1500)
        },
        err => this.errorHandlerService.handleErrors(err));
    }
}