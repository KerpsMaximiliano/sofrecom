import { Component, OnDestroy, ViewChild, Input } from '@angular/core';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { Subscription } from "rxjs/Subscription";
import { MessageService } from 'app/services/common/message.service';
import { Router } from '@angular/router';
import { PurchaseOrderStatus } from 'app/models/enums/purchaseOrderStatus';
import { PurchaseOrderService } from 'app/services/billing/purchaseOrder.service';
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
    this.isLoading = true;

    this.subscrip = this.purchaseOrderService.close(this.ocId, { comments: this.closeComments, mustReject: false}).subscribe(
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