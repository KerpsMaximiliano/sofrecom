import { OnDestroy, Component, ViewChild, Input, Output, EventEmitter } from "@angular/core";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from "app/services/common/message.service";
import { InvoiceService } from "app/services/billing/invoice.service";
import { InvoiceStatus } from "app/models/enums/invoiceStatus";

@Component({
    selector: 'invoice-status-reject',
    templateUrl: './invoice-status-reject.component.html'
  })
  export class InvoiceStatusRejectComponent implements OnDestroy  {

    @ViewChild('rejectModal') rejectModal;
    public rejectModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "rejectModal",
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

    reject(){
        this.isLoading = true;

        this.subscrip = this.invoiceService.changeStatus(this.invoiceId, InvoiceStatus.Rejected, this.comments, "").subscribe(data => {
            this.rejectModal.hide();
            this.isLoading = false;
            if(data.messages) this.messageService.showMessages(data.messages);
            
            if(this.history.observers.length > 0){
                this.history.emit();
            }

            if(this.updateStatus.observers.length > 0){
                var toModif = {
                    invoiceStatus: InvoiceStatus[InvoiceStatus.Rejected],
                    reloadUploader: true
                }

                this.updateStatus.emit(toModif);
            }
        },
        err => {
            this.isLoading = false;
            this.errorHandlerService.handleErrors(err);
        });
    }

    canRejectInvoice(){
        if(this.menuService.hasFunctionality('REM', 'REJEC') && this.status == InvoiceStatus[InvoiceStatus.Sent]){
            return true;
        }

        return false;
    }
  }