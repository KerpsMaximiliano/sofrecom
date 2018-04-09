import { Component, OnInit, OnDestroy } from "@angular/core";
import { MessageService } from "app/services/common/message.service";
import { Router, ActivatedRoute } from "@angular/router";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs";
import { CategoryService } from "app/services/admin/category.service";

@Component({
    selector: 'app-category-edit',
    templateUrl: './category-edit.component.html'
  })
  export class CategoryEditComponent implements OnInit, OnDestroy {

    private id: number;
    public description: string;

    public editSubscript: Subscription;
    public paramsSubscrip: Subscription;
    public getSubscrip: Subscription;

    constructor(private messageService: MessageService,
                private router: Router,
                private activatedRoute: ActivatedRoute,
                private categoryService: CategoryService,
                private errorHandlerService: ErrorHandlerService) { }

    ngOnInit(): void {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.id = params['id'];
            this.getDetails();
        });
    }

    ngOnDestroy(): void {
        if(this.editSubscript) this.editSubscript.unsubscribe();
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
    }

    getDetails(){
        this.messageService.showLoading();

        this.getSubscrip = this.categoryService.get(this.id).subscribe(response => {
            this.messageService.closeLoading();
            this.description = response.data.description;
        }, 
        error => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(error);   
        });
    }

    goBack(){
        this.router.navigate(["/admin/categories"]);  
    }

    update(){
        this.messageService.showLoading();

        this.editSubscript = this.categoryService.edit({ id: this.id, description: this.description }).subscribe(response => {
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