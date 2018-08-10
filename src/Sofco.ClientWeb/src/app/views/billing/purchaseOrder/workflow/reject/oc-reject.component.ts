import { Component, OnDestroy, ViewChild, Input } from '@angular/core';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { Subscription } from "rxjs";
import { MessageService } from 'app/services/common/message.service';
import { Router } from '@angular/router';
import { PurchaseOrderStatus } from 'app/models/enums/purchaseOrderStatus';
import { PurchaseOrderService } from 'app/services/billing/purchaseOrder.service';
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
  @Input() model: any;

  subscrip: Subscription;

  public rejectComments: string;

  constructor(private purchaseOrderService: PurchaseOrderService,
    private messageService: MessageService,
    private menuService: MenuService,
    private errorHandlerService: ErrorHandlerService,
    private router: Router) { }

  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  canSend(){
    if(this.ocId > 0 && this.menuService.hasFunctionality('PUROR', 'REJEC') &&
        (this.status === PurchaseOrderStatus.CompliancePending ||
          this.hasCommercialAccess() ||
          this.hasOperationAccess() ||
          this.status === PurchaseOrderStatus.DafPending)){
        return true;
    }

    return false;
  }

  hasCommercialAccess() {
    return (this.status === PurchaseOrderStatus.ComercialPending
      && this.menuService.hasAreaAccess(this.model.areaId));
  }

  hasOperationAccess() {
    return (this.status === PurchaseOrderStatus.OperativePending
      && this.menuService.hasSectorAccess(this.model.sectorIds));
  }

  showModal(){
    this.modal.show();
  }

  send(){
    this.subscrip = this.purchaseOrderService.changeStatus(this.ocId, { comments: this.rejectComments, mustReject: true}).subscribe(
        data => {
            this.modal.hide();
            if(data.messages) this.messageService.showMessages(data.messages);

            setTimeout(() => {
              this.router.navigate(['/billing/purchaseOrders/pendings']);
            }, 1000);
        },
        error => {
            this.modal.hide();
            this.errorHandlerService.handleErrors(error);
        });
    }
}