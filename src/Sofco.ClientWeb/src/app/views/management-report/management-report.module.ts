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

@NgModule({
    declarations: [
      ManagementReportDetailComponent
    ],
    imports: [
      CommonModule,
      RouterModule,
      FormsModule,
      TranslateModule,
      ManagementReportRouter,
      DatePickerModule,
      NumbersOnlyModule,
      Ng2ModalModule,
      PCheckModule,
      NgSelectModule
    ],
    providers: [  ],
    exports: [],
  })
  
  export class ManagementReportModule {
  }
  