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

import { SpinnerModule } from "app/components/spinner/spinner.module";
import { WidgetModule } from 'app/components/widget/widget.module';

import { SolfacReportComponent } from './solfac/solfac.component';

@NgModule({
declarations: [
    SolfacReportComponent
],

imports: [
    CommonModule, Ng2DatatablesModule, NgDatepickerModule, RouterModule, FormsModule, ICheckModule, Ng2ModalModule,
    TranslateModule, SpinnerModule, FileUploadModule, WidgetModule
],
  
providers: [
],

exports: []
})
  
export class ReportModule {}