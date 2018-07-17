import { PCheckComponent } from './pcheck.component';

import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [ PCheckComponent ],
  imports     : [CommonModule, FormsModule],
  exports     : [ PCheckComponent ],
})

export class PCheckModule {}
