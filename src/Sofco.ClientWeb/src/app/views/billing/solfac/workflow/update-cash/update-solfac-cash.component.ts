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

    public isLoading: boolean = false;

    constructor(private solfacService: SolfacService,
        private messageService: MessageService,
        private menuService: MenuService,
        private errorHandlerService: ErrorHandlerService,
        private router: Router) {}

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

        this.isLoading = true;

        this.subscrip = this.solfacService.updateCash(this.solfacId, json).subscribe(
            data => {
                this.isLoading = false;
                this.updateCashModal.hide();
                if(data.messages) this.messageService.showMessages(data.messages);
                
                if(this.history.observers.length > 0){
                    this.history.emit();
                }

                if(this.updateStatus.observers.length > 0){

                    var toModif = { cashedDate: this.cashedDate }
                    this.updateStatus.emit(toModif);
                }
            },
            error => {
                this.isLoading = false;
                this.updateCashModal.hide();
                this.errorHandlerService.handleErrors(error);
            });
    }
}