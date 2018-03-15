import { Component, OnInit, OnDestroy, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { Subscription } from "rxjs/Subscription";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from 'app/services/common/message.service';
import { Router } from '@angular/router';
import { I18nService } from 'app/services/common/i18n.service';
import { LicenseService } from 'app/services/human-resources/licenses.service';
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
  @Input() status: string;

  @Output() history: EventEmitter<any> = new EventEmitter();
  @Output() updateStatus: EventEmitter<any> = new EventEmitter();

  subscrip: Subscription;

  public isLoading: boolean = false;

  public statusToSend: any;

  constructor(private licenseService: LicenseService,
    private messageService: MessageService,
    private menuService: MenuService,
    private i18nService: I18nService,
    private errorHandlerService: ErrorHandlerService,
    private router: Router) { }

  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  canApproveWithPendingDocumentation(){
    if(this.licenseId > 0 && this.status == LicenseStatus[LicenseStatus.Pending]){
        return true;
    }

    return false;
  }

  canApprove(){
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

    this.isLoading = true;

    this.subscrip = this.licenseService.changeStatus(this.licenseId, json).subscribe(
        data => {
            this.pendingModal.hide();
            this.isLoading = false 
            if(data.messages) this.messageService.showMessages(data.messages);

            if(this.history.observers.length > 0){
                this.history.emit();
            }
        
            if(this.updateStatus.observers.length > 0){
                var toModif = {
                    statusId: this.statusToSend,
                    statusName: LicenseStatus[this.statusToSend]
                }

                this.updateStatus.emit(toModif);
            }
        },
        error => {
            this.pendingModal.hide();
            this.isLoading = false 
            this.errorHandlerService.handleErrors(error);
        });
    }
}