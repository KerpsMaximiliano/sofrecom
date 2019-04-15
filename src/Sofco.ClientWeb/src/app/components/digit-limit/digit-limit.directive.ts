import { Directive, ElementRef, HostListener, NgModule, Input } from '@angular/core';

@Directive({
 selector: '[digitLimit]'
})
export class DigitLimitDirective {
    private specialKeys: Array<string> = [ 'Tab', 'Backspace' ];

    @Input('digitLimit') digitLimit: number;

    constructor(private el: ElementRef) {}

    @HostListener('keydown', [ '$event' ])
    onKeyDown(event: KeyboardEvent) {
        if(this.specialKeys.indexOf(event.key) !== -1) return;

        let current: string = this.el.nativeElement.value;

        if (current.length >= this.digitLimit) {
            event.preventDefault();
        }
    }
}

@NgModule({
    declarations: [
        DigitLimitDirective
    ],
    exports: [
        DigitLimitDirective
    ],
    imports: []
})
export class DigitModule {
}
  