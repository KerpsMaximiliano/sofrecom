import { Component, Input, OnInit, Output, EventEmitter, ViewChild, SimpleChanges } from '@angular/core';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import * as moment from 'moment';

@Component({
  selector: 'date-picker',
  templateUrl: './date-picker.component.html'
})
export class DatePickerComponent implements OnInit {

    @ViewChild('element') element;

    public model: Date;

    @Input() date: Date;
    @Output() dateChange = new EventEmitter<Date>();

    bsConfig: Partial<BsDatepickerConfig>;

    constructor(private localeService: BsLocaleService){}

    ngOnInit(): void {
        this.localeService.use('es');
        this.bsConfig = Object.assign({}, { containerClass: 'theme-dark-blue', showWeekNumbers: false });
    }

    onValueChange(event){
        this.date = event;
        this.dateChange.emit(this.date);
    }

    ngOnChanges(changes: any) {
        if(changes.date){
            if(changes.date.currentValue){
                this.element.nativeElement.value = moment(changes.date.currentValue).format('DD/MM/YYYY');
            }
            else{
                this.element.nativeElement.value = null;
            }
        }
    }
}