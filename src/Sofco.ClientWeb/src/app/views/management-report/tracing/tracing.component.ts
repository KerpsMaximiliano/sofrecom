import { Component, OnInit, OnDestroy } from "@angular/core";
import { DataTableService } from "app/services/common/datatable.service";

@Component({
    selector: 'management-report-tracing',
    templateUrl: './tracing.component.html',
    styleUrls:['./tracing.module.scss']
})
export class TracingComponent implements OnInit, OnDestroy {
    
    AllMarginTracking: any[] = new Array()
    analytic: string
    
    constructor(private dataTableService : DataTableService){

    }
    
    ngOnInit(): void {
       
    }
    ngOnDestroy(): void {
        
    }

    open(marginTracking, analytic){

        this.hideColumnsDataTable()
        this.analytic = analytic
        let colum = 0
        let colums = []
        this.AllMarginTracking = marginTracking
        
        this.AllMarginTracking.forEach(margin => {
            margin.display = this.getDateShortDescrip(margin.Month, margin.Year)
            colums.push(colum)
            colum++
        })

        this.initGrid(colums)

     
    }

    getDateShortDescrip(month, year){
 
        switch (month)
        {
            case 1: return `Ene. ${year}`;
            case 2: return `Feb. ${year}`;
            case 3: return `Mar. ${year}`;
            case 4: return `Abr. ${year}`;;
            case 5: return `May. ${year}`;
            case 6: return `Jun.${year}`;
            case 7: return `Jul. ${year}`;
            case 8: return `Ago. ${year}`;
            case 9: return `Sep. ${year}`;
            case 10: return `Oct. ${year}`;
            case 11: return `Nov. ${year}`;
            case 12: return `Dic. ${year}`;
            default: return '';
        }
    }

    initGrid(colums) {
        var columns = colums
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

    hideColumnsDataTable(){
        setTimeout(function(){
            $('.dataTables_filter').hide()
            $('.dataTables_paginate.paging_simple_numbers').hide()
            $('.dataTables_info').hide()
            $('.dataTables_length').hide()
        }, 100);
    }
}