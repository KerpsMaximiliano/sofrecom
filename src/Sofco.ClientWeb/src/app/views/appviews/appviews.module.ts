import { FormsModule } from '@angular/forms';
import {NgModule} from "@angular/core";
import {RouterModule} from "@angular/router";

import {PeityModule } from '../../components/charts/peity';
import {SparklineModule } from '../../components/charts/sparkline';
import { ForbiddenComponent } from "app/views/appviews/errors/403/forbidden.component";
import { TranslateModule } from "@ngx-translate/core";
import { LoginComponent } from "app/views/appviews/login/login.component";
import { StarterViewComponent } from "app/views/appviews/home/starterview.component";
import { SpinnerModule } from 'app/components/spinner/spinner.module';
import { CryptographyService } from 'app/services/common/cryptography.service';
import { LaddaModule } from 'angular2-ladda';
import { UserService } from 'app/services/admin/user.service';
import { CommonModule } from '@angular/common';

@NgModule({
  declarations: [
    StarterViewComponent,
    LoginComponent,
    ForbiddenComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    PeityModule, 
    SparklineModule,
    FormsModule,
    TranslateModule,
    SpinnerModule,
    LaddaModule
  ],
  exports: [
    StarterViewComponent,
    LoginComponent,
    ForbiddenComponent
  ],
  providers: [ CryptographyService, UserService ]
})

export class AppviewsModule {
}
