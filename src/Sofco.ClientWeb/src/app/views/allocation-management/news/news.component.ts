import { Component, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Router } from "@angular/router";
import { DataTableService } from "app/services/common/datatable.service";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from "app/services/common/message.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";

@Component({
    selector: 'employee-news',
    templateUrl: './news.component.html'
})

export class NewsComponent implements OnInit, OnDestroy {

    public model: any[] = new Array<any>();

    getAllSubscrip: Subscription;

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
    }
}