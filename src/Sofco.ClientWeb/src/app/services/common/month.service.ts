import { Injectable } from "@angular/core";
import { I18nService } from './i18n.service';

@Injectable()
export class DatesService {

    constructor(private i18nService: I18nService) { }

    getMonth(date: Date) {
        var month = date.getMonth();
        var montDesc = "";
        var montShort = ""
        var year = date.getFullYear();

        switch (month) {
            case 0:
                montDesc = this.i18nService.translateByKey('months.january');
                montShort = this.i18nService.translateByKey('monthsShort.january');
                break;
            case 1:
                montDesc = this.i18nService.translateByKey('months.february');
                montShort = this.i18nService.translateByKey('monthsShort.february');
                break;
            case 2:
                montDesc = this.i18nService.translateByKey('months.march');
                montShort = this.i18nService.translateByKey('monthsShort.march');
                break;
            case 3:
                montDesc = this.i18nService.translateByKey('months.april');
                montShort = this.i18nService.translateByKey('monthsShort.april');
                break;
            case 4:
                montDesc = this.i18nService.translateByKey('months.may');
                montShort = this.i18nService.translateByKey('monthsShort.may');
                break;
            case 5:
                montDesc = this.i18nService.translateByKey('months.june');
                montShort = this.i18nService.translateByKey('monthsShort.june');
                break;
            case 6:
                montDesc = this.i18nService.translateByKey('months.july');
                montShort = this.i18nService.translateByKey('monthsShort.july');
                break;
            case 7:
                montDesc = this.i18nService.translateByKey('months.august');
                montShort = this.i18nService.translateByKey('monthsShort.august');
                break;
            case 8:
                montDesc = this.i18nService.translateByKey('months.september');
                montShort = this.i18nService.translateByKey('monthsShort.september');
                break;
            case 9:
                montDesc = this.i18nService.translateByKey('months.october');
                montShort = this.i18nService.translateByKey('monthsShort.october');
                break;
            case 10:
                montDesc = this.i18nService.translateByKey('months.november');
                montShort = this.i18nService.translateByKey('monthsShort.november');
                break;
            case 11:
                montDesc = this.i18nService.translateByKey('months.december');
                montShort = this.i18nService.translateByKey('monthsShort.december');
                break;
        }

        month += 1;
        return { month, montDesc, montShort, year };
    }
}