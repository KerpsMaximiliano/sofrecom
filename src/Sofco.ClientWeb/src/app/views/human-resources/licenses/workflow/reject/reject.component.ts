import { Component, OnDestroy, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { Ng2ModalConfig } from '../../../../../components/modal/ng2modal-config';
import { Subscription } from "rxjs";
import { MenuService } from "../../../../../services/admin/menu.service";
import { MessageService } from '../../../../../services/common/message.service';
import { LicenseService } from '../../../../../services/human-resources/licenses.service';
import { LicenseStatus } from '../../../../../models/enums/licenseStatus';

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
        private menuService: MenuService) { }


  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  canReject(){
    if(this.menuService.user.employeeId == this.employeeId) return false;

    if(this.licenseId == 0) return false;

    if(this.menuService.hasFunctionality('CTRLI', 'AUTH') && this.status == LicenseStatus[LicenseStatus.AuthPending]) return true;

    if(this.menuService.hasFunctionality('CTRLI', 'APROB') &&
        (this.status == LicenseStatus[LicenseStatus.AuthPending] 
        || this.status == LicenseStatus[LicenseStatus.Pending] 
        || this.status == LicenseStatus[LicenseStatus.ApprovePending])){
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
        this.rejectModal.resetButtons();
        return;
    }

    var json = {
        status: LicenseStatus.Rejected,
        comment: this.rejectComments
    }

    this.subscrip = this.licenseService.changeStatus(this.licenseId, json).subscribe(
        data => {
            this.rejectModal.hide();

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
        });
    }
}