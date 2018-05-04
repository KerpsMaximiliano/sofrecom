import { Component, OnInit, OnDestroy, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { Subscription } from "rxjs/Subscription";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from 'app/services/common/message.service';
import { WorktimeService } from '../../../services/worktime-management/worktime.service';
import { WorkTimeStatus } from '../../../models/enums/worktimestatus';

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

  public isLoading: boolean = false;

  public rejectComments: string;
  workTime: any;

  @Output() onSuccess: EventEmitter<any> = new EventEmitter();

  constructor(private messageService: MessageService,
    private menuService: MenuService,
    private worktimeService: WorktimeService,
    private errorHandlerService: ErrorHandlerService) { }

  ngOnDestroy(): void {
    if(this.subscrip) this.subscrip.unsubscribe();
  }

  reject(worktime){
    this.workTime = worktime;
    this.rejectModal.show();
  }

  confirm(){
    this.isLoading = true;

    this.subscrip = this.worktimeService.reject(this.workTime.id, this.rejectComments).subscribe(
        data => {
            this.rejectModal.hide();
            this.isLoading = false 
            if(data.messages) this.messageService.showMessages(data.messages);

            if(this.onSuccess.observers.length > 0){
                this.onSuccess.emit();
            }
        },
        error => {
            this.rejectModal.hide();
            this.isLoading = false 
            this.errorHandlerService.handleErrors(error);
        });
    }
}