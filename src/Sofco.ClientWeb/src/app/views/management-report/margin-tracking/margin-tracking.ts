import { Component, OnDestroy, OnInit, ViewChild, Input, Output, EventEmitter } from "@angular/core";
import { MenuService } from "app/services/admin/menu.service";
import { ManagementReportDetailComponent } from "../detail/mr-detail"
import * as moment from 'moment';
import { MarginTracking } from "app/models/management-report/marginTracking";
import { HitoStatus } from "app/models/enums/hitoStatus";
import { AnalyticStatus } from "app/models/enums/analyticStatus";
import { Worksheet } from "exceljs";
import { ManagementReportService } from "app/services/management-report/management-report.service";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";

@Component({
    selector: 'margin-tracking',
    templateUrl: './margin-tracking.html',
    styleUrls: ['./margin-tracking.scss']
})
export class MarginTrackingComponent implements OnInit, OnDestroy {

    isManager: boolean = false;
    isCdgOrDirector: boolean = false;

    billingDataLoaded: boolean = false;
    costDataLoaded: boolean = false;

    acumulatedCostsEditabled: boolean = false;
    acumulatedSalesEditabled: boolean = false;

    acumulatedSales: string;
    acumulatedCosts: string;
    period: string;

    private billingModel: any;
    private costsModel: any;

    public model: any;

    public marginTrackingSelected: MarginTracking = new MarginTracking();
    private allMarginTrackings: any[] = new Array();

    @Output() getData: EventEmitter<any> = new EventEmitter();

    updateAcumulatedSuscrip: Subscription;

    constructor(private menuService: MenuService,
        private messageService: MessageService,
        private managementReportService: ManagementReportService,
        private managementReportDetail: ManagementReportDetailComponent
    ) { }

    ngOnInit(): void {
        this.isManager = this.menuService.userIsManager;
        this.isCdgOrDirector = this.menuService.userIsDirector || this.menuService.userIsCdg;
    }

    ngOnDestroy(): void {
        if(this.updateAcumulatedSuscrip) this.updateAcumulatedSuscrip.unsubscribe();
    }

    isReadOnly() {
        this.model.analyticStatus == AnalyticStatus.Close;
    }

    updateAcumulated(){
        var json = {
            costs: this.acumulatedCosts,
            sales: this.acumulatedSales,
            period: this.period
        }

        this.messageService.showLoading();

        this.updateAcumulatedSuscrip = this.managementReportService.updateAcumulated(this.model.managementReportId, json).subscribe(() => {
            this.messageService.closeLoading();    
        },
        () => this.messageService.closeLoading());
    }

    setMarginTracking(month, year) {
        var marginTracking = this.allMarginTrackings.find(x => x.Month == month && x.Year == year);

        if (marginTracking && marginTracking != null) {
            this.marginTrackingSelected = marginTracking;
        }
        else {
            this.marginTrackingSelected = new MarginTracking();
        }
    }

