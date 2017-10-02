import { Component, OnInit, OnDestroy, ViewChild, Input } from '@angular/core';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { Subscription } from "rxjs/Subscription";
import { MessageService } from 'app/services/common/message.service';
import { Router } from '@angular/router';
import { InvoiceService } from 'app/services/billing/invoice.service';

@Component({
  selector: 'clone-invoice',
  templateUrl: './clone.component.html'
})
export class CloneInvoiceComponent implements OnDestroy  {

  @ViewChild('cloneModal') cloneModal;
  public cloneModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      "ACTIONS.confirmTitle",
      "cloneModal",
      true,
      true,
      "ACTIONS.ACCEPT",
      "ACTIONS.cancel"
  );

  @Input() invoiceId: number;

  subscrip: Subscription;

  constructor(private invoiceService: InvoiceService,
    private messageService: MessageService,
    private errorHandlerService: ErrorHandlerService,
    private router: Router) { }


  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  clone(){
      this.subscrip = this.invoiceService.clone(this.invoiceId).subscribe(data => {
          this.cloneModal.hide();
          if(data.messages) this.messageService.showMessages(data.messages);

          setTimeout(() => { 
            this.router.navigate([`/billing/invoice/${data.data.id}/project/${data.data.projectId}`]); 
          }, 500)
      },
      err => {
          this.cloneModal.hide();
          
          setTimeout(() => { 
            this.errorHandlerService.handleErrors(err);
          }, 500)
      });
  }
}