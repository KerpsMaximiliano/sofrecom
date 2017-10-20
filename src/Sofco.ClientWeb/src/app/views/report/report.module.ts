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
import { Daterangepicker } from 'ng2-daterangepicker';

import { SpinnerModule } from "app/components/spinner/spinner.module";
import { WidgetModule } from 'app/components/widget/widget.module';
import { DateRangePickerModule } from 'app/components/datepicker/date-range.picker.module'
import { ChartsModule } from 'ng2-charts/ng2-charts';

import { SolfacReportComponent } from './solfac/solfac.component';
import { SolfacChartComponent } from './solfac/solfac-chart.component';

import { SolfacReportService } from 'app/services/report/solfacReport.service';
import { IboxtoolsModule } from 'app/components/common/iboxtools/iboxtools.module';

@NgModule({
declarations: [
    SolfacReportComponent, SolfacChartComponent
],

imports: [
    CommonModule, Ng2DatatablesModule, NgDatepickerModule, RouterModule, FormsModule, ICheckModule, Ng2ModalModule,
    TranslateModule, SpinnerModule, FileUploadModule, WidgetModule, ChartsModule,
    DateRangePickerModule, IboxtoolsModule
],
  
providers: [ SolfacReportService ],

exports: []
})
  
export class ReportModule {}