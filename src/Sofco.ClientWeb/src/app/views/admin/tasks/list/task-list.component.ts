import { Component, OnInit, OnDestroy } from "@angular/core";
import { MessageService } from "app/services/common/message.service";
import { Router, ActivatedRoute } from "@angular/router";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs";
import { DataTableService } from "app/services/common/datatable.service";
import { TaskService } from "app/services/admin/task.service";
declare var moment: any;

@Component({
    selector: 'app-task-list',
    templateUrl: './task-list.component.html'
  })
  export class TaskListComponent implements OnInit, OnDestroy {

    public tasks: any[] = new Array(); 

    public getSubscrip: Subscription;
    public deactivateSubscrip: Subscription;
    public activateSubscrip: Subscription;

    constructor(private messageService: MessageService,
                private router: Router,
                private dataTableService: DataTableService,
                private taskService: TaskService,
                private errorHandlerService: ErrorHandlerService) { }

    ngOnInit(): void {
        this.getAll();
    }

    ngOnDestroy(): void {
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
        if(this.deactivateSubscrip) this.deactivateSubscrip.unsubscribe();
        if(this.activateSubscrip) this.activateSubscrip.unsubscribe();
    }

    getAll(){
        this.messageService.showLoading();

        this.getSubscrip = this.taskService.getAll().subscribe(response => {
            this.messageService.closeLoading();
            this.tasks = response;
            this.initGrid();
        }, 
        error => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(error);   
        });
    }

    goToDetail(task){
        this.router.navigate([`/admin/tasks/${task.id}/edit`]);
    }

    habInhabClick(task){
        if (task.active){
            this.deactivate(task);
        } else {
            this.activate(task);
        }
    }

    deactivate(task){
        this.deactivateSubscrip = this.taskService.active(task.id, false).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                task.active = false;
                task.endDate = moment.now();
            },
            err => this.errorHandlerService.handleErrors(err));
    }
  
    activate(task){
        this.activateSubscrip = this.taskService.active(task.id, true).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                task.active = true;
                task.endDate = null;
            },
            err => this.errorHandlerService.handleErrors(err));
    }

    initGrid(){
        var columns = [0, 1, 2];
        var title = `Tareas-${moment(new Date()).format("YYYYMMDD")}`;

        var params = {
            selector: '#taskTable',
            columns: columns,
            title: title,
            withExport: true
          }

          this.dataTableService.destroy(params.selector);
          this.dataTableService.init2(params);
    }
  }