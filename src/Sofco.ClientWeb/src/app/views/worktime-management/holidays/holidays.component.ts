import { Component, OnInit, OnDestroy } from '@angular/core';
import { MessageService } from 'app/services/common/message.service';
import { Subscription } from 'rxjs/Subscription';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { HolidayService } from 'app/services/worktime-management/holiday.service';

@Component({
  selector: 'app-holidays',
  templateUrl: './holidays.component.html'
})
export class HolidaysComponent implements OnInit, OnDestroy {

  public loading = false;
  public holidays: any[] = new Array();
  private subscription: Subscription;

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

  importExternalData() {
  }
}
