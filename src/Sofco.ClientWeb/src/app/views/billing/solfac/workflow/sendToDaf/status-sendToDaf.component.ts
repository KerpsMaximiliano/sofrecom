import { Component, OnInit, OnDestroy, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { SolfacService } from "app/services/billing/solfac.service";
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { Subscription } from "rxjs/Subscription";
import { SolfacStatus } from "app/models/enums/solfacStatus";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from 'app/services/common/message.service';
import { Router } from '@angular/router';

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
        private messageService: MessageService,
        private menuService: MenuService,
        private errorHandlerService: ErrorHandlerService,
        private router: Router) { }

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
            data => {
                this.sendToDafModal.hide();
                if(data.messages) this.messageService.showMessages(data.messages);
                
                if(this.history.observers.length > 0){
                    this.history.emit();
                }
            
                if(this.updateStatus.observers.length > 0){
                    var toModif = {
                        statusName: SolfacStatus[SolfacStatus.InvoicePending]
                    }
    
                    this.updateStatus.emit(toModif);
                }
            },
            error => {
                this.sendToDafModal.hide();
                this.errorHandlerService.handleErrors(error);
            });
    }
}