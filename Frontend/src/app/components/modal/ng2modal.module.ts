import { Ng2ModalConfig } from './ng2modal-config';
import { Ng2ModalComponent } from './ng2modal.component';

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TranslateModule } from "@ngx-translate/core";

@NgModule({
  imports: [
    CommonModule,
    TranslateModule
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
