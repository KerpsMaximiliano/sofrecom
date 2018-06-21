import {NgModule} from "@angular/core";
import {BsDropdownModule} from 'ngx-bootstrap';

import {IboxtoolsComponent} from "./iboxtools.component";

@NgModule({
  declarations: [IboxtoolsComponent],
  imports     : [BsDropdownModule.forRoot()],
  exports     : [IboxtoolsComponent],
})

export class IboxtoolsModule {}
