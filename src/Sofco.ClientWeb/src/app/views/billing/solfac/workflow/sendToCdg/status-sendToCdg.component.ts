import { Component, OnDestroy, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { Ng2ModalConfig } from '../../../../../components/modal/ng2modal-config';
import { SolfacService } from "../../../../../services/billing/solfac.service";
import { Subscription } from "rxjs";
import { SolfacStatus } from "../../../../../models/enums/solfacStatus";
import { MenuService } from "../../../../../services/admin/menu.service";
import { I18nService } from '../../../../../services/common/i18n.service';

@Component({
  selector: 'status-sendToCdg',
  templateUrl: './status-sendToCdg.component.html'
})
export class StatusSendToCdgComponent implements OnDestroy  {

  @ViewChild('sendToCdgModal') sendToCdgModal;
  public sendToCdgModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      "ACTIONS.confirmTitle",
      "sendToCdgModal",
      true,
      true,
      "ACTIONS.ACCEPT",
      "ACTIONS.cancel"
  );

  @Input() solfacId: number;
  @Input() status: string;
  @Input() attachments: number;

  @Output() history: EventEmitter<any> = new EventEmitter();
  @Output() updateStatus: EventEmitter<any> = new EventEmitter();
  @Output() back: EventEmitter<any> = new EventEmitter();

  subscrip: Subscription;

  message: string;

  constructor(private solfacService: SolfacService,
    private menuService: MenuService,
    private i18nService: I18nService) { }


  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  canSendToCDG(){
    if(this.solfacId > 0 &&
        (this.status == SolfacStatus[SolfacStatus.SendPending] || 
         this.status == SolfacStatus[SolfacStatus.ManagementControlRejected] ||
         this.status == SolfacStatus[SolfacStatus.RejectedByDaf])
       && this.menuService.hasFunctionality("SOLFA", "SCDG")){

        return true;
    }

    return false;
  }

  showModal(){
    if(this.attachments == 0){
        this.message = this.i18nService.translate('billing/solfac', 'solfacHasNoAttachmentsConfirm');
    }
    else{
        this.message = this.i18nService.translateByKey('ACTIONS.confirmBody');
    }

    this.sendToCdgModal.show();
  }

  sendToCDG(){
    var json = {
        status: SolfacStatus.PendingByManagementControl
    }

    this.subscrip = this.solfacService.changeStatus(this.solfacId, json).subscribe(
        () => {
            this.sendToCdgModal.hide();
            if (this.history.observers.length > 0) {
                this.history.emit();
            }
            if (this.updateStatus.observers.length > 0) {
                var toModif = {
                    statusName: SolfacStatus[SolfacStatus.PendingByManagementControl]
                };
                this.updateStatus.emit(toModif);
            }
            if (this.back.observers.length > 0) {
                var self = this;
                setTimeout(function () {
                    self.back.emit();
                }, 500);
            }
        },
        () => {
            this.sendToCdgModal.hide();
        });
    }
}