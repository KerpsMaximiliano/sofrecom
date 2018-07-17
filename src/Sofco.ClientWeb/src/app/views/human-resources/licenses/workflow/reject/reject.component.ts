import { Component, OnDestroy, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { Subscription } from "rxjs/Subscription";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from 'app/services/common/message.service';
import { LicenseService } from 'app/services/human-resources/licenses.service';
import { LicenseStatus } from 'app/models/enums/licenseStatus';

@Component({
  selector: 'status-reject',
  templateUrl: './reject.component.html'
})
export class LicenseRejectComponent implements OnDestroy  {

    @ViewChild('rejectModal') rejectModal;
    public rejectModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "billing.solfac.addComments",
        "rejectModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    @Input() licenseId: number;
    @Input() employeeId: number;
    @Input() status: string;

    @Output() history: EventEmitter<any> = new EventEmitter();
    @Output() updateStatus: EventEmitter<any> = new EventEmitter();

    subscrip: Subscription;

    public rejectComments: string;

    constructor(private licenseService: LicenseService,
        private messageService: MessageService,
        private menuService: MenuService,
        private errorHandlerService: ErrorHandlerService) { }


  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  canReject(){
    if(!this.menuService.hasFunctionality('CTRLI', 'REJEC') || this.menuService.user.employeeId == this.employeeId) return false;

    if(this.licenseId > 0 && this.menuService.userIsRrhh && (this.status == LicenseStatus[LicenseStatus.AuthPending] || 
                              this.status == LicenseStatus[LicenseStatus.Pending] || 
                              this.status == LicenseStatus[LicenseStatus.ApprovePending])){
        return true;
    }

    return false;
  }

  showModal(){
    this.rejectModal.show();
  }

  reject(){
    if(!this.rejectComments || this.rejectComments == ""){
        this.messageService.showError("billing.solfac.rejectCommentRequired");
        return;
    }
    
    var json = {
        status: LicenseStatus.Rejected,
        comment: this.rejectComments
    }

    this.subscrip = this.licenseService.changeStatus(this.licenseId, json).subscribe(
        data => {
            this.rejectModal.hide();
            if(data.messages) this.messageService.showMessages(data.messages);

            if(this.history.observers.length > 0){
                this.history.emit();
            }
        
            if(this.updateStatus.observers.length > 0){
                var toModif = {
                    statusId: LicenseStatus.Rejected,
                    statusName: LicenseStatus[LicenseStatus.Rejected]
                }

                this.updateStatus.emit(toModif);
            }
        },
        error => {
            this.rejectModal.hide();
            this.errorHandlerService.handleErrors(error);
        });
    }
}