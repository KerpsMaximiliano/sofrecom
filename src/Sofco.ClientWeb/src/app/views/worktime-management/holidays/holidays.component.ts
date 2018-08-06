import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { MessageService } from 'app/services/common/message.service';
import { Subscription } from 'rxjs';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { HolidayService } from 'app/services/worktime-management/holiday.service';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { DataTableService } from 'app/services/common/datatable.service';

@Component({
  selector: 'app-holidays',
  templateUrl: './holidays.component.html'
})
export class HolidaysComponent implements OnInit, OnDestroy {

  public loading = false;
  public holidays: any[] = new Array();
  private subscription: Subscription;
  public holidayModel: any = {};
  public selectedYear: number = new Date().getFullYear();

  public editModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      'workTimeManagement.holiday.title',
      'editModal',
      true,
      true,
      'ACTIONS.save',
      'ACTIONS.cancel');

  public confirmImportModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
    'ACTIONS.confirmTitle',
    'confirmImportModal',
    true,
    true,
    'ACTIONS.ACCEPT',
    'ACTIONS.cancel');

    public confirmDeleteModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      'ACTIONS.confirmTitle',
      'confirmDeleteModal',
      true,
      true,
      'ACTIONS.DELETE',
      'ACTIONS.cancel');

  @ViewChild('editModal') editModal;
  @ViewChild('confirmDeleteModal') confirmDeleteModal;
  @ViewChild('confirmImportModal') confirmImportModal;

  constructor(
      private messageService: MessageService,
      private holidayService: HolidayService,
      private datatableService: DataTableService,
      private errorHandlerService: ErrorHandlerService) {
  }

  ngOnInit() {
    this.initControls();
    this.getHolidays();
  }

  ngOnDestroy() {
  }

  initControls() {
    const self = this;
    const nextYear = new Date().getFullYear() + 1;

    (<any>$("#yearControl")).TouchSpin({
        min: 2018,
        max: nextYear
    }).on('change', function() {
      self.selectedYear = $("#yearControl").val();
      self.getHolidays();
    });
  }

  getHolidays() {
    this.subscription = this.holidayService.get(this.selectedYear).subscribe(response => {
      this.holidays = response.data;
      this.initGrid();
    },
    error => {
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
      this.getHolidays();
    },
    error => {
      this.errorHandlerService.handleErrors(error);
      this.editModal.isLoading = false;
    });
  }

  importExternalData() {
    this.confirmImportModal.show();
  }

  editHoliday(item) {
    this.holidayModel = item;
    this.showEditModal(false);
  }

  deleteHoliday(item) {
    this.holidayModel = item;
    this.confirmDeleteModal.show();
  }

  processImport() {
    this.confirmImportModal.hide();
    this.subscription = this.holidayService.importExternalData(this.selectedYear).subscribe(res => {
      if (res.messages) this.messageService.showMessages(res.messages);
      this.getHolidays();
    },
    error => {
      this.errorHandlerService.handleErrors(error);
    });
  }

  processDelete() {
    const id = this.holidayModel.id;
    this.confirmDeleteModal.hide();
    this.subscription = this.holidayService.delete(id).subscribe(response => {
      if (response.messages) this.messageService.showMessages(response.messages);
      this.getHolidays();
    },
    err => {
        this.errorHandlerService.handleErrors(err);
    });
  }

  initGrid() {
    const columns = [0, 1, 2];

    const params = {
        selector: '#holidaysTable',
        columns: columns,
        withExport: false
    };

    this.datatableService.destroy('#holidaysTable');

    this.datatableService.initialize(params);
  }
}
