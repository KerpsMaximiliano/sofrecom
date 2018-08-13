import { Component, OnDestroy, ViewChild, Output, EventEmitter } from '@angular/core';
import { Ng2ModalConfig } from '../../../components/modal/ng2modal-config';
import { Subscription } from "rxjs";
import { MessageService } from '../../../services/common/message.service';
import { WorktimeService } from '../../../services/worktime-management/worktime.service';

@Component({
  selector: 'worktime-status-reject',
  templateUrl: './status-reject.component.html'
})
export class WorkTimeStatusRejectComponent implements OnDestroy  {

  @ViewChild('rejectModal') rejectModal;
  public rejectModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      "ACTIONS.confirmTitle",
      "rejectModal",
      true,
      true,
      "ACTIONS.ACCEPT",
      "ACTIONS.cancel"
  );

  subscrip: Subscription;

  public rejectComments: string;
  workTime: any;

  @Output() onSuccess: EventEmitter<any> = new EventEmitter();

  constructor(private messageService: MessageService,
    private worktimeService: WorktimeService) { }

  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  reject(worktime){
    this.workTime = worktime;
    this.rejectModal.show();
  }
 
  confirm(){
    if(!this.rejectComments || this.rejectComments == ""){
        this.messageService.showError("billing.solfac.rejectCommentRequired");
        this.rejectModal.resetButtons();
        return;
    }

    this.subscrip = this.worktimeService.reject(this.workTime.id, this.rejectComments).subscribe(
        data => {
            this.rejectModal.hide();

            if(this.onSuccess.observers.length > 0){
                this.onSuccess.emit();
            }
        },
        error => {
            this.rejectModal.hide();
        });
    }
}