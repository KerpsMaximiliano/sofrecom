import { Component, OnInit, OnDestroy, ViewChild, Input, EventEmitter, Output } from '@angular/core';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { SolfacService } from "app/services/billing/solfac.service";
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { Subscription } from "rxjs/Subscription";
import { SolfacStatus } from "app/models/enums/solfacStatus";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from 'app/services/common/message.service';
import { Router } from '@angular/router';
import { DatepickerOptions } from 'ng2-datepicker';
declare var $: any;

@Component({
  selector: 'status-bill',
  templateUrl: './status-bill.component.html'
})
export class StatusBillComponent implements OnDestroy  {

    @ViewChild('billModal') billModal;
    public billModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "billing.solfac.includeInvoiceCode",
        "billModal", 
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    @Input() solfacId: number;
    @Input() status: string;

    @Output() history: EventEmitter<any> = new EventEmitter();
    @Output() updateStatus: EventEmitter<any> = new EventEmitter();

    subscrip: Subscription;

    invoiceDate: Date = new Date();
    invoiceCode: string;

    public options;

    constructor(private solfacService: SolfacService,
        private messageService: MessageService,
        private menuService: MenuService,
        private errorHandlerService: ErrorHandlerService,
        private router: Router) { 

            this.options = this.menuService.getDatePickerOptions();
        }

    ngOnDestroy(): void {
        if(this.subscrip) this.subscrip.unsubscribe();
    }

    canSendToBill(){
        if(this.status == SolfacStatus[SolfacStatus.InvoicePending] && 
        this.menuService.hasFunctionality("SOLFA", "BILL")){
            return true;
        }

        return false;
    } 
 
    sendToBill(){
        this.invoiceCode = $('#invoiceCode').val();

        if(this.invoiceCode && this.invoiceCode != "" && this.invoiceCode.length == 13){

            var json = {
                status: SolfacStatus.Invoiced,
                invoiceCode: this.invoiceCode,
                invoiceDate: this.invoiceDate
              }

            this.subscrip = this.solfacService.changeStatus(this.solfacId, json).subscribe(
                data => {
                    this.billModal.hide();
                    if(data.messages) this.messageService.showMessages(data.messages);
                 
                    if(this.history.observers.length > 0){
                        this.history.emit();
                    }

                    var toModif = {
                        statusName: SolfacStatus[SolfacStatus.Invoiced],
                        invoiceCode: this.invoiceCode,
                        invoiceDate: this.invoiceDate
                    }

                    this.updateStatus.emit(toModif);
                },
                error => {
                    this.errorHandlerService.handleErrors(error);
                });
        }else{
            this.messageService.showError("El número de factura es requerido");
        }
    }
}