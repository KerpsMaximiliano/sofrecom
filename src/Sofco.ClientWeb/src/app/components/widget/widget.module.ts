
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WidgetCurrencyComponent } from './widget-currency.component';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  imports: [
    CommonModule, TranslateModule
  ],
  declarations: [ 
    WidgetCurrencyComponent
  ],
  providers: [
  ],
  exports: [
    WidgetCurrencyComponent
  ]
})
export class WidgetModule { }
