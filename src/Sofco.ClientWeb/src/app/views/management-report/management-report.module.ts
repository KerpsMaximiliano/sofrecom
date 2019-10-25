import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";
import { FormsModule } from "@angular/forms";
import { TranslateModule } from "@ngx-translate/core";
import { DatePickerModule } from "app/components/date-picker/date-picker.module";
import { NumbersOnlyModule } from "app/components/numbersOnly/numberOnly.directive";
import { Ng2ModalModule } from "app/components/modal/ng2modal.module";
import { PCheckModule } from "app/components/pcheck/pcheck.module";
import { NgSelectModule } from "@ng-select/ng-select";
import { ManagementReportRouter } from "./management-report.routes";
import { ManagementReportDetailComponent } from "./detail/mr-detail";
import { ManagementReportService } from "app/services/management-report/management-report.service";
import { DatesService } from "app/services/common/month.service";
import { MarginTrackingComponent } from "./margin-tracking/margin-tracking";
import { CostDetailComponent } from "./cost-detail/cost-detail.component";
import { AmountFormatModule } from "app/pipes/amount-format.module";
import { ManagementReportBillingComponent } from "./mr-billing/mr-billing";
import { MenuService } from "app/services/admin/menu.service";
import { ProjectService } from "app/services/billing/project.service";
import { UtilsService } from "app/services/common/utils.service";
import { MessageService } from "app/services/common/message.service";
import { DecimalFormatModule } from "app/components/decimalFormat/decimal-format.directive";
import { CostDetailMonthComponent } from "./cost-detail-month/cost-detail-month";
import { ReactiveFormsModule } from '@angular/forms';
import { DigitModule } from "app/components/digit-limit/digit-limit.directive";
import { BsDatepickerModule } from 'ngx-bootstrap';
import { ModalEvalPropComponent } from "./modal-evalprop/modal-evalprop";
import { EmployeeService } from "app/services/allocation-management/employee.service"
import { fromDateModule } from "app/pipes/form-date.module";
import { TracingComponent } from "./tracing/tracing.component";
import { ManagementReportStaffService } from "app/services/management-report/management-report-staff.service";
import { ManagementReportDetailStaffComponent } from "./staff/detail/detail-staff";
import { BudgetStaffComponent } from "./staff/budget/budget-staff.component";
import { CostDetailMonthStaffComponent } from "./staff/cost-month-staff/cost-month-staff";
import { TracingStaffComponent } from "./staff/tracing/tracing-staff.component";
import { MathModule } from "app/components/math-input/math-input.module";
import { ServiceService } from "app/services/billing/service.service";
import { GenericOptionService } from "app/services/admin/generic-option.service";
import { LaddaModule } from "angular2-ladda";
import { ICheckModule } from "app/components/icheck/icheck.module";


@NgModule({
    declarations: [
      ManagementReportDetailComponent, MarginTrackingComponent, ManagementReportBillingComponent, CostDetailComponent, 
      CostDetailMonthComponent, ModalEvalPropComponent, TracingComponent, ManagementReportDetailStaffComponent, CostDetailMonthStaffComponent, 
      BudgetStaffComponent, TracingStaffComponent
    ],
    imports: [
      CommonModule,
      RouterModule,
      FormsModule,
      TranslateModule,
      ManagementReportRouter,
      DatePickerModule,
      NumbersOnlyModule,
      DecimalFormatModule,
      Ng2ModalModule, 
      LaddaModule,
      AmountFormatModule,
      fromDateModule,
      PCheckModule,
      ICheckModule,
      NgSelectModule,
      DigitModule,
      ReactiveFormsModule,
      BsDatepickerModule,
      MathModule
    ],
    providers: [ ManagementReportService, DatesService, UtilsService, ProjectService, EmployeeService, ManagementReportStaffService, ServiceService, GenericOptionService ],
    exports: [],
  })
  
  export class ManagementReportModule {
  }
  