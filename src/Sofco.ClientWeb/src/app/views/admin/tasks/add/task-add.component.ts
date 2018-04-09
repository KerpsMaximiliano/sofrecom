import { Component, OnInit, OnDestroy } from "@angular/core";
import { MessageService } from "app/services/common/message.service";
import { Router } from "@angular/router";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs";
import { CategoryService } from "app/services/admin/category.service";
import { TaskService } from "app/services/admin/task.service";

@Component({
    selector: 'app-task-add',
    templateUrl: './task-add.component.html'
  })
  export class TaskAddComponent implements OnInit, OnDestroy {

    public description: string;
    public categoryId: number = 0;

    public categories: any[] = new Array();

    public addSubscript: Subscription;
    public categoriesSubscript: Subscription;

    constructor(private messageService: MessageService,
                private router: Router,
                private taskService: TaskService,
                private categoryService: CategoryService,
                private errorHandlerService: ErrorHandlerService) { }

    ngOnInit(): void {
        this.messageService.showLoading();

        this.categoriesSubscript = this.categoryService.getOptions().subscribe(response => {
            this.messageService.closeLoading();
            this.categories = response;
        }, 
        error => {
            this.errorHandlerService.handleErrors(error);
            this.messageService.closeLoading();
        });
    }

    ngOnDestroy(): void {
        if(this.addSubscript) this.addSubscript.unsubscribe();
        if(this.categoriesSubscript) this.categoriesSubscript.unsubscribe();
    }

    goBack(){
        this.router.navigate(["/admin/tasks"]);  
    }
    
    save(){
        this.messageService.showLoading();

        this.addSubscript = this.taskService.add(this.description, this.categoryId).subscribe(response => {
            this.messageService.closeLoading();

            if(response.messages) this.messageService.showMessages(response.messages);
            this.router.navigate(["/admin/tasks"]);    
        }, 
        error => { 
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(error);
         });
    }
  }