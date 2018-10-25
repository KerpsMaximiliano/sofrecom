import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DatePickerComponent } from './date-picker.component';
import { FormsModule } from '@angular/forms';
import { DatepickerDirectiveModule } from './date-picker.directive';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';

@NgModule({
  imports: [
    CommonModule, FormsModule, DatepickerDirectiveModule, BsDatepickerModule
  ],
  declarations: [ 
    DatePickerComponent
  ],
  providers: [
  ],
  exports: [
    DatePickerComponent
  ]
})
export class DatePickerModule { }
