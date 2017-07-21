import { UsersComponent } from './users.component';
import { UsersRoutingModule } from './users.routing.module';
import { Http } from '@angular/http';
import { NgModule } from '@angular/core';
import { ConfigModule } from "app/singleton/config.module";


import { TranslateModule, TranslateLoader } from "@ngx-translate/core";
import {TranslateHttpLoader} from "@ngx-translate/http-loader";


@NgModule({
  imports: [
    ConfigModule,
    UsersRoutingModule,
    TranslateModule
  ],
  declarations: [ 
    UsersComponent
  ],
  providers: [
  ]
})
export class UsersModule { }