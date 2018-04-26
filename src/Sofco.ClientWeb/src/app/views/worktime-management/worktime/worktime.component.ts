import * as moment from 'moment';
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
import { WorktimeService } from 'app/services/worktime-management/worktime.service';
import { AnalyticService } from 'app/services/allocation-management/analytic.service';
import { EmployeeService } from 'app/services/allocation-management/employee.service';
import { TaskService } from 'app/services/admin/task.service';
import { WorkTimeTaskModel } from 'app/models/worktime-management/WorkTimeTaskModel';

@Component({
    selector: 'app-worktime',
    templateUrl: './worktime.component.html',
    styleUrls: ['./worktime.component.scss']
  })

export class WorkTimeComponent implements OnInit, OnDestroy {

  public analytics: any[] = new Array();
  public categories: any[] = new Array();
  public allTasks: any[] = new Array();
  public tasks: any[] = new Array();

  private idKey = 'id';
  private textKey = 'text';
  private subscription: Subscription;

  public model: any = {};
  public taskModel: WorkTimeTaskModel = new WorkTimeTaskModel();

  calendarOptions: Options;

  public loading = false;

  public editModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      'ADMIN.task.title',
      'editModal',
      true,
      true,
      'ACTIONS.save',
      'ACTIONS.cancel');

  @ViewChild('editModal') editModal;
  @ViewChild('dateControl') dateControl;

  constructor(private serviceService: ServiceService,
      private analyticService: AnalyticService,
      private employeeService: EmployeeService,
      private taskService: TaskService,
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
    this.getAnalytics();
    this.getCategories();

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

  getModel() {
    this.messageService.showLoading();

    this.subscription = this.worktimeService.get(moment().format('YYYY-MM-DD')).subscribe(response => {
      this.messageService.closeLoading();

      this.model = response.data;
    },
    error => {
      this.messageService.closeLoading();
      this.errorHandlerService.handleErrors(error);
    });
  }

  sendHours() {
  }

  showEditModal() {
    this.taskModel = new WorkTimeTaskModel();
    this.showSaveTask();
    this.editModal.show();
  }

  getAnalytics() {
    this.subscription = this.analyticService.getByCurrentUser().subscribe(res => {
        this.analytics = res.data;
    },
    error => {
        this.errorHandlerService.handleErrors(error);
    });
  }

  getCategories() {
    this.subscription = this.employeeService.getCurrentCategories().subscribe(res => {
      const categories = {};
      const tasks = [];
      res.data.forEach(item => {
        categories[item.categoryId] = item.category;
        tasks.push({
          [this.idKey]: item.taskId,
          [this.textKey]: item.task,
          categoryId: item.categoryId
        });
      });
      const uniqueCategories = [];
      for (const key in categories) {
        uniqueCategories.push({[this.idKey]: key, [this.textKey]: categories[key]});
      }

      this.categories = uniqueCategories;
      this.allTasks = tasks;
    },
    error => {
      this.errorHandlerService.handleErrors(error);
    });
  }

  getTasks() {
    this.subscription = this.taskService.getOptions().subscribe(res => {
      this.allTasks = res;
    },
    error => {
      this.errorHandlerService.handleErrors(error);
    });
  }

  categoryChange() {
    this.taskModel.taskId = 0;
    this.tasks = this.allTasks.filter(x => x.categoryId == this.taskModel.categoryId);
    this.showSaveTask();
  }

  saveTask() {
    this.editModal.isLoading = true;
    this.subscription = this.worktimeService.post(this.taskModel).subscribe(res => {
      this.editModal.isLoading = false;
      this.editModal.hide();
    },
    error => {
      this.errorHandlerService.handleErrors(error);
      this.editModal.isLoading = false;
    });
  }

  showSaveTask() {
    const taskModel = this.taskModel;
    if (taskModel.analyticId == 0) {
      this.editModal.isSaveEnabled = false;
      return;
    }
    if (taskModel.categoryId == 0) {
      this.editModal.isSaveEnabled = false;
      return;
    }
    if (taskModel.taskId == 0) {
      this.editModal.isSaveEnabled = false;
      return;
    }
    if (taskModel.date == null || !this.validateDate(taskModel.date)) {
      this.editModal.isSaveEnabled = false;
      return;
    }
    if (taskModel.hours == 0) {
      this.editModal.isSaveEnabled = false;
      return;
    }
    this.editModal.isSaveEnabled = true;
  }

  validateDate(date: Date) {
    const today = new Date();

    return date.getMonth() === today.getMonth();
  }
}
