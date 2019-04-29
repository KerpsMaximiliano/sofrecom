import { Component, OnDestroy, OnInit, ViewChild, Input } from "@angular/core";
import { MenuService } from "app/services/admin/menu.service";
import { DatesService } from "app/services/common/month.service";
import { ManagementReportDetailComponent } from "../detail/mr-detail"

@Component({
    selector: 'margin-tracking',
    templateUrl: './margin-tracking.html',
    styleUrls: ['./margin-tracking.scss']
})
export class MarginTrackingComponent implements OnInit, OnDestroy {

    isManager: boolean = false;
    isCdgOrDirector: boolean = false;

    @Input() model: any;
    public month: number;
    public year: number;
    public monthDesc: string;

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
    }

    addMonth(){
        var dateSplitted = this.model.endDate.split("-");

        if(this.year == dateSplitted[0] && (this.month+1) == dateSplitted[1]){
            return;
        }

        this.month += 1;
        var dateSetting = this.datesService.getMonth(new Date(this.year, this.month));
        this.setDate(dateSetting);  
    }

    substractMonth(){
        var dateSplitted = this.model.startDate.split("-");

        if(this.year == dateSplitted[0] && (this.month+1) == dateSplitted[1]){
            return;
        }

        this.month -= 1;
        var dateSetting = this.datesService.getMonth(new Date(this.year, this.month));
        this.setDate(dateSetting);  
    }

    seeCostDetailMonth(){
        this.managementReportDetail.seeCostDetailMonth(this.month, this.year)
    }
}