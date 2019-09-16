import { OnInit, Component, OnDestroy, ViewChild } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { MenuService } from "app/services/admin/menu.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { Router } from '@angular/router';
import { DatesService } from "app/services/common/month.service";
import { ManagementReportStatus } from "app/models/enums/managementReportStatus";
import { I18nService } from "app/services/common/i18n.service";
import { ManagementReportStaffService } from "app/services/management-report/management-report-staff.service";
import { FormControl, Validators } from "@angular/forms";
import * as moment from 'moment';
import { UserInfoService } from "app/services/common/user-info.service";
import { DataTableService } from "app/services/common/datatable.service";
import { ManagementReportService } from "app/services/management-report/management-report.service";

@Component({
    selector: 'management-report-detail-staff',
    templateUrl: './detail-staff.html',
    styleUrls: ['./detail-staff.scss']
})
export class ManagementReportDetailStaffComponent implements OnInit, OnDestroy {

    paramsSubscrip: Subscription;
    getDetailSubscrip: Subscription;
    updateDatesSubscrip: Subscription;
    sendSubscrip: Subscription;
    closeSubscrip: Subscription;

    public model: any;

    isManager: boolean = false;
    isCdgOrDirector: boolean = false;
    isCdg: boolean = false;

    selectedDate: Date = new Date(new Date().getFullYear(), new Date().getMonth(), 1);
    public selectedMonth: number;
    public selectedYear: number;
    public selectedMonthDesc: string;
    selectedExchanges: any[] = new Array();

    comments: string;
    allComments: any[] = new Array();
    months: any[] = new Array();