    calculate(manamementReportStartDate, manamementReportEndDate, selectedMonth, selectedYear) {

        if (!this.billingDataLoaded || !this.costDataLoaded) return;

        this.allMarginTrackings = [];

        var startDate = moment(manamementReportStartDate).toDate();
        var endDate = moment(manamementReportEndDate).toDate();

        startDate.setDate(1);
        endDate.setDate(1);

        var billingAcumulatedToDate = 0;
        var costsAcumulatedToDate = parseFloat(this.acumulatedCosts);
        var budgetAcumulatedToDate = 0;
        var billingRealAcumulatedToDate = parseFloat(this.acumulatedSales);        
        var totalAcumulatedToEnd = parseFloat(this.acumulatedSales);
        var totalCostsAcumulatedToEnd = parseFloat(this.acumulatedCosts);
        var evalpropTotalBilling = 0;
        var evalpropTotalCosts = 0;
        var evalpropTotalTracing = 0;
        var cantMonths = 0;
        var billingAcumulateProyected = 0
        var budgetRemaint = 0;
        var billingRemaint = 0;

        for (var initDate = startDate; initDate <= endDate; initDate.setMonth(initDate.getMonth() + 1)) {
            var marginTracking = new MarginTracking();
            var billingRealMonth = 0

            cantMonths += 1

            var month = initDate.getMonth() + 1;
            var year = initDate.getFullYear();

            marginTracking.Month = month;
            marginTracking.Year = year;
            marginTracking.monthYear = new Date(year, month - 1, 1)

            var billingMonth = this.billingModel.months.find(x => x.month == month && x.year == year);
            var costDetailMonth = this.costsModel.months.find(x => x.month == month && x.year == year);
           
            if (billingMonth) {
                evalpropTotalBilling += billingMonth.valueEvalProp;

                // Real del mes $$ (ventas)
                //marginTracking.SalesOnMonth += billingMonth.totalBilling;

                // billingAcumulatedToDate += billingMonth.totalBilling;
            
                //totalAcumulatedToEnd += billingMonth.totalBilling;
            }
            if (costDetailMonth) {
                evalpropTotalCosts += costDetailMonth.valueEvalProp;
            }

            //this.calculatePercentageExpected(billingMonth, costDetailMonth, marginTracking);

            var hitosMonth = this.billingModel.hitos.filter(hito => {
                var hitoDate = moment(hito.date).toDate();

                if (hitoDate.getFullYear() == initDate.getFullYear() && hitoDate.getMonth() == initDate.getMonth()) {
                    return hito;
                }
            });

            if (hitosMonth && hitosMonth.length > 0) {
                hitosMonth.forEach(element => {

                    element.values.forEach(valueMonth => {
                        if (valueMonth.month == month && valueMonth.year == year && valueMonth.valuePesos > 0) {
                            if (valueMonth.status == HitoStatus.Billed || valueMonth.status == HitoStatus.Cashed) {
                                billingAcumulatedToDate += valueMonth.valuePesos;

                                // // Real del mes $$ (ventas)
                                // marginTracking.SalesOnMonth += valueMonth.valuePesos;
                                billingRealMonth += valueMonth.valuePesos
                                totalAcumulatedToEnd += valueMonth.valuePesos;
                            }
                            else{
                                billingAcumulateProyected += valueMonth.valuePesos;
                            }

                            // Previsto $$ (ventas)
                            marginTracking.ExpectedSales += valueMonth.valuePesos;
                        }
                    });
                });
            }

            // // Acumulado a la fecha $$ (ventas)
            // marginTracking.SalesAccumulatedToDate += totalAcumulatedToEnd;

            if (costDetailMonth) {
                
                marginTracking.hasReal = costDetailMonth.hasReal
               // marginTracking.valueEvalProp = costDetailMonth.valueMarginEvalProp
                marginTracking.billingMonthId = costDetailMonth.billingMonthId
                
                // Previsto $$
                marginTracking.TotalExpensesExpected = costDetailMonth.budget.totalCost;
                budgetAcumulatedToDate += costDetailMonth.budget.totalCost;
                
                // Real al mes $$ (costos)
                marginTracking.TotalExpensesOnMonth = costDetailMonth.real.totalCost;
                costsAcumulatedToDate += costDetailMonth.real.totalCost;

                if(costDetailMonth.real.totalCost == null || costDetailMonth.real.totalCost == 0){
                    budgetRemaint += costDetailMonth.budget.totalCost;
                }
                
                // Real del mes $$ (ventas)
                if(costDetailMonth.totalBilling && costDetailMonth.totalBilling != null){
                marginTracking.SalesOnMonth = costDetailMonth.totalBilling;
                billingRealAcumulatedToDate += costDetailMonth.totalBilling;
                }
                else{
                    marginTracking.SalesOnMonth = billingRealMonth
                    billingRealAcumulatedToDate += billingRealMonth
                }

                if( marginTracking.SalesOnMonth == null ||  marginTracking.SalesOnMonth == 0){
                    billingRemaint +=  marginTracking.SalesOnMonth;
                }

                totalCostsAcumulatedToEnd += costDetailMonth.real.totalCost;
                
            }
            
            marginTracking.valueEvalProp = 0;
            if(billingMonth && costDetailMonth){
                if(billingMonth.valueEvalProp > 0)
                {
                    //Evalprop (((FACTURACION-COSTO)/FACTURACION)*100)
                    marginTracking.valueEvalProp = ((billingMonth.valueEvalProp - costDetailMonth.valueEvalProp) / billingMonth.valueEvalProp) * 100;
                }
            }
            
            // Previsto % (ventas)
           // if (marginTracking.valueEvalProp > 0) {
                marginTracking.PercentageExpected = marginTracking.valueEvalProp
                evalpropTotalTracing += marginTracking.valueEvalProp
            //}

            //Acumulado a la fecha de proyectado $$ (Vemtas -> hitos no facturados)
           //marginTracking.SalesRemainigToDate = billingAcumulateProyected;
            
           // Acumulado a la fecha $$ (ventas)
            marginTracking.SalesAccumulatedToDate = billingRealAcumulatedToDate
            
            //Acumulado a la fecha de proyectado + Real $$ (Ventas - hitos no facturados + Real Facturacion)
            marginTracking.TotalSalesToEnd = billingAcumulateProyected + billingRealAcumulatedToDate
           
            // Acumulado a la fecha real $$ (costos)
            marginTracking.TotalExpensesAccumulatedToDate = costsAcumulatedToDate;
            
            //Acumulado a la fecha de previsto $$ (costos)
            //marginTracking.TotalExpensesRemainigToDate = budgetAcumulatedToDate
 
            //Acumulado a la fecha de proyectado + Real $$ (costos previstos + costos real)
            marginTracking.TotalExpensesToEnd = costsAcumulatedToDate + budgetAcumulatedToDate

            // Real a la fecha % (costos)
            if (billingAcumulatedToDate > 0) {
                marginTracking.PercentageRealToDate = ((billingAcumulatedToDate - costsAcumulatedToDate) / billingAcumulatedToDate) * 100;
            }

            this.allMarginTrackings.push(marginTracking);
        }

        var percentageExpectedTotal = 0;
        if (evalpropTotalBilling > 0) {
            percentageExpectedTotal = ((evalpropTotalBilling - evalpropTotalCosts) / evalpropTotalBilling) * 100
        }
        
        // percentageExpectedTotal = evalpropTotalTracing / cantMonths

        this.setRemainingAndTotal(totalAcumulatedToEnd, totalCostsAcumulatedToEnd, billingRealAcumulatedToDate, budgetRemaint, billingAcumulateProyected, percentageExpectedTotal);

        this.setMarginTracking(selectedMonth, selectedYear);

        this.sendDataToDetailView(this.allMarginTrackings)
    }

