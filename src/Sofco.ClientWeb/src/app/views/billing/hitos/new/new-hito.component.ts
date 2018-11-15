import { Component, OnInit, OnDestroy, ViewChild, EventEmitter, Output } from '@angular/core';
import { Ng2ModalConfig } from '../../../../components/modal/ng2modal-config';
import { Subscription } from "rxjs";
import { ProjectService } from '../../../../services/billing/project.service';
import { NewHito } from '../../../../models/billing/solfac/newHito';
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

    constructor(private utilsService: UtilsService,
    private projectService: ProjectService) {}

    ngOnInit(): void {
        this.getCurrenciesSubscrip = this.utilsService.getCurrencies().subscribe(d => {
            this.currencies = d;
        });
    }

    ngOnDestroy(): void {
        if(this.subscrip) this.subscrip.unsubscribe();
        if(this.getCurrenciesSubscrip) this.getCurrenciesSubscrip.unsubscribe();
    }

    openModal(hito){
        var today = new Date();

        this.hito.month = today.getMonth()+1;
        this.hito.startDate = today;
        this.hito.moneyId = 1;
        this.hito.projectId = hito.projectId;
        this.hito.managerId = hito.managerId;
        this.hito.opportunityId = hito.opportunityId;

        this.newHitoModal.show();
    }

    save(){
        this.subscrip = this.projectService.createNewHito(this.hito).subscribe(() => {
            this.newHitoModal.hide();
            if (this.hitosReload.observers.length > 0) {
                this.hitosReload.emit();
            }
        }, 
        error => this.newHitoModal.resetButtons());
    }
}