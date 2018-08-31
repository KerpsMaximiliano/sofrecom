import { Component, OnDestroy, ViewChild, EventEmitter, Output } from '@angular/core';
import { Ng2ModalConfig } from '../../../components/modal/ng2modal-config';
import { Subscription } from "rxjs";
import { ProjectService } from '../../../services/billing/project.service';
import { NewHito } from '../../../models/billing/solfac/newHito';

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

  constructor(private projectService: ProjectService) {}

  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  openModal(hito){
    this.hitoSelected = hito;
    this.hito = new NewHito();
    this.hito.month = hito.month;
    this.hito.startDate = new Date(hito.startDate);
    this.spliHitoModal.show();
  }

  getCurrencySymbol(){
    if(!this.hitoSelected || !this.hitoSelected.money) return "";

    switch(this.hitoSelected.money){
      case "Peso": { return "$"; }
      case "Dolar": { return "U$D"; }
      case "Euro": { return "€"; }
    }
  }

  save(){
    this.hito.statusCode = this.hitoSelected.statusCode;
    this.hito.projectId = this.hitoSelected.projectId;
    this.hito.opportunityId = this.hitoSelected.opportunityId;
    this.hito.managerId = this.hitoSelected.managerId;
    this.hito.externalHitoId = this.hitoSelected.id;
    this.hito.moneyId = this.hitoSelected.moneyId;
    this.hito.ammountFirstHito = this.hitoSelected.ammount;

    this.subscrip = this.projectService.spltHito(this.hito).subscribe(() => {
      this.spliHitoModal.hide();
      if (this.hitosReload.observers.length > 0) {
        this.hitosReload.emit();
      }
    });
  }
}