    updateEvalpropValues(data, startDate, endDate) {
        var billingMonth = this.billingModel.months.find(x => x.month == data.month && x.year == data.year);
        var costDetailMonth = this.costsModel.months.find(x => x.month == data.month && x.year == data.year);

        if (data.type == 1) {
            billingMonth.valueEvalProp = data.value;
        }
        else {
            costDetailMonth.valueEvalProp = data.value;
        }
    }

    private setRemainingAndTotal(totalAcumulatedToEnd: number, totalCostsAcumulatedToEnd: number, billingRealAcumulated: number, budgetRemaint: number, billingAcumulateProyected: number, percentageExpectedTotal: number) {
        
        if (!this.allMarginTrackings || this.allMarginTrackings.length == 0) return;

        this.allMarginTrackings.forEach(element => {
            // Restante a la fecha (proyectado, ventas)
            element.SalesRemainigToDate = billingAcumulateProyected
            // Restante a la fecha (proyectado, costos)
           element.TotalExpensesRemainigToDate = budgetRemaint;

            // Total a terminacion
            element.TotalSalesToEnd = billingRealAcumulated + billingAcumulateProyected;
            element.TotalExpensesToEnd = totalCostsAcumulatedToEnd + budgetRemaint;
            element.PercentageExpectedTotal = percentageExpectedTotal;

            element.PercentageToEnd = ((totalAcumulatedToEnd - totalCostsAcumulatedToEnd) / totalAcumulatedToEnd) * 100;
        });
    }

