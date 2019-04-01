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
import { AmountFormatModule } from "app/pipes/amount-format.module";
import { ManagementReportBillingComponent } from "./mr-billing/mr-billing";
import { MenuService } from "app/services/admin/menu.service";
import { ProjectService } from "app/services/billing/project.service";
import { UtilsService } from "app/services/common/utils.service";
import { MessageService } from "app/services/common/message.service";
import { DecimalFormatModule } from "app/components/decimalFormat/decimal-format.directive";

@NgModule({
    declarations: [
      ManagementReportDetailComponent, MarginTrackingComponent, ManagementReportBillingComponent
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
      AmountFormatModule,
      PCheckModule,
      NgSelectModule
    ],
    providers: [ ManagementReportService, DatesService, MenuService, UtilsService, ProjectService, MessageService ],
    exports: [],
  })
  
  export class ManagementReportModule {
  }
  