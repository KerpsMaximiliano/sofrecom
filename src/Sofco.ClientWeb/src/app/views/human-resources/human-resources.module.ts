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
import { LicenseListRrhh } from 'app/views/human-resources/licenses/license-dahsboard-rrhh/license-list-rrhh.component';
import { LicenseListWidget } from 'app/views/human-resources/licenses/license-list-widget/license-list-widget.component';
import { SpinnerModule } from 'app/components/spinner/spinner.module';
import { LicenseListManager } from 'app/views/human-resources/licenses/license-list-manager/license-list-manager.component';
import { LicenseDetailComponent } from 'app/views/human-resources/licenses/detail/license-detail.component';
import { LicenseAuthPendingComponent } from 'app/views/human-resources/licenses/workflow/auth-pending/auth-pending.component';
import { LicensePendingComponent } from 'app/views/human-resources/licenses/workflow/pending/pending.component';
import { LicenseRejectComponent } from 'app/views/human-resources/licenses/workflow/reject/reject.component';
import { LicenseHistoryComponent } from 'app/views/human-resources/licenses/history/license-history.component';
import { DatePickerModule } from 'app/components/date-picker/date-picker.module';
import { LicenseCancelComponent } from 'app/views/human-resources/licenses/workflow/cancelled/cancelled.component';
import { UnemployeesSearchComponent } from 'app/views/human-resources/resources/search-unemployees/unemployees-search.component';
import { RrhhRouter } from 'app/views/human-resources/rrhh.router';
import { EmployeeService } from 'app/services/allocation-management/employee.service';
import { EmployeeNewsService } from 'app/services/allocation-management/employee-news.service';
import { NewsComponent } from 'app/views/human-resources/news/news.component';
import { AddLicenseModule } from 'app/views/human-resources/licenses/add/add-license.module';

@NgModule({
  declarations: [LicenseListRrhh, LicenseListWidget, LicenseListManager, LicenseDetailComponent, LicenseAuthPendingComponent,
                LicensePendingComponent, LicenseRejectComponent, LicenseHistoryComponent, LicenseCancelComponent, UnemployeesSearchComponent,
                NewsComponent],

  imports     : [CommonModule, Ng2DatatablesModule, RouterModule, FormsModule, ICheckModule, Ng2ModalModule, AddLicenseModule,
                 TranslateModule, FileUploadModule, Select2Module, LayoutsModule, SpinnerModule, DatePickerModule, RrhhRouter],

  providers   : [LicenseService, EmployeeService, EmployeeNewsService],
  
  exports     : []
})
 
export class HumanResourcesModule {} 