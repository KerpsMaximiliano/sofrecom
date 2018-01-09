import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Router } from "@angular/router";
import { DataTableService } from "app/services/common/datatable.service";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from "app/services/common/message.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";

@Component({
    selector: 'employee-news',
    templateUrl: './news.component.html'
})

export class NewsComponent implements OnInit, OnDestroy {

    public model: any[] = new Array<any>();

    getAllSubscrip: Subscription;
    deleteSubscrip: Subscription;

    newsToDelete: any;
    indexToDelete: number;

    @ViewChild('deleteModal') deleteModal;
    public deleteModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "deleteModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    constructor(private employeeService: EmployeeService,
                private router: Router,
                public menuService: MenuService,
                private messageService: MessageService,
                private dataTableService: DataTableService,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void {
        this.messageService.showLoading();

        this.getAllSubscrip = this.employeeService.getNews().subscribe(data => {
            this.model = data;
            this.dataTableService.init('#newsTable', false);
            this.messageService.closeLoading();
        },
        error => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(error);
        });
    }

    ngOnDestroy(): void {
        if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
        if(this.deleteSubscrip) this.deleteSubscrip.unsubscribe();
    }

    showConfirm(news, index){
        this.newsToDelete = news;
        this.indexToDelete = index;
        this.deleteModal.show();
    }

    delete(){
        this.messageService.showLoading();

        this.getAllSubscrip = this.employeeService.deleteNews(this.newsToDelete.id).subscribe(data => {
            if(data.messages) this.messageService.showMessages(data.messages);

            this.model.splice(this.indexToDelete, 1);

            this.messageService.closeLoading();
            this.deleteModal.hide();
        },
        error => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(error);
        });
    }
}