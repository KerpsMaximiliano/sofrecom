import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { DateRangePickerComponent } from "app/components/date-range-picker/date-range-picker.component";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { CurrencyExchangeService } from "app/services/management-report/currency-exchange.service";

declare var moment: any;

@Component({
    selector: 'currency-exchange',
    templateUrl: './currency-exchange.html'
})
export class CurrencyExchangeComponent implements OnInit, OnDestroy {

    public dates: any[] = new Array();

    private startDate: Date;
    private endDate: Date;

    @ViewChild('dateRangePicker') dateRangePicker:DateRangePickerComponent;

    private addSubscription: Subscription;
    private getSubscription: Subscription;

    constructor(private currencyExchangeService: CurrencyExchangeService,
                private messageService: MessageService){}

    ngOnInit(): void {
        var today = new Date();

        this.dateRangePicker.start = moment([today.getFullYear(), today.getMonth()]);
        this.dateRangePicker.end = moment();

        this.startDate = this.dateRangePicker.start.toDate();
        this.endDate = this.dateRangePicker.end.toDate();

        this.getModel();
    }    
    
    ngOnDestroy(): void {
        if(this.addSubscription) this.addSubscription.unsubscribe();
        if(this.getSubscription) this.getSubscription.unsubscribe();
    }

    dateChange(){
        this.startDate = this.dateRangePicker.start.toDate();
        this.endDate = this.dateRangePicker.end.toDate();
        this.getModel();
    }

    getModel(){
        this.messageService.showLoading();

        this.getSubscription = this.currencyExchangeService
                                    .get(this.startDate.getFullYear(), this.startDate.getMonth()+1, 
                                         this.endDate.getFullYear(), this.endDate.getMonth()+1)
                                    .subscribe(response => {
                                        this.messageService.closeLoading();
                                        this.dates = response.data;
                                    },
                                    error => this.messageService.closeLoading());
    }

    save(currencyExchange, date){
        if(currencyExchange.id && currencyExchange.id > 0){
            this.put(currencyExchange);
        }
        else{
            this.post(currencyExchange, date);
        }
    }

    post(currencyExchange, date){
        var json = {
            month: date.month,
            year: date.year,
            currencyId: currencyExchange.currencyId,
            exchange: currencyExchange.exchange
        };

        this.messageService.showLoading();

        this.addSubscription = this.currencyExchangeService.post(json).subscribe(response => {
            this.messageService.closeLoading();

            this.getModel();
        }, 
        error => this.messageService.closeLoading());
    }

    put(currencyExchange){
        var json = { exchange: currencyExchange.exchange };

        this.messageService.showLoading();

        this.addSubscription = this.currencyExchangeService.put(currencyExchange.id, json).subscribe(response => {
            this.messageService.closeLoading();

            this.getModel();
        }, 
        error => this.messageService.closeLoading());
    }

    dateDisable(year, month){
        var today = new Date();
        
        if(year < today.getFullYear()) {
            return true;
        }
        else { 
            if(year == today.getFullYear() && month < today.getMonth())
                return true;
        }

        return false;
    }
}