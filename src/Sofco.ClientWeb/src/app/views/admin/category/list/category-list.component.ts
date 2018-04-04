import { Component, OnInit, OnDestroy } from "@angular/core";
import { MessageService } from "app/services/common/message.service";
import { Router, ActivatedRoute } from "@angular/router";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs";
import { CategoryService } from "app/services/admin/category.service";
import { DataTableService } from "../../../../services/common/datatable.service";
import { MenuService } from "../../../../services/admin/menu.service";
declare var moment: any;

@Component({
    selector: 'app-category-list',
    templateUrl: './category-list.component.html'
  })
  export class CategoryListComponent implements OnInit, OnDestroy {

    public categories: any[] = new Array(); 

    public getSubscrip: Subscription;
    public deactivateSubscrip: Subscription;
    public activateSubscrip: Subscription;

    constructor(private messageService: MessageService,
                private router: Router,
                public menuService: MenuService,
                private dataTableService: DataTableService,
                private categoryService: CategoryService,
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

        this.getSubscrip = this.categoryService.getAll().subscribe(response => {
            this.messageService.closeLoading();
            this.categories = response;
            this.initGrid();
        }, 
        error => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(error);   
        });
    }

    goToDetail(category){
        this.router.navigate([`/admin/categories/${category.id}/edit`]);
    }

    habInhabClick(category){
        if (category.active){
            this.deactivate(category);
        } else {
            this.activate(category);
        }
    }

    deactivate(category){
        this.deactivateSubscrip = this.categoryService.active(category.id, false).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                category.active = false;
                category.endDate = moment.now();
            },
            err => this.errorHandlerService.handleErrors(err));
    }
  
    activate(category){
        this.activateSubscrip = this.categoryService.active(category.id, true).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                category.active = true;
                category.endDate = null;
            },
            err => this.errorHandlerService.handleErrors(err));
    }

    initGrid(){
        var columns = [0, 1, 2];
        var title = `Categorias-${moment(new Date()).format("YYYYMMDD")}`;

        var params = {
            selector: '#categoryTable',
            columns: columns,
            title: title,
            withExport: true
          }

          this.dataTableService.destroy(params.selector);
          this.dataTableService.init2(params);
    }
  }