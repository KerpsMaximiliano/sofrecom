import { Component, ViewChild, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { DataTableService } from "app/services/common/datatable.service";

declare var $: any;

@Component({
    selector: 'report-updown',
    templateUrl: './report-up-down.html',
})
export class ReportUpdownComponent implements OnDestroy {

    @ViewChild('accordion') accordion;

    public resources: any[] = new Array<any>();

    searchSubscrip: Subscription;

    public searchModel = {
        startDate: null,
        endDate: null
    };

    constructor(
        private messageService: MessageService,
        private employeeService: EmployeeService,
        private dataTableService: DataTableService){}

    ngOnDestroy(): void {
        if(this.searchSubscrip) this.searchSubscrip.unsubscribe();
    }

    search(){
        this.messageService.showLoading();

        this.searchSubscrip = this.employeeService.searchReportUpDown(this.searchModel).subscribe(response => {
            this.resources = response.data;

            this.initGrid();
            this.messageService.closeLoading();
            this.collapse();
        },
        error => {
            this.messageService.closeLoading();
        });
    }

    initGrid(){
        var columns = [0, 1, 2, 3, 4, 5, 6];
        var title = `Reporte altas y bajas`;

        var options = { 
            selector: "#resourcesTable", 
            columns: columns,
            title: title,
            withExport: true,
            columnDefs: [ {'aTargets': [3, 4], "sType": "date-uk"} ] 
        };

        this.dataTableService.destroy(options.selector); 
        this.dataTableService.initialize(options);
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