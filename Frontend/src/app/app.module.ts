
import { SharedModule } from './shared/shared.module';
import { CustomersService } from './services/customers.service';
import { MenuComponent } from './layout/menu/menu.component';

import { ConfigModule } from 'app/singleton/config.module';

import { AppRoutingModule } from './app.routing';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule, Http } from '@angular/http';
import {BsDropdownModule} from 'ngx-bootstrap';

import { AppComponent } from './app.component';
import { DashboardComponent } from './layout/dashboard/dashboard.component';


import { TranslateModule, TranslateLoader } from "@ngx-translate/core";
import {TranslateHttpLoader} from "@ngx-translate/http-loader";
import { CustomersComponent } from './components/customers/customers.component';
import { Service } from "app/services/service";
import { Configuration } from "app/services/configuration";
import { BasicLayoutComponent } from "app/components/common/layouts/basicLayout.component";
import { LayoutsModule } from "app/components/common/layouts/layouts.module";


// AoT requires an exported function for factories
export function HttpLoaderFactory(http: Http) {
    return new TranslateHttpLoader(http, "assets/i18n/", ".json");
}


@NgModule({
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    HttpModule,
    AppRoutingModule,
    SharedModule,
    LayoutsModule,
    //LayoutModule,
    ConfigModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [Http]
      }
    })
  ],
  declarations: [
    AppComponent,
    MenuComponent,
    DashboardComponent,
    CustomersComponent
  ],
  providers: [
    Service,
    Configuration,
    CustomersService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
