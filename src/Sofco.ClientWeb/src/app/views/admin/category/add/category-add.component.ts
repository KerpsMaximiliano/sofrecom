import { Component, OnDestroy } from "@angular/core";
import { MessageService } from "../../../../services/common/message.service";
import { Router } from "@angular/router";
import { Subscription } from "rxjs";
import { CategoryService } from "../../../../services/admin/category.service";

@Component({
    selector: 'app-category-add',
    templateUrl: './category-add.component.html'
  })
  export class CategoryAddComponent implements OnDestroy {

    public description: string;

    public addSubscript: Subscription;

    constructor(private messageService: MessageService,
                private router: Router,
                private categoryService: CategoryService) { }

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
            this.router.navigate(["/admin/categories"]);    
        }, 
        error => this.messageService.closeLoading());
    }
  }