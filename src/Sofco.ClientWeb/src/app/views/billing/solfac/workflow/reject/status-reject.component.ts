import { Component, OnDestroy, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { Ng2ModalConfig } from '../../../../../components/modal/ng2modal-config';
import { SolfacService } from "../../../../../services/billing/solfac.service";
import { Subscription } from "rxjs";
import { SolfacStatus } from "../../../../../models/enums/solfacStatus";
import { MenuService } from "../../../../../services/admin/menu.service";
import { MessageService } from '../../../../../services/common/message.service';

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

    constructor(private solfacService: SolfacService,
        private messageService: MessageService,
        private menuService: MenuService) { }

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
            this.rejectModal.resetButtons();
            return;
        }

        var json = {
            status: SolfacStatus.ManagementControlRejected,
            comment: this.rejectComments
        }

        this.subscrip = this.solfacService.changeStatus(this.solfacId, json).subscribe(
            data => {
                this.rejectModal.hide();
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
            });
    }
}