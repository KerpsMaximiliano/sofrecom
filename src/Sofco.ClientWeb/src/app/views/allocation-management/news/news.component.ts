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
import { I18nService } from 'app/services/common/i18n.service';

@Component({
    selector: 'employee-news',
    templateUrl: './news.component.html',
    styleUrls: ['./news.component.scss']
})

export class NewsComponent implements OnInit, OnDestroy {

    public model: any[] = new Array<any>();
    public endReasonTypes: any[] = new Array<any>();

    getAllSubscrip: Subscription;
    deleteSubscrip: Subscription;
    getEndReasonTypeSubscrip: Subscription;

    newsToConfirm: any;
    indexToConfirm: number;
    confirmBodyAction: string;

    public endReasonType: number = 0;

    public isLoading: boolean = false;

    public rejectComments: string;
 
    @ViewChild('confirmModal') confirmModal;
    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "confirmModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel",
        false
    );

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
                private employeeNewsService: EmployeeNewsService,
                public menuService: MenuService,
                private i18nService: I18nService,
                private messageService: MessageService,
                private dataTableService: DataTableService,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void {
        this.getAll();
        this.getEndReasonTypes();
    }

    ngOnDestroy(): void {
        if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
        if(this.deleteSubscrip) this.deleteSubscrip.unsubscribe();
        if(this.getEndReasonTypeSubscrip) this.getEndReasonTypeSubscrip.unsubscribe();
    }

    getAll(){
        this.messageService.showLoading();

        this.getAllSubscrip = this.employeeNewsService.getAll().subscribe(response => {
            this.model = response.data;

            var options = { selector: "#newsTable" }
            this.dataTableService.initialize(options);
            
            this.messageService.closeLoading();
        },
        error => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(error);
        });
    }

    getEndReasonTypes(){
        this.getEndReasonTypeSubscrip = this.employeeNewsService.getTypeEndReasons().subscribe(response => {
            this.endReasonTypes = response;
        },
        error => {
            this.errorHandlerService.handleErrors(error);
        });
    }

    showConfirmCancel(news, index){
        this.newsToConfirm = news;
        this.indexToConfirm = index;
        this.confirmModal.show();
        this.confirm = this.cancel;
        let statusText = news.isReincorporation ? "Reincorporation" : news.status;
        this.confirmBodyAction = this.i18nService.translateByKey('ACTIONS.annulment') +" "+ this.i18nService.translateByKey(statusText)
    }

    showConfirmAdd(news, index){
        this.newsToConfirm = news;
        this.indexToConfirm = index;
        this.confirmModal.show();
        this.confirm = this.add;
        let statusText = news.isReincorporation ? "Reincorporation" : news.status;
        this.confirmBodyAction = this.i18nService.translateByKey('ACTIONS.confirm') +" "+ this.i18nService.translateByKey(statusText)
    }

    showConfirmDelete(news, index){
        this.newsToConfirm = news;
        this.indexToConfirm = index;
        this.rejectComments = news.endReason;
        this.deleteModal.show();
        this.confirm = this.delete;
        this.confirmBodyAction = this.i18nService.translateByKey('ACTIONS.confirm') +" "+ this.i18nService.translateByKey(news.status)
    }

    confirm(){ }

    add(){
        this.isLoading = true;

        this.getAllSubscrip = this.employeeNewsService.add(this.newsToConfirm.id).subscribe(data => {
            if(data.messages) this.messageService.showMessages(data.messages);

            this.model.splice(this.indexToConfirm, 1);

            this.isLoading = false;
            this.confirmModal.hide();
        },
        error => {
            this.isLoading = false;
            this.errorHandlerService.handleErrors(error);
        });
    }

    cancel(){
        this.isLoading = true;

        this.getAllSubscrip = this.employeeNewsService.cancel(this.newsToConfirm.id).subscribe(data => {
            if(data.messages) this.messageService.showMessages(data.messages);

            this.model.splice(this.indexToConfirm, 1);

            this.isLoading = false;
            this.confirmModal.hide();
        },
        error => {
            this.isLoading = false;
            this.errorHandlerService.handleErrors(error);
        });
    }

    delete(){
        this.isLoading = true;

        var json = {
            comments: this.rejectComments,
            type: this.endReasonType
        }

        this.getAllSubscrip = this.employeeNewsService.delete(this.newsToConfirm.id, json).subscribe(data => {
            if(data.messages) this.messageService.showMessages(data.messages);

            this.model.splice(this.indexToConfirm, 1);

            this.isLoading = false;
            this.deleteModal.hide();
        },
        error => {
            this.isLoading = false;
            this.errorHandlerService.handleErrors(error);
        });
    }
}