import { Ng2ModalConfig } from './ng2modal-config';
import { Ng2ModalComponent } from './ng2modal.component';

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TranslateModule } from "@ngx-translate/core";
import { LaddaModule } from 'angular2-ladda';
import { FormsModule } from '@angular/forms';

@NgModule({
  imports: [
    CommonModule,
    TranslateModule,
    FormsModule,
    LaddaModule
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
