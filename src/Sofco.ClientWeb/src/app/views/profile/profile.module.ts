import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Ng2DatatablesModule } from '../../components/datatables/ng2-datatables.module';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ICheckModule } from "../../components/icheck/icheck.module";
import { Ng2ModalModule } from "../../components/modal/ng2modal.module";
import { TranslateModule } from "@ngx-translate/core";
import { FileUploadModule } from 'ng2-file-upload';
import { NgDatepickerModule } from 'ng2-datepicker';
import { Select2Module } from '../../components/select2/select2';
import { LayoutsModule } from '../../components/common/layouts/layouts.module';
import { LicenseService } from '../../services/human-resources/licenses.service';
import { SpinnerModule } from '../../components/spinner/spinner.module';
import { DatePickerModule } from '../../components/date-picker/date-picker.module';
import { WorkTimeComponent } from '../worktime-management/worktime/worktime.component';
import { EmployeeService } from '../../services/allocation-management/employee.service';
import { AnalyticService } from '../../services/allocation-management/analytic.service';
import { TaskService } from '../../services/admin/task.service';
import { WorktimeService } from '../../services/worktime-management/worktime.service';
import { ProfileRouter } from './profile.router';
import { IboxtoolsModule } from '../../components/common/iboxtools/iboxtools.module';
import { ResourceDetailModule } from '../allocation-management/resources/detail/resource-detail.module';
import { AddLicenseModule } from '../human-resources/licenses/add/add-license.module';
import { ButtonsModule } from '../../components/buttons/buttons.module';

@NgModule({
  declarations: [ WorkTimeComponent ],

  imports     : [CommonModule, Ng2DatatablesModule, NgDatepickerModule, RouterModule, FormsModule, ICheckModule, Ng2ModalModule,
                 TranslateModule, FileUploadModule, Select2Module, LayoutsModule, SpinnerModule, DatePickerModule, ProfileRouter,
                IboxtoolsModule, ResourceDetailModule, AddLicenseModule, ButtonsModule],

  providers   : [LicenseService, EmployeeService, AnalyticService, TaskService, WorktimeService],
  
  exports     : []
})
 
export class ProfileModule {} 