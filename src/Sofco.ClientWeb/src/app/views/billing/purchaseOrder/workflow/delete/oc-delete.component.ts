import { Component, OnDestroy, Input } from '@angular/core';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { Subscription } from "rxjs";
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

  send(){
    this.subscrip = this.purchaseOrderService.delete(this.ocId).subscribe(
        data => {
            if(data.messages) this.messageService.showMessages(data.messages);

            setTimeout(() => {
                this.router.navigate(['/billing/purchaseOrders/query']);
            }, 1000);
        },
        error => {
            this.errorHandlerService.handleErrors(error);
        },
        () => {
            this.messageService.closeLoading();
        });
    }
}