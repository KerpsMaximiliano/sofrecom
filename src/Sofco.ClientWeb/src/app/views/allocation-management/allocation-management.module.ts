import { FormsModule } from '@angular/forms';
import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";

import { PeityModule } from '../../components/charts/peity';
import { TranslateModule } from "@ngx-translate/core";
import { AnalyticService } from 'app/services/allocation-management/analytic.service';
import { SpinnerModule } from 'app/components/spinner/spinner.module';
import { Select2Module } from 'app/components/select2/select2';
import { AllocationService } from 'app/services/allocation-management/allocation.service';
import { NgDatepickerModule } from 'ng2-datepicker';
import { EmployeeService } from 'app/services/allocation-management/employee.service';
import { IboxtoolsModule } from 'app/components/common/iboxtools/iboxtools.module';
import { AddAllocationByResourceComponent } from 'app/views/allocation-management/allocation/add-by-resource/add-by-resource.component';
import { ResourceSearchComponent } from 'app/views/allocation-management/resources/search/resource-search.component';
import { Ng2ModalModule } from 'app/components/modal/ng2modal.module';
import { ICheckModule } from 'app/components/icheck/icheck.module';
import { PCheckModule } from 'app/components/pcheck/pcheck.module';
import { NumbersOnlyModule } from 'app/components/numbersOnly/numberOnly.directive';
import { AllocationReportComponent } from 'app/views/allocation-management/allocation/report/allocation-report.component';
import { DateRangePickerModule } from 'app/components/date-range-picker/date-range.picker.module';
import { DatePickerModule } from 'app/components/date-picker/date-picker.module';
import { WorkTimeApprovalDelegateComponent } from 'app/views/allocation-management/worktime/worktime-approval-delegate/worktime-approval-delegate.component';
import { WorkTimeApprovalDelegateService } from 'app/services/allocation-management/worktime-approval-delegate.service';
import { AllocationRouter } from 'app/views/allocation-management/allocation-management.router';
import { CategoryService } from '../../services/admin/category.service';
import { LicenseService } from '../../services/human-resources/licenses.service';
import { CommonModule } from '@angular/common';
import { ResourceDetailModule } from 'app/views/allocation-management/resources/detail/resource-detail.module';
import { AllocationAssingTableModule } from 'app/views/allocation-management/allocation/allocation-assignment-table/alloc-assing-table.module';

@NgModule({
  declarations: [
    AddAllocationByResourceComponent,
    ResourceSearchComponent, 
    AllocationReportComponent,
    WorkTimeApprovalDelegateComponent
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
    AllocationRouter
  ],
  providers: [ AnalyticService, AllocationService, EmployeeService, WorkTimeApprovalDelegateService, CategoryService, LicenseService ],

  exports: [],
})

export class AllocationManagementModule {
}
