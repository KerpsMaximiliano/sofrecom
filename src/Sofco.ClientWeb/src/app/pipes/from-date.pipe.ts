import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: 'fromDateFilter',
    pure: true
})

export class fromDateFilterPipe implements PipeTransform {
    transform(months: any[], fromMonth: Date): any[]{
         if(!months || !fromMonth){
             return months;
         }

        return months.filter(month => new Date(month.monthYear) >= fromMonth)
       // return months;
    }
}