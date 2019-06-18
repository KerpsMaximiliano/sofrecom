import { Component, OnDestroy, ViewChild, Input, OnInit } from '@angular/core';
import { Ng2ModalConfig } from '../../../../../components/modal/ng2modal-config';
import { Subscription } from "rxjs";
import { Router } from '@angular/router';
import { PurchaseOrderStatus } from '../../../../../models/enums/purchaseOrderStatus';
import { PurchaseOrderService } from '../../../../../services/billing/purchaseOrder.service';
import { MenuService } from '../../../../../services/admin/menu.service';

@Component({
  selector: 'oc-status-reopen',
  templateUrl: './oc-reopen.html'
})
export class OcStatusReopenComponent implements OnDestroy  {
  @ViewChild('reopenModal') modal;
  public reopenModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      "ACTIONS.confirmTitle",
      "reopenModal",
      true,
      true,
      "ACTIONS.ACCEPT",
      "ACTIONS.cancel"
  );

  @Input() ocId: number;
  @Input() status: number;
  @Input() model: any;

  subscrip: Subscription;

  public comments: string;

  constructor(private purchaseOrderService: PurchaseOrderService,
    private menuService: MenuService,
    private router: Router) { }

  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  canReopen(){
    if(this.ocId > 0 && this.menuService.hasFunctionality('PUROR', 'REOPEN') && this.status === PurchaseOrderStatus.Closed){
        return true;
    }

    return false;
  }

  showModal(){
    this.modal.show();
  }

  send(){
    this.subscrip = this.purchaseOrderService.reopen(this.ocId, { comments: this.comments, mustReject: true}).subscribe(
        data => {
            this.modal.hide();

            setTimeout(() => {
              this.router.navigate(['/billing/purchaseOrders/query']);
            }, 500);
        },
        error => {
            this.modal.hide();
        });
    }
}