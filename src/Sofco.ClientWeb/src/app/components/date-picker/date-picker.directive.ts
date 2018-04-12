import { OnInit, ElementRef, NgModule, Directive } from '@angular/core';

declare var jQuery:any;

@Directive({
    selector: 'input[datepicker]',
    exportAs: 'datepicker',
    host: {
        '(window:resize)': 'onResize()'
    }
})
export class DatepickerDirective implements OnInit {

    public element:ElementRef;

    public constructor(element:ElementRef) {
        this.element = element;
      }

    ngOnInit(): void {
       jQuery(this.element.nativeElement).datepicker({
        format: 'dd/mm/yyyy',
        autoclose: true,
        todayBtn: 'linked'
      });
    }
}

@NgModule({
    declarations: [
        DatepickerDirective
    ],
    exports: [
        DatepickerDirective
    ],
    imports: []
})
export class DatepickerDirectiveModule {
}
  