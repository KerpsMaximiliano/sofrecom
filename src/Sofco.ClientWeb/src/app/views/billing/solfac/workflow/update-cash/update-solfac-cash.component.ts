import { Component, OnDestroy, ViewChild, Input, EventEmitter, Output } from '@angular/core';
import { Ng2ModalConfig } from '../../../../../components/modal/ng2modal-config';
import { SolfacService } from "../../../../../services/billing/solfac.service";
import { Subscription } from "rxjs";
import { SolfacStatus } from "../../../../../models/enums/solfacStatus";
import { MenuService } from "../../../../../services/admin/menu.service";

@Component({
  selector: 'update-solfac-cash',
  templateUrl: './update-solfac-cash.component.html'
})
export class UpdateSolfacCashComponent implements OnDestroy  {

    @ViewChild('updateCashModal') updateCashModal;
    public updateCashModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "updateCashModal", 
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    @Input() solfacId: number;
    @Input() status: string;
    @Input() cashedDate: Date = new Date();

    @Output() updateStatus: EventEmitter<any> = new EventEmitter();
    @Output() history: EventEmitter<any> = new EventEmitter();

    subscrip: Subscription;

    constructor(private solfacService: SolfacService,
        private menuService: MenuService) {}

    ngOnDestroy(): void {
        if(this.subscrip) this.subscrip.unsubscribe();
    }

    canUpdateCash(){
        if(this.status == SolfacStatus[SolfacStatus.AmountCashed] && 
            this.menuService.hasFunctionality("SOLFA", "UPBIL")){
            return true;
        }

        return false;
    } 
 
    updateCash(){
        var json = { cashedDate: this.cashedDate }

        this.subscrip = this.solfacService.updateCash(this.solfacId, json).subscribe(
            () => {
                this.updateCashModal.hide();
                if (this.history.observers.length > 0) {
                    this.history.emit();
                }
                if (this.updateStatus.observers.length > 0) {
                    var toModif = { cashedDate: this.cashedDate };
                    this.updateStatus.emit(toModif);
                }
            },
            () => {
                this.updateCashModal.hide();
            });
    }
}