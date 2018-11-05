import { NgModule } from '@angular/core';

import { TranslateModule } from "@ngx-translate/core";
import { ApproversComponent } from './approvers.component';
import { PCheckModule } from '../../../components/pcheck/pcheck.module';
import { UserApproverService } from '../../../services/allocation-management/user-approver.service';
import { AnalyticService } from '../../../services/allocation-management/analytic.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { PeityModule } from '../../../components/charts/peity';
import { FormsModule } from '@angular/forms';
import { SpinnerModule } from '../../../components/spinner/spinner.module';
import { Select2Module } from '../../../components/select2/select2';
import { Ng2ModalModule } from '../../../components/modal/ng2modal.module';
import { NgSelectModule } from '@ng-select/ng-select';

@NgModule({
declarations: [
    ApproversComponent
],

imports: [
    CommonModule, RouterModule, PeityModule, FormsModule, SpinnerModule, Select2Module,
    TranslateModule, Ng2ModalModule, PCheckModule, NgSelectModule
],

providers: [ UserApproverService, AnalyticService],

exports: [ ApproversComponent ]
})

export class ApproversModule {}