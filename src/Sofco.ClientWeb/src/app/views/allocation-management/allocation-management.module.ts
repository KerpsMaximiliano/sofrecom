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
import { AddAllocationComponent } from 'app/views/allocation-management/allocation/add/add-allocation.component';
import { NgDatepickerModule } from 'ng2-datepicker';
import { EmployeeService } from 'app/services/allocation-management/employee.service';
import { AllocationListComponent } from 'app/views/allocation-management/allocation/list/allocation-list.component';

@NgModule({
  declarations: [
    AnalyticSearchComponent, AddAllocationComponent, AllocationListComponent
  ],
  imports: [
    BrowserModule,
    RouterModule,
    PeityModule,
    FormsModule,
    SpinnerModule,
    TranslateModule,
    Select2Module,
    NgDatepickerModule
  ],
  providers: [ AnalyticService, AllocationService, EmployeeService ],
  exports: [],
})

export class AllocationManagementModule {
}
