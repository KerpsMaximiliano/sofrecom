import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Ng2DatatablesModule } from '../../components/datatables/ng2-datatables.module';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ICheckModule } from "../../components/icheck/icheck.module";
import { Ng2ModalModule } from "../../components/modal/ng2modal.module";
import { TranslateModule } from "@ngx-translate/core";
import { FileUploadModule } from 'ng2-file-upload';
import { Select2Module } from '../../components/select2/select2';
import { LayoutsModule } from '../../components/common/layouts/layouts.module';
import { LicenseService } from '../../services/human-resources/licenses.service';
import { LicenseListRrhh } from './licenses/license-dahsboard-rrhh/license-list-rrhh.component';
import { LicenseListWidget } from './licenses/license-list-widget/license-list-widget.component';
import { SpinnerModule } from '../../components/spinner/spinner.module';
import { LicenseListManager } from './licenses/license-list-manager/license-list-manager.component';
import { LicenseDetailComponent } from './licenses/detail/license-detail.component';
import { LicenseAuthPendingComponent } from './licenses/workflow/auth-pending/auth-pending.component';
import { LicensePendingComponent } from './licenses/workflow/pending/pending.component';
import { LicenseRejectComponent } from './licenses/workflow/reject/reject.component';
import { LicenseHistoryComponent } from './licenses/history/license-history.component';
import { DatePickerModule } from '../../components/date-picker/date-picker.module';
import { LicenseCancelComponent } from './licenses/workflow/cancelled/cancelled.component';
import { UnemployeesSearchComponent } from './resources/search-unemployees/unemployees-search.component';
import { RrhhRouter } from './rrhh.router';
import { EmployeeService } from '../../services/allocation-management/employee.service';
import { EmployeeNewsService } from '../../services/allocation-management/employee-news.service';
import { NewsComponent } from './news/news.component';
import { AddLicenseModule } from './licenses/add/add-license.module';
import { LicenseDelegateComponent } from './licenses/license-delegate/license-delegate.component';
import { ServiceService } from '../../services/billing/service.service';
import { CustomerService } from '../../services/billing/customer.service';
import { ApproversModule } from '../common/approvers/approvers.module';
import { NgSelectModule } from '@ng-select/ng-select';
import { EndNotificationComponent } from './end-notification/end-notification.component';
import { PrepaidImportComponent } from './prepaid-import/prepaid-import';
import { UtilsService } from 'app/services/common/utils.service';
import { PrepaidService } from 'app/services/human-resources/prepaid.service';
import { PrepaidVerificationComponent } from './prepaid-verification/prepaid-verification';

@NgModule({
  declarations: [LicenseListRrhh, LicenseListWidget, LicenseListManager, LicenseDetailComponent, LicenseAuthPendingComponent,
                LicensePendingComponent, LicenseRejectComponent, LicenseHistoryComponent, LicenseCancelComponent, UnemployeesSearchComponent,
                NewsComponent, LicenseDelegateComponent, EndNotificationComponent, PrepaidImportComponent, PrepaidVerificationComponent],

  imports     : [CommonModule, Ng2DatatablesModule, RouterModule, FormsModule, ICheckModule, Ng2ModalModule, AddLicenseModule,
                 TranslateModule, FileUploadModule, Select2Module, LayoutsModule, SpinnerModule, DatePickerModule, RrhhRouter,
                ApproversModule, NgSelectModule],

  providers   : [LicenseService, EmployeeService, EmployeeNewsService, ServiceService, CustomerService, UtilsService, PrepaidService],

  exports     : []
})

export class HumanResourcesModule {}
