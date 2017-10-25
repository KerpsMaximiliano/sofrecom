import { Component } from '@angular/core';
import { Daterangepicker, DaterangepickerConfig } from 'ng2-daterangepicker';
import * as moment from 'moment';

@Component({
  selector: 'date-range-picker',
  templateUrl: './date-range-picker.component.html'
})
export class DateRangePickerComponent {

    public start = moment().subtract(4, 'month').startOf('month');
    public end = moment();

    constructor(private daterangepickerOptions: DaterangepickerConfig) {
        this.daterangepickerOptions.settings = {
            locale: { format: 'DD-MM-YYYY' },
            alwaysShowCalendars: false,
            ranges: {
               'Ultimo mes': [moment().subtract(1, 'month').startOf('month'), moment()],
               'Ultimos 3 meses': [moment().subtract(4, 'month').startOf('month'), moment()],
               'Ultimos 6 meses': [moment().subtract(6, 'month').startOf('month'), moment()],
               'Ultimos 12 meses': [moment().subtract(12, 'month').startOf('month'), moment()],
            }
        };
    }

    public selectedDate(value: any) {
        this.start = value.start;
        this.end = value.end;
    }
}