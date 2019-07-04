import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { HolidayService } from '../../../services/worktime-management/holiday.service';
import { Ng2ModalConfig } from '../../../components/modal/ng2modal-config';
import { DataTableService } from '../../../services/common/datatable.service';
import { MessageService } from '../../../services/common/message.service';

@Component({
  selector: 'app-holidays',
  templateUrl: './holidays.component.html'
})
export class HolidaysComponent implements OnInit, OnDestroy {

  public loading = false;
  public holidays: any[] = new Array();
  subscription: Subscription;
  public holidayModel: any = {};
  public selectedYear: number = new Date().getFullYear();
  public minDate: Date = new Date();
   

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
  @ViewChild('dateControl') dateControl;

  constructor(
      private holidayService: HolidayService,
      private messageService: MessageService,
      private datatableService: DataTableService) {
  }

  ngOnInit() {
    this.initControls();
    this.getHolidays();
  }

  ngOnDestroy() {
  }

  initControls() {    
    this.minDate.setDate(this.minDate.getDate() + 1);
    this.dateControl.minDate = this.minDate;

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
    });
  }

  showEditModal(isNew = true) {
    if (isNew) {
      this.holidayModel = {
        id: 0,
        name: "",
        date: this.minDate
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
      this.getHolidays();
      this.messageService.closeLoading();
    },
    error => {
      this.editModal.resetButtons()
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
    this.messageService.showLoading();
    this.confirmImportModal.hide();
    this.subscription = this.holidayService.importExternalData(this.selectedYear).subscribe(res => {
      this.messageService.closeLoading();
      this.getHolidays();
    },
    error => {
      this.messageService.closeLoading();
    });
  }

  processDelete() {
    const id = this.holidayModel.id;
    this.confirmDeleteModal.hide();
    this.subscription = this.holidayService.delete(id).subscribe(response => {
      this.getHolidays();
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
