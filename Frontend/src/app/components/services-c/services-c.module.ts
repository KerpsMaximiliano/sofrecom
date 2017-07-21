import { TranslateModule } from '@ngx-translate/core';
import { ServicesCRoutingModule } from './services-c.routing.module';
import { ServicesCComponent } from './services-c.component';
import { NgModule } from '@angular/core';
import { ConfigModule } from "app/singleton/config.module";

@NgModule({
  imports: [
    ConfigModule,
    ServicesCRoutingModule,
    TranslateModule
  ],
  declarations: [ 
    ServicesCComponent
  ],
  providers: [
  ]
})
export class ServicesCModule { }
