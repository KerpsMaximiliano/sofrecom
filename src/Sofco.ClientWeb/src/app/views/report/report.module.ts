import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Ng2DatatablesModule } from '../../components/datatables/ng2-datatables.module';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ICheckModule } from "../../components/icheck/icheck.module";
import { Ng2ModalModule } from "../../components/modal/ng2modal.module";
import { TranslateModule } from "@ngx-translate/core";
import { FileUploadModule } from 'ng2-file-upload';
import { NgDatepickerModule } from 'ng2-datepicker';

import { SpinnerModule } from "../../components/spinner/spinner.module";
import { WidgetModule } from '../../components/widget/widget.module';
import { ChartsModule } from 'ng2-charts/ng2-charts';

import { SolfacReportComponent } from './solfac/solfac.component';
import { SolfacChartComponent } from './solfac/solfac-chart.component';

import { SolfacReportService } from '../../services/report/solfacReport.service';
import { IboxtoolsModule } from '../../components/common/iboxtools/iboxtools.module';
import { DateRangePickerModule } from '../../components/date-range-picker/date-range.picker.module';
import { DatePickerModule } from '../../components/date-picker/date-picker.module';

@NgModule({
declarations: [
    SolfacReportComponent, SolfacChartComponent
],

imports: [
    CommonModule, Ng2DatatablesModule, NgDatepickerModule, RouterModule, FormsModule, ICheckModule, Ng2ModalModule,
    TranslateModule, SpinnerModule, FileUploadModule, WidgetModule, ChartsModule,
    DateRangePickerModule, IboxtoolsModule, DatePickerModule
],
  
providers: [ SolfacReportService ],

exports: []
})
  
export class ReportModule {}