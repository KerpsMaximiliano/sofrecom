import { Component, OnDestroy, ViewChild, Input, EventEmitter, Output } from '@angular/core';
import { Ng2ModalConfig } from '../../../../../components/modal/ng2modal-config';
import { SolfacService } from "../../../../../services/billing/solfac.service";
import { Subscription } from "rxjs";
import { SolfacStatus } from "../../../../../models/enums/solfacStatus";
import { MenuService } from "../../../../../services/admin/menu.service";


@Component({
  selector: 'status-cash',
  templateUrl: './status-cash.component.html'
})
export class StatusCashComponent implements OnDestroy  {

  @ViewChild('cashModal') cashModal;
  public cashModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      "ACTIONS.confirmTitle",
      "cashModal",
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

  cashedDate: Date = new Date();

  constructor(private solfacService: SolfacService,
    private menuService: MenuService) {}

  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  canSendToCash(){
    if(this.status == SolfacStatus[SolfacStatus.Invoiced] && 
       this.menuService.hasFunctionality("SOLFA", "CASH")){
        return true;
    }

    return false;
    }

    sendToCash(){
        var json = {
            status: SolfacStatus.AmountCashed,
            cashedDate: this.cashedDate
        }

        this.subscrip = this.solfacService.changeStatus(this.solfacId, json).subscribe(
            () => {
                this.cashModal.hide();
                if (this.history.observers.length > 0) {
                    this.history.emit();
                }
                if (this.updateStatus.observers.length > 0) {
                    var toModif = {
                        statusName: SolfacStatus[SolfacStatus.AmountCashed],
                        cashedDate: this.cashedDate
                    };
                    this.updateStatus.emit(toModif);
                }
            },
            () => {
                this.cashModal.hide();
            });
    }
}