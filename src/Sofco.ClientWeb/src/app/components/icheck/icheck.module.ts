import { FormsModule } from '@angular/forms';
import { ICheckComponent } from './icheck.component';
import { TranslateModule } from '@ngx-translate/core';
import { RouterModule } from '@angular/router';

import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";

@NgModule({
  declarations: [ICheckComponent],
  imports     : [CommonModule, RouterModule, TranslateModule, FormsModule],
  exports     : [ICheckComponent],
})

export class ICheckModule {}
