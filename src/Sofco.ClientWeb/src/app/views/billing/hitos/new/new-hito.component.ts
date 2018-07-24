import { Component, OnInit, OnDestroy, ViewChild, Input, EventEmitter, Output } from '@angular/core';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { Subscription } from "rxjs";
import { MessageService } from 'app/services/common/message.service';
import { ProjectService } from 'app/services/billing/project.service';
import { NewHito } from 'app/models/billing/solfac/newHito';
import { UtilsService } from '../../../../services/common/utils.service';
import { Option } from '../../../../models/option';

@Component({
  selector: 'new-hito',
  templateUrl: './new-hito.component.html'
})
export class NewHitoComponent implements OnDestroy, OnInit  {

  @ViewChild('newHitoModal') newHitoModal;
  public newHitoModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      "billing.project.detail.milestone.splitTitle",
      "newHitoModal",
      true,
      true,
      "ACTIONS.save",
      "ACTIONS.cancel"
  );

  subscrip: Subscription;
  getCurrenciesSubscrip: Subscription;

  @Output() hitosReload: EventEmitter<any> = new EventEmitter();

  public hito: NewHito = new NewHito();

  public currencies: Option[] = new Array();

    constructor(private messageService: MessageService,
    private utilsService: UtilsService,
    private projectService: ProjectService,
    private errorHandlerService: ErrorHandlerService) {}

    ngOnInit(): void {
        this.getCurrenciesSubscrip = this.utilsService.getCurrencies().subscribe(d => {
            this.currencies = d;
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    ngOnDestroy(): void {
        if(this.subscrip) this.subscrip.unsubscribe();
        if(this.getCurrenciesSubscrip) this.getCurrenciesSubscrip.unsubscribe();
    }

    openModal(hito){
        var today = new Date();

        this.hito.month = today.getMonth()+1;
        this.hito.startDate = today;
        this.hito.moneyId = '1';
        this.hito.projectId = hito.projectId;
        this.hito.managerId = hito.managerId;
        this.hito.opportunityId = hito.opportunityId;

        this.newHitoModal.show();
    }

    save(){
        this.subscrip = this.projectService.createNewHito(this.hito).subscribe(data => {
            if(data.messages) this.messageService.showMessages(data.messages);
            this.newHitoModal.hide();

            if(this.hitosReload.observers.length > 0){
                this.hitosReload.emit();
            }
        },
        err =>  {
            this.errorHandlerService.handleErrors(err)
        });
    }
}