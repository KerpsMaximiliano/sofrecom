import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Ng2DatatablesModule } from 'app/components/datatables/ng2-datatables.module';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ICheckModule } from "app/components/icheck/icheck.module";
import { Ng2ModalModule } from "app/components/modal/ng2modal.module";
import { TranslateModule } from "@ngx-translate/core";
import { FileUploadModule } from 'ng2-file-upload';
import { NgDatepickerModule } from 'ng2-datepicker';
import { Select2Module } from 'app/components/select2/select2';
import { LayoutsModule } from 'app/components/common/layouts/layouts.module';
import { SpinnerModule } from 'app/components/spinner/spinner.module';
import { DatePickerModule } from 'app/components/date-picker/date-picker.module';
import { WorkTimeComponent } from './worktime/worktime.component';
import { FullCalendarModule } from 'ng-fullcalendar';
import 'fullcalendar/dist/locale/es.js';
import { WorkTimeApprovalComponent } from 'app/views/worktime-management/approval/worktime-approval.component';
import { WorktimeService } from 'app/services/worktime-management/worktime.service';
import { IboxtoolsModule } from 'app/components/common/iboxtools/iboxtools.module';

@NgModule({
  declarations: [WorkTimeComponent, WorkTimeApprovalComponent],

  imports : [CommonModule, Ng2DatatablesModule, NgDatepickerModule, RouterModule, FormsModule, ICheckModule, Ng2ModalModule, 
    TranslateModule, FileUploadModule, Select2Module, LayoutsModule, SpinnerModule, DatePickerModule,
    FullCalendarModule, IboxtoolsModule],

  providers   : [WorktimeService],

  exports     : []
})

export class WorkTimeManagementModule {

}
