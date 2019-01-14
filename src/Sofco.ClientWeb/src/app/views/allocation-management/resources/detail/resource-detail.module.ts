import { CommonModule } from "@angular/common";
import { TranslateModule } from "@ngx-translate/core";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { Ng2ModalModule } from "../../../../components/modal/ng2modal.module";
import { ResourceDetailComponent } from "./resource-detail.component";
import { LicenseService } from "../../../../services/human-resources/licenses.service";
import { EmployeeService } from "../../../../services/allocation-management/employee.service";
import { RouterModule } from "@angular/router";
import { Select2Module } from "../../../../components/select2/select2";
import { EmployeeProfileHistoryService } from "../../../../services/allocation-management/employee-profile-history.service";
import { NgSelectModule } from "@ng-select/ng-select";
import { ICheckModule } from "app/components/icheck/icheck.module";

@NgModule({
    imports: [
      CommonModule,
      RouterModule,
      FormsModule,
      TranslateModule,
      Ng2ModalModule,
      Select2Module,
      NgSelectModule,
      ICheckModule
    ],
    declarations: [ 
        ResourceDetailComponent
    ],
    providers: [
        LicenseService, EmployeeService, EmployeeProfileHistoryService
    ],
    exports: [
        ResourceDetailComponent
    ]
  })
  export class ResourceDetailModule { }
   