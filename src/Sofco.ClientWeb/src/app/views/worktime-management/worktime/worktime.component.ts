import * as moment from 'moment';
import 'jqueryui';
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
import { OptionsInput } from 'fullcalendar';
import { WorktimeService } from 'app/services/worktime-management/worktime.service';
import { AnalyticService } from 'app/services/allocation-management/analytic.service';
import { EmployeeService } from 'app/services/allocation-management/employee.service';
import { TaskService } from 'app/services/admin/task.service';
import { WorkTimeTaskModel } from 'app/models/worktime-management/WorkTimeTaskModel';
import { RecentTaskModel } from 'app/models/worktime-management/recentTaskModel';
import { RecentAnalyticTaskModel } from 'app/models/worktime-management/recentAnalyticTaskModel';

@Component({
    selector: 'app-worktime',
    templateUrl: './worktime.component.html',
    styleUrls: ['./worktime.component.scss']
  })

export class WorkTimeComponent implements OnInit, OnDestroy {

  public pendingTaskColor = '#f8ac59';

  public analytics: any[] = new Array();
  public categories: any[] = new Array();
  public allTasks: any[] = new Array();
  public tasks: any[] = new Array();
  public recentAnalyticTasks: any[] = new Array();

  private idKey = 'id';
  private textKey = 'text';
  private subscription: Subscription;
  private calendarEvents: any[] = new Array();

