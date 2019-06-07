import { OnInit, Component, OnDestroy, ViewChild } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Subscription } from "rxjs";
import { ManagementReportService } from "app/services/management-report/management-report.service";
import { MessageService } from "app/services/common/message.service";
import { MenuService } from "app/services/admin/menu.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { DatesService } from "app/services/common/month.service";


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

    selectedDate: Date = new Date(new Date().getFullYear(), new Date().getMonth(), 1);
    public selectedMonth: number;
    public selectedYear: number;
    public selectedMonthDesc: string;

    ReportStartDate: Date;
    ReportEndDate: Date;
    ReportStartDateError: boolean = false
    ReportEndDateError: boolean = false
    ManagementReportId: number;

    @ViewChild("marginTracking") marginTracking;
    @ViewChild("billing") billing;
    @ViewChild("detailCost") detailCost;
    @ViewChild('tracing') tracing;
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
        private datesService: DatesService,
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
            
            if(this.menuService.user.id != this.model.managerId && this.menuService.userIsCdg == false){
                this.router.navigate(['/403']);
                return false;
            }

            this.ManagementReportId = response.data.managementReportId;
            this.marginTracking.model = response.data;

            this.setStartDate(this.model.manamementReportStartDate, this.model.manamementReportEndDate)

            this.billing.init(this.serviceId);
            // this.marginTracking.init(this.startDate)

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

    seeCostDetailMonth() {

        var resources = this.detailCost.getResourcesByMonth(this.selectedMonth, this.selectedYear);
        var AnalyticId = this.detailCost.getIdAnalytic();

        var totals = this.billing.getTotals(this.selectedMonth + 1, this.selectedYear);

        var data = {
            isCdg: this.menuService.userIsCdg,
            resources, totals, AnalyticId,
            month: this.selectedMonth, 
            monthDesc: this.selectedMonthDesc,
            year: this.selectedYear
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
        
        const dateReportStart = new Date(new Date(this.ReportStartDate).getFullYear(), new Date(this.ReportStartDate).getMonth(), 1)
        const dateReportEnd = new Date(new Date(this.ReportEndDate).getFullYear(), new Date(this.ReportEndDate).getMonth(), 1)
        const dateAnalyticStart = new Date(new Date(this.model.startDate).getFullYear(), new Date(this.model.startDate).getMonth(), 1)
        const dateAnalyticEnd = new Date(new Date(this.model.endDate).getFullYear(), new Date(this.model.endDate).getMonth(), 1)

        if (dateReportStart < dateAnalyticStart) {
            this.ReportStartDateError = true
            this.editDateModal.resetButtons()
            this.messageService.showError("managementReport.startDateOutOfRange")
        }
        else{
            this.ReportStartDateError = false
        }

        if (dateReportEnd > dateAnalyticEnd) {
            this.ReportEndDateError = true
            this.editDateModal.resetButtons()
            this.messageService.showError("managementReport.endDateOutOfRange")
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
                
                this.setStartDate(this.ReportStartDate, this.ReportEndDate)

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
        this.marginTracking.calculate(this.model.manamementReportStartDate, this.model.manamementReportEndDate, this.selectedMonth, this.selectedYear);
    }

    getCostsData(costsModel){
        this.marginTracking.costDataLoaded = true;
        this.marginTracking.costsModel = costsModel;
        this.marginTracking.calculate(this.model.manamementReportStartDate, this.model.manamementReportEndDate, this.selectedMonth, this.selectedYear);
    }

    getMarginTracking(allMarginTrackings){    
      this.tracing.open(allMarginTrackings, this.model.analytic)
    }

    getEvalPropData(data){
        this.marginTracking.updateEvalpropValues(data, this.model.manamementReportStartDate, this.model.manamementReportEndDate);
    }

    setStartDate(reportStartDate, reportEndDate){
        
        const dateReportStart = new Date(new Date(reportStartDate).getFullYear(), new Date(reportStartDate).getMonth(), 1)
        const dateReportEnd = new Date(new Date(reportEndDate).getFullYear(), new Date(reportEndDate).getMonth(), 1)

        if(this.selectedDate < dateReportStart){
            this.selectedDate = dateReportStart
        }
        if(this.selectedDate > dateReportEnd){
            this.selectedDate = dateReportEnd
        }

        var dateSetting = this.datesService.getMonth(this.selectedDate);
        this.setDate(dateSetting); 
    }

    setDate(dateSetting){
        this.selectedMonthDesc = dateSetting.montDesc;
        this.selectedMonth = dateSetting.month;
        this.selectedYear = dateSetting.year;

        this.marginTracking.setMarginTracking(this.selectedMonth, this.selectedYear);
        this.detailCost.setFromDate(this.selectedDate)
        this.billing.setFromDate(this.selectedDate)
    }

    addMonth(){
        var dateSplitted = this.model.manamementReportEndDate.split("-");

        if(this.selectedYear == dateSplitted[0] && this.selectedMonth == dateSplitted[1]){
            return;
        }

        var dateSetting = this.datesService.getMonth(new Date(this.selectedYear, this.selectedMonth));
        this.selectedDate = new Date(this.selectedYear, this.selectedMonth)

        this.setDate(dateSetting);  
    }

    substractMonth(){
        var dateSplitted = this.model.manamementReportStartDate.split("-");

        if(this.selectedYear == dateSplitted[0] && this.selectedMonth == dateSplitted[1]){
            return;
        }

        this.selectedMonth -= 2;
        var dateSetting = this.datesService.getMonth(new Date(this.selectedYear, this.selectedMonth));
        this.selectedDate = new Date(this.selectedYear, this.selectedMonth)
        
        this.setDate(dateSetting);  
    }
    

}