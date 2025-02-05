import { Component, ViewChild, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { DataTableService } from "app/services/common/datatable.service";
import { WorktimeService } from "../../../services/worktime-management/worktime.service";
import { UtilsService } from "../../../services/common/utils.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { RrhhService } from "app/services/human-resources/rrhh.service";
import * as FileSaver from "file-saver";
import { WorktimeControlService } from "app/services/worktime-management/worktime-control.service";
import { MenuService } from "app/services/admin/menu.service";

declare var $: any;

@Component({
    selector: 'worktime-report',
    templateUrl: './worktime-report.component.html',
    styleUrls: ['./worktime-report.component.scss']
})
export class WorkTimeReportComponent implements OnInit, OnDestroy {

    @ViewChild('accordion') accordion;

    public data: any[] = new Array<any>();
    public employeesWithHoursMissing: any[] = new Array<any>();
    public employeesWithAllocationMissing: any[] = new Array<any>();

    public resources: any[] = new Array<any>();
    public analytics: any[] = new Array<any>();
    public months: any[] = new Array<any>();
    public years: any[] = new Array<any>();
    public managers: any[] = new Array<any>();

    public gridIsVisible: boolean = false;
    public isCompleted: boolean = false;
    public isMissingData: boolean = false;
    public exportTigerVisible: boolean = false;
    public canExportSingleTigerFile: boolean = false;
    public workTimeReportByHours: boolean = true;

    searchSubscrip: Subscription;
    getResourcesSubscrip: Subscription;
    getAnalyticSubscrip: Subscription;
    getMonthsSubscrip: Subscription;
    getYearsSubscrip: Subscription;
    getManagersSubscrip: Subscription;

    public searchModel = {
        closeMonthId: 0,
        managerId: null,
        analyticId: null,
        employeeId: 0,
        exportTigerVisible: false
    };

    constructor(private messageService: MessageService,
        private worktimeService: WorktimeService,
        private utilsService: UtilsService,
        private menuService: MenuService,
        private worktimeControlService: WorktimeControlService,
        private rrhhService: RrhhService,
        private employeeService: EmployeeService,
        private dataTableService: DataTableService){}

    ngOnInit(): void {
        this.getMonths();
        this.getResources();
        this.getManagers();
        this.getAnalytics();

        const data = JSON.parse(sessionStorage.getItem('lastWorktimeReportQuery'));

        if(data){
            this.searchModel = data;
            this.search();
        }
    }

    ngOnDestroy(): void {
        if(this.searchSubscrip) this.searchSubscrip.unsubscribe();
        if(this.getResourcesSubscrip) this.getResourcesSubscrip.unsubscribe();
        if(this.getAnalyticSubscrip) this.getAnalyticSubscrip.unsubscribe();
        if(this.getMonthsSubscrip) this.getMonthsSubscrip.unsubscribe();
        if(this.getYearsSubscrip) this.getYearsSubscrip.unsubscribe();
        if(this.getManagersSubscrip) this.getManagersSubscrip.unsubscribe();
    }

    getManagers(){
        this.getManagersSubscrip = this.employeeService.getManagers().subscribe(data => {
            this.managers = data;
        });
    }

    getMonths(){
        this.getMonthsSubscrip = this.utilsService.getCloseMonths().subscribe(data => this.months = data);
    }

    getResources(){
        this.getResourcesSubscrip = this.employeeService.getAllForWorkTimeReport().subscribe(data => {
            this.resources = data;
        });
    }

    getAnalytics(){
        this.getAnalyticSubscrip = this.worktimeControlService.getAnalytics().subscribe(
            res => {
                this.analytics = res;
            });
    }

    search(){
        this.messageService.showLoading();
        this.gridIsVisible = false;
        this.isMissingData = false;
        this.isCompleted = false;

        if(this.searchModel.analyticId == null) this.searchModel.analyticId = [];
        if(this.searchModel.managerId == null) this.searchModel.managerId = [];

        this.searchModel.exportTigerVisible = this.menuService.userIsRrhh || this.menuService.userIsCdg;
        this.exportTigerVisible = this.menuService.userIsRrhh || this.menuService.userIsCdg;

        this.searchSubscrip = this.worktimeService.createReport(this.searchModel).subscribe(response => {
            this.data = response.data.items;
            this.workTimeReportByHours = response.data.workTimeReportByHours;
            this.employeesWithHoursMissing = response.data.employeesMissingHours;
            this.canExportSingleTigerFile = response.data.canExportSingleTigerFile;

            this.isCompleted = response.data.isCompleted;
            this.isMissingData = !response.data.isCompleted;

            if(this.isMissingData){
                this.employeesWithAllocationMissing = response.data.employeesAllocationResume.filter(item => item.missAnyPercentageAllocation == true);
            }

            if(this.data.length > 0){
                this.initGrid();
                this.collapse();
            }

            this.messageService.closeLoading();

            sessionStorage.setItem('lastWorktimeReportQuery', JSON.stringify(this.searchModel));
        },
        () => {
            this.messageService.closeLoading();
        });
    }
 
    initGrid(){
        var columns = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        var title = "Reporte Insumido vs Previsto";

        var options = { 
            selector: "#reportTable",
            columns: columns,
            title: title,
            withExport: true
        };

        this.dataTableService.destroy(options.selector); 
        this.dataTableService.initialize(options);
        this.gridIsVisible = true;

        var columnsMissingHours = [0, 1, 2, 3, 4, 5];
        var titleMissingHours = "Recursos con horas faltantes";

        var options2 = { 
            selector: "#missingHoursTable",
            columns: columnsMissingHours,
            title: titleMissingHours,
            withExport: true
        };

        this.dataTableService.destroy(options2.selector); 
        this.dataTableService.initialize(options2);

        var columnsMissingAllocation = [0, 1, 2, 3, 4];
        var titleMissingAllocation = "Recursos con asignación faltante";

        var options3 = { 
            selector: "#missingAllocationTable",
            columns: columnsMissingAllocation,
            title: titleMissingAllocation,
            withExport: true
        };

        this.dataTableService.destroy(options3.selector); 
        this.dataTableService.initialize(options3);

        setTimeout(() => {
            $("#reportTable_wrapper").css("float","left");
            $("#reportTable_wrapper").css("padding-bottom","50px");
            $("#reportTable_filter label").addClass('search-filter');
            $("#reportTable_wrapper .html5buttons").addClass('export-buttons');
            $("#reportTable_paginate").addClass('table-pagination');
            $("#reportTable_length").css("margin-right","10px");
            $("#reportTable_info").css("padding-top","4px");
        }, 500);
    }

    collapse(){
        if($("#collapseOne").hasClass('in')){
            $("#collapseOne").removeClass('in');
        }
        else{
            $("#collapseOne").addClass('in');
        }

        this.changeIcon();
    }

    changeIcon(){
        if($("#collapseOne").hasClass('in')){
            $("#search-icon").toggleClass('fa-caret-down').toggleClass('fa-caret-up');
        }
        else{
            $("#search-icon").toggleClass('fa-caret-up').toggleClass('fa-caret-down');
        } 
    }

    changeIcon2(){
        if($("#collapseTwo").hasClass('in')){
            $("#search-icon2").toggleClass('fa-caret-down').toggleClass('fa-caret-up');
        }
        else{
            $("#search-icon2").toggleClass('fa-caret-up').toggleClass('fa-caret-down');
        }
    }

    clean(){
        this.searchModel = {
            closeMonthId: 0,
            managerId: null,
            analyticId: null,
            employeeId: 0,
            exportTigerVisible: false
        };

        sessionStorage.removeItem('lastWorktimeReportQuery')
    }

    getTigetTxt(allUsers){
        this.messageService.showLoading();

        this.rrhhService.getTigerTxt(allUsers).subscribe(file => {
            this.messageService.closeLoading();

            FileSaver.saveAs(file, "tiger-txt-file.txt");
        },
        () => {
            this.messageService.closeLoading();
        });
    }
}