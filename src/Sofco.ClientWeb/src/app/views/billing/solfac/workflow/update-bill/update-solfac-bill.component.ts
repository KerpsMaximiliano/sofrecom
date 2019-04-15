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
  selector: 'update-solfac-bill',
  templateUrl: './update-solfac-bill.component.html'
})
export class UpdateSolfacBillComponent implements OnDestroy  {
    @ViewChild('updateBillModal') updateBillModal;
    public updateBillModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "billing.solfac.includeInvoiceCode",
        "updateBillModal", 
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    @Input() solfacId: number;
    @Input() status: string;
    @Input() invoiceCode: string;
    @Input() currencyExchange: number;
    @Input() currencyId: number;
    @Input() invoiceDate: Date = new Date();

    @Output() updateStatus: EventEmitter<any> = new EventEmitter();
    @Output() history: EventEmitter<any> = new EventEmitter();

    subscrip: Subscription;

    constructor(private solfacService: SolfacService,
        private messageService: MessageService,
        private menuService: MenuService) {}

    ngOnDestroy(): void {
        if(this.subscrip) this.subscrip.unsubscribe();
    }

    openModal(){
        $('#updateInvoiceCode').val(this.invoiceCode);
        this.updateBillModal.show();
    }

    isCurrencyPesos(){
        if(this.currencyId == environment.currencyPesosId) return true;

        return false;
    }

    canUpdateBill(){
        if((this.status == SolfacStatus[SolfacStatus.Invoiced] || this.status == SolfacStatus[SolfacStatus.AmountCashed]) && 
            this.menuService.hasFunctionality("SOLFA", "UPBIL")){
            return true;
        }

        return false;
    } 
 
    updateBill(){
        this.invoiceCode = $('#updateInvoiceCode').val();

        if(this.invoiceCode && this.invoiceCode != "" && this.invoiceCode.length == 13){

            var json = {
                invoiceCode: this.invoiceCode,
                currencyExchange: this.currencyExchange,
                invoiceDate: this.invoiceDate
              }

            this.subscrip = this.solfacService.updateBill(this.solfacId, json).subscribe(
                () => {
                    this.updateBillModal.hide();
                    if (this.history.observers.length > 0) {
                        this.history.emit();
                    }
                    if (this.updateStatus.observers.length > 0) {
                        var toModif = {
                            invoiceCode: this.invoiceCode,
                            invoiceDate: this.invoiceDate,
                            currencyExchange: this.currencyExchange
                        };
                        this.updateStatus.emit(toModif);
                    }
                },
                () => {
                    this.updateBillModal.hide();
                });
        }
        else{
            this.updateBillModal.hide();
            this.messageService.showError("billing.solfac.includeInvoiceCode");
        }
    }
}