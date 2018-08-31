import { Component, OnDestroy, ViewChild, Output, EventEmitter } from '@angular/core';
import { Ng2ModalConfig } from '../../../components/modal/ng2modal-config';
import { Subscription } from "rxjs";
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

  constructor(private worktimeService: WorktimeService) { }

  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  approve(worktime){
    this.workTime = worktime;
    this.approveModal.show();
  }

  confirm(){
    this.subscrip = this.worktimeService.approve(this.workTime.id).subscribe(
        () => {
            this.approveModal.hide();
            if (this.onSuccess.observers.length > 0) {
                this.onSuccess.emit();
            }
        },
        () => {
            this.approveModal.hide();
        });
    }
}