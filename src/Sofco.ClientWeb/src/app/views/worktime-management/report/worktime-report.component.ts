import { Component, ViewChild, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { DataTableService } from "app/services/common/datatable.service";
import { WorktimeService } from "../../../services/worktime-management/worktime.service";
import { UtilsService } from "../../../services/common/utils.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { AnalyticService } from "../../../services/allocation-management/analytic.service";
import { CustomerService } from "../../../services/billing/customer.service";
import { RrhhService } from "app/services/human-resources/rrhh.service";
import * as FileSaver from "file-saver";

declare var $: any;

@Component({
    selector: 'worktime-report',
    templateUrl: './worktime-report.component.html',
    styleUrls: ['./worktime-report.component.scss']
})
export class WorkTimeReportComponent implements OnInit, OnDestroy {

    @ViewChild('accordion') accordion;

    public data: any[] = new Array<any>();

    public resources: any[] = new Array<any>();
    public analytics: any[] = new Array<any>();
    public customers: any[] = new Array<any>();
    public months: any[] = new Array<any>();
    public years: any[] = new Array<any>();
    public managers: any[] = new Array<any>();

    public gridIsVisible: boolean = false;
    public isCompleted: boolean = false;

    searchSubscrip: Subscription;
    getResourcesSubscrip: Subscription;
    getAnalyticSubscrip: Subscription;
    getCustomerSubscrip: Subscription;
    getMonthsSubscrip: Subscription;
    getYearsSubscrip: Subscription;
    getManagersSubscrip: Subscription;

    public searchModel = {
        closeMonthId: 0,
        clientId: 0,
        managerId: 0,
        analyticId: 0,
        employeeId: 0
    };

    constructor(private messageService: MessageService,
        private worktimeService: WorktimeService,
        private utilsService: UtilsService,
        private analyticService: AnalyticService,
        private rrhhService: RrhhService,
        private customerService: CustomerService,
        private employeeService: EmployeeService,
        private dataTableService: DataTableService){}

    ngOnInit(): void {
        this.getCustomers();
        this.getMonths();
        this.getResources();
        this.getManagers();

        var data = JSON.parse(sessionStorage.getItem('lastWorktimeReportQuery'));

        if(data){
            this.searchModel = data;
            this.search();

            if(data.clientId && data.clientId != ''){
                this.getAnalytics();
            }
        }
    }

    ngOnDestroy(): void {
        if(this.searchSubscrip) this.searchSubscrip.unsubscribe();
        if(this.getResourcesSubscrip) this.getResourcesSubscrip.unsubscribe();
        if(this.getAnalyticSubscrip) this.getAnalyticSubscrip.unsubscribe();
        if(this.getCustomerSubscrip) this.getCustomerSubscrip.unsubscribe();
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
        this.getResourcesSubscrip = this.employeeService.getOptions().subscribe(data => {
            this.resources = data;
        });
    }

    getAnalytics(){
        this.getAnalyticSubscrip = this.analyticService.getClientId(this.searchModel.clientId).subscribe(
            data => {
                this.analytics = data;
            });
    }

    getCustomers(){
        this.getCustomerSubscrip = this.customerService.getOptions().subscribe(res => {
            this.customers = res.data;
        });
    }

    search(){
        this.messageService.showLoading();
        this.gridIsVisible = false;

        this.searchSubscrip = this.worktimeService.createReport(this.searchModel).subscribe(response => {
            this.data = response.data.items;
            this.isCompleted = response.data.isCompleted;

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
        var columns = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
        var title = "Reporte Estimado vs Insumido";

        var options = { 
            selector: "#reportTable",
            columns: columns,
            title: title,
            order: [[ 1, "desc" ]],
            withExport: true
        };

        this.dataTableService.destroy(options.selector); 
        this.dataTableService.initialize(options);
        this.gridIsVisible = true;

        setTimeout(() => {
            $("#reportTable_wrapper").css("float","left");
            $("#reportTable_wrapper").css("padding-bottom","50px");
            $("#reportTable_filter label").addClass('search-filter');
            $(".html5buttons").addClass('export-buttons');
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
    
    clean(){
        this.searchModel = {
            closeMonthId: 0,
            clientId: 0,
            managerId: 0,
            analyticId: 0,
            employeeId: 0
        };

        sessionStorage.removeItem('lastWorktimeReportQuery')
    }

    getTigetTxt(){
        this.messageService.showLoading();

        this.rrhhService.getTigerTxt().subscribe(file => {
            this.messageService.closeLoading();

            FileSaver.saveAs(file, "tiger-txt-file.txt");
        },
        () => {
            this.messageService.closeLoading();
        });
    }
}