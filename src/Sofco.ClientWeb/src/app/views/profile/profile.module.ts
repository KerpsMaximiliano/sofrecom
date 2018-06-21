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
import { LicenseService } from 'app/services/human-resources/licenses.service';
import { SpinnerModule } from 'app/components/spinner/spinner.module';
import { DatePickerModule } from 'app/components/date-picker/date-picker.module';
import { WorkTimeComponent } from 'app/views/worktime-management/worktime/worktime.component';
import { EmployeeService } from 'app/services/allocation-management/employee.service';
import { AnalyticService } from 'app/services/allocation-management/analytic.service';
import { TaskService } from 'app/services/admin/task.service';
import { WorktimeService } from 'app/services/worktime-management/worktime.service';
import { ProfileRouter } from 'app/views/profile/profile.router';
import { IboxtoolsModule } from '../../components/common/iboxtools/iboxtools.module';
import { ResourceDetailModule } from 'app/views/allocation-management/resources/detail/resource-detail.module';
import { AddLicenseModule } from 'app/views/human-resources/licenses/add/add-license.module';

@NgModule({
  declarations: [ WorkTimeComponent ],

  imports     : [CommonModule, Ng2DatatablesModule, NgDatepickerModule, RouterModule, FormsModule, ICheckModule, Ng2ModalModule,
                 TranslateModule, FileUploadModule, Select2Module, LayoutsModule, SpinnerModule, DatePickerModule, ProfileRouter,
                IboxtoolsModule, ResourceDetailModule, AddLicenseModule],

  providers   : [LicenseService, EmployeeService, AnalyticService, TaskService, WorktimeService],
  
  exports     : []
})
 
export class ProfileModule {} 