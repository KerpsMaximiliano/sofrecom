import { Router} from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { Cookie } from 'ng2-cookies/ng2-cookies';

import { DateRangePickerComponent } from 'app/components/datepicker/date-range-picker.component'

import { I18nService } from 'app/services/common/i18n.service';
import { ReportHelper } from "app/views/report/common/report-helper"
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { DataTableService } from "app/services/common/datatable.service";
import { MenuService } from 'app/services/admin/menu.service';
import { SolfacReportService } from 'app/services/report/solfacReport.service';
import { SolfacChartComponent } from './solfac-chart.component';

declare var jQuery:any;

@Component({
    selector: 'app-solfac-report',
    templateUrl: './solfac.component.html'
})
 
export class SolfacReportComponent implements OnInit, OnDestroy {
    @ViewChild('chart') chart:SolfacChartComponent;
    @ViewChild('dateRangePicker') dateRangePicker:DateRangePickerComponent;
    public loading: boolean = true;
    public dateOptions;
    getDataSubscription: Subscription;
    data;
    dateSince: Date = new Date();
    dateTo: Date = new Date();
    
    constructor (
        private router: Router,
        private menuService: MenuService,
        private i18nService: I18nService,
        private service: SolfacReportService,
        private datatableService: DataTableService,
        private errorHandlerService: ErrorHandlerService) 
    {
        this.dateOptions = this.menuService.getDatePickerOptions();
    }

    ngOnInit(){
        jQuery('#dataSource').slideUp();

        this.init();
    }

    init(){
        this.getData();
    }

    ngOnDestroy(){
        if(this.getDataSubscription) this.getDataSubscription.unsubscribe();
    }

    apply(){
        this.getData();
    }

    getData(){
        this.dateSince = this.dateRangePicker.start.toDate();
        this.dateTo = this.dateRangePicker.end.toDate();
        var parameters = {
            dateSince: this.dateSince,
            dateTo: this.dateTo
        }

        this.getDataSubscription = this.service.getData(parameters).subscribe(response => {
            this.data = response.data;
            this.loading = false;
            this.setGraphic();
        },
          err => {
            this.loading = false;
            this.errorHandlerService.handleErrors(err);
        });
    }

    setGraphic(){
        let labels = ReportHelper.DateMonthIntervalToLabels(this.dateSince, this.dateTo);

        let projectKeys = [];
        for(let i=0; i<this.data.length; i++){
            let key = this.data[i].projectName;
            if(projectKeys[key] == null)
            {
                projectKeys[key] = [];
            }
        }

        let reportData = [];
        for (let projectKey in projectKeys) {
            let projectData = [];
            let datesData = [];
            for(let idx=0; idx<labels.length; idx++){
                let yValue = null;
                for(let i=0; i<this.data.length; i++){
                    let itemKey = this.data[i].projectName;
                    var dateKey = ReportHelper.DateMonthToLabel(this.data[i].invoiceDate);
                    
                    if(dateKey == labels[idx]){
                        if(itemKey == projectKey){
                            yValue = this.data[i].amount;
                        }
                    }
                }
                projectData.push(yValue);
            }

            reportData.push({
                data: projectData,
                label: projectKey
            });
        }

        this.chart.setData(labels, reportData);
    }
}
