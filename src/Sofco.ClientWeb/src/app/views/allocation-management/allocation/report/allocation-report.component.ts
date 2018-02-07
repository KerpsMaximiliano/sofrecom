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
    public includeStaff: boolean = false;
    public dateOptions;

    private lastQuery: any;
    public loaded: boolean = false;

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

        var data = JSON.parse(sessionStorage.getItem('lastReportQuery'));

        if(data){
            this.lastQuery = data;
            this.dateSince = data.startDate
            this.dateTo = data.endDate
            this.includeStaff = data.includeStaff;
        }
    }

    ngOnDestroy(): void {
        if(this.createReportSubscrip) this.createReportSubscrip.unsubscribe();
        if(this.getAllocationResourcesSubscrip) this.getAllocationResourcesSubscrip.unsubscribe();
        if(this.getAnalyticsSubscrip) this.getAnalyticsSubscrip.unsubscribe();
    }

    getAllocationResources(){
        this.getAllocationResourcesSubscrip = this.employeeService.getOptions().subscribe(data => {
            this.resources = data;

            if(this.lastQuery){
                setTimeout(() => {
                    $('#employeeId').val(this.lastQuery.employeeId == null ? 0 : this.lastQuery.employeeId).trigger('change');
                }, 0);
            }
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    getAnalytics(){
        this.getAnalyticsSubscrip = this.analyticService.getOptions().subscribe(data => {
            this.analytics = data;

            if(this.lastQuery){
                setTimeout(() => {
                    $('#analyticId').val(this.lastQuery.analyticId == null ? 0 : this.lastQuery.analyticId).trigger('change');
                }, 0);
            }
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    getPercentages(){
        this.getAnalyticsSubscrip = this.allocationService.getAllPercentages().subscribe(data => {
            this.percentages = data;

            if(this.lastQuery){
                setTimeout(() => {
                    $('#percentageId').val(this.lastQuery.percentage).trigger('change');
                }, 0);
            }
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    search(){
        var parameters = {
            startDate: this.dateSince,
            endDate: this.dateTo,
            includeStaff: this.includeStaff,
            analyticId: $('#analyticId').val() == 0 ? null : $('#analyticId').val(),
            employeeId: $('#employeeId').val() == 0 ? null : $('#employeeId').val(),
            percentage: $('#percentageId').val() == '' || undefined ? null : $('#percentageId').val(),
        }

        this.messageService.showLoading();
        this.loaded = false;

        this.createReportSubscrip = this.allocationService.createReport(parameters).subscribe(response => {
          
            if(response.messages) this.messageService.showMessages(response.messages);

            this.model = response.data;
            this.initGrid();

            sessionStorage.setItem('lastReportQuery', JSON.stringify(parameters));
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

        setTimeout(() => {
            this.messageService.closeLoading();
            this.loaded = true;

            this.dataTableService.destroy(options.selector);
            this.dataTableService.init2(options);
        }, 500);
       
        setTimeout(() => {
            $("#resourcesTable_wrapper").css("float","left");
            $("#resourcesTable_wrapper").css("padding-bottom","50px");
            $("#resourcesTable_filter label").addClass('search-filter');
            $(".html5buttons").addClass('export-buttons');
            $("#resourcesTable_paginate").addClass('table-pagination');
        }, 1000);
    }

    clean(){
        this.dateSince = new Date();
        this.dateTo = new Date();
        $('#analyticId').val(0).trigger('change');
        $('#employeeId').val(0).trigger('change');
        $('#percentageId').val('').trigger('change');
        sessionStorage.removeItem('lastReportQuery')
    }
}