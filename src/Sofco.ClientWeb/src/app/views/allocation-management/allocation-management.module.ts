import { FormsModule } from '@angular/forms';
import {NgModule} from "@angular/core";
import {BrowserModule} from "@angular/platform-browser";
import {RouterModule} from "@angular/router";

import {PeityModule } from '../../components/charts/peity';
import { ForbiddenComponent } from "app/views/appviews/errors/403/forbidden.component";
import { TranslateModule } from "@ngx-translate/core";
import { AnalyticService } from 'app/services/allocation-management/analytic.service';
import { SpinnerModule } from 'app/components/spinner/spinner.module';
import { Select2Module } from 'app/components/select2/select2';
import { AllocationService } from 'app/services/allocation-management/allocation.service';
import { AnalyticSearchComponent } from 'app/views/allocation-management/analytics/search/analytic-search.component';
import { AddAllocationComponent } from 'app/views/allocation-management/allocation/add-by-analytic/add-by-analytic.component';
import { NgDatepickerModule } from 'ng2-datepicker';
import { EmployeeService } from 'app/services/allocation-management/employee.service';
import { DateRangePickerModule } from 'app/components/datepicker/date-range.picker.module';
import { IboxtoolsModule } from 'app/components/common/iboxtools/iboxtools.module';
import { AddAllocationByResourceComponent } from 'app/views/allocation-management/allocation/add-by-resource/add-by-resource.component';
import { AllocationAssignmentTableComponent } from 'app/views/allocation-management/allocation/allocation-assignment-table/alloc-assig-table.component';
import { ResourceSearchComponent } from 'app/views/allocation-management/resources/search/resource-search.component';
import { Ng2ModalModule } from 'app/components/modal/ng2modal.module';
import { ResourceTimelineComponent } from 'app/views/allocation-management/allocation/resource-timeline/resource-timeline.component';
import { CostCenterService } from 'app/services/allocation-management/cost-center.service';
import { AddCostCenterComponent } from 'app/views/allocation-management/cost-center/add/add-cost-center.component';
import { ListCostCenterComponent } from 'app/views/allocation-management/cost-center/list/list-cost-center.component';
import { NewAnalyticComponent } from 'app/views/allocation-management/analytics/new/new-analytic.component';
import { AnalyticFormComponent } from 'app/views/allocation-management/analytics/analytic-form/analytic-form.component';
import { ICheckModule } from 'app/components/icheck/icheck.module';
import { NewsComponent } from 'app/views/allocation-management/news/news.component';
import { EditAnalyticComponent } from 'app/views/allocation-management/analytics/edit/edit-analytic.component';
import { EmployeeNewsService } from 'app/services/allocation-management/employee-news.service';
import { ResourceByServiceComponent } from 'app/views/allocation-management/resources/by-service/resource-by-service.component';
import { NumbersOnlyModule } from 'app/components/numbersOnly/numberOnly.directive';

@NgModule({
  declarations: [
    AnalyticSearchComponent, AddAllocationComponent, ResourceTimelineComponent, AddAllocationByResourceComponent, AllocationAssignmentTableComponent, ResourceSearchComponent, AddCostCenterComponent,
    ListCostCenterComponent, NewAnalyticComponent, AnalyticFormComponent, NewsComponent, EditAnalyticComponent, ResourceByServiceComponent
  ],
  imports: [
    BrowserModule,
    RouterModule,
    PeityModule,
    FormsModule,
    SpinnerModule,
    TranslateModule,
    Select2Module,
    NgDatepickerModule,
    DateRangePickerModule,
    IboxtoolsModule,
    ICheckModule,
    NumbersOnlyModule,
    Ng2ModalModule
  ],
  providers: [ AnalyticService, AllocationService, EmployeeService, CostCenterService, EmployeeNewsService ],
  exports: [],
})

export class AllocationManagementModule {
}
