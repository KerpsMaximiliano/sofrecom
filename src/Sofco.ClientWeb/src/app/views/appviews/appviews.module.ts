import { FormsModule } from '@angular/forms';
import {NgModule} from "@angular/core";
import {RouterModule} from "@angular/router";

import {PeityModule } from '../../components/charts/peity';
import {SparklineModule } from '../../components/charts/sparkline';
import { ForbiddenComponent } from "./errors/403/forbidden.component";
import { TranslateModule } from "@ngx-translate/core";
import { LoginComponent } from "./login/login.component";
import { StarterViewComponent } from "./home/starterview.component";
import { SpinnerModule } from '../../components/spinner/spinner.module';
import { CryptographyService } from '../../services/common/cryptography.service';
import { LaddaModule } from 'angular2-ladda';
import { UserService } from '../../services/admin/user.service';
import { CommonModule } from '@angular/common';
import { FileService } from '../../services/common/file.service';
import { PdfComponent } from './pdf/pdf.component';

@NgModule({
  declarations: [
    StarterViewComponent,
    LoginComponent,
    ForbiddenComponent,
    PdfComponent
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
    ForbiddenComponent,
    PdfComponent
  ],
  providers: [ CryptographyService, UserService, FileService ]
})

export class AppviewsModule {
}
