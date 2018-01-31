import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Router } from "@angular/router";
import { DataTableService } from "app/services/common/datatable.service";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from "app/services/common/message.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { EmployeeNewsService } from "app/services/allocation-management/employee-news.service";

@Component({
    selector: 'employee-news',
    templateUrl: './news.component.html',
    styleUrls: ['./news.component.scss']
})

export class NewsComponent implements OnInit, OnDestroy {

    public model: any[] = new Array<any>();

    getAllSubscrip: Subscription;
    deleteSubscrip: Subscription;

    newsToConfirm: any;
    indexToConfirm: number;

    @ViewChild('confirmModal') confirmModal;
    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "confirmModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    constructor(private employeeService: EmployeeService,
                private router: Router,
                private employeeNewsService: EmployeeNewsService,
                public menuService: MenuService,
                private messageService: MessageService,
                private dataTableService: DataTableService,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void {
        this.messageService.showLoading();

        this.getAllSubscrip = this.employeeNewsService.getAll().subscribe(response => {
            this.model = response.data;
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

    showConfirmCancel(news, index){
        this.newsToConfirm = news;
        this.indexToConfirm = index;
        this.confirmModal.show();
        this.confirm = this.cancel;
    }

    showConfirmAdd(news, index){
        this.newsToConfirm = news;
        this.indexToConfirm = index;
        this.confirmModal.show();
        this.confirm = this.add;
    }

    showConfirmDelete(news, index){
        this.newsToConfirm = news;
        this.indexToConfirm = index;
        this.confirmModal.show();
        this.confirm = this.delete;
    }

    confirm(){ }

    add(){
        this.messageService.showLoading();

        this.getAllSubscrip = this.employeeNewsService.add(this.newsToConfirm.id).subscribe(data => {
            if(data.messages) this.messageService.showMessages(data.messages);

            this.model.splice(this.indexToConfirm, 1);

            this.messageService.closeLoading();
            this.confirmModal.hide();
        },
        error => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(error);
        });
    }

    cancel(){
        this.messageService.showLoading();

        this.getAllSubscrip = this.employeeNewsService.delete(this.newsToConfirm.id).subscribe(data => {
            if(data.messages) this.messageService.showMessages(data.messages);

            this.model.splice(this.indexToConfirm, 1);

            this.messageService.closeLoading();
            this.confirmModal.hide();
        },
        error => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(error);
        });
    }

    delete(){
        this.messageService.showLoading();

        this.getAllSubscrip = this.employeeNewsService.delete(this.newsToConfirm.id).subscribe(data => {
            if(data.messages) this.messageService.showMessages(data.messages);

            this.model.splice(this.indexToConfirm, 1);

            this.messageService.closeLoading();
            this.confirmModal.hide();
        },
        error => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(error);
        });
    }
}