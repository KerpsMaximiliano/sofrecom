import { FormsModule } from '@angular/forms';
import {NgModule} from "@angular/core";
import {BrowserModule} from "@angular/platform-browser";
import {RouterModule} from "@angular/router";

import {PeityModule } from '../../components/charts/peity';
import {SparklineModule } from '../../components/charts/sparkline';
import { ForbiddenComponent } from "app/views/appviews/errors/403/forbidden.component";
import { TranslateModule } from "@ngx-translate/core";
import { LoginComponent } from "app/views/appviews/login/login.component";
import { StarterViewComponent } from "app/views/appviews/home/starterview.component";

@NgModule({
  declarations: [
    StarterViewComponent,
    LoginComponent,
    ForbiddenComponent
  ],
  imports: [
    BrowserModule,
    RouterModule,
    PeityModule,
    SparklineModule,
    FormsModule,
    TranslateModule
  ],
  exports: [
    StarterViewComponent,
    LoginComponent,
    ForbiddenComponent
  ],
})

export class AppviewsModule {
}
