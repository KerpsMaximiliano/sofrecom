import { Component, Input } from '@angular/core';

@Component({
  selector: 'widget-currency',
  templateUrl: './widget-currency.component.html',
  styleUrls: ['./widget-currency.component.scss']
})
export class WidgetCurrencyComponent {

    @Input() style;
    @Input() label;
    @Input() incomes;
}