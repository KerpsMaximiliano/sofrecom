import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Ng2DatatablesModule } from 'app/components/datatables/ng2-datatables.module';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ICheckModule } from "app/components/icheck/icheck.module";
import { Ng2ModalModule } from "app/components/modal/ng2modal.module";
import { TranslateModule } from "@ngx-translate/core";
import { FileUploadModule } from 'ng2-file-upload';

import { NgDatepickerModule } from 'ng2-datepicker';

import { Select2Module } from 'app/components/select2/select2';
import { LayoutsModule } from 'app/components/common/layouts/layouts.module';
import { LicenseService } from 'app/services/human-resources/licenses.service';
import { AddLicenseComponent } from 'app/views/human-resources/licenses/add/add-license.componente';

@NgModule({
  declarations: [AddLicenseComponent],

  imports     : [CommonModule, Ng2DatatablesModule, NgDatepickerModule, RouterModule, FormsModule, ICheckModule, Ng2ModalModule,
                 TranslateModule, FileUploadModule, Select2Module, LayoutsModule],

  providers   : [LicenseService],
  
  exports     : []
})
 
export class HumanResourcesModule {} 