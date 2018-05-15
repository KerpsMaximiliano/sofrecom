import { Component, OnInit, OnDestroy } from '@angular/core';
import { MessageService } from 'app/services/common/message.service';
import { Subscription } from 'rxjs/Subscription';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';

@Component({
  selector: 'app-holidays',
  templateUrl: './holidays.component.html'
})
export class HolidaysComponent implements OnInit, OnDestroy {

  public loading = false;

  constructor(
      private messageService: MessageService,
      private errorHandlerService: ErrorHandlerService) {
  }

  ngOnInit() {
  }

  ngOnDestroy() {
  }

  importExternalData() {
  }
}
