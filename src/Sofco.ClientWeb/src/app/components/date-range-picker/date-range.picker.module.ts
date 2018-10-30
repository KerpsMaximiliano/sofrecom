import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Daterangepicker, DaterangepickerConfig } from 'ng2-daterangepicker';
import { DateRangePickerComponent } from './date-range-picker.component';

@NgModule({
  imports: [
    CommonModule, Daterangepicker
  ],
  declarations: [
    DateRangePickerComponent
  ],
  providers: [
    DaterangepickerConfig
  ],
  exports: [
    DateRangePickerComponent
  ]
})
export class DateRangePickerModule { }
