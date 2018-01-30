import { BillingModule } from './views/billing/billing.module';
import { ReportModule } from './views/report/report.module';
import { AuthGuard } from './guards/auth.guard';
import { Service } from 'app/services/common/service';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule, APP_INITIALIZER } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { XHRBackend, Http, HttpModule, RequestOptions, ConnectionBackend } from '@angular/http';
import { RouterModule } from "@angular/router";
import { LocationStrategy, HashLocationStrategy } from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { ROUTES } from "./app.routes";
import { AppComponent } from './app.component';

// App views
import { AppviewsModule } from "./views/appviews/appviews.module";
import { ChartsModule } from 'ng2-charts/ng2-charts';

// App modules/components
import { LayoutsModule } from "./components/common/layouts/layouts.module";
import { AdminModule } from "app/views/admin/admin.module";
import { ToastrModule } from 'toastr-ng2';

import { TranslateModule, TranslateLoader } from "@ngx-translate/core";
import { TranslateHttpLoader } from "@ngx-translate/http-loader";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { AuthenticationService } from "app/services/common/authentication.service";
import { Configuration } from "app/services/common/configuration";
import { MessageService } from "app/services/common/message.service";
import { MenuService } from "app/services/admin/menu.service";
import { DataTableService } from "app/services/common/datatable.service";
import { I18nService } from 'app/services/common/i18n.service';
import { AppSettingService } from 'app/services/common/app-setting.service';
import { AppSetting } from 'app/services/common/app-setting';
import { CryptographyService } from 'app/services/common/cryptography.service';

import { HttpAuth } from "app/services/common/http-auth";
import { AllocationManagementModule } from 'app/views/allocation-management/allocation-management.module';
import { LaddaModule } from 'angular2-ladda';

export function HttpLoaderFactory(http: Http) {
    return new TranslateHttpLoader(http, "assets/i18n/", ".json");
}

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    HttpModule,
    LayoutsModule,
    AppviewsModule,
    ToastrModule.forRoot(),
    RouterModule.forRoot(ROUTES),
    AdminModule,
    BillingModule,
    ChartsModule,
    LaddaModule.forRoot({
      style: "zoom-in",
      spinnerSize: 30,
      spinnerColor: "white",
      spinnerLines: 12
    }),
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [Http]
      }
    }),
    ReportModule,
    AllocationManagementModule
  ],
  providers: [
    {provide: LocationStrategy, useClass: HashLocationStrategy},
    Configuration,
    Service,
    MenuService,
    DataTableService,
    I18nService,
    ErrorHandlerService,
    MessageService,
    AuthGuard,
    AuthenticationService,
    {
       provide: HttpAuth,
       useFactory: getHttpAuth,
       deps: [XHRBackend, RequestOptions, Service]
    },
    AppSetting,
    AppSettingService,
    {
      provide: APP_INITIALIZER,
      useFactory: appSettingFactory,
      deps: [AppSettingService], 
      multi: true
    },
    CryptographyService
  ],
  bootstrap: [AppComponent]
})

export class AppModule { }

export function getHttpAuth(backend: ConnectionBackend, defaultOptions: RequestOptions, service: Service) {
  return new HttpAuth(backend, defaultOptions, service);
}

export function appSettingFactory(service: AppSettingService) {
  return () => { return service.load() };
}
