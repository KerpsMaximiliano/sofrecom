import { Ng2ModalComponent } from './ng2modal.component';

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TranslateModule } from "@ngx-translate/core";
import { LaddaModule } from 'angular2-ladda';
import { FormsModule } from '@angular/forms';
import { ButtonsModule } from '../buttons/buttons.module';

@NgModule({
  imports: [
    CommonModule,
    TranslateModule,
    FormsModule,
    LaddaModule,
    ButtonsModule
  ],
  declarations: [
      Ng2ModalComponent
  ],
  providers: [
  ],
  exports: [
      Ng2ModalComponent
  ]
})
export class Ng2ModalModule { }
