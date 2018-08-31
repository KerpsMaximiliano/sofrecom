import { Component, OnDestroy, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { Ng2ModalConfig } from '../../../../../components/modal/ng2modal-config';
import { SolfacService } from "../../../../../services/billing/solfac.service";
import { Subscription } from "rxjs";
import { SolfacStatus } from "../../../../../models/enums/solfacStatus";
import { MenuService } from "../../../../../services/admin/menu.service";

@Component({
  selector: 'status-sendToDaf',
  templateUrl: './status-sendToDaf.component.html'
})
export class StatusSendToDafComponent implements OnDestroy  {

  @ViewChild('sendToDafModal') sendToDafModal;
  public sendToDafModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      "ACTIONS.confirmTitle",
      "sendToDafModal",
      true,
      true,
      "ACTIONS.ACCEPT",
      "ACTIONS.cancel"
  );

  @Input() solfacId: number;
  @Input() status: string;

  @Output() history: EventEmitter<any> = new EventEmitter();
  @Output() updateStatus: EventEmitter<any> = new EventEmitter();

  subscrip: Subscription;
  public comments: string;

  constructor(private solfacService: SolfacService,
        private menuService: MenuService) { }

    ngOnDestroy(): void {
        if(this.subscrip) this.subscrip.unsubscribe();
    }

    canSendToDAF(){
        if(this.status == SolfacStatus[SolfacStatus.PendingByManagementControl] &&
            this.menuService.hasFunctionality("SOLFA", "SDAF")){
            return true;
        }

        return false;
    }

    sendToDAF(){
        var json = {
            status: SolfacStatus.InvoicePending,
            comment: this.comments
        }

        this.subscrip = this.solfacService.changeStatus(this.solfacId, json).subscribe(
            () => {
                this.sendToDafModal.hide();
                if (this.history.observers.length > 0) {
                    this.history.emit();
                }
                if (this.updateStatus.observers.length > 0) {
                    var toModif = {
                        statusName: SolfacStatus[SolfacStatus.InvoicePending]
                    };
                    this.updateStatus.emit(toModif);
                }
            },
            () => {
                this.sendToDafModal.hide();
            });
    }
}