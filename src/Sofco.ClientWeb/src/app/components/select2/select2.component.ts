import { Component, Input, OnInit, Output, EventEmitter, ViewChild, SimpleChanges } from '@angular/core';

declare var $: any;

@Component({
  selector: 'select-two',
  templateUrl: './select2.component.html'
})
export class Select2Component implements OnInit{

    @ViewChild('element') element;

    @Input() label: string;
    @Input() value: any;
    @Input() options: any[] = new Array();
    @Output() valueChange = new EventEmitter<any>();

    private callChange: boolean = false;

    constructor() {} 

    ngOnInit(): void {
        var self = this;
        $(this.element.nativeElement).on('change', function() { 
            if(self.element.nativeElement.value && self.element.nativeElement.value != undefined){
                self.value = self.element.nativeElement.value;
                self.valueChange.emit(self.value);
            }
        });
    }

    ngOnChanges(changes: any) {
        if(changes.value && changes.value.currentValue != undefined && this.options.length > 0){
            $(this.element.nativeElement).val(changes.value.currentValue).trigger('change');
        }

        if(changes.options && changes.options.currentValue.length > 0){
            setTimeout(() => {
                $(this.element.nativeElement).val(this.value).trigger('change');
            }, 0);
        }
    }
}