import { Component, OnDestroy, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { Ng2ModalConfig } from '../../../../../components/modal/ng2modal-config';
import { Subscription } from "rxjs";
import { MenuService } from "../../../../../services/admin/menu.service";
import { MessageService } from '../../../../../services/common/message.service';
import { InvoiceService } from '../../../../../services/billing/invoice.service';
import { InvoiceStatus } from '../../../../../models/enums/invoiceStatus';
declare var $: any;

@Component({
  selector: 'invoice-status-approve',
  templateUrl: './status-approve.component.html'
})
export class StatusApproveComponent implements OnDestroy  {

    @ViewChild('approveConfirmModal') approveConfirmModal;
    public approveConfirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "billing.invoice.includeInvoiceNumber",
        "approveConfirmModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    @Input() invoiceId: string;
    @Input() status: string;
    @Input() pdfFileName: string;

    @Output() history: EventEmitter<any> = new EventEmitter();
    @Output() updateStatus: EventEmitter<any> = new EventEmitter();

    subscrip: Subscription;
    invoiceNumber;

    constructor(private invoiceService: InvoiceService,
        private messageService: MessageService,
        private menuService: MenuService) { }

    ngOnDestroy(): void {
        if(this.subscrip) this.subscrip.unsubscribe();
    }

    canApprovedInvoice(){
        if(this.menuService.hasFunctionality('REM', 'APROB') && this.status == InvoiceStatus[InvoiceStatus.Sent] && this.pdfFileName){
            return true;
        }

        return false;
    }

    approve(){
        this.invoiceNumber = $('#invoiceNumber').val();

        if(this.invoiceNumber && this.invoiceNumber != "" && this.invoiceNumber.length == 13){

            this.subscrip = this.invoiceService.changeStatus(this.invoiceId, InvoiceStatus.Approved, "", this.invoiceNumber).subscribe(data => {
                this.approveConfirmModal.hide();

                if(this.history.observers.length > 0){
                    this.history.emit();
                }

                if(this.updateStatus.observers.length > 0){
                    var toModif = {
                        invoiceStatus: InvoiceStatus[InvoiceStatus.Approved],
                        invoiceNumber: this.invoiceNumber,
                        reloadUploader: true
                    }
    
                    this.updateStatus.emit(toModif);
                }
            });
        }
        else{
            this.messageService.showError("billing.invoice.invoiceNumberRequired");
        }
    }
}