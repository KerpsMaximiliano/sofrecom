
import { ConfigService } from 'app/singleton/config.service';
import { HttpModule } from '@angular/http';
import { Http } from '@angular/http';

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';


@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [ 
  ],
  providers: [
    ConfigService
  ],
  exports: [
    CommonModule
  ]
})
export class ConfigModule { }