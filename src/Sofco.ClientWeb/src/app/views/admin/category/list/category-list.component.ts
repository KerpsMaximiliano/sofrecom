import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { MessageService } from "../../../../services/common/message.service";
import { Router } from "@angular/router";
import { Subscription } from "rxjs";
import { CategoryService } from "../../../../services/admin/category.service";
import { DataTableService } from "../../../../services/common/datatable.service";
import { MenuService } from "../../../../services/admin/menu.service";
import { Ng2ModalConfig } from "../../../../components/modal/ng2modal-config";
declare var moment: any;

@Component({
    selector: 'app-category-list',
    templateUrl: './category-list.component.html'
  })
  export class CategoryListComponent implements OnInit, OnDestroy {

    @ViewChild('confirmModal') confirmModal;
    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "confirmModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    public categories: any[] = new Array(); 

    public getSubscrip: Subscription;
    public deactivateSubscrip: Subscription;
    public activateSubscrip: Subscription;

    private categorySelected;

    constructor(private messageService: MessageService,
                private router: Router,
                public menuService: MenuService,
                private dataTableService: DataTableService,
                private categoryService: CategoryService) { }

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
        error => this.messageService.closeLoading());
    }

    goToDetail(category){
        this.router.navigate([`/admin/categories/${category.id}/edit`]);
    }

    habInhabClick(category){
        this.categorySelected = category

        if (category.active){
            this.confirm = this.deactivate;    
        } else {
            this.confirm = this.activate; 
        }

        this.confirmModal.show();
    }

    confirm(){}
    
    deactivate(){
        this.deactivateSubscrip = this.categoryService.active(this.categorySelected.id, false).subscribe(
            data => {
                this.confirmModal.hide();

                this.categorySelected.active = false;
                this.categorySelected.endDate = moment.now();

                this.categorySelected = null;
            },
            err => this.confirmModal.hide());
    }
  
    activate(){
        this.activateSubscrip = this.categoryService.active(this.categorySelected.id, true).subscribe(
            data => {
                this.confirmModal.hide();

                this.categorySelected.active = true;
                this.categorySelected.endDate = null;

                this.categorySelected = null;
            },
            err => this.confirmModal.hide());
    }

    initGrid(){
        var columns = [0, 1, 2];
        var title = `Categorias-${moment(new Date()).format("YYYYMMDD")}`;

        var params = {
            selector: '#categoryTable',
            columns: columns,
            title: title,
            withExport: true,
            columnDefs: [ {'aTargets': [1, 2], "sType": "date-uk"} ]
          }

          this.dataTableService.destroy(params.selector);
          this.dataTableService.initialize(params);
    }
  }