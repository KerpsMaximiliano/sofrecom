import { OnDestroy, Component, ViewChild, Input, Output, EventEmitter } from "@angular/core";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from "app/services/common/message.service";
import { InvoiceService } from "app/services/billing/invoice.service";
import { InvoiceStatus } from "app/models/enums/invoiceStatus";

@Component({
    selector: 'invoice-status-sendToDaf',
    templateUrl: './status-sendToDaf.component.html'
  })
  export class InvoiceStatusSendToDafComponent implements OnDestroy  {

    @ViewChild('sendToDafModal') sendToDafModal;
    public sendToDafModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "sendToDafModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    @Input() invoiceId: number;
    @Input() status: string;
    @Input() excelFileName: string;

    @Output() history: EventEmitter<any> = new EventEmitter();
    @Output() callback: EventEmitter<any> = new EventEmitter();

    subscrip: Subscription;
    public comments: string;

    constructor(private invoiceService: InvoiceService,
        private messageService: MessageService,
        private menuService: MenuService,
        private errorHandlerService: ErrorHandlerService) { }

    ngOnDestroy(): void {
        if(this.subscrip) this.subscrip.unsubscribe();
    }

    sendToDAF(){
        this.invoiceService.changeStatus(this.invoiceId, InvoiceStatus.Sent, this.comments, "").subscribe(data => {
            this.sendToDafModal.hide();
            if(data.messages) this.messageService.showMessages(data.messages);

            if(this.history.observers.length > 0){
                this.history.emit();
            }

            if(this.callback.observers.length > 0){
                var toModif = {
                    invoiceStatus: InvoiceStatus[InvoiceStatus.Sent],
                    reloadUploader: true
                }

                this.callback.emit(toModif);
            }
        },
        err => {
            this.sendToDafModal.hide();
            this.errorHandlerService.handleErrors(err)
        });
    }

    canSendToDaf(){
        if(this.excelFileName && this.invoiceId > 0 &&
           this.menuService.hasFunctionality('REM', 'SEND') &&
          (this.status == InvoiceStatus[InvoiceStatus.SendPending] || this.status == InvoiceStatus[InvoiceStatus.Rejected])){
            return true;
        }

        return false;
    }
  }