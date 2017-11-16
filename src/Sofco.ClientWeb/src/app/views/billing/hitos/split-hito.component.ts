import { Component, OnInit, OnDestroy, ViewChild, Input, EventEmitter, Output } from '@angular/core';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { Subscription } from "rxjs/Subscription";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from 'app/services/common/message.service';
import { Router } from '@angular/router';
import { ProjectService } from 'app/services/billing/project.service';

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
  cant: number = 2;

  @Output() hitosReload: EventEmitter<any> = new EventEmitter();

  public hitos: any[] = new Array();
  public hitoSelected: any;

  constructor(private messageService: MessageService,
    private menuService: MenuService,
    private projectService: ProjectService,
    private errorHandlerService: ErrorHandlerService,
    private router: Router) {}

  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  openModal(hito){
    this.hitoSelected = hito;
    this.spliHitoModal.show();
  }

  split(){
    if(!this.cant || this.cant < 2 || this.cant > 5){
        this.messageService.showWarningByFolder("billing/projects", "hitoQuantityBetween2and5");
        this.cant = 2;
    }

    this.hitos = new Array();

    for(var i = 0; i < this.cant; i++){

        var newHito = {
            name: `${this.hitoSelected.name} - ${i+1}`,
            ammount: this.hitoSelected.ammount,
            statusCode: 717620003,
            startDate: this.hitoSelected.startDate,
            month: this.hitoSelected.month,
            projectId: this.hitoSelected.projectId,
            opportunityId: this.hitoSelected.opportunityId,
            managerId: this.hitoSelected.managerId,
            externalHitoId: this.hitoSelected.id,
            moneyId: this.hitoSelected.currencyId
        }

        this.hitos.push(newHito);
    }
  }

  save(){
    this.subscrip = this.projectService.splitHito(this.hitos).subscribe(data => {
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