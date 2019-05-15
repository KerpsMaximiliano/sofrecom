import { Injectable } from "@angular/core";
import { I18nService } from './i18n.service';

@Injectable()
export class DatesService {

    constructor(private i18nService: I18nService){}

    getMonth(date: Date){
        var month = date.getMonth();
        var montDesc = "";
        var year = date.getFullYear();

        switch(month){
            case 0: montDesc = this.i18nService.translateByKey('months.january'); break;
            case 1: montDesc = this.i18nService.translateByKey('months.february'); break;
            case 2: montDesc = this.i18nService.translateByKey('months.march'); break;
            case 3: montDesc = this.i18nService.translateByKey('months.april'); break;
            case 4: montDesc = this.i18nService.translateByKey('months.may'); break;
            case 5: montDesc = this.i18nService.translateByKey('months.june'); break;
            case 6: montDesc = this.i18nService.translateByKey('months.july'); break;
            case 7: montDesc = this.i18nService.translateByKey('months.august'); break;
            case 8: montDesc = this.i18nService.translateByKey('months.september'); break;
            case 9: montDesc = this.i18nService.translateByKey('months.october'); break;
            case 10: montDesc = this.i18nService.translateByKey('months.november'); break;
            case 11: montDesc = this.i18nService.translateByKey('months.december'); break;
        }

        month += 1;
        return { month, montDesc, year };
    }
}