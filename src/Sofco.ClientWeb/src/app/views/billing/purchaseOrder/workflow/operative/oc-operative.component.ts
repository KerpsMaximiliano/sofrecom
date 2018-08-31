import { Component, OnDestroy, Input } from '@angular/core';
import { Subscription } from "rxjs";
import { MessageService } from '../../../../../services/common/message.service';
import { Router } from '@angular/router';
import { PurchaseOrderStatus } from '../../../../../models/enums/purchaseOrderStatus';
import { PurchaseOrderService } from '../../../../../services/billing/purchaseOrder.service';
import { MenuService } from '../../../../../services/admin/menu.service';

@Component({
  selector: 'oc-status-operative',
  templateUrl: './oc-operative.component.html'
})
export class OcStatusOperativeComponent implements OnDestroy  {

  @Input() ocId: number;
  @Input() status: number;
  @Input() model: any;

  subscrip: Subscription;

  constructor(private purchaseOrderService: PurchaseOrderService,
    private messageService: MessageService,
    private menuService: MenuService,
    private router: Router) { }

  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  canSend(){
    if(this.ocId > 0 
        && this.status == PurchaseOrderStatus.OperativePending
        && this.menuService.hasFunctionality('PUROR', 'OPERA')
        && this.menuService.hasSectorAccess(this.model.sectorIds)
    ){
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
        error => {},
        () => {
            this.messageService.closeLoading();
        });
    }
}