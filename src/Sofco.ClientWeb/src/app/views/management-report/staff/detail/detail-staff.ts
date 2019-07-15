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
    budgetSubscrip: Subscription;

    budgetAmount = new FormControl('', [Validators.required, Validators.min(0), Validators.max(999999999)]);
    budgetDescription = new FormControl('', [Validators.required, Validators.maxLength(200)]);
    budgetDate = new FormControl(null, [Validators.required]);

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
    budgetMode: string;
    budgetId: number;

    @ViewChild('dateReportStart') dateReportStart;
    @ViewChild('dateReportEnd') dateReportEnd;
    @ViewChild('editDateModal') editDateModal;
    @ViewChild('budgetModal') budgetModal;
    @ViewChild('costDetailMonth') costDetailMonth;

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

    private today: Date = new Date();

    constructor(private activatedRoute: ActivatedRoute,
        private messageService: MessageService,
        private menuService: MenuService,
        private managementReportService: ManagementReportStaffService,
        private datesService: DatesService,
        private i18nService: I18nService,
        private router: Router) { }

    ngOnInit(): void {
        this.editDateModal.size = 'modal-sm'

        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.ManagementReportId = params['id'];

            this.getDetail();
        });

        this.isManager = this.menuService.userIsManager;
        this.isCdgOrDirector = this.menuService.userIsDirector || this.menuService.userIsCdg;
        this.isCdg = this.menuService.userIsCdg;
    }

    ngOnDestroy(): void {
        if (this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if (this.getDetailSubscrip) this.getDetailSubscrip.unsubscribe();
        if (this.updateDatesSubscrip) this.updateDatesSubscrip.unsubscribe();
        if (this.sendSubscrip) this.sendSubscrip.unsubscribe();
        if (this.closeSubscrip) this.closeSubscrip.unsubscribe();
        if (this.budgetSubscrip) this.budgetSubscrip.unsubscribe();
    }

    getDetail() {
        this.messageService.showLoading();

        this.getDetailSubscrip = this.managementReportService.getDetail(this.ManagementReportId).subscribe(response => {

            this.model = response.data;
            
            if(this.menuService.user.id != this.model.managerId && this.menuService.userIsCdg == false){
                this.router.navigate(['/403']);
                return false;
            }

            this.ManagementReportId = response.data.managementReportId;

            this.setStartDate(this.model.manamementReportStartDate, this.model.manamementReportEndDate)

            this.readOnly = !this.canEdit();

            this.messageService.closeLoading();
        },
        () => this.messageService.closeLoading());
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

        this.costDetailMonth.open(data, this.readOnly || this.isClosed);
    }

    updateDetailCost() {
        // this.detailCost.getCost();
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
            this.updateDatesSubscrip = this.managementReportService.updateDates(this.ManagementReportId, model).subscribe(() => {

                this.messageService.closeLoading();
                this.model.manamementReportStartDate = this.ReportStartDate
                this.model.manamementReportEndDate = this.ReportEndDate
          
                this.getDetail()
                
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

    canEdit(){
        if(!this.model || !this.model.status) return false;

        if(this.model.status == ManagementReportStatus.CdgPending && this.isCdg) return true;

        if(this.model.status == ManagementReportStatus.ManagerPending && this.isManager) return true;

        return false;
    }

    close(){
        var json = {
            date: this.selectedDate,
            // billingId: this.billing.getId(this.selectedDate),
            // detailCostId: this.detailCost.getId(this.selectedDate)
        };
    
        this.messageService.showConfirm(() => {
            this.messageService.showLoading();

            this.closeSubscrip = this.managementReportService.close(json).subscribe(() => {
                this.messageService.closeLoading();
            },
            () => this.messageService.closeLoading());
        });
    }

    isSaveEnabled(){
        if(this.budgetAmount.valid && this.budgetDescription.valid && this.budgetDate.valid) return true;

        return false;
    }

    openBudgetModal(mode, item){
        this.budgetMode = mode;

        if(this.budgetMode == 'edit'){
            this.budgetAmount.setValue(item.value);
            this.budgetDescription.setValue(item.description);
            this.budgetDate.setValue(moment(item.startDate).format("DD/MM/YYYY"));
            this.budgetModalConfig.deleteButton = true;
            this.budgetId = item.id;
        }
        else{
            this.budgetAmount.setValue(null);
            this.budgetDescription.setValue(null);
            this.budgetDate.setValue(null);
            this.budgetModalConfig.deleteButton = false;
        }

        this.budgetModal.show();
    }

    saveBudget(){
        if(this.budgetMode == 'edit'){
            this.updateBudget();
        }
        else{
            this.addBudget();
        }
    }

    addBudget(){
        var json = {
            id: 0,
            description: this.budgetDescription.value,
            value: this.budgetAmount.value,
            startDate: this.budgetDate.value
        };

        this.budgetSubscrip = this.managementReportService.addBudget(this.ManagementReportId, json).subscribe(response => {
            this.budgetModal.hide();

            if(response.data){
                this.model.budgets.push(response.data);
            }
        },
        () => this.budgetModal.hide());
    }

    updateBudget(){
        var json = {
            id: this.budgetId,
            description: this.budgetDescription.value,
            value: this.budgetAmount.value,
            startDate: this.budgetDate.value
        };

        this.budgetSubscrip = this.managementReportService.updateBudget(this.ManagementReportId, json).subscribe(() => {
            this.budgetModal.hide();

            var item = this.model.budgets.find(x => x.id == this.budgetId);

            if(item){
                item.description = this.budgetDescription.value;
                item.value = this.budgetAmount.value;
                item.startDate = json.startDate;
            }

            this.budgetId = 0;
        },
        () => this.budgetModal.hide());
    }

    deleteBudget(){
        this.budgetSubscrip = this.managementReportService.deleteBudget(this.ManagementReportId, this.budgetId).subscribe(() => {
            this.budgetModal.hide();

            var index = this.model.budgets.findIndex(x => x.id == this.budgetId);

            if(index > 0){
                this.model.budgets.splice(index, 1);
            }
        },
        () => this.budgetModal.hide());
    } 
}