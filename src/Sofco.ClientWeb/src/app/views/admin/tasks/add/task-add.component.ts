import { Component, OnInit, OnDestroy } from "@angular/core";
import { MessageService } from "app/services/common/message.service";
import { Router } from "@angular/router";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs";
import { CategoryService } from "app/services/admin/category.service";

@Component({
    selector: 'app-task-add',
    templateUrl: './task-add.component.html'
  })
  export class TaskAddComponent implements OnDestroy {

    public description: string;
    public categoryId: string;

    public addSubscript: Subscription;

    constructor(private messageService: MessageService,
                private router: Router,
                private categoryService: CategoryService,
                private errorHandlerService: ErrorHandlerService) { }

    ngOnDestroy(): void {
        if(this.addSubscript) this.addSubscript.unsubscribe();
    }

    goBack(){
        this.router.navigate(["/admin/categories"]);  
    }
    
    save(){
        this.messageService.showLoading();

        this.addSubscript = this.categoryService.add(this.description).subscribe(response => {
            this.messageService.closeLoading();

            if(response.messages) this.messageService.showMessages(response.messages);
            this.router.navigate(["/admin/categories"]);    
        }, 
        error => { 
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(error);
         });
    }
  }