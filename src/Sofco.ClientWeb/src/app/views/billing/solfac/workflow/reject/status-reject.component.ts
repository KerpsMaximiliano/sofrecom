import { Component, OnInit, OnDestroy, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { SolfacService } from "app/services/billing/solfac.service";
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { Subscription } from "rxjs/Subscription";
import { SolfacStatus } from "app/models/enums/solfacStatus";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from 'app/services/common/message.service';
import { Router } from '@angular/router';

@Component({
  selector: 'status-reject',
  templateUrl: './status-reject.component.html'
})
export class StatusRejectComponent implements OnDestroy  {

    @ViewChild('rejectModal') rejectModal;
    public rejectModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "billing.solfac.addComments",
        "rejectModal",
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
    public isLoading: boolean = false;

    constructor(private solfacService: SolfacService,
        private messageService: MessageService,
        private menuService: MenuService,
        private errorHandlerService: ErrorHandlerService,
        private router: Router) { }

    ngOnDestroy(): void {
        if(this.subscrip) this.subscrip.unsubscribe();
    }

    canRejectByCDG(){
        if(this.status == SolfacStatus[SolfacStatus.PendingByManagementControl] &&
           this.menuService.hasFunctionality("SOLFA", "REJEC")){
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
            status: SolfacStatus.ManagementControlRejected,
            comment: this.rejectComments
        }

        this.isLoading = true;

        this.subscrip = this.solfacService.changeStatus(this.solfacId, json).subscribe(
            data => {
                this.rejectModal.hide();
                this.isLoading = false;
                if(data.messages) this.messageService.showMessages(data.messages);
                
                if(this.history.observers.length > 0){
                    this.history.emit();
                }
            
                if(this.updateStatus.observers.length > 0){
                    var toModif = {
                        statusName: SolfacStatus[SolfacStatus.ManagementControlRejected]
                    }
    
                    this.updateStatus.emit(toModif);
                }
            },
            error => {
                this.rejectModal.hide();
                this.isLoading = false;
                this.errorHandlerService.handleErrors(error);
            });
    }
}