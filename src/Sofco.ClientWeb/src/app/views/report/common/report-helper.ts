import * as moment from 'moment';

export class ReportHelper {

    public static DateMonthIntervalToLabels(dateSince: Date, dateTo: Date): Array<string>
    {
        var months;
        months = (dateTo.getFullYear() - dateSince.getFullYear()) * 12;
        months -= dateSince.getMonth() + 1;
        months += dateTo.getMonth();
        var monthDiff = months <= 0 ? 0 : months;

        let labels: string[] = [];

        var current = moment(dateSince);
        
        for(let i = 0; i <= monthDiff; i++){
            labels.push(current.add(1, 'month').format("MM/YYYY"));
        }

        return labels;
    }

    public static DateIntervalToLabels(dateSince: Date, dateTo: Date): Array<string>
    {
        let second=1000, minute=second*60, hour=minute*60, day=hour*24, week=day*7;

        let timeDiff = Math.abs(dateSince.getTime()-dateTo.getTime());

        let daysDiff = Math.ceil(timeDiff / day);

        let labels: string[] = [];

        let currentDate: Date = new Date();

        for(let i = 0; i <= daysDiff; i++){
            var dateLabel = currentDate.setTime(dateSince.getTime());

            currentDate.setTime(currentDate.getTime() + day * i);

            labels.push(currentDate.toLocaleDateString());
        }

        return labels;
    }

    public static DateToLabel(dateText:string):string
    {
        let date:Date = new Date(dateText);

        return date.toLocaleDateString();
    }

    public static DateMonthToLabel(dateText:string):string
    {
        let date:Date = new Date(dateText);

        return moment(date).format('MM/YYYY');
    }
}