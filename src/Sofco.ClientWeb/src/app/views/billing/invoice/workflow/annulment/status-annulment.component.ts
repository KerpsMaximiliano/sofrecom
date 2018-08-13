import { OnDestroy, Component, ViewChild, Input, Output, EventEmitter } from "@angular/core";
import { Ng2ModalConfig } from "../../../../../components/modal/ng2modal-config";
import { Subscription } from "rxjs";
import { MenuService } from "../../../../../services/admin/menu.service";
import { InvoiceService } from "../../../../../services/billing/invoice.service";
import { InvoiceStatus } from "../../../../../models/enums/invoiceStatus";

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

    constructor(private invoiceService: InvoiceService,
        private menuService: MenuService) { }

    ngOnDestroy(): void {
        if(this.subscrip) this.subscrip.unsubscribe();
    }

    annulment(){
        this.subscrip = this.invoiceService.changeStatus(this.invoiceId, InvoiceStatus.Cancelled, this.comments, "").subscribe(() => {
            this.annulmentModal.hide();
            if (this.history.observers.length > 0) {
                this.history.emit();
            }
            if (this.updateStatus.observers.length > 0) {
                var toModif = {
                    invoiceStatus: InvoiceStatus[InvoiceStatus.Cancelled],
                    reloadUploader: true
                };
                this.updateStatus.emit(toModif);
            }
        },
        () => this.annulmentModal.hide());
    }

    canCancel(){
        return this.invoiceId > 0 && this.menuService.hasFunctionality('REM', 'ANNUL') 
                                    && (this.status == InvoiceStatus[InvoiceStatus.Sent] 
                                    || this.status == InvoiceStatus[InvoiceStatus.Approved]
                                    || this.status == InvoiceStatus[InvoiceStatus.RequestAnnulment]
                                    || this.status == InvoiceStatus[InvoiceStatus.Rejected])
    }
}