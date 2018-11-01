import { FormsModule } from '@angular/forms';
import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";

import { PeityModule } from '../../components/charts/peity';
import { TranslateModule } from "@ngx-translate/core";
import { AnalyticService } from '../../services/allocation-management/analytic.service';
import { SpinnerModule } from '../../components/spinner/spinner.module';
import { Select2Module } from '../../components/select2/select2';
import { AllocationService } from '../../services/allocation-management/allocation.service';
import { NgDatepickerModule } from 'ng2-datepicker';
import { EmployeeService } from '../../services/allocation-management/employee.service';
import { IboxtoolsModule } from '../../components/common/iboxtools/iboxtools.module';
import { AddAllocationByResourceComponent } from './allocation/add-by-resource/add-by-resource.component';
import { ResourceSearchComponent } from './resources/search/resource-search.component';
import { Ng2ModalModule } from '../../components/modal/ng2modal.module';
import { ICheckModule } from '../../components/icheck/icheck.module';
import { PCheckModule } from '../../components/pcheck/pcheck.module';
import { AllocationReportComponent } from './allocation/report/allocation-report.component';
import { DateRangePickerModule } from '../../components/date-range-picker/date-range.picker.module';
import { DatePickerModule } from '../../components/date-picker/date-picker.module';
import { WorkTimeApproverComponent } from './worktime/worktime-approval-delegate/worktime-approver.component';
import { AllocationRouter } from './allocation-management.router';
import { CategoryService } from '../../services/admin/category.service';
import { LicenseService } from '../../services/human-resources/licenses.service';
import { EmployeeProfileHistoryService } from '../../services/allocation-management/employee-profile-history.service';
import { CommonModule } from '@angular/common';
import { ResourceDetailModule } from './resources/detail/resource-detail.module';
import { AllocationAssingTableModule } from './allocation/allocation-assignment-table/alloc-assing-table.module';
import { ExternalUserComponent } from 'app/views/human-resources/external-user/external-user.component';
import { NumbersOnlyModule } from 'app/components/numbersOnly/numberOnly.directive';
import { ApproversModule } from '../common/approvers/approvers.module';
import { NgSelectModule } from '@ng-select/ng-select';

@NgModule({
  declarations: [
    AddAllocationByResourceComponent,
    ResourceSearchComponent, 
    AllocationReportComponent,
    WorkTimeApproverComponent,
    ExternalUserComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    AllocationAssingTableModule,
    ResourceDetailModule,
    PeityModule,
    FormsModule,
    SpinnerModule,
    TranslateModule,
    Select2Module,
    NgDatepickerModule,
    DatePickerModule,
    DateRangePickerModule,
    IboxtoolsModule,
    ICheckModule,
    NumbersOnlyModule,
    Ng2ModalModule,
    PCheckModule,
    AllocationRouter,
    ApproversModule,
    NgSelectModule
  ],
  providers: [ AnalyticService, AllocationService, EmployeeService, CategoryService, LicenseService, EmployeeProfileHistoryService ],

  exports: [],
})

export class AllocationManagementModule {
}
