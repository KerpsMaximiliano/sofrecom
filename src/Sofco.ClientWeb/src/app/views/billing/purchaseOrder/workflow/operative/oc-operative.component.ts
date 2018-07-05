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
  selector: 'oc-status-operative',
  templateUrl: './oc-operative.component.html'
})
export class OcStatusOperativeComponent implements OnDestroy  {

  @ViewChild('operativeModal') modal;
  public operativeModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      "ACTIONS.confirmTitle",
      "operativeModal",
      true,
      true,
      "ACTIONS.ACCEPT",
      "ACTIONS.cancel"
  );

  @Input() ocId: number;
  @Input() status: number;

  subscrip: Subscription;

  public isLoading: boolean = false;

  constructor(private purchaseOrderService: PurchaseOrderService,
    private messageService: MessageService,
    private menuService: MenuService,
    private errorHandlerService: ErrorHandlerService,
    private router: Router) { }

  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  canSend(){
    if(this.ocId > 0 && this.status == PurchaseOrderStatus.OperativePending && this.menuService.hasFunctionality('PUROR', 'OPERA')){
        return true;
    }

    return false;
  }

  showModal(){
    this.modal.show();
  }

  send(){
    this.isLoading = true;

    this.subscrip = this.purchaseOrderService.changeStatus(this.ocId, {}).subscribe(
        data => {
            this.modal.hide();
            this.isLoading = false;
            if(data.messages) this.messageService.showMessages(data.messages);

            setTimeout(() => {
                this.router.navigate(['/billing/purchaseOrders/pendings']);
            }, 1000);
        },
        error => {
            this.modal.hide();
            this.isLoading = false;
            this.errorHandlerService.handleErrors(error);
        });
    }
}