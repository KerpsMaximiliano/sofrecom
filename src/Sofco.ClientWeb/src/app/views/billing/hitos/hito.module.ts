import { CommonModule } from "@angular/common";
import { TranslateModule } from "@ngx-translate/core";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { NgSelectModule } from "@ng-select/ng-select";
import { UtilsService } from "app/services/common/utils.service";
import { ProjectService } from "app/services/billing/project.service";
import { NewHitoComponent } from "./new/new-hito.component";
import { Ng2ModalModule } from "app/components/modal/ng2modal.module";
import { SplitHitoComponent } from "./split/split-hito.component";
import { DatePickerModule } from "app/components/date-picker/date-picker.module";

@NgModule({
    imports: [
      CommonModule,
      RouterModule,
      FormsModule,
      TranslateModule,
      Ng2ModalModule,
      DatePickerModule,
      NgSelectModule,
    ],
    declarations: [ 
        SplitHitoComponent, NewHitoComponent
    ],
    providers: [
        UtilsService, ProjectService
    ],
    exports: [
        SplitHitoComponent, NewHitoComponent
    ]
  })
  export class HitoModule { }
   