import { Http } from '@angular/http';
import { AssignmentsRoutingModule } from './assignments.routing.module';
import { AssignmentsComponent } from './assignments.component';
import { NgModule } from '@angular/core';
import { ConfigModule } from "app/singleton/config.module";


import { TranslateModule, TranslateLoader } from "@ngx-translate/core";
import {TranslateHttpLoader} from "@ngx-translate/http-loader";


// AoT requires an exported function for factories
/*export function HttpLoaderFactory(http: Http) {
    return new TranslateHttpLoader(http, "assets/i18n/", ".json");
}*/

@NgModule({
  imports: [
    ConfigModule,
    AssignmentsRoutingModule,
    TranslateModule
  ],
  declarations: [ 
    AssignmentsComponent
  ],
  providers: [
  ]
})
export class AssignmentsModule { }
