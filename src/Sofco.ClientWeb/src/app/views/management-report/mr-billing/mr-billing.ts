import { Component, OnDestroy, OnInit, Input } from "@angular/core";
import { ManagementReportService } from "app/services/management-report/management-report.service";
import { Subscription } from "rxjs";

@Component({
    selector: 'management-report-billing',
    templateUrl: './mr-billing.html',
    styleUrls: ['./mr-billing.scss']
})
export class ManagementReportBillingComponent implements OnDestroy {

    getBillingSubscrip: Subscription;
    months: any[] = new Array();
    hitos: any[] = new Array();
    
    columnsCount: number = 1;

    constructor(private managementReportService: ManagementReportService){}

    ngOnDestroy(): void {
        if(this.getBillingSubscrip) this.getBillingSubscrip.unsubscribe();
    }

    init(serviceId){
        this.getBillingSubscrip = this.managementReportService.getBilling(serviceId).subscribe(response => {

            this.months = response.data.monthsHeader.map(item => {
                item.total = 0;
                return item;
            });

            this.columnsCount = this.months.length;

            response.data.rows.forEach(row => {

                var hito = { description: "", values: [] };
                hito.description = row.description;

                this.months.forEach(month => {
                    var monthValue = row.monthValues.find(x => x.month == month.month && x.year == month.year);
    
                    if(monthValue){
                        month.total += monthValue.value;
                        hito.values.push(monthValue);
                    }
                    else {
                        hito.values.push({ month: month.month, year: month.year, value: null  });
                    }
                });

                this.hitos.push(hito);
            });
        });
    }
}