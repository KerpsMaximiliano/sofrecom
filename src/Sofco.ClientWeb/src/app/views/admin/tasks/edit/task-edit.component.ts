import { Component, OnInit, OnDestroy } from "@angular/core";
import { MessageService } from "../../../../services/common/message.service";
import { Router, ActivatedRoute } from "@angular/router";
import { Subscription } from "rxjs";
import { TaskService } from "../../../../services/admin/task.service";
import { CategoryService } from "../../../../services/admin/category.service";

@Component({
    selector: 'app-task-edit',
    templateUrl: './task-edit.component.html'
  })
  export class TaskEditComponent implements OnInit, OnDestroy {

    private id: number;
    public description: string;
    public categoryId: number;

    public categories: any[] = new Array();
    
    public editSubscript: Subscription;
    public paramsSubscrip: Subscription;
    public getSubscrip: Subscription;
    public categoriesSubscript: Subscription;

    constructor(private messageService: MessageService,
                private router: Router,
                private activatedRoute: ActivatedRoute,
                private taskService: TaskService,
                private categoryService: CategoryService) { }

    ngOnInit(): void {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.id = params['id'];
            this.getDetails();
        });

        this.categoriesSubscript = this.categoryService.getOptions().subscribe(response => {
            this.messageService.closeLoading();
            this.categories = response;
        }, 
        error => this.messageService.closeLoading());
    }

    ngOnDestroy(): void {
        if(this.editSubscript) this.editSubscript.unsubscribe();
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
        if(this.categoriesSubscript) this.categoriesSubscript.unsubscribe();
    }

    getDetails(){
        this.messageService.showLoading();

        this.getSubscrip = this.taskService.get(this.id).subscribe(response => {
            this.messageService.closeLoading();
            this.description = response.data.description;
            this.categoryId = response.data.categoryId;
        }, 
        error => this.messageService.closeLoading());
    }

    goBack(){
        this.router.navigate(["/admin/tasks"]);  
    }

    update(){
        this.messageService.showLoading();

        this.editSubscript = this.taskService.edit({ id: this.id, description: this.description, categoryId: this.categoryId }).subscribe(response => {
            this.messageService.closeLoading();
            this.router.navigate(["/admin/tasks"]);    
        }, 
        error => this.messageService.closeLoading());
    }
  }