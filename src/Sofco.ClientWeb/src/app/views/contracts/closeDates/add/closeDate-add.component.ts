import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { DateRangePickerComponent } from "app/components/date-range-picker/date-range-picker.component";
import { CloseDateService } from "app/services/human-resources/closeDate.service";
import { Subscription } from "rxjs";
import { MessageService } from "../../../../services/common/message.service";
import { SettingsService } from "app/services/admin/settings.service";

declare var moment: any;

@Component({
    selector: 'add-closeDate',
    templateUrl: './closeDate-add.component.html'
})
export class AddCloseDateComponent implements OnInit, OnDestroy {

    public model: any[] = new Array();

    private startDate: Date;
    private endDate: Date;

    public closeMonthDefault: number = 0;

    @ViewChild('dateRangePicker') dateRangePicker:DateRangePickerComponent;

    private addSubscription: Subscription;
    private getSubscription: Subscription;
    private settingSubscrip: Subscription;

    constructor(private closeDateService: CloseDateService,
                private settingService: SettingsService,
                private messageService: MessageService){}

    ngOnInit(): void {
        var today = new Date();

        this.dateRangePicker.start = moment([today.getFullYear(), today.getMonth()-1]);
        this.dateRangePicker.end = moment();

        this.startDate = this.dateRangePicker.start.toDate();
        this.endDate = this.dateRangePicker.end.toDate();

        this.getModel();
    }    
    
    ngOnDestroy(): void {
        if(this.addSubscription) this.addSubscription.unsubscribe();
        if(this.getSubscription) this.getSubscription.unsubscribe();
        if(this.settingSubscrip) this.settingSubscrip.unsubscribe();
    }

    dateChange(){
        this.startDate = this.dateRangePicker.start.toDate();
        this.endDate = this.dateRangePicker.end.toDate();
        this.getModel();
    }

    getModel(){
        this.messageService.showLoading();

        this.getSubscription = this.closeDateService
                                    .get(this.startDate.getFullYear(), this.startDate.getMonth()+1, 
                                         this.endDate.getFullYear(), this.endDate.getMonth()+1)
                                    .subscribe(response => {
                                        this.messageService.closeLoading();
                                        this.model = response.data.items;
                                        this.closeMonthDefault = response.data.closeMonthValue;
                                    });
    }

    save(){
        this.messageService.showLoading();

        this.addSubscription = this.closeDateService.post(this.model).subscribe(response => {
            this.messageService.closeLoading();

            this.getModel();
        });
    }

    dateDisable(year, month){
        var today = new Date();
        
        if(year < today.getFullYear()) {
            return true;
        }
        else { 
            if(year == today.getFullYear() && month < today.getMonth()+1)
                return true;
        }

        return false;
    }

    saveCloseMonthDefault(){
        if(this.closeMonthDefault < 1 || this.closeMonthDefault > 28){
            this.messageService.showErrorByFolder('rrhh/closeDate', 'dayError');
            return;
        }

        this.messageService.showLoading();

        this.settingSubscrip = this.settingService.put({ key: "CloseMonth", value: this.closeMonthDefault }).subscribe(
          response => { 
              this.messageService.closeLoading();
              this.messageService.succes('ADMIN.settings.saveSuccess');
          },
          err => this.messageService.closeLoading());
    }
}