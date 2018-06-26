import { OnDestroy, Component, ViewChild, Input, Output, EventEmitter, transition } from "@angular/core";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from "app/services/common/message.service";
import { InvoiceService } from "app/services/billing/invoice.service";
import { InvoiceStatus } from "app/models/enums/invoiceStatus";

@Component({
    selector: 'invoice-status-annulment',
    templateUrl: './status-annulment.component.html'
})
export class InvoiceStatusAnnulmentComponent implements OnDestroy  {

    @ViewChild('annulmentModal') annulmentModal;
    public annulmentModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "annulmentModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    @Input() invoiceId: number;
    @Input() status: string;

    @Output() history: EventEmitter<any> = new EventEmitter();
    @Output() updateStatus: EventEmitter<any> = new EventEmitter();

    subscrip: Subscription;
    public comments: string;
    public isLoading: boolean = false;

    constructor(private invoiceService: InvoiceService,
        private messageService: MessageService,
        private menuService: MenuService,
        private errorHandlerService: ErrorHandlerService) { }

    ngOnDestroy(): void {
        if(this.subscrip) this.subscrip.unsubscribe();
    }

    annulment(){
        this.isLoading = true;

        this.subscrip = this.invoiceService.changeStatus(this.invoiceId, InvoiceStatus.Cancelled, "", "").subscribe(data => {
            this.isLoading = false;
            this.annulmentModal.hide();
            if(data.messages) this.messageService.showMessages(data.messages);
            
            if(this.history.observers.length > 0){
                this.history.emit();
            }

            if(this.updateStatus.observers.length > 0){
                var toModif = {
                    invoiceStatus: InvoiceStatus[InvoiceStatus.Cancelled],
                    reloadUploader: true
                }

                this.updateStatus.emit(toModif);
            }
        },
        err => {
            this.isLoading = false;
            this.annulmentModal.hide();
            this.errorHandlerService.handleErrors(err)
        });
    }

    canCancel(){
        return this.invoiceId > 0 && this.menuService.hasFunctionality('REM', 'ANNUL') 
                                    && (this.status == InvoiceStatus[InvoiceStatus.Sent] 
                                    || this.status == InvoiceStatus[InvoiceStatus.Approved]
                                    || this.status == InvoiceStatus[InvoiceStatus.RequestAnnulment]
                                    || this.status == InvoiceStatus[InvoiceStatus.Rejected])
    }
}