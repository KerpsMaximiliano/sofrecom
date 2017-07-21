
import { ProfilesComponent } from './profiles.component';
import { ProfilesRoutingModule } from './profiles.routing.module';
import { Http } from '@angular/http';
import { NgModule } from '@angular/core';
import { ConfigModule } from "app/singleton/config.module";


import { TranslateModule, TranslateLoader } from "@ngx-translate/core";
import {TranslateHttpLoader} from "@ngx-translate/http-loader";


@NgModule({
  imports: [
    ConfigModule,
    ProfilesRoutingModule,
    TranslateModule
  ],
  declarations: [ 
    ProfilesComponent
  ],
  providers: [
  ],
  exports: [
  ]
})
export class ProfilesModule { }
