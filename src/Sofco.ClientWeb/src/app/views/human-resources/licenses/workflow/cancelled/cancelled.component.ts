import { Component, OnDestroy, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { Ng2ModalConfig } from '../../../../../components/modal/ng2modal-config';
import { Subscription } from "rxjs";
import { MenuService } from "../../../../../services/admin/menu.service";
import { MessageService } from '../../../../../services/common/message.service';
import { LicenseService } from '../../../../../services/human-resources/licenses.service';
import { LicenseStatus } from '../../../../../models/enums/licenseStatus';

@Component({
  selector: 'status-cancel',
  templateUrl: './cancelled.component.html'
})
export class LicenseCancelComponent implements OnDestroy  {

    @ViewChild('cancelModal') cancelModal;
    public cancelModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "rrhh.license.addCancelComments",
        "cancelModal",
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

  canCancel(){
    if(!this.menuService.hasFunctionality('CTRLI', 'CANCE')) return false;

    if(this.licenseId > 0 && this.menuService.userIsRrhh && this.status == LicenseStatus[LicenseStatus.Approved]){
        return true;
    }

    return false;
  }

  showModal(){
    this.cancelModal.show();
  }

  cancel(){
    if(!this.rejectComments || this.rejectComments == ""){
        this.messageService.showError("rrhh.license.cancelComments");
        return;
    }
    
    var json = {
        status: LicenseStatus.Cancelled,
        comment: this.rejectComments
    }

    this.subscrip = this.licenseService.changeStatus(this.licenseId, json).subscribe(
        () => {
            this.cancelModal.hide();
            if (this.history.observers.length > 0) {
                this.history.emit();
            }
            if (this.updateStatus.observers.length > 0) {
                var toModif = {
                    statusId: LicenseStatus.Cancelled,
                    statusName: LicenseStatus[LicenseStatus.Cancelled]
                };
                this.updateStatus.emit(toModif);
            }
        },
        () => {
            this.cancelModal.hide();
        });
    }
}