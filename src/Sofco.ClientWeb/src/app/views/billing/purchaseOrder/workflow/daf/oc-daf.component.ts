import { Component, OnDestroy, ViewChild, Input } from '@angular/core';
import { Subscription } from "rxjs";
import { MessageService } from '../../../../../services/common/message.service';
import { Router } from '@angular/router';
import { PurchaseOrderStatus } from '../../../../../models/enums/purchaseOrderStatus';
import { PurchaseOrderService } from '../../../../../services/billing/purchaseOrder.service';
import { MenuService } from '../../../../../services/admin/menu.service';

@Component({
  selector: 'oc-status-daf',
  templateUrl: './oc-daf.component.html'
})
export class OcStatusDafComponent implements OnDestroy  {

  @Input() ocId: number;
  @Input() status: number;

  subscrip: Subscription;

  constructor(private purchaseOrderService: PurchaseOrderService,
    private messageService: MessageService,
    private menuService: MenuService,
    private router: Router) { }

  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  canSend(){
    if(this.ocId > 0 && this.status == PurchaseOrderStatus.DafPending && this.menuService.hasFunctionality('PUROR', 'APDAF')){
        return true;
    }

    return false;
  }


  send(){
    this.subscrip = this.purchaseOrderService.changeStatus(this.ocId, {}).subscribe(
        data => {
            setTimeout(() => {
                this.router.navigate(['/billing/purchaseOrders/pendings']);
            }, 1000);
        },
        () => {}, 
        () => {
            this.messageService.closeLoading();
        });
    }
}