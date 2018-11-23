import { Component, ViewChild, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { MessageService } from "../../../services/common/message.service";
import { DataTableService } from "../../../services/common/datatable.service";
import { EmployeeService } from "../../../services/allocation-management/employee.service";
import { I18nService } from "app/services/common/i18n.service";
declare var moment: any;
declare var $: any;

@Component({
    selector: 'app-end-notification',
    templateUrl: './end-notification.component.html',
    styleUrls: ['./end-notification.component.scss']
})
export class EndNotificationComponent implements OnInit, OnDestroy {

    @ViewChild('accordion') accordion;

    public data: any[] = new Array<any>();

    public resources: any[] = new Array<any>();
    public analytics: any[] = new Array<any>();
    public applicants: any[] = new Array<any>();
    public statuses: any[] = new Array<any>();

    public gridIsVisible = false;

    private storageKey = 'lastEndNotificationSearchQuery';

    searchSubscrip: Subscription;
    getResourcesSubscrip: Subscription;

    public searchModel = {
        startDate: null,
        endDate: null,
        applicantId: null,
        employeeId: null
    };

    constructor(private messageService: MessageService,
        private employeeService: EmployeeService,
        private dataTableService: DataTableService,
        private i18nService: I18nService){}

    ngOnInit(): void {
        this.getResources();

        let data = JSON.parse(sessionStorage.getItem(this.storageKey));

        if(data == null) {
            data = this.getDefaultData();
        }

        if(data){
            this.searchModel = data;
            this.search();
        }
    }

    ngOnDestroy(): void {
        if(this.searchSubscrip) this.searchSubscrip.unsubscribe();
        if(this.getResourcesSubscrip) this.getResourcesSubscrip.unsubscribe();
    }

    getResources(){
        this.getResourcesSubscrip = this.employeeService.getOptions().subscribe(res => {
            this.applicants = res;
            this.resources = res;
        });
    }

    search(){
        const model = {
            startDate: moment(this.searchModel.startDate).format("YYYY-MM-DD"),
            endDate: moment(this.searchModel.endDate).format("YYYY-MM-DD"),
            applicantEmployeeId: this.searchModel.applicantId,
            employeeId: this.searchModel.employeeId
        };

        this.messageService.showLoading();
        this.searchSubscrip = this.employeeService.getEmployeeEndNotification(model).subscribe(response => {
            this.messageService.closeLoading();
            this.data = response.data;
            this.initGrid();
            this.collapse();
            sessionStorage.setItem(this.storageKey, JSON.stringify(this.searchModel));
        },
        () => {
              this.messageService.closeLoading();
        });
    }

    initGrid(){
        const columns = [0, 1, 2, 3, 4, 5];
        const title = this.i18nService.translateByKey('rrhh.endNotification.title');
        const self = this;

        const options = {
            selector: "#searchTable",
            columns: columns,
            columnDefs: [ 
                { "targets": [ 0 ], "visible": false, "searchable": false },
                { "aTargets": [ 4,5 ], "sType": "date-uk" }
            ],
            title: title,
            withExport: true,
            customizeExcelExportData: function(data) {
                self.customizeExcelExportData(data);
            },
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

        this.searchModel = this.getDefaultData();

        sessionStorage.removeItem(this.storageKey);
    }

    customizeExcelExportData(data) {
        const idIndex = 0;
        const endDateIndex = 3;
        const createdDateIndex = 4;
        data.header.splice(0, 1);
        const dataBody = data.body;
        for(let index = 0; index < dataBody.length; index++) {
            const dataBodyItem = dataBody[index];
            const itemId = dataBodyItem[idIndex];
            dataBodyItem.splice(0, 1);
            const item = this.data.find(x => x.id == itemId);
            if(item === undefined) continue;
            dataBodyItem[createdDateIndex] = this.getCreatedDate(item);
            dataBodyItem[endDateIndex] = this.getEndDate(item);
        };
    }

    getCreatedDate(item):any {
        return moment(item.createdDate).format("YYYY-MM-DD");
    }

    getEndDate(item):any {
        return moment(item.endDate).format("YYYY-MM-DD");
    }

    getDefaultData():any {
        return {
            startDate: moment().startOf('month').toDate(),
            endDate: moment().endOf('month').toDate(),
            applicantId: null,
            employeeId: null
        };
    }
}
