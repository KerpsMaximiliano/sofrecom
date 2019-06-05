import { Component, OnInit, OnDestroy } from "@angular/core";
import { DataTableService } from "app/services/common/datatable.service";
import { DatesService } from "app/services/common/month.service";

@Component({
    selector: 'management-report-tracing',
    templateUrl: './tracing.component.html',
    styleUrls: ['./tracing.module.scss']
})
export class TracingComponent implements OnInit, OnDestroy {

    AllMarginTracking: any[] = new Array()
    analytic: string

    constructor(private dataTableService: DataTableService,
        private datesService: DatesService) {

    }

    ngOnInit(): void {

    }
    ngOnDestroy(): void {

    }

    open(marginTracking, analytic) {
       
        this.hideColumnsDataTable()
        this.analytic = analytic
        let column = 0
        let columns = []
        this.AllMarginTracking = marginTracking

        this.AllMarginTracking.forEach(margin => {
            var month = this.datesService.getMonth(margin.monthYear)
            margin.display = `${month.montShort} ${month.year}`

            columns.push(column)
            column++
        })

        columns.push(column)
        this.initGrid(columns)
    }

    initGrid(columns) {

        var title = `${this.analytic}`;

        var params = {
            selector: '#tracingTable',
            columns: columns,
            title: title,
            withExport: true,
            withOutSorting: true
        }

        this.dataTableService.destroy(params.selector);
        this.dataTableService.initialize(params);
    }

    hideColumnsDataTable() {

        setTimeout(function () {
            $('.dataTables_filter').hide()
            $('.dataTables_paginate.paging_simple_numbers').hide()
            $('.dataTables_info').hide()
            $('.dataTables_length').hide()

        }, 1000);
    }

}