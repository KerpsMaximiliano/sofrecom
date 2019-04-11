import { Component, OnDestroy, ViewChild, Input, EventEmitter, Output } from '@angular/core';
import { Ng2ModalConfig } from '../../../../../components/modal/ng2modal-config';
import { SolfacService } from "../../../../../services/billing/solfac.service";
import { Subscription } from "rxjs";
import { SolfacStatus } from "../../../../../models/enums/solfacStatus";
import { MenuService } from "../../../../../services/admin/menu.service";
import { MessageService } from '../../../../../services/common/message.service';
import { environment } from 'environments/environment'
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
    currencyExchange: number;

    currencyId: number;
    isCurrencyPesos: boolean = false;

    constructor(private solfacService: SolfacService,
        private messageService: MessageService,
        private menuService: MenuService) {}


    ngOnDestroy(): void {
        if(this.subscrip) this.subscrip.unsubscribe();
    }

    setCurrencyId(currencyId){
        this.currencyId = currencyId;

        if(environment.currencyPesosId == this.currencyId){
            this.isCurrencyPesos = true;
       }
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
                invoiceDate: this.invoiceDate,
                currencyExchange: this.currencyExchange
              }

            this.subscrip = this.solfacService.changeStatus(this.solfacId, json).subscribe(
                data => {
                    this.billModal.hide();
                    
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
                    this.billModal.hide();
                });
        }
        else {
            this.billModal.hide();
            this.messageService.showError("billing.solfac.includeInvoiceCode");
        }
    }
}