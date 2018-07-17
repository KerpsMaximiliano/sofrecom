import { Component, OnDestroy, ViewChild, Input } from '@angular/core';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { Subscription } from "rxjs/Subscription";
import { MessageService } from 'app/services/common/message.service';
import { Router } from '@angular/router';
import { PurchaseOrderStatus } from 'app/models/enums/purchaseOrderStatus';
import { PurchaseOrderService } from 'app/services/billing/purchaseOrder.service';
import { MenuService } from 'app/services/admin/menu.service';

@Component({
  selector: 'oc-status-delete',
  templateUrl: './oc-delete.component.html'
})
export class OcStatusDeleteComponent implements OnDestroy  {

  @ViewChild('deleteModal') modal;
  public deleteModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      "ACTIONS.confirmTitle",
      "deleteModal",
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
    private errorHandlerService: ErrorHandlerService,
    private menuService: MenuService,
    private router: Router) { }

  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  canSend(){
    if(this.ocId > 0 && this.status == PurchaseOrderStatus.Draft && this.menuService.hasFunctionality('PUROR', 'DELET')){
        return true;
    }

    return false;
  }

  showModal(){
    this.modal.show();
  }

  send(){
    this.subscrip = this.purchaseOrderService.delete(this.ocId).subscribe(
        data => {
            this.modal.hide();
            if(data.messages) this.messageService.showMessages(data.messages);

            setTimeout(() => {
                this.router.navigate(['/billing/purchaseOrders/query']);
            }, 1000);
        },
        error => {
            this.modal.hide();
            this.errorHandlerService.handleErrors(error);
        });
    }
}