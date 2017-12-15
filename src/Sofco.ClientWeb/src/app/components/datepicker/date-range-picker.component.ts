import { Component, Input, OnInit } from '@angular/core';
import { Daterangepicker, DaterangepickerConfig } from 'ng2-daterangepicker';
import * as moment from 'moment';
import { ReportHelper } from 'app/views/report/common/report-helper';

@Component({
  selector: 'date-range-picker',
  templateUrl: './date-range-picker.component.html'
})
export class DateRangePickerComponent implements OnInit {

    public start = moment().subtract(3, 'month').startOf('month');
    public end = moment();

    @Input() datePickerOptionRange: string;

    constructor(private daterangepickerOptions: DaterangepickerConfig) {}

    ngOnInit(): void {
        var ranges;
        
        if(this.datePickerOptionRange == "next"){
            this.start = moment();
            this.end =  moment().add(3, 'month').startOf('month');
            ranges = this.getNextRanges();
        }
        else{
            ranges = this.getPreviousRanges();
        }

        this.daterangepickerOptions.settings = {
            locale: { format: 'DD-MM-YYYY' },
            alwaysShowCalendars: false,
            ranges: ranges
        };
    }

    public selectedDate(value: any) {
        this.start = value.start;
        this.end = value.end;
    }

    public getNextRanges(){
        return {
            'Resto del mes': [moment(), moment().endOf('month')],
            'Proximo mes': [moment().add(1, 'month').startOf('month'), moment().add(1, 'month').endOf('month')],
            'Proximos 3 meses': [moment().add(1, 'month').startOf('month'), moment().add(3, 'month').endOf('month')],
            'Proximos 6 meses': [moment().add(1, 'month').startOf('month'), moment().add(6, 'month').endOf('month')],
            'Proximos 12 meses': [moment().add(1, 'month').startOf('month'),moment().add(12, 'month').endOf('month') ],
         }
    }

    public getPreviousRanges(){
        return {
            'Ultimo mes': [moment().subtract(1, 'month').startOf('month'), moment()],
            'Ultimos 3 meses': [moment().subtract(3, 'month').startOf('month'), moment()],
            'Ultimos 6 meses': [moment().subtract(6, 'month').startOf('month'), moment()],
            'Ultimos 12 meses': [moment().subtract(12, 'month').startOf('month'), moment()],
         }
    }
}