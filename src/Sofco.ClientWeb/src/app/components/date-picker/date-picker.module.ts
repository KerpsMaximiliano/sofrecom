import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DatePickerComponent } from 'app/components/date-picker/date-picker.component';
import { FormsModule } from '@angular/forms';
import { DatepickerDirectiveModule } from 'app/components/date-picker/date-picker.directive';

@NgModule({
  imports: [
    CommonModule, FormsModule, DatepickerDirectiveModule
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
