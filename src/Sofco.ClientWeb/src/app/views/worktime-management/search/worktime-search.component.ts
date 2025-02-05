import { Component, ViewChild, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { MessageService } from "../../../services/common/message.service";
import { DataTableService } from "../../../services/common/datatable.service";
import { WorktimeService } from "../../../services/worktime-management/worktime.service";
import { EmployeeService } from "../../../services/allocation-management/employee.service";
import { AnalyticService } from "../../../services/allocation-management/analytic.service";
import { MenuService } from "app/services/admin/menu.service";
import { WorkTimeStatus } from "app/models/enums/worktimestatus";
import { FormControl, Validators } from "@angular/forms";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";

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
    totalHours: number;
    totalLicenseHours: number;

    searchSubscrip: Subscription;
    getResourcesSubscrip: Subscription;
    getAnalyticSubscrip: Subscription;
    getCustomerSubscrip: Subscription;
    getManagersSubscrip: Subscription;
    getStatusSubscrip: Subscription;
    deleteSubscription: Subscription;

    public searchModel = {
        startDate: null,
        endDate: null,
        status: null,
        managerId: null,
        analyticId: null,
        employeeId: null
    };

    adminModel = {
        analyticId: new FormControl(null, [Validators.required]),
        statusId: new FormControl(null, [Validators.required]),
        id: null
    }

    @ViewChild('adminUpdateModal') adminUpdateModal;
    public adminUpdateModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Actualizar hora",
        "adminUpdateModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

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

        if(this.canSeeManagers()){
            this.getResourcesSubscrip = this.employeeService.getOptionsFull().subscribe(res => {
                this.resources = res;
            });
        }
    }

    ngOnDestroy(): void {
        if(this.searchSubscrip) this.searchSubscrip.unsubscribe();
        if(this.getResourcesSubscrip) this.getResourcesSubscrip.unsubscribe();
        if(this.getAnalyticSubscrip) this.getAnalyticSubscrip.unsubscribe();
        if(this.getCustomerSubscrip) this.getCustomerSubscrip.unsubscribe();
        if(this.getManagersSubscrip) this.getManagersSubscrip.unsubscribe();
        if(this.getStatusSubscrip) this.getStatusSubscrip.unsubscribe();
        if(this.deleteSubscription) this.deleteSubscription.unsubscribe();
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
        if(!this.canSeeManagers()){
            this.getResourcesSubscrip = this.analyticService.getResources(this.searchModel.analyticId).subscribe(data => {
                this.resources = data;
            });
        }
    }

    getAnalytics(){
        this.getAnalyticSubscrip = this.worktimeService.getAnalytics().subscribe(data => {
            this.analytics = data;
        });
    }

    search(){
        if(this.searchModel.startDate && this.searchModel.endDate){
            if(this.searchModel.endDate < this.searchModel.startDate){
                this.messageService.showError('dateToLessThanSince');
                return;
            }
        }

        if(this.searchModel.analyticId == null) this.searchModel.analyticId = [];
        if(this.searchModel.managerId == null) this.searchModel.managerId = [];

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

            this.calculateTotalHours();
        },
        () => {
                this.messageService.closeLoading();
            });
    }

    calculateTotalHours(){
        this.totalHours = 0;
        this.totalLicenseHours = 0;

        this.data.forEach(x => {
            if(x.status == "License"){
                this.totalLicenseHours += x.hours;
            }
            else{
                this.totalHours += x.hours;
            }
        });
    }

    initGrid(){
        var columns = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
        var title = "Consulta de horas";

        var options = { 
            selector: "#searchTable",
            columns: columns, 
            columnDefs: [ {"aTargets": [10], "sType": "date-uk"} ],
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

    canDelete(item){
        return item.canDelete == true;
    }

    delete(item){
        this.messageService.showConfirm(() => {
            this.messageService.showLoading();

            this.deleteSubscription = this.worktimeService.delete(item.id).subscribe(response => {
                this.messageService.closeLoading();

                var index = this.data.findIndex(x => x.id == item.id);

                if(index > -1){
                    this.data.splice(index, 1);
                    this.initGrid();
                }
            },
            error => this.messageService.closeLoading());
        });
    }

    canAdminUpdate(){
        return this.menuService.hasAdminMenu();
    }

    adminUpdateWorkTime(){
        var json = {
            analyticId: this.adminModel.analyticId.value,
            statusId: this.adminModel.statusId.value,
        }

        this.deleteSubscription = this.worktimeService.adminUpdate(this.adminModel.id, json).subscribe(response => {
            this.adminUpdateModal.resetButtons();

            var worktime = this.data.find(x => x.id == this.adminModel.id);

            if(worktime){
                var statusOption = this.statuses.find(x => x.id == this.adminModel.statusId.value);

                if(statusOption){
                    worktime.status = statusOption.text;
                    worktime.statusId = this.adminModel.statusId.value;
                }

                var analyticOption = this.analytics.find(x => x.id == this.adminModel.analyticId.value);

                if(analyticOption){
                    worktime.analyticId = this.adminModel.analyticId.value;

                    var text = analyticOption.text.split(' - ');

                    if(text && text.length > 1){
                        worktime.analytic = text[text.length-1];
                        worktime.analyticTitle = text[text.length-2];
                    }
                }
            }
        },
        error => this.adminUpdateModal.resetButtons());
    }

    setWorkTime(item){
        this.adminModel.analyticId.setValue(item.analyticId);
        this.adminModel.statusId.setValue(item.statusId);
        this.adminModel.id = item.id;

        this.adminUpdateModal.show();
    }
}