import { Component, OnInit, OnDestroy, ViewChild, Input } from '@angular/core';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { SolfacService } from "app/services/billing/solfac.service";
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { Subscription } from "rxjs/Subscription";
import { SolfacStatus } from "app/models/enums/solfacStatus";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from 'app/services/common/message.service';
import { Router } from '@angular/router';

@Component({
  selector: 'status-delete',
  templateUrl: './status-delete.component.html'
})
export class StatusDeleteComponent implements OnDestroy  {

  @ViewChild('deleteModal') deleteModal;
  public deleteModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      "ACTIONS.confirmTitle",
      "deleteModal",
      true,
      true,
      "ACTIONS.ACCEPT",
      "ACTIONS.cancel"
  );

  @Input() solfacId: number;
  @Input() status: string;

  @Input() customerId: string;
  @Input() serviceId: string;
  @Input() projectId: string;

  subscrip: Subscription;

  constructor(private solfacService: SolfacService,
    private messageService: MessageService,
    private menuService: MenuService,
    private errorHandlerService: ErrorHandlerService,
    private router: Router) { }


  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  canDelete(){
      if(this.solfacId > 0 && (this.status == SolfacStatus[SolfacStatus.SendPending] || 
                               this.status == SolfacStatus[SolfacStatus.ManagementControlRejected])){
          return true;
      }

      return false;
  }

  delete(){
      this.solfacService.delete(this.solfacId).subscribe(data => {
          this.deleteModal.hide();
          if(data.messages) this.messageService.showMessages(data.messages);

          setTimeout(() => { 
            this.router.navigate([`/billing/customers/${this.customerId}/services/${this.serviceId}/projects/${this.projectId}`]); 
          }, 500)
      },
      err => {
          this.deleteModal.hide();
          this.errorHandlerService.handleErrors(err);
      });
  }
}