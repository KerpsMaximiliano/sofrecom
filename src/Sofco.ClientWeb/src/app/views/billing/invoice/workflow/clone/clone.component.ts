import { Component, OnDestroy, ViewChild, Input } from '@angular/core';
import { Ng2ModalConfig } from '../../../../../components/modal/ng2modal-config';
import { Subscription } from "rxjs";
import { Router } from '@angular/router';
import { InvoiceService } from '../../../../../services/billing/invoice.service';
import { MenuService } from '../../../../../services/admin/menu.service';

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
    private menuService: MenuService,
    private router: Router) { }


  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  canClone(){
    return this.menuService.hasFunctionality("REM", "CLONE") && this.invoiceId > 0;
  }

  clone(){
    this.subscrip = this.invoiceService.clone(this.invoiceId).subscribe(data => {
        this.cloneModal.hide();

        setTimeout(() => { 
          this.router.navigate([`/billing/invoice/${data.data.id}/project/${data.data.projectId}`]); 
        }, 500)
    },
    () => this.cloneModal.hide());
  }
}