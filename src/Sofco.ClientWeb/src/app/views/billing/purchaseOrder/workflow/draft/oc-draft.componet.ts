import { Component, OnDestroy, ViewChild, Input } from '@angular/core';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { Subscription } from "rxjs/Subscription";
import { MessageService } from 'app/services/common/message.service';
import { Router } from '@angular/router';
import { PurchaseOrderStatus } from '../../../../../models/enums/purchaseOrderStatus';
import { PurchaseOrderService } from '../../../../../services/billing/purchaseOrder.service';
import { MenuService } from '../../../../../services/admin/menu.service';

@Component({
  selector: 'oc-status-draft',
  templateUrl: './oc-draft.component.html'
})
export class OcStatusDraftComponent implements OnDestroy  {

  @ViewChild('draftModal') draftModal;
  public draftModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      "ACTIONS.confirmTitle",
      "draftModal",
      true,
      true,
      "ACTIONS.ACCEPT",
      "ACTIONS.cancel"
  );

  @Input() ocId: number;
  @Input() status: number;

  subscrip: Subscription;

  constructor(private purchaseOrderService: PurchaseOrderService,
    private messageService: MessageService,
    private menuService: MenuService,
    private errorHandlerService: ErrorHandlerService,
    private router: Router) { }


  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  canSend(){
    if(this.ocId > 0 && (this.status == PurchaseOrderStatus.Draft || this.status == PurchaseOrderStatus.Reject) && this.menuService.hasFunctionality('PUROR', 'DRAFT')){
        return true;
    }

    return false;
  }

  showModal(){
    this.draftModal.show();
  }

  send(){
    this.subscrip = this.purchaseOrderService.changeStatus(this.ocId, {}).subscribe(
        data => {
            this.draftModal.hide();
            if(data.messages) this.messageService.showMessages(data.messages);

            setTimeout(() => {
                this.router.navigate(['/billing/purchaseOrders/pendings']);
            }, 1000);
        },
        error => {
            this.draftModal.hide();
            this.errorHandlerService.handleErrors(error);
        });
    }
}