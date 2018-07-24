import { Component, OnInit, OnDestroy, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { SolfacService } from "app/services/billing/solfac.service";
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { Subscription } from "rxjs";
import { SolfacStatus } from "app/models/enums/solfacStatus";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from 'app/services/common/message.service';
import { Router } from '@angular/router';

@Component({
  selector: 'status-reject-daf',
  templateUrl: './status-reject-daf.component.html'
})
export class StatusRejectDafComponent implements OnDestroy  {

    @ViewChild('rejectByDafModal') rejectByDafModal;
    public rejectByDafModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "billing.solfac.addComments",
        "rejectByDafModal",
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

    public rejectComments: string;

    constructor(private solfacService: SolfacService,
        private messageService: MessageService,
        private menuService: MenuService,
        private errorHandlerService: ErrorHandlerService) { }

    ngOnDestroy(): void {
        if(this.subscrip) this.subscrip.unsubscribe();
    }

    canRejectByDaf(){
        if(this.status == SolfacStatus[SolfacStatus.InvoicePending] &&
           this.menuService.hasFunctionality("SOLFA", "RJDAF")){
            return true;
        }

        return false;
    }

    rejectByCDG(){
        if(!this.rejectComments || this.rejectComments == ""){
            this.messageService.showError("billing.solfac.rejectCommentRequired");
            return;
        }

        var json = {
            status: SolfacStatus.RejectedByDaf,
            comment: this.rejectComments
        }

        this.subscrip = this.solfacService.changeStatus(this.solfacId, json).subscribe(
            data => {
                this.rejectByDafModal.hide();
                if(data.messages) this.messageService.showMessages(data.messages);
                
                if(this.history.observers.length > 0){
                    this.history.emit();
                }
            
                if(this.updateStatus.observers.length > 0){
                    var toModif = {
                        statusName: SolfacStatus[SolfacStatus.RejectedByDaf]
                    }
    
                    this.updateStatus.emit(toModif);
                }
            },
            error => {
                this.rejectByDafModal.hide();
                this.errorHandlerService.handleErrors(error);
            });
    }
}