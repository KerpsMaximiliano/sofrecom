import { Component, ViewChild, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { MessageService } from "../../../../services/common/message.service";
import { DataTableService } from "../../../../services/common/datatable.service";
import { RrhhService } from "app/services/human-resources/rrhh.service";

declare var $: any;

@Component({
    selector: 'salary-report',
    templateUrl: './salary-report.html',
    styleUrls: ['./salary-report.scss']
})
export class SalaryReportComponent implements OnDestroy {

    @ViewChild('accordion') accordion;

    public resources: any[] = new Array<any>();
    public months: any[] = new Array<any>();

    searchSubscrip: Subscription;
    
    public searchModel = {
        startDate: null,
        endDate: null
    };

date: Date;

    constructor(
        private messageService: MessageService,
        private rrhhService: RrhhService,
        private dataTableService: DataTableService){}

    ngOnDestroy(): void {
        if(this.searchSubscrip) this.searchSubscrip.unsubscribe();
    }
    
    search(){
        this.messageService.showLoading();

        this.searchSubscrip = this.rrhhService.getSalaryReport(this.searchModel.startDate.toUTCString(), this.searchModel.endDate.toUTCString()).subscribe(response => {
            this.resources = response.data.items;
            this.months = response.data.months;

            this.initGrid();
            this.messageService.closeLoading();
            this.collapse();
        },
        () => {
            this.messageService.closeLoading();
        });
    }

    initGrid(){
        var title = `Reporte Salarios`;
        var columns = [0, 1, 2, 3, 4, 5, 6, 7, 8];
        var currencyColumns = [];

        var index = 8;

        this.months.forEach(x => {
            index++;
            columns.push(index);
            currencyColumns.push(index);
        });

        var options = { 
            selector: "#resourcesTable", 
            columns: columns,
            title: title,
            withExport: true,
            currencyColumns: currencyColumns,
            columnDefs: [ {'aTargets': [7], "sType": "date-uk"} ] 
        };

        this.dataTableService.destroy(options.selector); 
        this.dataTableService.initialize(options);

        setTimeout(() => {
            $("#resourcesTable_wrapper").css("float","left");
            $("#resourcesTable_wrapper").css("padding-bottom","50px");
            $("#resourcesTable_filter label").addClass('search-filter');
            $(".html5buttons").addClass('export-buttons');
            $("#resourcesTable_paginate").addClass('table-pagination');
            $("#resourcesTable_length").css("margin-right","10px");
            $("#resourcesTable_info").css("padding-top","4px");
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
}