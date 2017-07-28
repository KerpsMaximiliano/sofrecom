import { Ng2ModalConfig } from './ng2modal-config';
import { Ng2ModalComponent } from './ng2modal.component';

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

@NgModule({
  imports: [
    CommonModule,
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