    sendDataToDetailView(allMarginTrackings) {
        if (this.getData.observers.length > 0) {
            this.getData.emit(allMarginTrackings);
        }
    }

    createWorksheet(workbook){
        let worksheet: Worksheet = workbook.addWorksheet('Margen');

        var row1 = ["Margen a Terminación", "", "", "", "Ventas", "Costos Totales"];

        var row2 = ["Previsto EvalProp a terminación", 
                    this.marginTrackingSelected.PercentageExpectedTotal || 0, 
                    "", 
                    "Previsto", 
                    this.marginTrackingSelected.ExpectedSales, 
                    this.marginTrackingSelected.TotalExpensesExpected];
        
        var row3 = ["Real A terminación", 
                    this.marginTrackingSelected.PercentageToEnd || 0, 
                    "", 
                    "Previsto", 
                    this.marginTrackingSelected.SalesOnMonth, 
                    this.marginTrackingSelected.TotalExpensesOnMonth];
                      
        var row4 = ["", 
                    "", 
                    "", 
                    "Acumulado a la fecha", 
                    this.marginTrackingSelected.SalesAccumulatedToDate, 
                    this.marginTrackingSelected.TotalExpensesAccumulatedToDate];

        var row5 = ["Margen Mensual", 
                    "", 
                    "", 
                    "Restante a la fecha (proyectado)", 
                    this.marginTrackingSelected.SalesRemainigToDate, 
                    this.marginTrackingSelected.TotalExpensesRemainigToDate];

        var row6 = ["Previsto EvalProp mensual", 
                    this.marginTrackingSelected.PercentageExpected, 
                    "", 
                    "Total a la terminación (real + proyectado)", 
                    this.marginTrackingSelected.TotalSalesToEnd, 
                    this.marginTrackingSelected.TotalExpensesToEnd];

        var row7 = ["Real a la fecha", 
                    this.marginTrackingSelected.PercentageRealToDate];

        worksheet.addRows([row1, row2, row3, row4, row5, row6, row7]);

        worksheet.mergeCells("A1:B1");
        worksheet.mergeCells("A5:B5");
       
        worksheet.getCell("B2").numFmt = '#,##0.00 "%"';
        worksheet.getCell("B3").numFmt = '#,##0.00 "%"';
        worksheet.getCell("B6").numFmt = '#,##0.00 "%"';
        worksheet.getCell("B7").numFmt = '#,##0.00 "%"';
        worksheet.getCell("E2").numFmt = '"$" #,##0.00';
        worksheet.getCell("E3").numFmt = '"$" #,##0.00';
        worksheet.getCell("E4").numFmt = '"$" #,##0.00';
        worksheet.getCell("E5").numFmt = '"$" #,##0.00';
        worksheet.getCell("E6").numFmt = '"$" #,##0.00';
        worksheet.getCell("F2").numFmt = '"$" #,##0.00';
        worksheet.getCell("F3").numFmt = '"$" #,##0.00';
        worksheet.getCell("F4").numFmt = '"$" #,##0.00';
        worksheet.getCell("F5").numFmt = '"$" #,##0.00';
        worksheet.getCell("F6").numFmt = '"$" #,##0.00';

        worksheet.getColumn(1).width = 35;
        worksheet.getColumn(2).width = 12;
        worksheet.getColumn(3).width = 10;
        worksheet.getColumn(4).width = 37;
        worksheet.getColumn(5).width = 16;
        worksheet.getColumn(6).width = 16;

        worksheet.getCell("A1").style = { font: { bold: true } };
        worksheet.getCell("A5").style = { font: { bold: true } };
        worksheet.getCell("E1").style = { font: { bold: true } };
        worksheet.getCell("F1").style = { font: { bold: true } };

        worksheet.getCell("A1").alignment = { horizontal:'center'};
        worksheet.getCell("A5").alignment = { horizontal:'center'};
        worksheet.getCell("E1").alignment = { horizontal:'center'};
        worksheet.getCell("F1").alignment = { horizontal:'center'};

        const borderBlack = "FF000000";

        worksheet.getCell("A1").border = { bottom: { style:'thin', color: {argb: borderBlack} }};
        worksheet.getCell("B1").border = { 
            right: { style:'thin', color: {argb: borderBlack} },
            bottom: { style:'thin', color: {argb: borderBlack} }
        };

        worksheet.getCell("A3").border = { bottom: { style:'thin', color: {argb: borderBlack} }};
        worksheet.getCell("A7").border = { bottom: { style:'thin', color: {argb: borderBlack} }};

        worksheet.getCell("A6").border = { top: { style:'thin', color: {argb: borderBlack} }};

        worksheet.getCell("A4").border = { bottom: { style:'thin', color: {argb: borderBlack} }};
        worksheet.getCell("B4").border = { bottom: { style:'thin', color: {argb: borderBlack} }};
     
        worksheet.getCell("B2").border = { right: { style:'thin', color: {argb: borderBlack} }};
        worksheet.getCell("B3").border = {
             right: { style:'thin', color: {argb: borderBlack} },
             bottom: { style:'thin', color: {argb: borderBlack} }
        };
        worksheet.getCell("B5").border = { right: { style:'thin', color: {argb: borderBlack} }};

        worksheet.getCell("B6").border = { 
            right: { style:'thin', color: {argb: borderBlack} },
            top: { style:'thin', color: {argb: borderBlack} }
        };

        worksheet.getCell("B7").border = { 
            right: { style:'thin', color: {argb: borderBlack} },
            bottom: { style:'thin', color: {argb: borderBlack} }
        };

        worksheet.getCell("F1").border = { 
            right: { style:'thin', color: {argb: borderBlack} },
            bottom: { style:'thin', color: {argb: borderBlack} }
        };

        worksheet.getCell("E1").border = { bottom: { style:'thin', color: {argb: borderBlack} }};
        worksheet.getCell("E6").border = { bottom: { style:'thin', color: {argb: borderBlack} }};

        worksheet.getCell("D1").border = { 
            left: { style:'thin', color: {argb: borderBlack} },
            bottom: { style:'thin', color: {argb: borderBlack} }
        };

        worksheet.getCell("D2").border = { 
            left: { style:'thin', color: {argb: borderBlack} },
            right: { style:'thin', color: {argb: borderBlack} }
        };
        worksheet.getCell("D3").border = { 
            left: { style:'thin', color: {argb: borderBlack} },
            right: { style:'thin', color: {argb: borderBlack} }
        };
        worksheet.getCell("D4").border = { 
            left: { style:'thin', color: {argb: borderBlack} },
            right: { style:'thin', color: {argb: borderBlack} }
        };
        worksheet.getCell("D5").border = { 
            left: { style:'thin', color: {argb: borderBlack} },
            right: { style:'thin', color: {argb: borderBlack} }
        };
        worksheet.getCell("D6").border = { 
            left: { style:'thin', color: {argb: borderBlack} },
            bottom: { style:'thin', color: {argb: borderBlack} },
            right: { style:'thin', color: {argb: borderBlack} }
        };

        worksheet.getCell("F2").border = { right: { style:'thin', color: {argb: borderBlack} }};
        worksheet.getCell("F3").border = { right: { style:'thin', color: {argb: borderBlack} }};
        worksheet.getCell("F4").border = { right: { style:'thin', color: {argb: borderBlack} }};
        worksheet.getCell("F5").border = { right: { style:'thin', color: {argb: borderBlack} }};

        worksheet.getCell("F6").border = { 
            right: { style:'thin', color: {argb: borderBlack} },
            bottom: { style:'thin', color: {argb: borderBlack} }
        };
    }
}