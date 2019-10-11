import { Component, OnInit, OnDestroy, Output, EventEmitter } from "@angular/core";
import { DatesService } from "app/services/common/month.service";
import { ManagementReportService } from "app/services/management-report/management-report.service";
import { MessageService } from "app/services/common/message.service";
import { Subscription } from "rxjs";
import * as FileSaver from "file-saver";
import { MenuService } from "app/services/admin/menu.service";
import { Worksheet } from "exceljs";

@Component({
    selector: 'management-report-tracing',
    templateUrl: './tracing.component.html',
    styleUrls: ['./tracing.module.scss']
})
export class TracingComponent implements OnInit, OnDestroy {

    getExcelSubscrip: Subscription;

    AllMarginTracking: any[] = new Array()
    analytic: string

    //@Output() openEvalPropModal: EventEmitter<any> = new EventEmitter();

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

    createWorksheet(workbook){
        let worksheet: Worksheet = workbook.addWorksheet('Seguimiento');

        var columns = [];
        var monthItem = { header: "Meses", width: 50 };
        columns.push(monthItem);

        var percentageExpectedTotal = ["Margen a la Fecha (%)"];
        var percentageToEnd = ["Margen a Terminacion (%)"];
        var evalprop = ["EVALPROP (%)"];

        this.AllMarginTracking.forEach(month => {
            columns.push({ header: month.display, width: 15, style: { numFmt: '#,##0.00' }, alignment: { horizontal:'center'} });

            if(month.hasReal){
                percentageExpectedTotal.push(month.PercentageExpectedTotal || 0);
                percentageToEnd.push(month.PercentageToEnd || 0);
            }
            else{
                percentageExpectedTotal.push("");
                percentageToEnd.push("");
            }

            evalprop.push(month.valueEvalProp || 0);
        });

        worksheet.columns = columns; 

        worksheet.addRow(percentageExpectedTotal);
        worksheet.addRow(percentageToEnd);
        worksheet.addRow(evalprop);

        const borderBlack = "FF000000";

        var column = worksheet.getColumn(1);
        column.eachCell(cell => {
            cell.border = { right: { style:'thin', color: { argb: borderBlack} }};
        });

        var lastColumn = worksheet.getColumn(worksheet.columnCount);
        lastColumn.eachCell(cell => {
            cell.border = { right: { style:'thin', color: { argb: borderBlack} }};
        });

        var row1 = worksheet.getRow(1);

        row1.eachCell(cell => {
            cell.style = { font: { bold: true } };
            cell.alignment = { horizontal: 'center' };
            cell.border = { 
                right: { style:'thin', color: { argb: borderBlack} },
                bottom: { style:'thin', color: { argb: borderBlack} }
            };
        });

        var lastRow = worksheet.getRow(worksheet.rowCount);
        lastRow.eachCell(cell => {
            if(cell.border && cell.border.right){
                cell.border.bottom = { style:'thin', color: { argb: borderBlack} };
            }            
            else{
                cell.border = { bottom: { style:'thin', color: { argb: borderBlack} }};
            }
        });
    }
}