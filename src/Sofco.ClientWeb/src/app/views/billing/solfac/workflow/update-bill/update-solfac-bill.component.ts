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
  selector: 'update-solfac-bill',
  templateUrl: './update-solfac-bill.component.html'
})
export class UpdateSolfacBillComponent implements OnDestroy, OnInit  {


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
    @Input() invoiceDate: Date = new Date();

    @Output() updateStatus: EventEmitter<any> = new EventEmitter();
    @Output() history: EventEmitter<any> = new EventEmitter();

    subscrip: Subscription;

    public options;

    constructor(private solfacService: SolfacService,
        private messageService: MessageService,
        private menuService: MenuService,
        private errorHandlerService: ErrorHandlerService,
        private router: Router) {

            this.options = this.menuService.getDatePickerOptions();
         }

    ngOnInit(): void {
        
    }

    ngOnDestroy(): void {
        if(this.subscrip) this.subscrip.unsubscribe();
    }

    openModal(){
        $('#updateInvoiceCode').val(this.invoiceCode);
        this.updateBillModal.show();
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
                invoiceDate: this.invoiceDate
              }

            this.subscrip = this.solfacService.updateBill(this.solfacId, json).subscribe(
                data => {
                    this.updateBillModal.hide();
                    if(data.messages) this.messageService.showMessages(data.messages);
                 
                    if(this.history.observers.length > 0){
                        this.history.emit();
                    }

                    if(this.updateStatus.observers.length > 0){
                        var toModif = {
                            invoiceCode: this.invoiceCode,
                            invoiceDate: this.invoiceDate
                        }
        
                        this.updateStatus.emit(toModif);
                    }
                },
                error => {
                    this.updateBillModal.hide();
                    this.errorHandlerService.handleErrors(error);
                });
        }else{
            this.messageService.showError("El número de factura es requerido");
        }
    }
}