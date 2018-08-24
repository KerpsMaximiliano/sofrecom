import * as moment from 'moment';
import 'jqueryui';
import { Component, Input, Output, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { MessageService } from '../../../services/common/message.service';
import { Ng2ModalConfig } from '../../../components/modal/ng2modal-config';
import { OptionsInput } from 'fullcalendar';
import { WorktimeService } from '../../../services/worktime-management/worktime.service';
import { AnalyticService } from '../../../services/allocation-management/analytic.service';
import { EmployeeService } from '../../../services/allocation-management/employee.service';
import { TaskService } from '../../../services/admin/task.service';
import { WorkTimeTaskModel } from '../../../models/worktime-management/workTimeTask.model';
import { RecentTaskModel } from '../../../models/worktime-management/recentTaskModel';
import { RecentAnalyticTaskModel } from '../../../models/worktime-management/recentAnalyticTaskModel';
import { AppSetting } from '../../../services/common/app-setting';

@Component({
    selector: 'app-worktime',
    templateUrl: './worktime.component.html',
    styleUrls: ['./worktime.component.scss']
  })

export class WorkTimeComponent implements OnInit, OnDestroy {

  public draftStatus = 1;
  public sentStatus = 2;
  public approvedStatus = 3;
  public rejectedStatus = 4;
  public licenseStatus = 5;

  public draftTaskColor = '#f8ac59';
  public sentTaskColor = '#1d84c6';
  public approvedTaskColor = '#1cb394';
  public rejectedTaskColor = '#ea5865';
  public licenseTaskColor = '#AFAFAF';
  public taskColors: any[] = new Array();

  public analytics: any[] = new Array();
  public categories: any[] = new Array();
  public allTasks: any[] = new Array();
  public tasks: any[] = new Array();
  public recentAnalyticTasks: any[] = new Array();
  public holidays: any[] = new Array();

  private idKey = 'id';
  private textKey = 'text';
  private subscription: Subscription;
  private calendarEvents: any[] = new Array();
  private calendarCurrentDateText = "";
  private calendarCurrentDate = new Date();

  public model: any = {};
  public taskModel: WorkTimeTaskModel = new WorkTimeTaskModel();

  public editModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      'ADMIN.task.title',
      'editModal',
      false,
      true,
      'ACTIONS.save',
      'ACTIONS.cancel');

  @ViewChild('editModal') editModal;
  @ViewChild('dateControl') dateControl;

  constructor(private analyticService: AnalyticService,
      private employeeService: EmployeeService,
      private taskService: TaskService,
      private worktimeService: WorktimeService,
      private messageService: MessageService,
      private appSetting: AppSetting) {

        this.editModalConfig.deleteButton = true;
        this.editModalConfig.acceptInlineButton = true;
        this.editModalConfig.isDraggable = true;
  }

  ngOnInit() {
    this.setTaskColors();
    this.calendarInit();
    this.eventDragInitWithDocumentReady();
    this.getRecentTask();
    this.getModel();
    this.getAnalytics();
    this.getCategories();
  }

  toggleNavigation(): void {
    $(".theme-config-box").toggleClass("show");
  }

  setTaskColors() {
    this.taskColors[0] = this.draftTaskColor;
    this.taskColors[this.draftStatus] = this.draftTaskColor;
    this.taskColors[this.sentStatus] = this.sentTaskColor;
    this.taskColors[this.approvedStatus] = this.approvedTaskColor;
    this.taskColors[this.rejectedStatus] = this.rejectedTaskColor;
    this.taskColors[this.licenseStatus] = this.licenseTaskColor;
  }

  deleteTask(){
    this.subscription = this.worktimeService.delete(this.taskModel.id).subscribe(response => {
      var taskToRemove = this.model.calendar.findIndex(item => item.id == this.taskModel.id);
      this.model.calendar.splice(taskToRemove, 1);
      this.updateCalendarEvents();

      this.getModel();
    },
    error => {},
    () => {
      this.editModal.hide();
    });
  }

  calendarInit() {
    const self = this;

    $('#calendar').fullCalendar({
      weekends: true,
      header: {
        left: 'prev,next today',
        center: 'title',
        right: 'month'
      },
      navLinks: false,
      editable: false,
      eventLimit: false,
      events: this.calendarEvents,
      droppable: true,
      drop: function(date, jsEvent, ui) {
        self.calendarDropHandler(date, jsEvent);
      },
      eventClick: function(calEvent, jsEvent, view) {
        self.eventClickHandler(calEvent);
      },
      dayRender: function(date, cell) {
        self.setNotWorkingDayCalendar(date, cell);
      },
      viewRender: function(view, element) {
        self.viewRenderHandler(view, element);
      },
      eventAfterAllRender: function(view) {
        self.eventAfterAllRender(view);
      }
    });

    (<any>$("#hoursControl")).TouchSpin({
        min: 1,
        max: 24
    }).on('change', function() {
      self.taskModel.hours = $("#hoursControl").val();
      self.showSaveTask();
    });
  }

  viewRenderHandler(view, element) {
   const calendarMonth = view.start.add(1, 'M');

   this.calendarCurrentDate = calendarMonth;
   this.calendarCurrentDateText = calendarMonth.format('YYYY-MM-DD');

    this.getModel();
  }

  eventClickHandler(calEvent) {
    const task = this.model.calendar.find(x => x.id == calEvent.id);
    if (task.status === this.licenseStatus) { return; }
    const cloned = Object.assign({}, task);
    this.editTask(cloned);
  }

  editTask(task) {
    this.taskModel = task;
    $("#hoursControl").val(this.taskModel.hours);
    this.taskModel.date = moment(task.date).toDate();
    const storedTask = this.allTasks.find(x => x.id == task.taskId);
    if (storedTask == null) {
      return;
    }
    this.taskModel.categoryId = storedTask.categoryId;
    this.showEditModal(false);
  }

  setNotWorkingDayCalendar(date, cell) {
    if (date.isoWeekday() > 5) {
      cell.css("background-color", '#EFEFEF');
    }
  }

  eventDragInitWithDocumentReady() {
    $(document).ready(this.eventDragInit);
  }

  eventDragInit() {
      const self = this;
      $('.external-event').each(function() {
        $(this).data('event', {
            id: 0,
            title: $(this).data('task'),
            color: $(this).data('color'),
            stick: true,
            taskId: $(this).data('taskId'),
            analyticId: $(this).data('analyticId'),
            hours: $(this).data('hours')
        });
        $(this).draggable({
            helper: 'clone',
            zIndex: 1111999,
            revertDuration: 0,
            revert: function(droppableObj) { return droppableObj === false; },
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
    taskModel.date = date.toDate();

    this.saveDropTask(taskModel);
  }

  ngOnDestroy() {
      if (this.subscription) {
          this.subscription.unsubscribe();
      }
  }

  getModel() {
    const calendarDate = this.calendarCurrentDateText;

    this.messageService.showLoading();

    this.subscription = this.worktimeService.get(calendarDate).subscribe(response => {
      this.messageService.closeLoading();
      this.model = response.data;
      this.updateCalendarEvents();
    },
    error => {
      this.messageService.closeLoading();
    });
  }

  updateCalendarEvents() {
    this.calendarEvents = this.translateToEvent(this.model.calendar);
    $('#calendar').fullCalendar('removeEventSources', null);
    $('#calendar').fullCalendar('addEventSource', this.calendarEvents);
  }

  sendHours() {
    this.subscription = this.worktimeService.sendHours().subscribe(response => {
      this.getModel();
    },
    error => {},
    () => {
      this.messageService.closeLoading();
    });
  }

  showEditModal(isNew = true) {
    $("#hoursControl").prop('disabled', false);
    this.editModalConfig.cancelButtonText = 'ACTIONS.cancel';
    if (isNew) {
      this.taskModel = new WorkTimeTaskModel();
    } else {
      this.updateModalTaskCombo();
      if (this.taskModel.status !== this.draftStatus
        && this.taskModel.status !== this.rejectedStatus) {
        this.editModalConfig.acceptInlineButton = false;
        this.editModalConfig.cancelButtonText = 'ACTIONS.close';
        $("#hoursControl").prop('disabled', true);
      }
    }
    this.showSaveTask();

    if(this.taskModel.status != 1 && this.taskModel.status != 4){
      this.editModalConfig.deleteButton = false;
    }
    else{
      this.editModalConfig.deleteButton = true;
    }

    this.editModal.show();
  }

  getAnalytics() {
    this.subscription = this.analyticService.getByCurrentUser().subscribe(res => {
        this.analytics = res.data;
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
    });
  }

  getTasks() {
    this.subscription = this.taskService.getOptions().subscribe(res => {
      this.allTasks = res;
    });
  }

  categoryChange() {
    this.taskModel.taskId = 0;
    this.tasks = this.allTasks.filter(x => x.categoryId == this.taskModel.categoryId);
    this.showSaveTask();
  }

  updateModalTaskCombo() {
    this.tasks = this.allTasks.filter(x => x.categoryId == this.taskModel.categoryId);
  }

  saveTask() {
    if (!this.validateHoursPerDay(this.taskModel)) { return; }

    this.subscription = this.worktimeService.post(this.taskModel).subscribe(res => {
      this.editModal.hide();
      this.getModel();
      this.addRecentTask(res.data);
    },
    error => {
      this.editModal.resetButtons();
    });
  }

  showSaveTask() {
    const taskModel = this.taskModel;

    if (taskModel.status !== 0
        && taskModel.status !== this.draftStatus
        && taskModel.status !== this.rejectedStatus) {
      this.editModal.isSaveEnabled = false;
      return;
    }

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

  validateDate(date) {
    const today = new Date();

    return date.getMonth() === today.getMonth();
  }

  translateToEvent(data: Array<any>) {
    const events = [];

    data.forEach(item => {
      events.push(this.setTaskEvent(item));
    });

    return events;
  }

  setTaskEvent(item: any) {
    const color = this.translateStatusColor(item.status);
    const className = (item.status !== this.licenseStatus) ? '' : 'eventTask';
    const title = item.analyticTitle + ' ' + item.hours + 'h-' + item.taskName;
    return {
      id: item.id,
      title: title,
      start: item.date,
      color: color,
      allDay: true,
      className: className,
      hours: item.hours
    };
  }

  translateStatusColor(status): string {
    return this.taskColors[status];
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
    if (!this.validateHoursPerDay(taskModel)) { return; }

    this.messageService.showLoading();

    this.subscription = this.worktimeService.post(taskModel).subscribe(res => {
      this.messageService.closeLoading();
      this.getModel();
    },
    error => {
      this.messageService.closeLoading();
      this.getModel();
    });
  }

  validateHoursPerDay(task: WorkTimeTaskModel) {
    const events = (<any>$('#calendar')).fullCalendar('clientEvents');

    const dayTask = task.date.getUTCDate();
    const monthTask = task.date.getUTCMonth();

    const dayEvents: Array<any> = events.filter(x => x.start.date() === dayTask
      && x.start.month() === monthTask
      && x.id !== task.id);

    let totalHours = parseInt(task.hours.toString());
    for (const item of dayEvents) { totalHours += item.hours; }

    if (totalHours > this.appSetting.WorkingHoursPerDaysMax) {
      this.messageService.showErrorByFolder("workTimeManagement/workTime", "hoursMaxError?" + this.appSetting.WorkingHoursPerDaysMax);
      (<any>$('#calendar')).fullCalendar('removeEvents', function(evt) { return evt.id === 0; });

      this.editModal.resetButtons();
      return false;
    }

    return true;
  }

  private eventAfterAllRender(view) {
    const self = this;
    $('.fc-day:not(.fc-disabled-day)').each(function(index, element) {
      const calendarDate = (<any>element).dataset.date;

      const holidays = self.model.holidays;

      if (holidays == undefined) { return; }

      let isHoliday = false;

      for (const item of holidays) {
        const holidayDate = moment(item.date).format('YYYY-MM-DD');
        if (holidayDate === calendarDate) { isHoliday = true; }
      }

      if (!isHoliday) { return; }

      $(this).css('background-color', '#EFEFEF');
    });
  }
}
