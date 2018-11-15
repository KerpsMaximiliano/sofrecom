import { HttpClientModule, HTTP_INTERCEPTORS, HttpClient } from '@angular/common/http';
import { AuthGuard } from './guards/auth.guard';
import { Service } from './services/common/service';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule, APP_INITIALIZER, LOCALE_ID } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { LocationStrategy, HashLocationStrategy } from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { registerLocaleData } from '@angular/common';
import localeEs from '@angular/common/locales/es-AR';

import { appRouter } from "./app.routes";
import { AppComponent } from './app.component';

// App views
import { AppviewsModule } from "./views/appviews/appviews.module";
import { ChartsModule } from 'ng2-charts/ng2-charts';

// App modules/components
import { LayoutsModule } from "./components/common/layouts/layouts.module";

import { TranslateModule, TranslateLoader } from "@ngx-translate/core";
import { TranslateHttpLoader } from "@ngx-translate/http-loader";
import { ErrorHandlerService } from "./services/common/errorHandler.service";
import { AuthenticationService } from "./services/common/authentication.service";
import { Configuration } from "./services/common/configuration";
import { MessageService } from "./services/common/message.service";
import { MenuService } from "./services/admin/menu.service";
import { DataTableService } from "./services/common/datatable.service";
import { I18nService } from './services/common/i18n.service';
import { AppSettingService } from './services/common/app-setting.service';
import { AppSetting } from './services/common/app-setting';
import { CryptographyService } from './services/common/cryptography.service';

import { LaddaModule } from 'angular2-ladda';
import { RequestInterceptorService } from './services/common/request-interceptor.service';
import { AuthService } from './services/common/auth.service';
import { SettingsService } from './services/admin/settings.service';
import { ToastrModule } from 'ngx-toastr';
import { HttpServiceInterceptor } from 'app/services/common/httpServiceInterceptor';

import { DatepickerModule, BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { esLocale } from 'ngx-bootstrap/locale';
import { defineLocale, TooltipModule } from 'ngx-bootstrap';
import { NgSelectModule } from '@ng-select/ng-select';

defineLocale('es', esLocale);

export function HttpLoaderFactory(http: HttpClient) {
    return new TranslateHttpLoader(http, "assets/i18n/", ".json");
}

registerLocaleData(localeEs, 'es');

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
    BsDatepickerModule.forRoot(),
    DatepickerModule.forRoot(),
    TooltipModule.forRoot(),
    NgSelectModule,
    ToastrModule.forRoot(),
    appRouter,
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
        deps: [HttpClient]
      }
    }),
    HttpClientModule,
  ],
  providers: [
    { provide: LocationStrategy, useClass: HashLocationStrategy },
    Configuration,
    Service,
    MenuService,
    DataTableService,
    I18nService,
    ErrorHandlerService,
    MessageService,
    AuthGuard,
    AuthenticationService,
    SettingsService,
    AppSetting,
    AppSettingService,
    {
      provide: APP_INITIALIZER,
      useFactory: appSettingFactory,
      deps: [AppSettingService],
      multi: true
    },
    CryptographyService,
    { provide: HTTP_INTERCEPTORS, useClass: RequestInterceptorService, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: HttpServiceInterceptor, multi: true },
    AuthService,
    { provide: LOCALE_ID, useValue: "es-Ar" }
  ],
  bootstrap: [AppComponent]
})

export class AppModule { }

export function appSettingFactory(service: AppSettingService) {
  return () => { return service.load(); };
}
