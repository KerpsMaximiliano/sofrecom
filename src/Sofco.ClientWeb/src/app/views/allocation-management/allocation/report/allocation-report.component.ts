import { Component, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { DataTableService } from "../../../../services/common/datatable.service";
import { MenuService } from "../../../../services/admin/menu.service";
import { MessageService } from "../../../../services/common/message.service";
import { AllocationService } from "../../../../services/allocation-management/allocation.service";
import { EmployeeService } from "../../../../services/allocation-management/employee.service";
import { AnalyticService } from "../../../../services/allocation-management/analytic.service";

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

    createReportSubscrip: Subscription;
    getAllocationResourcesSubscrip: Subscription;
    getAnalyticsSubscrip: Subscription;

    public dateSince: Date = new Date();
    public dateTo: Date = new Date();
    public includeStaff: boolean = false;
    public unassigned: boolean = false;
    public generateReportPowerBi: boolean = false;
    public includeAnalyticId = "1";

    private lastQuery: any;
    public loaded: boolean = false;

    constructor(private allocationService: AllocationService,
                private employeeService: EmployeeService,
                private analyticService: AnalyticService,
                public menuService: MenuService,
                private messageService: MessageService,
                private dataTableService: DataTableService){}
 
    ngOnInit(): void {
        this.getAllocationResources();
        this.getAnalytics();

        var data = JSON.parse(sessionStorage.getItem('lastReportQuery'));


        if(data){
            this.lastQuery = data;
            this.dateSince = moment(data.startDate).toDate();
            this.dateTo = moment(data.endDate).toDate();
            this.includeStaff = data.includeStaff;
            this.unassigned = data.unassigned;
            this.includeAnalyticId = data.includeAnalyticId;
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
        });
    }

    getAnalytics(){
        this.getAnalyticsSubscrip = this.analyticService.getOptions().subscribe(data => {
            this.analytics = data;

            if(this.lastQuery){
                setTimeout(() => {
                    $('#analyticId').val(this.lastQuery.analyticIds).trigger('change');
                }, 1000);
            }
        });
    }

    search(){
        var parameters = {
            startDate: this.dateSince,
            endDate: this.dateTo,
            includeStaff: this.includeStaff,
            generateReportPowerBi: this.generateReportPowerBi,
            unassigned: this.unassigned,
            includeAnalyticId: this.includeAnalyticId,
            analyticIds: $('#analyticId').val(),
            employeeId: $('#employeeId').val() == 0 ? null : $('#employeeId').val()
        }

        this.messageService.showLoading();
        this.messageService.removeMessages();
        this.loaded = false;

        this.createReportSubscrip = this.allocationService.createReport(parameters).subscribe(response => {
            this.model = response.data;
            this.initGrid();

            sessionStorage.setItem('lastReportQuery', JSON.stringify(parameters));
        },
        error => this.messageService.closeLoading());
    }

    initGrid(){
        var title = `PMO Report ${moment(this.dateSince).format("YYYYMMDD")} - ${moment(this.dateTo).format("YYYYMMDD")}`;

        var options = { 
            selector: "#resourcesTable",
            title: title,
            order: [[ 2, "asc" ]],
            withExport: true,
            customNumberFormat: this.customFormat
        };

        setTimeout(() => {
            this.messageService.closeLoading();

            if(this.model.rows.length > 0){
                this.loaded = true;
            }

            this.dataTableService.destroy(options.selector);
            this.dataTableService.initialize(options);
        }, 500);
       
        setTimeout(() => {
            $("#resourcesTable_wrapper").css("float","left");
            $("#resourcesTable_wrapper").css("padding-bottom","50px");
            $("#resourcesTable_filter label").addClass('search-filter');
            $(".html5buttons").addClass('export-buttons');
            $("#resourcesTable_paginate").addClass('table-pagination');
            $("#resourcesTable_length").css("margin-right","10px");
            $("#resourcesTable_info").css("padding-top","4px");
        }, 1000);
    };

    customFormat(data, row, column, node) {
        data = $('<p>' + data + '</p>').text();
        return $.isNumeric(data.replace('.', '').replace(',', '.')) ? data.replace(',', '.') : data;
    }

    clean(){
        this.dateSince = new Date();
        this.dateTo = new Date();
        $('#analyticId').val(null).trigger('change');
        $('#employeeId').val(0).trigger('change');
        this.unassigned = false;
        sessionStorage.removeItem('lastReportQuery')
    }

    unassignedChanged(){
        if(this.unassigned){
            this.generateReportPowerBi = false;
            $('#analyticId').val(null).trigger('change');
            $('#employeeId').val(0).trigger('change');
            this.includeAnalyticId = "1";
        }
    }
}