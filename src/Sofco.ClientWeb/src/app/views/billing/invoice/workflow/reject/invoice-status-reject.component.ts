import { OnDestroy, Component, ViewChild, Input, Output, EventEmitter } from "@angular/core";
import { Ng2ModalConfig } from "../../../../../components/modal/ng2modal-config";
import { Subscription } from "rxjs";
import { MenuService } from "../../../../../services/admin/menu.service";
import { InvoiceService } from "../../../../../services/billing/invoice.service";
import { InvoiceStatus } from "../../../../../models/enums/invoiceStatus";

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

    constructor(private invoiceService: InvoiceService,
        private menuService: MenuService) { }

    ngOnDestroy(): void {
        if(this.subscrip) this.subscrip.unsubscribe();
    }

    reject(){
        this.subscrip = this.invoiceService.changeStatus(this.invoiceId, InvoiceStatus.Rejected, this.comments, "").subscribe(() => {
            this.rejectModal.hide();
            if (this.history.observers.length > 0) {
                this.history.emit();
            }
            if (this.updateStatus.observers.length > 0) {
                var toModif = {
                    invoiceStatus: InvoiceStatus[InvoiceStatus.Rejected],
                    reloadUploader: true
                };
                this.updateStatus.emit(toModif);
            }
        });
    }

    canRejectInvoice(){
        if(this.menuService.hasFunctionality('REM', 'REJEC') && this.status == InvoiceStatus[InvoiceStatus.Sent]){
            return true;
        }

        return false;
    }
  }