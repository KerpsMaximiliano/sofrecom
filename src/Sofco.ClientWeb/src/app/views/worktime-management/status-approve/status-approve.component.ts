import { Component, OnDestroy, ViewChild, Output, EventEmitter } from '@angular/core';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { Subscription } from "rxjs";
import { MessageService } from 'app/services/common/message.service';
import { WorktimeService } from '../../../services/worktime-management/worktime.service';

@Component({
  selector: 'worktime-status-approve',
  templateUrl: './status-approve.component.html'
})
export class WorkTimeStatusApproveComponent implements OnDestroy  {

  @ViewChild('approveModal') approveModal;
  public approveModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      "ACTIONS.confirmTitle",
      "approveModal",
      true,
      true,
      "ACTIONS.ACCEPT",
      "ACTIONS.cancel"
  );

  subscrip: Subscription;

  workTime: any;

  @Output() onSuccess: EventEmitter<any> = new EventEmitter();

  constructor(private messageService: MessageService,
    private worktimeService: WorktimeService,
    private errorHandlerService: ErrorHandlerService) { }

  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  approve(worktime){
    this.workTime = worktime;
    this.approveModal.show();
  }

  confirm(){
    this.subscrip = this.worktimeService.approve(this.workTime.id).subscribe(
        data => {
            this.approveModal.hide();
            if(data.messages) this.messageService.showMessages(data.messages);

            if(this.onSuccess.observers.length > 0){
                this.onSuccess.emit();
            }
        },
        error => {
            this.approveModal.hide();
            this.errorHandlerService.handleErrors(error);
        });
    }
}