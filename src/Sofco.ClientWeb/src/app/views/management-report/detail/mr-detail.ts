import { OnInit, Component, OnDestroy, ViewChild } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Subscription } from "rxjs";
import { ManagementReportService } from "app/services/management-report/management-report.service";
import { MessageService } from "app/services/common/message.service";
import { MenuService } from "app/services/admin/menu.service";

@Component({
    selector: 'management-report-detail',
    templateUrl: './mr-detail.html',
    styleUrls: ['./mr-detail.scss']
})
export class ManagementReportDetailComponent implements OnInit, OnDestroy {

    paramsSubscrip: Subscription;
    getDetailSubscrip: Subscription;

    customerId: string;
    serviceId: string;
    customerName: string;
    serviceName: string;

    public model: any;

    isManager: boolean = false;
    isCdgOrDirector: boolean = false;

    @ViewChild("marginTracking") marginTracking;
    @ViewChild("billing") billing;
    @ViewChild("detailCost") detailCost;
    @ViewChild('costDetailMonth') costDetailMonth;

    constructor(private activatedRoute: ActivatedRoute,
                private messageService: MessageService,
                private menuService: MenuService,
                private managementReportService: ManagementReportService){}

    ngOnInit(): void {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.customerId = params['customerId'];
            this.serviceId = params['serviceId'];
            this.customerName = sessionStorage.getItem('customerName');
            this.serviceName = sessionStorage.getItem('serviceName');

            this.getDetail();
          });

          this.isManager = this.menuService.userIsManager;
          this.isCdgOrDirector = this.menuService.userIsDirector || this.menuService.userIsCdg;
    }    

    ngOnDestroy(): void {
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.getDetailSubscrip) this.getDetailSubscrip.unsubscribe();
    }

    getDetail(){
        this.messageService.showLoading();

        this.getDetailSubscrip = this.managementReportService.getDetail(this.serviceId).subscribe(response => {

            this.model = response.data;
            this.marginTracking.model = response.data;
            this.billing.init(this.serviceId);
            this.messageService.closeLoading();
        },
        error => this.messageService.closeLoading());
    }

    collapse() {
        if ($("#collapseOne").hasClass('in')) {
            $("#collapseOne").removeClass('in');
        }
        else {
            $("#collapseOne").addClass('in');
        }

        this.changeIcon();
    }

    changeIcon() {
        if ($("#collapseOne").hasClass('in')) {
            $("#search-icon").toggleClass('fa-caret-down').toggleClass('fa-caret-up');
        }
        else {
            $("#search-icon").toggleClass('fa-caret-up').toggleClass('fa-caret-down');
        }
    }

    seeCostDetailMonth(month, year){
        var resources = this.detailCost.getResourcesByMonth(month+1, year);
        var totals = this.billing.getTotals(month+1, year);

        this.costDetailMonth.open({ isCdg: this.menuService.userIsCdg, resources, totals });
    }
}