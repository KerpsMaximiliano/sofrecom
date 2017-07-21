import { CommonModule } from '@angular/common';
import { DashboardsRoutingModule } from './dashboards.routing.module';
import {NgModule} from "@angular/core";

import {Dashboard1Component} from "./dashboard1.component";
import {Dashboard2Component} from "./dashboard2.component";
import {Dashboard3Component} from "./dashboard3.component";
import {Dashboard4Component} from "./dashboard4.component";
import {Dashboard41Component} from "./dashboard41.component";
import {Dashboard5Component} from "./dashboard5.component";

// Chart.js Angular 2 Directive by Valor Software (npm)
import { ChartsModule } from 'ng2-charts/ng2-charts';

import { FlotModule } from 'app/directives/charts/flotChart';
import { IboxtoolsModule } from '../../components/common/iboxtools/iboxtools.module';
import { PeityModule } from 'app/directives/charts/peity';
import { SparklineModule } from 'app/directives/charts/sparkline';
import { JVectorMapModule } from 'app/directives/map/jvectorMap';


@NgModule({
  declarations: [
    Dashboard1Component,
    Dashboard2Component,
    Dashboard3Component,
    Dashboard4Component,
    Dashboard41Component,
    Dashboard5Component
  ],
  imports     : [
    CommonModule,
    ChartsModule, 
    FlotModule,
    IboxtoolsModule,
    PeityModule,
    SparklineModule,
    JVectorMapModule,
    DashboardsRoutingModule
  ],
  exports     : [
    Dashboard1Component,
    Dashboard2Component,
    Dashboard3Component,
    Dashboard4Component,
    Dashboard41Component,
    Dashboard5Component
  ],
})

export class DashboardsModule {}
