import { Component, OnDestroy, OnInit, ViewChild, Input } from "@angular/core";
import { MenuService } from "app/services/admin/menu.service";
import { DatesService } from "app/services/common/month.service";
import { ManagementReportDetailComponent } from "../detail/mr-detail"
import * as moment from 'moment';
import { MarginTracking } from "app/models/management-report/marginTracking";
import { HitoStatus } from "app/models/enums/hitoStatus";

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
    public month: number;
    public year: number;
    public monthDesc: string;

    public marginTrackingSelected: MarginTracking = new MarginTracking();
    private allMarginTrackings: any[] = new Array();

    constructor(private menuService: MenuService,
                private datesService: DatesService,
                private managementReportDetail: ManagementReportDetailComponent
                ){}

    ngOnInit(): void {
        this.isManager = this.menuService.userIsManager;
        this.isCdgOrDirector = this.menuService.userIsDirector || this.menuService.userIsCdg;

        var dateSetting = this.datesService.getMonth(new Date());
        this.setDate(dateSetting);  
    }

    ngOnDestroy(): void {
    }

    setDate(dateSetting){
        this.monthDesc = dateSetting.montDesc;
        this.month = dateSetting.month;
        this.year = dateSetting.year;

        this.setMarginTracking();
    }

    setMarginTracking(){
        var marginTracking = this.allMarginTrackings.find(x => x.Month == this.month && x.Year == this.year);

        if(marginTracking && marginTracking != null){
            this.marginTrackingSelected = marginTracking;
        }
        else{
            this.marginTrackingSelected = new MarginTracking();
        }
    }

    addMonth(){
        var dateSplitted = this.model.manamementReportEndDate.split("-");

        if(this.year == dateSplitted[0] && this.month == dateSplitted[1]){
            return;
        }

        var dateSetting = this.datesService.getMonth(new Date(this.year, this.month));
        this.setDate(dateSetting);  
    }

    substractMonth(){
        var dateSplitted = this.model.manamementReportStartDate.split("-");

        if(this.year == dateSplitted[0] && this.month == dateSplitted[1]){
            return;
        }

        this.month -= 2;
        var dateSetting = this.datesService.getMonth(new Date(this.year, this.month));
        this.setDate(dateSetting);  
    }

    seeCostDetailMonth(){
        this.managementReportDetail.seeCostDetailMonth(this.month, this.year)
    }

    calculate(manamementReportStartDate, manamementReportEndDate){
        if(!this.billingDataLoaded || !this.costDataLoaded) return;

        this.allMarginTrackings = [];

        var startDate = moment(manamementReportStartDate).toDate();        
        var endDate = moment(manamementReportEndDate).toDate(); 

        startDate.setDate(1);
        endDate.setDate(1);
        
        var billingAcumulatedToDate = 0;
        var costsAcumulatedToDate = 0;
        var totalAcumulatedToEnd = 0;
        var totalCostsAcumulatedToEnd = 0;

        for(var initDate = startDate; initDate <= endDate; initDate.setMonth(initDate.getMonth()+1)){
            var marginTracking = new MarginTracking();

            var month = initDate.getMonth()+1;
            var year = initDate.getFullYear();

            marginTracking.Month = month;
            marginTracking.Year = year;

            var billingMonth = this.billingModel.months.find(x => x.month == month && x.year == year);
            var costDetailMonth = this.costsModel.months.find(x => x.month == month && x.year == year);

            marginTracking.ExpectedSales = 0;
            marginTracking.TotalExpensesExpected = 0;

            this.calculatePercentageExpected(billingMonth, costDetailMonth, marginTracking);

            var hitosMonth = this.billingModel.hitos.filter(hito => {
                var hitoDate = moment(hito.date).toDate();

                if(hitoDate.getFullYear() == initDate.getFullYear() && hitoDate.getMonth() == initDate.getMonth()){
                    return hito;
                }
            });

            marginTracking.SalesOnMonth = 0;
            marginTracking.TotalSalesToEnd = 0;
            marginTracking.TotalExpensesToEnd = 0;
            marginTracking.SalesAccumulatedToDate = 0;
            marginTracking.ExpectedSales = 0;

            if(hitosMonth && hitosMonth.length > 0){
                hitosMonth.forEach(element => {

                    element.values.forEach(valueMonth => {
                        if(valueMonth.month == month &&  valueMonth.year == year && valueMonth.valuePesos > 0){
                            if(valueMonth.status == HitoStatus.Billed || valueMonth.status == HitoStatus.Cashed){
                                billingAcumulatedToDate += valueMonth.valuePesos;

                                // Real del mes $$ (ventas)
                                marginTracking.SalesOnMonth += valueMonth.valuePesos;
                            }
                        
                            // Previsto $$ (ventas)
                            marginTracking.ExpectedSales += valueMonth.valuePesos;
                            totalAcumulatedToEnd += valueMonth.valuePesos;
                        }
                    });
                });
            }

            // Acumulado a la fecha $$ (ventas)
            marginTracking.SalesAccumulatedToDate += totalAcumulatedToEnd;
            marginTracking.TotalExpensesExpected = 0;

            if(costDetailMonth){
                costsAcumulatedToDate += costDetailMonth.value;
         
                // Previsto $$
                marginTracking.TotalExpensesExpected = costDetailMonth.value;

                // Real al mes $$ (costos)
                marginTracking.TotalExpensesOnMonth = costDetailMonth.value;
                totalCostsAcumulatedToEnd += costDetailMonth.value;
            }

            // Real a la fecha % (costos)
            if(billingAcumulatedToDate > 0){
                marginTracking.PercentageRealToDate = ((billingAcumulatedToDate-costsAcumulatedToDate) / billingAcumulatedToDate) * 100;
            }
            else{
                marginTracking.PercentageRealToDate = 0;
            }

            // Acumulado a la fecha $$ (costos)
            marginTracking.TotalExpensesAccumulatedToDate = costsAcumulatedToDate;

            this.allMarginTrackings.push(marginTracking);
        }

        this.setRemainingAndTotal(totalAcumulatedToEnd, totalCostsAcumulatedToEnd);

        this.setMarginTracking();
    }

    private setRemainingAndTotal(totalAcumulatedToEnd: number, totalCostsAcumulatedToEnd: number) {
        if(!this.allMarginTrackings || this.allMarginTrackings.length == 0) return;

        this.allMarginTrackings.forEach(element => {
            // Restante a la fecha (proyectado, ventas)
            element.SalesRemainigToDate = totalAcumulatedToEnd-element.SalesAccumulatedToDate;
            // Restante a la fecha (proyectado, costos)
            element.TotalExpensesRemainigToDate = totalCostsAcumulatedToEnd-element.TotalExpensesAccumulatedToDate;

            // Total a terminacion
            element.TotalSalesToEnd += totalAcumulatedToEnd;
            element.TotalExpensesToEnd += totalCostsAcumulatedToEnd;

            element.PercentageToEnd = ((totalAcumulatedToEnd-totalCostsAcumulatedToEnd) / totalAcumulatedToEnd) * 100;
        });
    }

    private calculatePercentageExpected(billingMonth: any, costDetailMonth: any, marginTracking: MarginTracking) {

        if(!billingMonth){
            marginTracking.PercentageExpected = 0;
            return;
        }

        var evalpropBillingValue = billingMonth.valueEvalProp;
        var evalpropCostValue = 0;

        if(costDetailMonth){
            evalpropCostValue = costDetailMonth.valueEvalProp;
        }

        // Previsto % (ventas)
        if (evalpropBillingValue > 0) {
            marginTracking.PercentageExpected = ((evalpropBillingValue - evalpropCostValue) / evalpropBillingValue) * 100;
        }
        else {
            marginTracking.PercentageExpected = 0;
        }
    }
}