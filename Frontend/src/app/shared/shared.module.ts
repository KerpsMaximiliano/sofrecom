import { Ng2ModalModule } from './modal/ng2modal.module';
import { Ng2ModalConfig } from 'app/shared/modal/ng2modal-config';
import { Ng2ModalComponent } from './modal/ng2modal.component';

import { HttpModule } from '@angular/http';
import { Http } from '@angular/http';

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';


@NgModule({
  imports: [
    CommonModule,
    Ng2ModalModule
  ],
  declarations: [ 
    //Ng2ModalComponent
  ],
  providers: [
  ],
  exports: [
    CommonModule,
    Ng2ModalModule
    //Ng2ModalComponent
  ]
})
export class SharedModule { }
