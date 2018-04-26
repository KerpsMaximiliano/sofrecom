import { OnInit, ElementRef, NgModule, Directive } from '@angular/core';
import { Select2Component } from 'app/components/select2/select2.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';

declare var jQuery:any;

@Directive({
    selector: 'select[select2]',
    exportAs: 'select'
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
        Select2Directive, Select2Component
    ],
    exports: [
        Select2Directive, Select2Component
    ],
    imports: [ CommonModule, FormsModule, TranslateModule ]
})
export class Select2Module {
}
  