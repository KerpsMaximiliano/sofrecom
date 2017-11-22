import { Component, OnInit, OnDestroy, ViewChild, Input, EventEmitter, Output } from '@angular/core';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { Subscription } from "rxjs/Subscription";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from 'app/services/common/message.service';
import { Router } from '@angular/router';
import { ProjectService } from 'app/services/billing/project.service';
import { NewHito } from 'app/models/billing/solfac/newHito';

@Component({
  selector: 'split-hito',
  templateUrl: './split-hito.component.html'
})
export class SplitHitoComponent implements OnDestroy  {

  @ViewChild('spliHitoModal') spliHitoModal;
  public spliHitoModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      "billing.project.detail.milestone.splitTitle",
      "spliHitoModal",
      true,
      true,
      "ACTIONS.save",
      "ACTIONS.cancel"
  );

  subscrip: Subscription;

  @Output() hitosReload: EventEmitter<any> = new EventEmitter();

  public hito: NewHito = new NewHito();
  public hitoSelected: any;
  public options;

  constructor(private messageService: MessageService,
    private menuService: MenuService,
    private projectService: ProjectService,
    private errorHandlerService: ErrorHandlerService,
    private router: Router) {
      
      this.options = this.menuService.getDatePickerOptions();
    }

  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  openModal(hito){
    this.hitoSelected = hito;
    this.hito = new NewHito();
    this.spliHitoModal.show();
  }

  save(){
    this.hito.statusCode = 717620003;
    this.hito.projectId = this.hitoSelected.projectId;
    this.hito.opportunityId = this.hitoSelected.opportunityId;
    this.hito.managerId = this.hitoSelected.managerId;
    this.hito.externalHitoId = this.hitoSelected.id;
    this.hito.moneyId = this.hitoSelected.currencyId;
    this.hito.ammountFirstHito = this.hitoSelected.ammount;

    this.subscrip = this.projectService.createNewHito(this.hito).subscribe(data => {
        if(data.messages) this.messageService.showMessages(data.messages);
        this.spliHitoModal.hide();

        if(this.hitosReload.observers.length > 0){
          this.hitosReload.emit();
      }
    },
    err =>  {
        this.errorHandlerService.handleErrors(err)
    });
  }
}