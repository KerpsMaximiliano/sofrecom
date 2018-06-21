import { CommonModule } from "@angular/common";
import { TranslateModule } from "@ngx-translate/core";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { Ng2ModalModule } from "app/components/modal/ng2modal.module";
import { ResourceDetailComponent } from "app/views/allocation-management/resources/detail/resource-detail.component";
import { LicenseService } from "app/services/human-resources/licenses.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { RouterModule } from "@angular/router";

@NgModule({
    imports: [
      CommonModule,
      RouterModule,
      FormsModule,
      TranslateModule,
      Ng2ModalModule,
    ],
    declarations: [ 
        ResourceDetailComponent
    ],
    providers: [
        LicenseService, EmployeeService
    ],
    exports: [
        ResourceDetailComponent
    ]
  })
  export class ResourceDetailModule { }
   