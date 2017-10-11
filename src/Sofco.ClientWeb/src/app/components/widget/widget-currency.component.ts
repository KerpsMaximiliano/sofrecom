import { Component, Input } from '@angular/core';

@Component({
  selector: 'widget-currency',
  templateUrl: './widget-currency.component.html'
})
export class WidgetCurrencyComponent {

    @Input() style;
    @Input() label;
    @Input() value;
}