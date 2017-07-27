import { TranslateModule } from '@ngx-translate/core';
import { RouterModule } from '@angular/router';
import { Ng2DatatablesComponent } from './ng2-datatables.component';

import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";

@NgModule({
  declarations: [Ng2DatatablesComponent],
  imports     : [CommonModule, RouterModule, TranslateModule],
  exports     : [Ng2DatatablesComponent],
})

export class Ng2DatatablesModule {}
