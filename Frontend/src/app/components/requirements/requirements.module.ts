import { TranslateModule } from '@ngx-translate/core';
import { RequirementsRoutingModule } from './requirements.routing.module';
import { RequirementsComponent } from './requirements.component';
import { NgModule } from '@angular/core';
import { ConfigModule } from "app/singleton/config.module";

@NgModule({
  imports: [
    ConfigModule,
    RequirementsRoutingModule,
    TranslateModule
  ],
  declarations: [ 
    RequirementsComponent
  ],
  providers: [
  ]
})
export class RequirementsModule { }
