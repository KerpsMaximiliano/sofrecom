import { Component, OnDestroy, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { Ng2ModalConfig } from '../../../../../components/modal/ng2modal-config';
import { Subscription } from "rxjs";
import { MenuService } from "../../../../../services/admin/menu.service";
import { MessageService } from '../../../../../services/common/message.service';
import { LicenseService } from '../../../../../services/human-resources/licenses.service';
import { LicenseStatus } from '../../../../../models/enums/licenseStatus';

@Component({
  selector: 'status-auth-pending',
  templateUrl: './auth-pending.component.html'
})
export class LicenseAuthPendingComponent implements OnDestroy  {

  @ViewChild('authPendingModal') authPendingModal;
  public authPendingModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      "ACTIONS.confirmTitle",
      "authPendingModal",
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

  constructor(private licenseService: LicenseService,
    private messageService: MessageService,
    private menuService: MenuService) { }


  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  canAuthorize(){
    if(!this.menuService.hasFunctionality('CTRLI', 'AUTH') || this.menuService.user.employeeId == this.employeeId) return false;

    if(this.licenseId > 0 && this.status == LicenseStatus[LicenseStatus.AuthPending]){
        return true;
    }

    return false;
  }

  showModal(){
    this.authPendingModal.show();
  }

  authorize(){
    var json = {
        status: LicenseStatus.Pending
    }

    this.subscrip = this.licenseService.changeStatus(this.licenseId, json).subscribe(
        data => {
            this.authPendingModal.hide();
            if(this.history.observers.length > 0){
                this.history.emit();
            }
        
            if(this.updateStatus.observers.length > 0){
                var toModif = {
                    statusId: LicenseStatus.Pending,
                    statusName: LicenseStatus[LicenseStatus.Pending]
                }

                this.updateStatus.emit(toModif);
            }
        },
        error => {
            this.authPendingModal.hide();
        });
    }
}