import { Directive, ElementRef, HostListener, NgModule } from '@angular/core';

@Directive({
 selector: '[decimalFormat]'
})
export class DecimalFormatDirective {
    // Allow decimal numbers and negative values
    private regex: RegExp = new RegExp(/^[0-9]+(.[0-9]{0,2})?$/g);
    // Allow key codes for special events. Reflect :
    // Backspace, tab, end, home
    private specialKeys: Array<string> = [ 'Backspace', 'Tab', 'End', 'Home', '-' ];

    constructor(private el: ElementRef) {}

    @HostListener('keydown', [ '$event' ])
    onKeyDown(event: KeyboardEvent) {
        // Allow Backspace, tab, end, and home keys
        if (this.specialKeys.indexOf(event.key) !== -1) {
            return;
        }

        if(event.key.includes('.')){
            event.preventDefault();
        }

        let current: string = this.el.nativeElement.value;
        let next: string = current.concat(event.key);

        if (next && !String(next).match(this.regex)) {
            event.preventDefault();
        }
    }
}

@NgModule({
    declarations: [
        DecimalFormatDirective
    ],
    exports: [
        DecimalFormatDirective
    ],
    imports: []
})
export class DecimalFormatModule {
}
  