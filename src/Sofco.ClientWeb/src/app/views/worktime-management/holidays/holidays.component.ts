import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { MessageService } from 'app/services/common/message.service';
import { Subscription } from 'rxjs/Subscription';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { HolidayService } from 'app/services/worktime-management/holiday.service';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';

@Component({
  selector: 'app-holidays',
  templateUrl: './holidays.component.html'
})
export class HolidaysComponent implements OnInit, OnDestroy {

  public loading = false;
  public holidays: any[] = new Array();
  private subscription: Subscription;
  public holidayModel: any = {};

  public editModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      'workTimeManagement.holiday.title',
      'editModal',
      true,
      true,
      'ACTIONS.save',
      'ACTIONS.cancel');

  @ViewChild('editModal') editModal;


  constructor(
      private messageService: MessageService,
      private holidayService: HolidayService,
      private errorHandlerService: ErrorHandlerService) {
  }

  ngOnInit() {
    this.getHolidays();
  }

  ngOnDestroy() {
  }

  getHolidays() {
    this.messageService.showLoading();

    this.subscription = this.holidayService.get().subscribe(response => {
      this.messageService.closeLoading();
      this.holidays = response.data;
    },
    error => {
      this.messageService.closeLoading();
      this.errorHandlerService.handleErrors(error);
    });
  }

  showEditModal(isNew = true) {
    if (isNew) {
      this.holidayModel = {
        id: 0,
        name: "",
        date: new Date()
      };
    }
    this.showSaveModal();
    this.editModal.show();
  }

  showSaveModal() {
    const model = this.holidayModel;

    if (model.name === "") {
      this.editModal.isSaveEnabled = false;
      return;
    }

    if (model.date == null) {
      this.editModal.isSaveEnabled = false;
      return;
    }

    this.editModal.isSaveEnabled = true;
  }

  saveModal() {
    this.editModal.isLoading = true;
    this.subscription = this.holidayService.post(this.holidayModel).subscribe(res => {
      this.editModal.isLoading = false;
      this.editModal.hide();
      if (res.messages) this.messageService.showMessages(res.messages);
      this.holidays.push(res.data);
    },
    error => {
      this.errorHandlerService.handleErrors(error);
      this.editModal.isLoading = false;
    });
  }

  importExternalData() {
    this.messageService.showLoading();
    this.subscription = this.holidayService.importExternalData().subscribe(res => {
      this.messageService.closeLoading();
      if (res.messages) this.messageService.showMessages(res.messages);
      this.holidays = res.data;
    },
    error => {
      this.messageService.closeLoading();
      this.errorHandlerService.handleErrors(error);
    });
  }
}
