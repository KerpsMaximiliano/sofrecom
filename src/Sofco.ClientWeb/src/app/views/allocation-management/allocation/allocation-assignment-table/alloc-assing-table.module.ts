import { CommonModule } from "@angular/common";
import { TranslateModule } from "@ngx-translate/core";
import { NgModule } from "@angular/core";
import { AllocationService } from "../../../../services/allocation-management/allocation.service";
import { FormsModule } from "@angular/forms";
import { Ng2ModalModule } from "../../../../components/modal/ng2modal.module";
import { DatePickerModule } from "../../../../components/date-picker/date-picker.module";
import { Select2Module } from "../../../../components/select2/select2";
import { AllocationAssignmentTableComponent } from "./alloc-assig-table.component";

@NgModule({
    imports: [
      CommonModule,
      FormsModule,
      TranslateModule,
      Ng2ModalModule,
      DatePickerModule,
      Select2Module,
    ],
    declarations: [ 
        AllocationAssignmentTableComponent
    ],
    providers: [
        AllocationService
    ],
    exports: [
        AllocationAssignmentTableComponent
    ]
  })
  export class AllocationAssingTableModule { }
   