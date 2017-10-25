import { OnInit, ElementRef, NgModule, Directive } from '@angular/core';

declare var jQuery:any;

@Directive({
    selector: 'select[select2]',
    exportAs: 'select',
    host: {
        '(window:resize)': 'onResize()'
    }
})
export class Select2Directive implements OnInit {

    private element:ElementRef;

    public constructor(element:ElementRef) {
        this.element = element;
      }

    ngOnInit(): void {
       jQuery(this.element.nativeElement).select2();

       jQuery('span.select2-container').attr('style', function(i, style)
       {
           return style && style.replace(/width[^;]+;?/g, '');
       });
    }
}

@NgModule({
    declarations: [
        Select2Directive
    ],
    exports: [
        Select2Directive
    ],
    imports: []
})
export class Select2Module {
}
  