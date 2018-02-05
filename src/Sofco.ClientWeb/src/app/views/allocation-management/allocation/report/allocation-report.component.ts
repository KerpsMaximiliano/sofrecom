import { Component, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Router } from "@angular/router";
import { DataTableService } from "app/services/common/datatable.service";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from "app/services/common/message.service";
import { AllocationService } from "app/services/allocation-management/allocation.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { AnalyticService } from "app/services/allocation-management/analytic.service";

declare var $: any;
declare var moment: any;

@Component({
    selector: 'allocation-report',
    templateUrl: './allocation-report.component.html',
    styleUrls: ['./allocation-report.component.scss']
})
export class AllocationReportComponent implements OnInit, OnDestroy {

    public model: any;
    public resources: any[] = new Array<any>();
    public analytics: any[] = new Array<any>();
    public percentages: any[] = new Array<any>();

    createReportSubscrip: Subscription;
    getAllocationResourcesSubscrip: Subscription;
    getAnalyticsSubscrip: Subscription;

    public dateSince: Date = new Date();
    public dateTo: Date = new Date();
    public dateOptions;

    constructor(private allocationService: AllocationService,
                private router: Router,
                private employeeService: EmployeeService,
                private analyticService: AnalyticService,
                public menuService: MenuService,
                private messageService: MessageService,
                private dataTableService: DataTableService,
                private errorHandlerService: ErrorHandlerService){

                this.dateOptions = this.menuService.getDatePickerOptions();
    }
 
    ngOnInit(): void {
        this.getAllocationResources();
        this.getAnalytics();
        this.getPercentages();
    }

    ngOnDestroy(): void {
        if(this.createReportSubscrip) this.createReportSubscrip.unsubscribe();
        if(this.getAllocationResourcesSubscrip) this.getAllocationResourcesSubscrip.unsubscribe();
        if(this.getAnalyticsSubscrip) this.getAnalyticsSubscrip.unsubscribe();
    }

    getAllocationResources(){
        this.getAllocationResourcesSubscrip = this.employeeService.getOptions().subscribe(data => {
            this.resources = data;
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    getAnalytics(){
        this.getAnalyticsSubscrip = this.analyticService.getOptions().subscribe(data => {
            this.analytics = data;
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    getPercentages(){
        this.getAnalyticsSubscrip = this.allocationService.getAllPercentages().subscribe(data => {
            this.percentages = data;
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    search(){
        var parameters = {
            startDate: this.dateSince,
            endDate: this.dateTo,
            analyticId: $('#analyticId').val() == 0 ? null : $('#analyticId').val(),
            employeeId: $('#employeeId').val()== 0 ? null : $('#employeeId').val(),
            percentage: $('#percentageId').val()== 0 ? null : $('#percentageId').val(),
        }

        this.messageService.showLoading();

        this.createReportSubscrip = this.allocationService.createReport(parameters).subscribe(response => {
            this.messageService.closeLoading();
            if(response.messages) this.messageService.showMessages(response.messages);
            this.model = response.data;
            this.initGrid();
        },
        error => {
            this.errorHandlerService.handleErrors(error);
            this.messageService.closeLoading();
        });
    }

    initGrid(){
        var columns = [0, 1, 2, 3, 4, 5, 6, 7, 8];

        this.model.monthsHeader.forEach((element, index) => {
            columns.push(index + 9);
        });

        var title = `PMO Report ${moment(this.dateSince).format("YYYYMMDD")} - ${moment(this.dateTo).format("YYYYMMDD")}`;

        var options = { 
            selector: "#resourcesTable",
            columns: columns,
            title: title,
            withExport: true
        };

        this.dataTableService.destroy(options.selector);
        this.dataTableService.init2(options);
    }
}