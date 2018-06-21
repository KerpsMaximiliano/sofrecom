import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Ng2DatatablesModule } from 'app/components/datatables/ng2-datatables.module';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ICheckModule } from "app/components/icheck/icheck.module";
import { PCheckModule } from "app/components/pcheck/pcheck.module";
import { Ng2ModalModule } from "app/components/modal/ng2modal.module";
import { TranslateModule } from "@ngx-translate/core";
import { FileUploadModule } from 'ng2-file-upload';
import { NgDatepickerModule } from 'ng2-datepicker';
import { Select2Module } from 'app/components/select2/select2';
import { LayoutsModule } from 'app/components/common/layouts/layouts.module';
import { SpinnerModule } from 'app/components/spinner/spinner.module';
import { DatePickerModule } from 'app/components/date-picker/date-picker.module';
import { WorkTimeApprovalComponent } from 'app/views/worktime-management/approval/worktime-approval.component';
import { WorktimeService } from 'app/services/worktime-management/worktime.service';
import { IboxtoolsModule } from 'app/components/common/iboxtools/iboxtools.module';
import { WorkTimeStatusApproveComponent } from 'app/views/worktime-management/status-approve/status-approve.component';
import { WorkTimeStatusRejectComponent } from 'app/views/worktime-management/status-reject/status-reject.component';
import { UtilsService } from 'app/services/common/utils.service';
import { WorkTimeReportComponent } from 'app/views/worktime-management/report/worktime-report.component';
import { WorkTimeSearchComponent } from 'app/views/worktime-management/search/worktime-search.component';
import { HolidaysComponent } from './holidays/holidays.component';
import { HolidayService } from 'app/services/worktime-management/holiday.service';
import { WorkTimeRouter } from 'app/views/worktime-management/worktime.router';
import { EmployeeService } from '../../services/allocation-management/employee.service';
import { AnalyticService } from '../../services/allocation-management/analytic.service';
import { CustomerService } from '../../services/billing/customer.service';

@NgModule({
  declarations: [WorkTimeApprovalComponent, WorkTimeStatusApproveComponent, WorkTimeStatusRejectComponent, 
                 WorkTimeReportComponent, WorkTimeSearchComponent, HolidaysComponent],

  imports : [CommonModule, Ng2DatatablesModule, NgDatepickerModule, RouterModule, FormsModule, ICheckModule, Ng2ModalModule,
             TranslateModule, FileUploadModule, Select2Module, LayoutsModule, SpinnerModule, DatePickerModule, IboxtoolsModule, 
             PCheckModule, WorkTimeRouter],

  providers   : [WorktimeService, UtilsService, HolidayService, EmployeeService, AnalyticService, CustomerService],

  exports     : []
})

export class WorkTimeManagementModule {

}
