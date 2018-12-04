import { Component, Input, OnInit, Output, EventEmitter, ViewChild } from '@angular/core';

declare var $: any;

@Component({
  selector: 'select-two',
  templateUrl: './select2.component.html'
})
export class Select2Component implements OnInit{

    @ViewChild('element') element;

    @Input() label: string;
    @Input() value: any;
    @Input() disabled = false;
    @Input() options: any[] = new Array();
    @Input() firstOptionEmpty = true;
    @Output() valueChange = new EventEmitter<any>();

    constructor() {}

    ngOnInit(): void {
        const self = this;

        if(this.disabled){
            $(this.element.nativeElement).attr('disabled', 'disabled');
        }

        $(this.element.nativeElement).on('change', function() { 
            if(self.element.nativeElement.value && self.element.nativeElement.value != undefined){

                if(self.value != self.element.nativeElement.value){
                    self.value = self.element.nativeElement.value;
                    self.valueChange.emit(self.value);
                }
            }
        });
    }

    ngOnChanges(changes: any) {
        if(changes.value && changes.value.currentValue != undefined && this.options && this.options.length > 0){
            $(this.element.nativeElement).val(changes.value.currentValue).trigger('change');
        }

        if(changes.options && changes.options.currentValue && changes.options.currentValue.length > 0){
            setTimeout(() => {
                $(this.element.nativeElement).val(this.value).trigger('change');
            }, 0);
        }

        if(changes.disabled && changes.disabled.currentValue){
            if(changes.disabled.currentValue){
                $(this.element.nativeElement).attr('disabled', 'disabled');
            }
            else{
                $(this.element.nativeElement).removeAttr('disabled');
            }
        }
    }
}