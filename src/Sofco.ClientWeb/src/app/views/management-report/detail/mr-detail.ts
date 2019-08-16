import { OnInit, Component, OnDestroy, ViewChild } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Subscription } from "rxjs";
import { ManagementReportService } from "app/services/management-report/management-report.service";
import { MessageService } from "app/services/common/message.service";
import { MenuService } from "app/services/admin/menu.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { DatesService } from "app/services/common/month.service";
import { AnalyticStatus } from "app/models/enums/analyticStatus";
import { ManagementReportStatus } from "app/models/enums/managementReportStatus";
import { I18nService } from "app/services/common/i18n.service";


@Component({
    selector: 'management-report-detail',
    templateUrl: './mr-detail.html',
    styleUrls: ['./mr-detail.scss']
})
export class ManagementReportDetailComponent implements OnInit, OnDestroy {
    paramsSubscrip: Subscription;
    getDetailSubscrip: Subscription;
    updateDatesSubscrip: Subscription;
    sendSubscrip: Subscription;
    closeSubscrip: Subscription;
    addCommentSubscrip: Subscription;
    getCommentsSubscrip: Subscription;

    customerId: string;
    serviceId: string;
    customerName: string;
    serviceName: string;

    comments: string;

    public model: any;

    isManager: boolean = false;
    isCdgOrDirector: boolean = false;
    isCdg: boolean = false;

    selectedDate: Date = new Date(new Date().getFullYear(), new Date().getMonth(), 1);
    public selectedMonth: number;
    public selectedYear: number;
    public selectedMonthDesc: string;

    ReportStartDate: Date;
    ReportEndDate: Date;
    ReportStartDateError: boolean = false;
    ReportEndDateError: boolean = false;
    readOnly: boolean = false;
    isClosed: boolean = false;
    ManagementReportId: number;

    allComments: any[] = new Array();

    @ViewChild("marginTracking") marginTracking;
    @ViewChild("billing") billing;
    @ViewChild("detailCost") detailCost;
    @ViewChild('tracing') tracing;
    @ViewChild('costDetailMonth') costDetailMonth;
    @ViewChild('dateReportStart') dateReportStart;
    @ViewChild('dateReportEnd') dateReportEnd;
    @ViewChild('editDateModal') editDateModal;
    @ViewChild('modalEvalProp') modalEvalProp;
    @ViewChild('addCommentModal') addCommentModal;
 
