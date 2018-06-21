import { CommonModule } from "@angular/common";
import { TranslateModule } from "@ngx-translate/core";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { Ng2ModalModule } from "app/components/modal/ng2modal.module";
import { ResourceDetailComponent } from "app/views/allocation-management/resources/detail/resource-detail.component";
import { LicenseService } from "app/services/human-resources/licenses.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { RouterModule } from "@angular/router";
import { Select2Module } from "../../../../components/select2/select2";
import { FileUploadModule } from "ng2-file-upload/file-upload/file-upload.module";
import { ICheckModule } from "app/components/icheck/icheck.module";
import { DatePickerModule } from "app/components/date-picker/date-picker.module";
import { AddLicenseComponent } from "app/views/human-resources/licenses/add/add-license.componente";

@NgModule({
    imports: [
      CommonModule,
      RouterModule,
      FormsModule,
      TranslateModule,
      Ng2ModalModule,
      Select2Module,
      FileUploadModule,
      ICheckModule,
      DatePickerModule
    ],
    declarations: [ 
        AddLicenseComponent
    ],
    providers: [
        LicenseService, EmployeeService
    ],
    exports: [
        AddLicenseComponent
    ]
  })
  export class AddLicenseModule { }
   