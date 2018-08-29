import { Component, OnDestroy, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { Ng2ModalConfig } from '../../../../../components/modal/ng2modal-config';
import { Subscription } from "rxjs";
import { MenuService } from "../../../../../services/admin/menu.service";
import { LicenseService } from '../../../../../services/human-resources/licenses.service';
import { LicenseStatus } from '../../../../../models/enums/licenseStatus';

@Component({
  selector: 'status-pending',
  templateUrl: './pending.component.html'
})
export class LicensePendingComponent implements OnDestroy  {

  @ViewChild('pendingModal') pendingModal;
  public pendingModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      "ACTIONS.confirmTitle",
      "pendingModal",
      true,
      true,
      "ACTIONS.ACCEPT",
      "ACTIONS.cancel"
  );

  @Input() licenseId: number;
  @Input() employeeId: number;
  @Input() status: string;
  @Input() certificateRequired: boolean;
  @Input() hasCertificate: boolean;

  @Output() history: EventEmitter<any> = new EventEmitter();
  @Output() updateStatus: EventEmitter<any> = new EventEmitter();

  subscrip: Subscription;

  public statusToSend: any;

  constructor(private licenseService: LicenseService,
    private menuService: MenuService) { }

  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  canApproveWithPendingDocumentation(){
    if(!this.menuService.hasFunctionality('CTRLI', 'APROB') || this.menuService.user.employeeId == this.employeeId) return false;

    if(!this.certificateRequired) return false;

    if(this.hasCertificate) return false;

    if(this.licenseId > 0 && this.menuService.userIsRrhh && this.status == LicenseStatus[LicenseStatus.Pending]){
        return true;
    }

    return false;
  }
 
  canApprove(){
    if(!this.menuService.hasFunctionality('CTRLI', 'APROB') || this.menuService.user.employeeId == this.employeeId) return false;

    if(this.licenseId > 0 && this.menuService.userIsRrhh && (this.status == LicenseStatus[LicenseStatus.Pending] || this.status == LicenseStatus[LicenseStatus.ApprovePending])){
        return true;
    }

    return false;
  }

  approveWithPendingDocumentation(){
    this.statusToSend = LicenseStatus.ApprovePending;
    this.pendingModal.show();
  }

  approve(){
    this.statusToSend = LicenseStatus.Approved;
    this.pendingModal.show();
  }

  confirm(){
    var json = {
        status: this.statusToSend
    }

    this.subscrip = this.licenseService.changeStatus(this.licenseId, json).subscribe(
        () => {
            this.pendingModal.hide();
            if (this.history.observers.length > 0) {
                this.history.emit();
            }
            if (this.updateStatus.observers.length > 0) {
                var toModif = {
                    statusId: this.statusToSend,
                    statusName: LicenseStatus[this.statusToSend]
                };
                this.updateStatus.emit(toModif);
            }
        },
        () => {
            this.pendingModal.hide();
        });
    }
}