    public editDateModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Editar Fechas",
        "editDateModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    public addCommentModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Agregar comentario",
        "addCommentModal",
        true,
        true,
        "ACTIONS.ADD",
        "ACTIONS.cancel"
    );

    private today: Date = new Date();
    private startNewPeriod: Date = new Date(this.today.getFullYear(), this.today.getMonth(), 1);

    constructor(private activatedRoute: ActivatedRoute,
        private messageService: MessageService,
        private menuService: MenuService,
        private managementReportService: ManagementReportService,
        private datesService: DatesService,
        private i18nService: I18nService,
        private router: Router) { }

    ngOnInit(): void {
        this.editDateModal.size = 'modal-sm'

        this.isManager = this.menuService.userIsManager || this.menuService.isManagementReportDelegate;
        this.isCdgOrDirector = this.menuService.userIsDirector || this.menuService.userIsCdg;
        this.isCdg = this.menuService.userIsCdg;

        if(!this.menuService.userIsDirector && !this.menuService.userIsManager && !this.menuService.isManagementReportDelegate && this.menuService.userIsCdg == false){
            this.router.navigate(['/403']);
        }
        else{
            this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
                this.customerId = params['customerId'];
                this.serviceId = params['serviceId'];
                this.customerName = sessionStorage.getItem('customerName');
                this.serviceName = sessionStorage.getItem('serviceName');
    
                this.getDetail();
            });
        }
    }

    ngOnDestroy(): void {
        if (this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if (this.getDetailSubscrip) this.getDetailSubscrip.unsubscribe();
        if (this.updateDatesSubscrip) this.updateDatesSubscrip.unsubscribe();
        if (this.sendSubscrip) this.sendSubscrip.unsubscribe();
        if (this.closeSubscrip) this.closeSubscrip.unsubscribe();
        if (this.addCommentSubscrip) this.addCommentSubscrip.unsubscribe();
        if (this.getCommentsSubscrip) this.getCommentsSubscrip.unsubscribe();
    }

    getDetail() {
        this.messageService.showLoading();

        this.getDetailSubscrip = this.managementReportService.getDetail(this.serviceId).subscribe(response => {

            this.model = response.data;

            this.ManagementReportId = response.data.managementReportId;
            this.marginTracking.model = response.data;

            this.getComment();

            this.billing.manamementReportStartDate = this.model.manamementReportStartDate
            this.billing.manamementReportEndDate = this.model.manamementReportEndDate

            this.setStartDate(this.model.manamementReportStartDate, this.model.manamementReportEndDate)

            this.billing.init(this.serviceId);
            this.billing.managementReportId = this.ManagementReportId;
            
            this.modalEvalProp.managementReportId = this.ManagementReportId;

            this.billing.readOnly = !this.canEdit();
            this.detailCost.readOnly = !this.canEdit();
            this.detailCost.managementReportId = this.ManagementReportId;

            this.readOnly = !this.canEdit();

            this.messageService.closeLoading();
        },
        responseError => {
            this.messageService.closeLoading();

            if(responseError.error.messages && responseError.error.messages.length > 0){
                var forbidden = responseError.error.messages.find(x => x.code == 'forbidden');

                if(forbidden){
                    this.router.navigate(['/403']);
                }
            }
        });
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

        this.costDetailMonth.open(data, this.readOnly || this.isClosed);
    }

    updateDetailCost() {
        this.detailCost.getCost();
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
        this.detailCost.setResourceQuantity(billingModel.months)
    }

    updateMarginTracking(){
        this.billing.init(this.serviceId);
    }

    getCostsData(costsModel){
        this.marginTracking.costDataLoaded = true;
        this.marginTracking.costsModel = costsModel;
        this.marginTracking.calculate(this.model.manamementReportStartDate, this.model.manamementReportEndDate, this.selectedMonth, this.selectedYear);
        this.billing.setResourceQuantity(costsModel.months)
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

        this.isClosed = this.billing.isClosed(this.selectedDate);
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

    canSendManager(){
        if(!this.model || !this.model.status) return false;

        if(this.isClosed) return false;

        if(this.isManager && this.menuService.hasFunctionality('MANRE', 'SEND-MANAGER') && this.model.status == ManagementReportStatus.ManagerPending){
            return true;
        }

        return false;
    }

    canSendCdg(){
        if(!this.model || !this.model.status) return false;
        
        if(this.isClosed) return false;

        if(this.isCdg && this.menuService.hasFunctionality('MANRE', 'SEND-CDG') && this.model.status == ManagementReportStatus.CdgPending){
            return true;
        }

        return false;
    }

    sendManager(){
        this.send(ManagementReportStatus.CdgPending);
    }

    sendCdg(){
        this.send(ManagementReportStatus.ManagerPending);
    }

    send(status){
        var json = {
            id: this.ManagementReportId,
            status: status
        }
    
        this.messageService.showConfirm(() => {
            this.messageService.showLoading();

            this.updateDatesSubscrip = this.managementReportService.send(json).subscribe(response => {
                this.model.status = status;
                this.billing.readOnly = !this.canEdit();
                this.detailCost.readOnly = !this.canEdit();
                this.messageService.closeLoading();
            },
            error => this.messageService.closeLoading());
        });
    }

    getStatusDesc(){
        if(!this.model || !this.model.status) return "";

        if(this.model.status && this.model.status == ManagementReportStatus.CdgPending){
            return this.i18nService.translateByKey('managementReport.cdgPending')
        }

        if(this.model.status && this.model.status == ManagementReportStatus.ManagerPending){
            return this.i18nService.translateByKey('managementReport.managerPending')
        }

        if(this.model.status && this.model.status == ManagementReportStatus.Closed){
            return this.i18nService.translateByKey('managementReport.closed')
        }

        return "";
    }

    canEdit(){
        if(!this.model || !this.model.status) return false;

        if(this.model.status == ManagementReportStatus.CdgPending && this.isCdg) return true;

        if(this.model.status == ManagementReportStatus.ManagerPending && this.isManager) return true;

        return false;
    }

    close(){
        var json = {
            date: this.selectedDate,
            billingId: this.billing.getId(this.selectedDate),
            detailCostId: this.detailCost.getId(this.selectedDate)
        };
    
        this.messageService.showConfirm(() => {
            this.messageService.showLoading();

            this.closeSubscrip = this.managementReportService.close(json).subscribe(response => {

                this.messageService.closeLoading();
            },
            error => this.messageService.closeLoading());
        });
    }

    openComments(){
        this.addCommentModal.show();
    }

    saveComment(){
        var json = {
            id: this.ManagementReportId,
            comment: this.comments
        };

        this.getDetailSubscrip = this.managementReportService.addComment(json).subscribe(response => {
            this.addCommentModal.hide();

            this.allComments.push(response.data);
            this.comments = "";
        },
        error => this.addCommentModal.hide());
    }

    getComment(){
        this.getDetailSubscrip = this.managementReportService.getComments(this.ManagementReportId).subscribe(response => {
            this.allComments = response.data;
        },
        error => {});
    }
}