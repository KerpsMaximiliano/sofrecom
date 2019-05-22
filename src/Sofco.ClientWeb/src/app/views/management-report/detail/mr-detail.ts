import { OnInit, Component, OnDestroy, ViewChild } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Subscription } from "rxjs";
import { ManagementReportService } from "app/services/management-report/management-report.service";
import { MessageService } from "app/services/common/message.service";
import { MenuService } from "app/services/admin/menu.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';


@Component({
    selector: 'management-report-detail',
    templateUrl: './mr-detail.html',
    styleUrls: ['./mr-detail.scss']
})
export class ManagementReportDetailComponent implements OnInit, OnDestroy {

    paramsSubscrip: Subscription;
    getDetailSubscrip: Subscription;
    updateDatesSubscrip: Subscription;

    customerId: string;
    serviceId: string;
    customerName: string;
    serviceName: string;

    public model: any;

    isManager: boolean = false;
    isCdgOrDirector: boolean = false;

    ReportStartDate: Date;
    ReportEndDate: Date;
    ReportStartDateError: boolean = false
    ReportEndDateError: boolean = false
    ManagementReportId: number;

    @ViewChild("marginTracking") marginTracking;
    @ViewChild("billing") billing;
    @ViewChild("detailCost") detailCost;
    @ViewChild('costDetailMonth') costDetailMonth;
    @ViewChild('dateReportStart') dateReportStart;
    @ViewChild('dateReportEnd') dateReportEnd;
    @ViewChild('editDateModal') editDateModal;
    @ViewChild('modalEvalProp') modalEvalProp;

    public editDateModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Editar Fechas",
        "editDateModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    private today: Date = new Date();
    private startNewPeriod: Date = new Date(this.today.getFullYear(), this.today.getMonth(), 1);

    constructor(private activatedRoute: ActivatedRoute,
        private messageService: MessageService,
        private menuService: MenuService,
        private managementReportService: ManagementReportService,
        private router: Router) { }

    ngOnInit(): void {

        this.editDateModal.size = 'modal-sm'

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
        if (this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if (this.getDetailSubscrip) this.getDetailSubscrip.unsubscribe();
        if (this.updateDatesSubscrip) this.updateDatesSubscrip.unsubscribe();
    }

    getDetail() {
        this.messageService.showLoading();

        this.getDetailSubscrip = this.managementReportService.getDetail(this.serviceId).subscribe(response => {

            this.model = response.data;
            
            if(this.menuService.user.id != this.model.managerId || this.menuService.userIsCdg == false){
                this.router.navigate(['/403']);
                return false;
            }

            this.ManagementReportId = response.data.managementReportId;
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

    seeCostDetailMonth(month, year) {

        var resources = this.detailCost.getResourcesByMonth(month, year);
        var AnalyticId = this.detailCost.getIdAnalytic();

        var totals = this.billing.getTotals(month + 1, year);

        var data = {
            isCdg: this.menuService.userIsCdg,
            resources, totals, AnalyticId,
            month: month, year
        }

        this.costDetailMonth.open(data);
    }

    updateDetailCost() {
        this.detailCost.getCost()
    }

    openEditDateModal() {

        this.ReportStartDate = this.model.manamementReportStartDate;
        this.ReportEndDate = this.model.manamementReportEndDate;

        this.ReportStartDateError = false
        this.ReportEndDateError = false

        this.editDateModal.show();
    }

    EditDate() {

        this.ReportStartDateError = false
        this.ReportEndDateError = false

        if (new Date(this.ReportStartDate) < new Date(this.model.startDate)) {
            this.ReportStartDateError = true
            this.editDateModal.resetButtons()
            this.messageService.showError("managementReport.startDateOutOfRange")
        }
        else{
            this.ReportStartDateError = false
        }

        if (new Date(this.ReportEndDate) > new Date(this.model.endDate)) {
            this.ReportEndDateError = true
            this.editDateModal.resetButtons()
            this.messageService.showError("managementReport.startDateOutOfRange")
        }
        else{
            this.ReportEndDateError = false
        }

        if (this.ReportStartDateError == false && this.ReportEndDateError == false) {

            var model = {
                StartDate: new Date(this.ReportStartDate),
                EndDate: new Date(this.ReportEndDate)
            }
            this.updateDatesSubscrip = this.managementReportService.updateDates(this.ManagementReportId, model).subscribe(response => {

                this.messageService.closeLoading();
                this.model.manamementReportStartDate = this.ReportStartDate
                this.model.manamementReportEndDate = this.ReportEndDate
          
                this.getDetail()
                this.detailCost.getCost()

                this.editDateModal.hide();
            },
                error => {
                    this.messageService.closeLoading()
                    this.editDateModal.resetButtons()
                }
            );

        }
    }

    openEvalPropModal(month){
        this.modalEvalProp.openEditEvalProp(month);
    }

    getBillingData(billingModel){
        this.marginTracking.billingDataLoaded = true;
        this.marginTracking.billingModel = billingModel;
        this.marginTracking.calculate(this.model.manamementReportStartDate, this.model.manamementReportEndDate);
    }

    getCostsData(costsModel){
        this.marginTracking.costDataLoaded = true;
        this.marginTracking.costsModel = costsModel;
        this.marginTracking.calculate(this.model.manamementReportStartDate, this.model.manamementReportEndDate);
    }

    getEvalPropData(data){
        this.marginTracking.updateEvalpropValues(data, this.model.manamementReportStartDate, this.model.manamementReportEndDate);
    }
}