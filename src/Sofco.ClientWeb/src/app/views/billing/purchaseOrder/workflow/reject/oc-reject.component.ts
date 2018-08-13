import { Component, OnDestroy, ViewChild, Input } from '@angular/core';
import { Ng2ModalConfig } from '../../../../../components/modal/ng2modal-config';
import { Subscription } from "rxjs";
import { Router } from '@angular/router';
import { PurchaseOrderStatus } from '../../../../../models/enums/purchaseOrderStatus';
import { PurchaseOrderService } from '../../../../../services/billing/purchaseOrder.service';
import { MenuService } from '../../../../../services/admin/menu.service';

@Component({
  selector: 'oc-status-reject',
  templateUrl: './oc-reject.component.html'
})
export class OcStatusRejectComponent implements OnDestroy  {

  @ViewChild('rejectModal') modal;
  public rejectModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      "ACTIONS.confirmTitle",
      "rejectModal",
      true,
      true,
      "ACTIONS.ACCEPT",
      "ACTIONS.cancel"
  );

  @Input() ocId: number;
  @Input() status: number;

  subscrip: Subscription;

  public rejectComments: string;

  constructor(private purchaseOrderService: PurchaseOrderService,
    private menuService: MenuService,
    private router: Router) { }

  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  canSend(){
    if(this.ocId > 0 && this.menuService.hasFunctionality('PUROR', 'REJEC') && 
        (this.status == PurchaseOrderStatus.CompliancePending || 
          this.status == PurchaseOrderStatus.ComercialPending || 
          this.status == PurchaseOrderStatus.OperativePending || 
          this.status == PurchaseOrderStatus.DafPending)){
        return true;
    }

    return false;
  }

  showModal(){
    this.modal.show();
  }

  send(){
    this.subscrip = this.purchaseOrderService.changeStatus(this.ocId, { comments: this.rejectComments, mustReject: true}).subscribe(
        data => {
            this.modal.hide();

            setTimeout(() => {
              this.router.navigate(['/billing/purchaseOrders/pendings']);
            }, 1000);
        },
        error => {
            this.modal.hide();
        });
    }
}