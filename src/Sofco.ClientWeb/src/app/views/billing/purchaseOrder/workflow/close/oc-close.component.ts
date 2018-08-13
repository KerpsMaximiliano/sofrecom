import { Component, OnDestroy, ViewChild, Input } from '@angular/core';
import { Ng2ModalConfig } from '../../../../../components/modal/ng2modal-config';
import { Subscription } from "rxjs";
import { Router } from '@angular/router';
import { PurchaseOrderStatus } from '../../../../../models/enums/purchaseOrderStatus';
import { PurchaseOrderService } from '../../../../../services/billing/purchaseOrder.service';
import { MenuService } from '../../../../../services/admin/menu.service';

@Component({
  selector: 'oc-status-close',
  templateUrl: './oc-close.component.html'
})
export class OcStatusCloseComponent implements OnDestroy  {

  @ViewChild('closeModal') modal;
  public closeModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      "ACTIONS.confirmTitle",
      "closeModal",
      true,
      true,
      "ACTIONS.ACCEPT",
      "ACTIONS.cancel"
  );

  @Input() ocId: number;
  @Input() status: number;

  subscrip: Subscription;

  public closeComments: string;

  constructor(private purchaseOrderService: PurchaseOrderService,
    private menuService: MenuService,
    private router: Router) { }

  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  canSend(){
    if(this.ocId > 0 && this.menuService.hasFunctionality('PUROR', 'CLOSE') && 
                        (this.status == PurchaseOrderStatus.Valid || this.status == PurchaseOrderStatus.Consumed)){
        return true;
    }

    return false;
  }

  showModal(){
    this.modal.show();
  }

  send(){
    this.subscrip = this.purchaseOrderService.close(this.ocId, { comments: this.closeComments, mustReject: false}).subscribe(
        () => {
        this.modal.hide();
        setTimeout(() => {
          this.router.navigate(['/billing/purchaseOrders/pendings']);
        }, 1000);
      },
        () => {
        this.modal.hide();
      });
    }
}