  public model: any = {};
  public taskModel: WorkTimeTaskModel = new WorkTimeTaskModel();

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
    this.calendarInit();
    this.eventDragInitWithDocumentReady();
    this.getRecentTask();
    this.getModel();
    this.getAnalytics();
    this.getCategories();
  }

  calendarInit() {
      const self = this;

      $('#calendar').fullCalendar({
        weekends: true,
        header: {
          left: 'prev,next today',
          center: 'title',
          right: 'month,agendaWeek,agendaDay,listWeek'
        },
        navLinks: true, // can click day/week names to navigate views
        editable: false,
        eventLimit: false, // allow "more" link when too many events
        events: this.calendarEvents,
        droppable: true,
        drop: function(date, jsEvent, ui) {
          self.calendarDropHandler(date, jsEvent);
        }
      });
  }

  eventDragInitWithDocumentReady() {
    $(document).ready(this.eventDragInit);
  }

  eventDragInit() {
      const self = this;
      $('#external-events div.external-event').each(function() {
        $(this).data('event', {
            id: 0,
            title: $(this).data('task'),
            color: $(this).data('color'),
            stick: true,
            taskId: $(this).data('taskId'),
            analyticId: $(this).data('analyticId'),
            hours: $(this).data('hours')
        });
        $(this).data('eventObject', {title: 'MyDrag Title'});
        $(this).draggable({
            helper: 'clone',
            zIndex: 1111999,
            // revert: true,
            revertDuration: 0,
            revert: function(droppableObj) {
              if (droppableObj === false) {
                  console.log('Not a droppable object');
                  return true;
              } else {
                return false;
              }
          },
          start: function(e, ui) {
              $(ui.helper).css('width', 180);
          }
        });
      });
  }

  calendarDropHandler(date, source) {
    const element = source.target;
    const analyticId = element.dataset.analyticid;
    const taskId = element.dataset.taskid;
    const hours = element.dataset.hours;

    const taskModel = new WorkTimeTaskModel();
    taskModel.analyticId = analyticId;
    taskModel.taskId = taskId;
    taskModel.hours = hours;
    taskModel.date = date;

    this.saveDropTask(taskModel);
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
      this.updateCalendarEvents();
    },
    error => {
      this.messageService.closeLoading();
      this.errorHandlerService.handleErrors(error);
    });
  }

  updateCalendarEvents() {
    this.calendarEvents = this.translateToEvent(this.model.calendar);
    $('#calendar').fullCalendar('removeEventSources', null);
    $('#calendar').fullCalendar('addEventSource', this.calendarEvents);
    $('#calendarControl').droppable();
  }

  sendHours() {
    this.messageService.showLoading();

    this.subscription = this.worktimeService.sendHours().subscribe(response => {
      this.messageService.closeLoading();
      if(response.messages) this.messageService.showMessages(response.messages);
    },
    error => {
      this.messageService.closeLoading();
      this.errorHandlerService.handleErrors(error);
    });
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
      this.getModel();
      this.addRecentTask(res.data);
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

  translateToEvent(data: Array<any>) {
    const events = [];

    data.forEach(item => {
      events.push(this.setPendingTaskEvent(item));
    });

    return events;
  }

  setPendingTaskEvent(item: any) {
    return {
      id: item.id,
      title: item.taskName,
      start: item.date,
      color: this.pendingTaskColor,
      allDay: true
    };
  }

  translateWorktimeToRecentTask(data) {
    const taskId = data.taskId;

    const task = this.allTasks.find(x => x.id == taskId);

    if (task == null) return;

    const analytic = this.analytics.find(x => x.id == data.analyticId);

    if (analytic == null) return;

    const category = this.categories.find(x => x.id == task.categoryId);

    const recentTask: RecentTaskModel = new RecentTaskModel();

    recentTask.categoryId = task.categoryId;
    recentTask.category = category[this.textKey];
    recentTask.taskId = task[this.idKey];
    recentTask.task = task[this.textKey];
    recentTask.analyticId = analytic[this.idKey];
    recentTask.analytic = analytic[this.textKey];
    recentTask.hours = data.hours;

    return recentTask;
  }

  getRecentTask() {
    const result = JSON.parse(localStorage.getItem('recentAnalyticTasks'));
    if (result == null) return;
    this.recentAnalyticTasks = result;
  }

  addRecentTask(data) {
    const recentTask: RecentTaskModel = this.translateWorktimeToRecentTask(data);

    const recentAnalyticTask = this.recentAnalyticTasks.find(x => x.analyticId == recentTask.analyticId);

    if (recentAnalyticTask == null) {
      const recentItem = new RecentAnalyticTaskModel();
      recentItem.analyticId = recentTask.analyticId;
      recentItem.analytic = recentTask.analytic;
      recentItem.tasks.push(recentTask);
      this.recentAnalyticTasks.push(recentItem);
    } else {
      const storedTask = recentAnalyticTask.tasks.find(x => x.taskId == recentTask.taskId);

      if (storedTask == null) {
        recentAnalyticTask.tasks.push(recentTask);
      }
    }

    this.updateLocalStorage();

    window.setTimeout(this.eventDragInit, 500);
  }

  removeTask(recentTask) {
    const recentAnalyticTask = this.recentAnalyticTasks.find(x => x.analyticId == recentTask.analyticId);

    if (recentAnalyticTask == null) {
      return;
    }

    let storedTaskIndex = recentAnalyticTask.tasks.findIndex(x => x.taskId == recentTask.taskId);
    if (storedTaskIndex > -1) {
      recentAnalyticTask.tasks.splice(storedTaskIndex, 1);
    }

    if (recentAnalyticTask.tasks.length == 0) {
      storedTaskIndex = this.recentAnalyticTasks.findIndex(x => x.analyticId == recentTask.analyticId);
      if (storedTaskIndex > -1) {
        this.recentAnalyticTasks.splice(storedTaskIndex, 1);
      }
    }

    this.updateLocalStorage();
  }

  updateLocalStorage() {
    localStorage.setItem('recentAnalyticTasks', JSON.stringify(this.recentAnalyticTasks));
  }

  saveDropTask(taskModel: WorkTimeTaskModel) {
    this.subscription = this.worktimeService.post(taskModel).subscribe(res => {
      this.getModel();
    },
    error => {
      this.errorHandlerService.handleErrors(error);
      this.removeLastEvent();
    });
  }

  removeLastEvent() {
    this.getModel();
  }
}
