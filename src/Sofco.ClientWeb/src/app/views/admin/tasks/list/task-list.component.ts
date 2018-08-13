import { Component, OnInit, OnDestroy } from "@angular/core";
import { MessageService } from "../../../../services/common/message.service";
import { Router } from "@angular/router";
import { Subscription } from "rxjs";
import { DataTableService } from "../../../../services/common/datatable.service";
import { TaskService } from "../../../../services/admin/task.service";
import { MenuService } from "../../../../services/admin/menu.service";
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
                public menuService: MenuService,
                private dataTableService: DataTableService,
                private taskService: TaskService) { }

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
        error => this.messageService.closeLoading());
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
                task.active = false;
                task.endDate = moment.now();
            });
    }
  
    activate(task){
        this.activateSubscrip = this.taskService.active(task.id, true).subscribe(
            data => {
                task.active = true;
                task.endDate = null;
            });
    }

    initGrid(){
        var columns = [0, 1, 2, 3];
        var title = `Tareas-${moment(new Date()).format("YYYYMMDD")}`;

        var params = {
            selector: '#taskTable',
            columns: columns,
            title: title,
            withExport: true,
            columnDefs: [ {'aTargets': [2, 3], "sType": "date-uk"} ]
          }

          this.dataTableService.destroy(params.selector);
          this.dataTableService.initialize(params);
    }
  }