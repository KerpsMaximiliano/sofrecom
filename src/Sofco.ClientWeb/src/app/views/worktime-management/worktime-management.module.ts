import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Ng2DatatablesModule } from '../../components/datatables/ng2-datatables.module';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ICheckModule } from "../../components/icheck/icheck.module";
import { PCheckModule } from "../../components/pcheck/pcheck.module";
import { Ng2ModalModule } from "../../components/modal/ng2modal.module";
import { TranslateModule } from "@ngx-translate/core";
import { FileUploadModule } from 'ng2-file-upload';
import { NgDatepickerModule } from 'ng2-datepicker';
import { Select2Module } from '../../components/select2/select2';
import { LayoutsModule } from '../../components/common/layouts/layouts.module';
import { SpinnerModule } from '../../components/spinner/spinner.module';
import { DatePickerModule } from '../../components/date-picker/date-picker.module';
import { IboxtoolsModule } from '../../components/common/iboxtools/iboxtools.module';

import { UtilsService } from '../../services/common/utils.service';
import { WorktimeService } from '../../services/worktime-management/worktime.service';
import { WorktimeControlService } from '../../services/worktime-management/worktime-control.service';
import { HolidayService } from '../../services/worktime-management/holiday.service';
import { EmployeeService } from '../../services/allocation-management/employee.service';
import { AnalyticService } from '../../services/allocation-management/analytic.service';
import { CustomerService } from '../../services/billing/customer.service';
import { ServiceService } from '../../services/billing/service.service';
import { I18nService } from '../../services/common/i18n.service';

import { WorkTimeApprovalComponent } from './approval/worktime-approval.component';
import { WorkTimeStatusApproveComponent } from './status-approve/status-approve.component';
import { WorkTimeStatusRejectComponent } from './status-reject/status-reject.component';
import { WorkTimeReportComponent } from './report/worktime-report.component';
import { WorkTimeSearchComponent } from './search/worktime-search.component';
import { HolidaysComponent } from './holidays/holidays.component';
import { WorkTimeRouter } from './worktime.router';
import { ImportWorkTimesComponent } from 'app/views/worktime-management/import/import-worktime.component';
import { WorkTimeControlComponent } from './worktime-control/worktime-control.component';
import { DateRangePickerModule } from '../../components/date-range-picker/date-range.picker.module';
import { RrhhService } from 'app/services/human-resources/rrhh.service';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NgSelectModule } from '@ng-select/ng-select';

@NgModule({
  declarations: [WorkTimeApprovalComponent, WorkTimeStatusApproveComponent, WorkTimeStatusRejectComponent, 
                 WorkTimeReportComponent, WorkTimeSearchComponent, HolidaysComponent, ImportWorkTimesComponent,
                WorkTimeControlComponent],

  imports : [CommonModule, Ng2DatatablesModule, NgDatepickerModule, RouterModule, FormsModule, ICheckModule, Ng2ModalModule,
             TranslateModule, FileUploadModule, Select2Module, LayoutsModule, SpinnerModule, DatePickerModule, IboxtoolsModule, 
             PCheckModule, WorkTimeRouter, DateRangePickerModule, TooltipModule, NgSelectModule],

  providers   : [WorktimeService, UtilsService, HolidayService, EmployeeService, AnalyticService, CustomerService, ServiceService, 
                I18nService, WorktimeControlService, RrhhService],

  exports     : []
})

export class WorkTimeManagementModule {

}
