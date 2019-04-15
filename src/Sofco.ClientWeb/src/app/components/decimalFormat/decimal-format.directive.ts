import { Directive, ElementRef, HostListener, NgModule, Input, OnInit } from '@angular/core';

@Directive({
 selector: '[decimalFormat]'
})
export class DecimalFormatDirective implements OnInit {
    private regex: RegExp ;
    // Allow key codes for special events. Reflect :
    // Backspace, tab, end, home
    private specialKeys: Array<string> = [ 'Tab', 'End', 'Home', '-' ];

    @Input("decimalFormat") digits: number;

    constructor(private el: ElementRef) {}

    ngOnInit(): void {
        if(this.digits && this.digits > 0){
            var expression = '^[0-9]{1,' + this.digits + '}([.][0-9][0-9])?$';

            this.regex = new RegExp(expression, 'g');
        }
        else{
            this.regex = new RegExp(/^[0-9]+(.[0-9]{0,2})?$/g);
        }
    }

    @HostListener('keydown', [ '$event' ])
    onKeyDown(event: KeyboardEvent) {
        // Allow Backspace, tab, end, and home keys
        if (this.specialKeys.indexOf(event.key) !== -1) {
            return;
        }

        if(event.key.includes('.')){
            event.preventDefault();
        }

        if(event.key == "Backspace") return;

        let current: string = this.el.nativeElement.value;
        let next: string = current.concat(event.key);

        if(!(event.key == "v" && event.ctrlKey == true)){
            if (next && !String(next).match(this.regex)) {
                event.preventDefault();
            }
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
  