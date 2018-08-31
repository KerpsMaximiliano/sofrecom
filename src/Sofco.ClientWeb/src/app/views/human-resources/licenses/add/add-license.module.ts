import { CommonModule } from "@angular/common";
import { TranslateModule } from "@ngx-translate/core";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { Ng2ModalModule } from "../../../../components/modal/ng2modal.module";
import { ResourceDetailComponent } from "../../../allocation-management/resources/detail/resource-detail.component";
import { LicenseService } from "../../../../services/human-resources/licenses.service";
import { EmployeeService } from "../../../../services/allocation-management/employee.service";
import { RouterModule } from "@angular/router";
import { Select2Module } from "../../../../components/select2/select2";
import { FileUploadModule } from "ng2-file-upload/file-upload/file-upload.module";
import { ICheckModule } from "../../../../components/icheck/icheck.module";
import { DatePickerModule } from "../../../../components/date-picker/date-picker.module";
import { AddLicenseComponent } from "./add-license.componente";

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
   