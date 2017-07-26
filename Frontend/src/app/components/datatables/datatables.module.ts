import { TranslateModule } from '@ngx-translate/core';
import { RouterModule } from '@angular/router';
import { DatatablesComponent } from './datatables.component';

import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";

@NgModule({
  declarations: [DatatablesComponent],
  imports     : [CommonModule, RouterModule, TranslateModule],
  exports     : [DatatablesComponent],
})

export class DatatablesModule {}