    public addCommentModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Agregar comentario",
        "addCommentModal",
        true,
        true,
        "ACTIONS.ADD",
        "ACTIONS.cancel"
    );

    ReportStartDate: Date;
    ReportEndDate: Date;
    ReportStartDateError: boolean = false;
    ReportEndDateError: boolean = false;
    readOnly: boolean = false;
    isClosed: boolean = false;
    ManagementReportId: number;
    actualState: string

    @ViewChild('dateReportStart') dateReportStart;
    @ViewChild('dateReportEnd') dateReportEnd;
    @ViewChild('editDateModal') editDateModal;
    @ViewChild('budgetHistoryModal') budgetHistoryModal;
    @ViewChild('costDetailMonth') costDetailMonth;
    @ViewChild("budget") budgetView;
    @ViewChild("tracing") tracingView;
    @ViewChild('addCommentModal') addCommentModal;

    public editDateModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Editar Fechas",
        "editDateModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    public budgetModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Presupuesto",
        "budgetModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    public budgetHistoryModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Presupuesto",
        "budgetHistorialModal",
        false,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.close"
    );

    private today: Date = new Date();

    constructor(private activatedRoute: ActivatedRoute,
        private messageService: MessageService,
        private dataTableService: DataTableService,
        private menuService: MenuService,
        private managementReportService: ManagementReportStaffService,
        private mrService: ManagementReportService,
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
                this.ManagementReportId = params['id'];
    
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
    }
 
    getDetail() {
        this.messageService.showLoading();
        const userInfo = UserInfoService.getUserInfo();

        this.getDetailSubscrip = this.managementReportService.getDetail(this.ManagementReportId).subscribe(response => {
            this.messageService.closeLoading();

            this.getComment();

            this.model = response.data;
            this.months = response.data.months;

            if(this.isManager){
                if(this.model.managerId != userInfo.id && this.model.delegateId != userInfo.id){
                    this.isManager = false;
                }
            }

            this.ManagementReportId = response.data.managementReportId;

            this.setStartDate(this.model.manamementReportStartDate, this.model.manamementReportEndDate)
            this.readOnly = !this.canEdit();
            this.budgetView.readOnly = !this.canEdit();
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
        var data = {
            isCdg: this.menuService.userIsCdg,
            managementReportId: this.ManagementReportId,
            resources: [], 
            totals: [], 
            AnalyticId: this.model.analyticId,
            month: this.selectedMonth, 
            monthDesc: this.selectedMonthDesc,
            year: this.selectedYear
        }
        this.costDetailMonth.open(data, !this.canEditDetailMonth() || this.isClosed);
    }

    updateBudgetView() {
        this.budgetView.getCost(this.ManagementReportId);
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
        const dateAnalyticStart = new Date(new Date(this.model.startDate).getFullYear(), new Date(this.model.startDate).getMonth(), 1)

        if (dateReportStart < dateAnalyticStart) {
            this.ReportStartDateError = true
            this.editDateModal.resetButtons()
            this.messageService.showError("managementReport.startDateOutOfRange")
        }
        else{
            this.ReportStartDateError = false
        }

        if (this.ReportStartDateError == false && this.ReportEndDateError == false) {

            var model = {
                StartDate: new Date(this.ReportStartDate),
                EndDate: new Date(this.ReportEndDate)
            }
            this.updateDatesSubscrip = this.managementReportService.updateDates(this.ManagementReportId, model).subscribe(() => {

                this.messageService.closeLoading();
                this.model.manamementReportStartDate = this.ReportStartDate
                this.model.manamementReportEndDate = this.ReportEndDate
          
                this.getDetail()
                this.updateBudgetView()
                this.setStartDate(this.ReportStartDate, this.ReportEndDate)

                this.editDateModal.hide();
            },
                () => {
                    this.messageService.closeLoading()
                    this.editDateModal.resetButtons()
                }
            );
        }
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

        this.isClosed = this.budgetView.isClosed(this.selectedDate);

        var month = this.months.find(x => x.year == this.selectedYear && x.month == this.selectedMonth);

        if(month){
            this.selectedExchanges = month.items;
        }

        //this.budgetView.selectDefaultColumn(this.selectedDate)
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

            this.updateDatesSubscrip = this.managementReportService.send(json).subscribe(() => {
                this.model.status = status;
                this.messageService.closeLoading();
            },
            () => this.messageService.closeLoading());
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

    canEditDetailMonth(){
        if(!this.model || !this.model.status) return false;

        if(this.model.status == ManagementReportStatus.CdgPending && this.isCdg) return true;

        return false;
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
            detailCostId: this.budgetView.getId(this.selectedDate)
        };
    
        this.messageService.showConfirm(() => {
            this.messageService.showLoading();

            this.closeSubscrip = this.managementReportService.close(json).subscribe(() => {
                this.messageService.closeLoading();
            },
            () => this.messageService.closeLoading());
        });
    }

    getBudgetData(event){
        this.tracingView.calculateAccumulated(event.months)
        this.actualState = event.actualState
    }

    openBudgetHistory(){
        this.budgetHistoryModal.show();    

        var params = {
            selector: '#budgetTable',
            columnDefs: [ {"aTargets": [3], "sType": "date-uk"} ]
          }

        this.dataTableService.destroy(params.selector);
        this.dataTableService.initialize(params)
    }

    saveBudget(){
        this.budgetView.save()
    }

    closeState(){
        this.budgetView.save(true)
    }
    generatePFA1(){
        this.budgetView.generatePfa(1)
    }

    generatePFA2(){
        this.budgetView.generatePfa(2)
    }

    openComments(){
        this.addCommentModal.show();
    }

    saveComment(){
        var json = {
            id: this.ManagementReportId,
            comment: this.comments
        };

        this.getDetailSubscrip = this.mrService.addComment(json).subscribe(response => {
            this.addCommentModal.hide();

            this.allComments.unshift(response.data);
            this.comments = "";
        },
        error => this.addCommentModal.hide());
    }

    getComment(){
        this.getDetailSubscrip = this.mrService.getComments(this.ManagementReportId).subscribe(response => {
            this.allComments = response.data;
        },
        error => {});
    }
}