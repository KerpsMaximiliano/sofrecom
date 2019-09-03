import { Component, OnInit, OnDestroy, Output, EventEmitter } from "@angular/core";
import { DatesService } from "app/services/common/month.service";
import { ManagementReportService } from "app/services/management-report/management-report.service";
import { MessageService } from "app/services/common/message.service";
import { Subscription } from "rxjs";
import * as FileSaver from "file-saver";
import { MenuService } from "app/services/admin/menu.service";

@Component({
    selector: 'management-report-tracing',
    templateUrl: './tracing.component.html',
    styleUrls: ['./tracing.module.scss']
})
export class TracingComponent implements OnInit, OnDestroy {

    getExcelSubscrip: Subscription;

    AllMarginTracking: any[] = new Array()
    analytic: string

    @Output() openEvalPropModal: EventEmitter<any> = new EventEmitter();

    constructor(private datesService: DatesService,
        private managementReportService: ManagementReportService,
        private messageService: MessageService,
        private menuService: MenuService) {

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

    canEditCdg() {
        return this.menuService.userIsCdg;
    }

    openEditEvalProp(month) {

        if (month.closed) return;

        if (this.openEvalPropModal.observers.length > 0) {
            month.type = 3;
            month.icon = "%";
            this.openEvalPropModal.emit(month);
        }
    }

}