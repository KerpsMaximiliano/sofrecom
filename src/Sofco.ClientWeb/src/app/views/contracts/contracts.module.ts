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
import { Ng2ModalModule } from '../../components/modal/ng2modal.module';
import { CostCenterService } from '../../services/allocation-management/cost-center.service';
import { ICheckModule } from '../../components/icheck/icheck.module';
import { PCheckModule } from '../../components/pcheck/pcheck.module';
import { DateRangePickerModule } from '../../components/date-range-picker/date-range.picker.module';
import { DatePickerModule } from '../../components/date-picker/date-picker.module';
import { AnalyticSearchComponent } from './analytics/search/analytic-search.component';
import { AddCostCenterComponent } from './cost-center/add/add-cost-center.component';
import { EditAnalyticComponent } from './analytics/edit/edit-analytic.component';
import { EditCostCenterComponent } from './cost-center/edit/edit-cost-center.component';
import { ViewAnalyticComponent } from './analytics/view/view-analytic.component';
import { ListCostCenterComponent } from './cost-center/list/list-cost-center.component';
import { NewAnalyticComponent } from './analytics/new/new-analytic.component';
import { AnalyticFormComponent } from './analytics/analytic-form/analytic-form.component';
import { ContractsRouter } from './contracts.router';
import { AddAllocationComponent } from '../allocation-management/allocation/add-by-analytic/add-by-analytic.component';
import { ResourceByAnalyticComponent } from '../allocation-management/resources/by-analytic/resource-by-analytic.component';
import { ResourceTimelineComponent } from '../allocation-management/allocation/resource-timeline/resource-timeline.component';
import { CommonModule } from '@angular/common';
import { CustomerService } from '../../services/billing/customer.service';
import { ServiceService } from '../../services/billing/service.service';
import { AllocationAssingTableModule } from '../allocation-management/allocation/allocation-assignment-table/alloc-assing-table.module';
import { NumbersOnlyModule } from 'app/components/numbersOnly/numberOnly.directive';
import { AddCloseDateComponent } from 'app/views/contracts/closeDates/add/closeDate-add.component';
import { CloseDateService } from 'app/services/human-resources/closeDate.service';
import { NgSelectModule } from '@ng-select/ng-select';
import { CurrencyExchangeService } from 'app/services/management-report/currency-exchange.service';
import { CurrencyExchangeComponent } from './currency-exchange/currency-exchange';
import { CategoryService } from 'app/services/admin/category.service';
import { ResourceByServiceComponent } from '../allocation-management/resources/by-service/resource-by-service.component';

@NgModule({
  declarations: [
    AnalyticSearchComponent, AddCostCenterComponent, ListCostCenterComponent, NewAnalyticComponent, AnalyticFormComponent,
    EditAnalyticComponent, ViewAnalyticComponent, EditCostCenterComponent, AddAllocationComponent, ResourceByAnalyticComponent,
    ResourceTimelineComponent, AddCloseDateComponent, CurrencyExchangeComponent, ResourceByServiceComponent
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
    ContractsRouter,
    NgSelectModule
  ],
  providers: [ AnalyticService, AllocationService, EmployeeService, CostCenterService, CustomerService, ServiceService, CloseDateService, CurrencyExchangeService, CategoryService ],
  exports: [],
})

export class ContractsModule {
}
