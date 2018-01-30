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

  public options;

  public isLoading: boolean = false;

  constructor(private solfacService: SolfacService,
    private messageService: MessageService,
    private menuService: MenuService,
    private errorHandlerService: ErrorHandlerService,
    private router: Router) {

        this.options = this.menuService.getDatePickerOptions();
     }

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

        this.isLoading = true;

        this.subscrip = this.solfacService.changeStatus(this.solfacId, json).subscribe(
            data => {
                this.cashModal.hide();
                this.isLoading = false;
                if(data.messages) this.messageService.showMessages(data.messages);

                if(this.history.observers.length > 0){
                    this.history.emit();
                }

                if(this.updateStatus.observers.length > 0){
                    var toModif = {
                        statusName: SolfacStatus[SolfacStatus.AmountCashed],
                        cashedDate: this.cashedDate
                    }
    
                    this.updateStatus.emit(toModif);
                }
            },
            error => {
                this.isLoading = false;
                this.errorHandlerService.handleErrors(error);
            });
    }
}