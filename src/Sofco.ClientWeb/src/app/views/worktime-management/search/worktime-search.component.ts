import { Component, ViewChild, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { MessageService } from "../../../services/common/message.service";
import { DataTableService } from "../../../services/common/datatable.service";
import { WorktimeService } from "../../../services/worktime-management/worktime.service";
import { EmployeeService } from "../../../services/allocation-management/employee.service";
import { AnalyticService } from "../../../services/allocation-management/analytic.service";
import { MenuService } from "app/services/admin/menu.service";

declare var $: any;

@Component({
    selector: 'worktime-search',
    templateUrl: './worktime-search.component.html',
    styleUrls: ['./worktime-search.component.scss']
})
export class WorkTimeSearchComponent implements OnInit, OnDestroy {

    @ViewChild('accordion') accordion;

    public data: any[] = new Array<any>();

    public resources: any[] = new Array<any>();
    public analytics: any[] = new Array<any>();
    public managers: any[] = new Array<any>();
    public statuses: any[] = new Array<any>();

    public gridIsVisible: boolean = false;

    searchSubscrip: Subscription;
    getResourcesSubscrip: Subscription;
    getAnalyticSubscrip: Subscription;
    getCustomerSubscrip: Subscription;
    getManagersSubscrip: Subscription;
    getStatusSubscrip: Subscription;

    public searchModel = {
        startDate: null,
        endDate: null,
        status: null,
        managerId: null,
        analyticId: null,
        employeeId: null
    };

    constructor(private messageService: MessageService,
        private worktimeService: WorktimeService,
        private menuService: MenuService,
        private analyticService: AnalyticService,
        private employeeService: EmployeeService,
        private dataTableService: DataTableService){}

    ngOnInit(): void {
        this.getAnalytics();
        this.getResources();
        this.getManagers();
        this.getStatus();

        var data = JSON.parse(sessionStorage.getItem('lastWorktimeSearchQuery'));

        if(data){
            this.searchModel = data;
            this.search();
        }
    }

    ngOnDestroy(): void {
        if(this.searchSubscrip) this.searchSubscrip.unsubscribe();
        if(this.getResourcesSubscrip) this.getResourcesSubscrip.unsubscribe();
        if(this.getAnalyticSubscrip) this.getAnalyticSubscrip.unsubscribe();
        if(this.getCustomerSubscrip) this.getCustomerSubscrip.unsubscribe();
        if(this.getManagersSubscrip) this.getManagersSubscrip.unsubscribe();
        if(this.getStatusSubscrip) this.getStatusSubscrip.unsubscribe();
    }

    getStatus(){
        this.getStatusSubscrip = this.worktimeService.getStatus().subscribe(data => {
            this.statuses = data;
        });
    }

    getManagers(){
        this.getManagersSubscrip = this.employeeService.getManagers().subscribe(data => {
            this.managers = data;
        });
    }

    getResources(){
        if(this.canSeeManagers()){
            this.getResourcesSubscrip = this.employeeService.getOptions().subscribe(res => {
                this.resources = res;
            });
        }
        else{
            this.getResourcesSubscrip = this.employeeService.getEmployeesOptionByCurrentManager().subscribe(res => {
                this.resources = res.data;
            });
        }
    }

    getAnalytics(){
        this.getAnalyticSubscrip = this.analyticService.getByManager().subscribe(
            response => {
                this.analytics = response.data;
            });
    }

    search(){
        var model = {
            startDate: this.searchModel.startDate,
            endDate: this.searchModel.endDate,
            status: this.searchModel.status ? this.searchModel.status : 0,
            managerId: this.searchModel.managerId,
            analyticId: this.searchModel.analyticId,
            employeeId: this.searchModel.employeeId
        };

        this.messageService.showLoading();

        this.searchSubscrip = this.worktimeService.search(model).subscribe(response => {
            this.data = response.data;

            this.initGrid();
            this.messageService.closeLoading();
            this.collapse();

            sessionStorage.setItem('lastWorktimeSearchQuery', JSON.stringify(this.searchModel));
        },
        () => {
                this.messageService.closeLoading();
            });
    }

    initGrid(){
        var columns = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11];
        var title = "Consulta de horas";

        var options = { 
            selector: "#searchTable",
            columns: columns, 
            columnDefs: [ {"aTargets": [9], "sType": "date-uk"} ],
            title: title,
            withExport: true
        };

        this.dataTableService.destroy(options.selector); 
        this.dataTableService.initialize(options);
        this.gridIsVisible = true;

        setTimeout(() => {
            $("#searchTable_wrapper").css("float","left");
            $("#searchTable_wrapper").css("padding-bottom","50px");
            $("#searchTable_filter label").addClass('search-filter');
            $(".html5buttons").addClass('export-buttons');
            $("#searchTable_paginate").addClass('table-pagination');
            $("#searchTable_length").css("margin-right","10px");
            $("#searchTable_info").css("padding-top","4px");
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
        $('.datepicker').val('');
        
        this.searchModel = {
            startDate: null,
            endDate: null,
            status: null,
            managerId: null,
            analyticId: null,
            employeeId: null
        };

        sessionStorage.removeItem('lastWorktimeSearchQuery')
    }

    canSeeManagers(){
        return this.menuService.userIsDirector || this.menuService.userIsRrhh || this.menuService.userIsCdg;
    }
}