import { Component, OnInit, OnDestroy, ViewChild, Input, EventEmitter, Output } from '@angular/core';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { SolfacService } from "app/services/billing/solfac.service";
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { Subscription } from "rxjs/Subscription";
import { SolfacStatus } from "app/models/enums/solfacStatus";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from 'app/services/common/message.service';
import { Router } from '@angular/router';
import { DatepickerOptions } from 'ng2-datepicker';
declare var $: any;

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

  public options: DatepickerOptions = {
    minYear: 1970,
    maxYear: 2030,
    displayFormat: 'DD/MM/YYYY',
    barTitleFormat: 'MMMM YYYY',
    firstCalendarDay: 1
  };

  constructor(private solfacService: SolfacService,
    private messageService: MessageService,
    private menuService: MenuService,
    private errorHandlerService: ErrorHandlerService,
    private router: Router) { }

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
            data => {
                this.cashModal.hide();
                if(data.messages) this.messageService.showMessages(data.messages);

                var toModif = {
                    statusName: SolfacStatus[SolfacStatus.AmountCashed],
                    cashedDate: this.cashedDate
                }

                this.history.emit();
                this.updateStatus.emit(toModif);
            },
            error => {
                this.cashModal.hide();
                this.errorHandlerService.handleErrors(error);
            });
    }
}