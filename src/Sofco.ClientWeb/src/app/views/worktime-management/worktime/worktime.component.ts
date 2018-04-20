import { Component, Input, Output, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { ServiceService } from 'app/services/billing/service.service';
import { CustomerService } from 'app/services/billing/customer.service';
import { UserService } from 'app/services/admin/user.service';
import { I18nService } from 'app/services/common/i18n.service';
import { DataTableService } from 'app/services/common/datatable.service';
import { MessageService } from 'app/services/common/message.service';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { Ng2DatatablesModule } from 'app/components/datatables/ng2-datatables.module';
import { CalendarComponent } from 'ng-fullcalendar';
import { Options } from 'fullcalendar';
import { WorktimeService } from '../../../services/worktime-management/worktime.service';

import * as moment from 'moment';

@Component({
    selector: 'app-worktime',
    templateUrl: './worktime.component.html',
    styleUrls: ['./worktime.component.scss']
  })

export class WorkTimeComponent implements OnInit, OnDestroy {

    private subscription: Subscription;

    @Input()
    public model: any;

    private idKey = 'id';
    private textKey = 'text';

    private itemSelected: any;

    calendarOptions: Options;

    public loading = false;

    public modalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        'ACTIONS.confirmTitle',
        'confirmModal',
        true,
        true,
        'ACTIONS.DELETE',
        'ACTIONS.cancel');

    @ViewChild('confirmModal') confirmModal;

    constructor(private serviceService: ServiceService,
        private customerService: CustomerService,
        private usersService: UserService,
        private worktimeService: WorktimeService,
        private errorHandlerService: ErrorHandlerService,
        private dataTableService: DataTableService,
        private messageService: MessageService,
        private i18nService: I18nService,
        private router: Router) {
    }

    ngOnInit() {
      this.getModel();

      this.calendarOptions = {
        weekends: true,
          header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,agendaWeek,agendaDay,listWeek'
          },
          navLinks: true, // can click day/week names to navigate views
          editable: true,
          eventLimit: false, // allow "more" link when too many events
          events: [
            {
              title: '8 h - Gestión de Servicio',
              start: '2018-01-01',
            },
            {
              title: '8 h - Gestión de Servicio',
              start: '2018-04-07',
              end: '2018-04-10'
            },
            {
              id: 999,
              title: '4 h - Gestión de Servicio',
              start: '2018-04-16T09:00:00'
            },
            {
              title: '8 h - Gestión de Servicio',
              start: '2018-04-12T09:00:00'
            },
            {
              title: '6 h - Gestión de Servicio',
              start: '2018-04-13T09:00:00'
            }
          ]
        };
    }

    ngOnDestroy() {
        if (this.subscription) {
            this.subscription.unsubscribe();
        }
    }

    getModel(){
      this.messageService.showLoading();

      this.subscription = this.worktimeService.get(moment().format('YYYY-MM-DD')).subscribe(response => {
        this.messageService.closeLoading();

        this.model = response.data;
      },
      error =>{
        this.messageService.closeLoading();
        this.errorHandlerService.handleErrors(error);
      });
    }

    sendHours() {
      console.log('> sendHours ---');
    }
}
