import { Component, OnDestroy, ViewChild, Input } from '@angular/core';
import { Ng2ModalConfig } from '../../../../../components/modal/ng2modal-config';
import { SolfacService } from "../../../../../services/billing/solfac.service";
import { Subscription } from "rxjs";
import { SolfacStatus } from "../../../../../models/enums/solfacStatus";
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
    private router: Router) { }


  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  canDelete(){
      if(this.solfacId > 0 && (this.status == SolfacStatus[SolfacStatus.SendPending] || 
                               this.status == SolfacStatus[SolfacStatus.RejectedByDaf] || 
                               this.status == SolfacStatus[SolfacStatus.ManagementControlRejected])){
          return true;
      }

      return false;
  }

  delete(){
    this.solfacService.delete(this.solfacId).subscribe(() => {
      this.deleteModal.hide();
      setTimeout(() => {
        this.router.navigate([`/billing/customers/${this.customerId}/services/${this.serviceId}/projects/${this.projectId}`]);
      }, 500);
    },
    () => {
        this.deleteModal.hide();
      });
  }
}