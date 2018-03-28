import { Component, Input, OnInit, Output, EventEmitter, ViewChild, SimpleChanges } from '@angular/core';
import * as moment from 'moment';

declare var $: any;

@Component({
  selector: 'date-picker',
  templateUrl: './date-picker.component.html'
})
export class DatePickerComponent implements OnInit{

    @ViewChild('element') element;

    @Input() date: Date;
    @Output() dateChange = new EventEmitter<Date>();

    private callChange: boolean = false;

    constructor() {} 

    ngOnInit(): void {
        var self = this;
        $(this.element.nativeElement).on('change', function() { 
            if(this.callChange == true){
                self.setDate();
            }
            
            this.callChange = true;
        });

        if(this.date){
            this.callChange = false

            if(this.date instanceof Date){
                this.element.nativeElement.value = this.date.toLocaleDateString();
            }
            else {
                var date = moment(this.date).toDate();
                this.element.nativeElement.value = date.toLocaleDateString();
            }
        }
    }

    ngOnChanges(changes: any) {
        if(changes.date && changes.date.currentValue){
            this.element.nativeElement.value = moment(changes.date.currentValue).format('DD/MM/YYYY');
        }
    }
    
    setDate(){
        var value = this.element.nativeElement.value;

        if(!value || value.length != 10){
            this.date = null;
            this.dateChange.emit(this.date);
            return;
        }

        var split = value.split('/');

        if(split.length != 3){
            this.date = null;
            this.dateChange.emit(this.date);
            return;
        }

        var date = moment(split[2] + split[1] + split[0]).toDate();

        if(!date || !(date instanceof Date)){
            this.date = null;
            this.dateChange.emit(this.date);
            return;
        }

        this.date = date;
        this.dateChange.emit(this.date);
    }
}