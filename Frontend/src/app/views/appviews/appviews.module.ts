import { CommonModule } from '@angular/common';
import {NgModule} from "@angular/core";
import {RouterModule} from "@angular/router";

import {StarterViewComponent} from "./starterview.component";
import {LoginComponent} from "./login.component";

import {PeityModule } from 'app/directives/charts/peity';
import {SparklineModule } from 'app/directives/charts/sparkline';

@NgModule({
  declarations: [
    StarterViewComponent,
    LoginComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    PeityModule,
    SparklineModule
  ],
  exports: [
    StarterViewComponent,
    LoginComponent
  ],
})

export class AppviewsModule {
}
