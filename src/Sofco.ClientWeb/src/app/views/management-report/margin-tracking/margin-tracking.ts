import { Component, OnDestroy, OnInit, ViewChild, Input, Output, EventEmitter } from "@angular/core";
import { MenuService } from "app/services/admin/menu.service";
// import { DatesService } from "app/services/common/month.service";
import { ManagementReportDetailComponent } from "../detail/mr-detail"
import * as moment from 'moment';
import { MarginTracking } from "app/models/management-report/marginTracking";
import { HitoStatus } from "app/models/enums/hitoStatus";
import { AnalyticStatus } from "app/models/enums/analyticStatus";

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

    private billingModel: any;
    private costsModel: any;

    public model: any;

    public marginTrackingSelected: MarginTracking = new MarginTracking();
    private allMarginTrackings: any[] = new Array();

    @Output() getData: EventEmitter<any> = new EventEmitter();

    constructor(private menuService: MenuService,
        private managementReportDetail: ManagementReportDetailComponent
    ) { }

    ngOnInit(): void {
        this.isManager = this.menuService.userIsManager;
        this.isCdgOrDirector = this.menuService.userIsDirector || this.menuService.userIsCdg;
    }

    ngOnDestroy(): void {
    }

    isReadOnly() {
        this.model.analyticStatus == AnalyticStatus.Close;
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
        var costsAcumulatedToDate = 0;
        var totalAcumulatedToEnd = 0;
        var totalCostsAcumulatedToEnd = 0;
        var evalpropTotalBilling = 0;
        var evalpropTotalCosts = 0;
        var evalpropTotalTracing = 0;
        var cantMonths = 0;

        for (var initDate = startDate; initDate <= endDate; initDate.setMonth(initDate.getMonth() + 1)) {
            var marginTracking = new MarginTracking();

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
                marginTracking.SalesOnMonth += billingMonth.totalBilling;

                billingAcumulatedToDate += billingMonth.totalBilling;
            
                totalAcumulatedToEnd += billingMonth.totalBilling;
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
                              //  billingAcumulatedToDate += valueMonth.valuePesos;

                                // // Real del mes $$ (ventas)
                                // marginTracking.SalesOnMonth += valueMonth.valuePesos;
                            }

                            // Previsto $$ (ventas)
                            marginTracking.ExpectedSales += valueMonth.valuePesos;
                           // totalAcumulatedToEnd += valueMonth.valuePesos;
                        }
                    });
                });
            }

            // Acumulado a la fecha $$ (ventas)
            marginTracking.SalesAccumulatedToDate += totalAcumulatedToEnd;

            if (costDetailMonth) {

                costsAcumulatedToDate += costDetailMonth.real.totalCost;

                marginTracking.hasReal = costDetailMonth.hasReal
                marginTracking.valueEvalProp = costDetailMonth.valueMarginEvalProp
                marginTracking.billingMonthId = costDetailMonth.billingMonthId

                // Previsto $$
                marginTracking.TotalExpensesExpected += costDetailMonth.budget.totalCost;

                // Real al mes $$ (costos)
                marginTracking.TotalExpensesOnMonth = costDetailMonth.real.totalCost;
              
                totalCostsAcumulatedToEnd += costDetailMonth.budget.totalCost;

                // Previsto % (ventas)
                if (costDetailMonth.valueMarginEvalProp > 0) {
                    marginTracking.PercentageExpected = costDetailMonth.valueMarginEvalProp
                    evalpropTotalTracing += costDetailMonth.valueMarginEvalProp
                }
            }

            // Real a la fecha % (costos)
            if (billingAcumulatedToDate > 0) {
                marginTracking.PercentageRealToDate = ((billingAcumulatedToDate - costsAcumulatedToDate) / billingAcumulatedToDate) * 100;
            }

            // Acumulado a la fecha $$ (costos)
            marginTracking.TotalExpensesAccumulatedToDate = costsAcumulatedToDate;

            this.allMarginTrackings.push(marginTracking);
        }

         var percentageExpectedTotal = 0;
        // if (evalpropTotalBilling > 0) {
        //     percentageExpectedTotal = ((evalpropTotalBilling - evalpropTotalCosts) / evalpropTotalBilling) * 100
        // }
        
        percentageExpectedTotal = evalpropTotalTracing / cantMonths

        this.setRemainingAndTotal(totalAcumulatedToEnd, totalCostsAcumulatedToEnd, percentageExpectedTotal);

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

        startDate = moment(startDate).toDate();
        endDate = moment(endDate).toDate();

        startDate.setDate(1);
        endDate.setDate(1);

        var evalpropTotalBilling = 0;
        var evalpropTotalCosts = 0;
        var evalpropTotalTracing = 0;

        for (var initDate = startDate; initDate <= endDate; initDate.setMonth(initDate.getMonth() + 1)) {
            var month = initDate.getMonth() + 1;
            var year = initDate.getFullYear();

            billingMonth = this.billingModel.months.find(x => x.month == month && x.year == year);
            costDetailMonth = this.costsModel.months.find(x => x.month == month && x.year == year);
            var marginTracking = this.allMarginTrackings.find(x => x.Month == month && x.Year == year);

            if (billingMonth) evalpropTotalBilling += billingMonth.valueEvalProp;
            if (costDetailMonth) {
                evalpropTotalCosts += costDetailMonth.valueEvalProp;
                evalpropTotalTracing += costDetailMonth.valueMarginEvalProp
            }

            this.calculatePercentageExpected(billingMonth, costDetailMonth, marginTracking);
        }

        var percentageExpectedTotal = 0;
        if (evalpropTotalBilling > 0) {
            percentageExpectedTotal = ((evalpropTotalBilling - evalpropTotalCosts) / evalpropTotalBilling) * 100
        }

        this.allMarginTrackings.forEach(element => {
            element.PercentageExpectedTotal = percentageExpectedTotal;
        });
    }

    private setRemainingAndTotal(totalAcumulatedToEnd: number, totalCostsAcumulatedToEnd: number, percentageExpectedTotal: number) {
        if (!this.allMarginTrackings || this.allMarginTrackings.length == 0) return;

        this.allMarginTrackings.forEach(element => {
            // Restante a la fecha (proyectado, ventas)
            element.SalesRemainigToDate = totalAcumulatedToEnd - element.SalesAccumulatedToDate;
            // Restante a la fecha (proyectado, costos)
            element.TotalExpensesRemainigToDate = totalCostsAcumulatedToEnd - element.TotalExpensesAccumulatedToDate;

            // Total a terminacion
            element.TotalSalesToEnd += totalAcumulatedToEnd;
            element.TotalExpensesToEnd += totalCostsAcumulatedToEnd;
            element.PercentageExpectedTotal = percentageExpectedTotal;

            element.PercentageToEnd = ((totalAcumulatedToEnd - totalCostsAcumulatedToEnd) / totalAcumulatedToEnd) * 100;
        });
    }

    private calculatePercentageExpected(billingMonth: any, costDetailMonth: any, marginTracking: MarginTracking) {

        if (!billingMonth) return;

        var evalpropBillingValue = billingMonth.valueEvalProp;

        var evalpropCostValue = 0;

        if (costDetailMonth) {
            evalpropCostValue = costDetailMonth.valueEvalProp;
        }

        // Previsto % (ventas)
        if (evalpropBillingValue > 0) {
            marginTracking.PercentageExpected = ((evalpropBillingValue - evalpropCostValue) / evalpropBillingValue) * 100;
        }
    }

    sendDataToDetailView(allMarginTrackings) {

        if (this.getData.observers.length > 0) {
            this.getData.emit(allMarginTrackings);
        }
    }
}