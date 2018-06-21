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
import { Ng2ModalModule } from 'app/components/modal/ng2modal.module';
import { CostCenterService } from 'app/services/allocation-management/cost-center.service';
import { ICheckModule } from 'app/components/icheck/icheck.module';
import { PCheckModule } from 'app/components/pcheck/pcheck.module';
import { NumbersOnlyModule } from 'app/components/numbersOnly/numberOnly.directive';
import { DateRangePickerModule } from 'app/components/date-range-picker/date-range.picker.module';
import { DatePickerModule } from 'app/components/date-picker/date-picker.module';
import { AnalyticSearchComponent } from 'app/views/contracts/analytics/search/analytic-search.component';
import { AddCostCenterComponent } from 'app/views/contracts/cost-center/add/add-cost-center.component';
import { EditAnalyticComponent } from 'app/views/contracts/analytics/edit/edit-analytic.component';
import { EditCostCenterComponent } from 'app/views/contracts/cost-center/edit/edit-cost-center.component';
import { ViewAnalyticComponent } from 'app/views/contracts/analytics/view/view-analytic.component';
import { ListCostCenterComponent } from 'app/views/contracts/cost-center/list/list-cost-center.component';
import { NewAnalyticComponent } from 'app/views/contracts/analytics/new/new-analytic.component';
import { AnalyticFormComponent } from 'app/views/contracts/analytics/analytic-form/analytic-form.component';
import { ContractsRouter } from 'app/views/contracts/contracts.router';
import { AddAllocationComponent } from 'app/views/allocation-management/allocation/add-by-analytic/add-by-analytic.component';
import { ResourceByAnalyticComponent } from 'app/views/allocation-management/resources/by-analytic/resource-by-analytic.component';
import { ResourceTimelineComponent } from 'app/views/allocation-management/allocation/resource-timeline/resource-timeline.component';
import { CommonModule } from '@angular/common';
import { CustomerService } from '../../services/billing/customer.service';
import { ServiceService } from '../../services/billing/service.service';
import { AllocationAssingTableModule } from 'app/views/allocation-management/allocation/allocation-assignment-table/alloc-assing-table.module';

@NgModule({
  declarations: [
    AnalyticSearchComponent, AddCostCenterComponent, ListCostCenterComponent, NewAnalyticComponent, AnalyticFormComponent,
    EditAnalyticComponent, ViewAnalyticComponent, EditCostCenterComponent, AddAllocationComponent, ResourceByAnalyticComponent,
    ResourceTimelineComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    PeityModule,
    AllocationAssingTableModule,
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
    ContractsRouter
  ],
  providers: [ AnalyticService, AllocationService, EmployeeService, CostCenterService, CustomerService, ServiceService ],
  exports: [],
})

export class ContractsModule {
}
