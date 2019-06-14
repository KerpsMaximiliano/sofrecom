import { Component, OnInit, OnDestroy } from "@angular/core";
import { DatesService } from "app/services/common/month.service";
import { ManagementReportService } from "app/services/management-report/management-report.service";
import { MessageService } from "app/services/common/message.service";
import { Subscription } from "rxjs";
import * as FileSaver from "file-saver";

@Component({
    selector: 'management-report-tracing',
    templateUrl: './tracing.component.html',
    styleUrls: ['./tracing.module.scss']
})
export class TracingComponent implements OnInit, OnDestroy {

    getExcelSubscrip: Subscription;

    AllMarginTracking: any[] = new Array()
    analytic: string


    constructor(private datesService: DatesService,
        private managementReportService: ManagementReportService,
        private messageService: MessageService) {

    }

    ngOnInit(): void {

    }
    ngOnDestroy(): void {
        if (this.getExcelSubscrip) this.getExcelSubscrip.unsubscribe();
    }

    open(marginTracking, analytic) {

        this.analytic = analytic
        this.AllMarginTracking = marginTracking

        this.AllMarginTracking.forEach(margin => {
            var month = this.datesService.getMonth(margin.monthYear)
            margin.display = `${month.montShort} ${month.year}`
        })    
    }

    excelExport() {
        this.messageService.showLoading();

        var tracing = {
            MonthsTracking: this.AllMarginTracking,
            AnalyticName: this.analytic
        }
        this.managementReportService.ExportTracing(tracing).subscribe(
            file => {
                this.messageService.closeLoading();
                FileSaver.saveAs(file, `Seguimiento.xlsx`);
            },
            err => {
                this.messageService.closeLoading();
            });
    }

}