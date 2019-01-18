import { OnInit, OnDestroy, Component, ViewChild } from "@angular/core";
import { DataTableService } from "../../../services/common/datatable.service";
import { MessageService } from "../../../services/common/message.service";
import { Subscription } from "rxjs";
import { AnalyticService } from "../../../services/allocation-management/analytic.service";
import { WorktimeService } from "../../../services/worktime-management/worktime.service";
import { Ng2ModalConfig } from "../../../components/modal/ng2modal-config";
import { DateRangePickerComponent } from "app/components/date-range-picker/date-range-picker.component";

declare var $: any;
declare var moment: any;

@Component({
    selector: 'app-worktime-approval',
    templateUrl: './worktime-approval.component.html',
    styleUrls: ['./worktime-approval.component.scss']
})
export class WorkTimeApprovalComponent implements OnInit, OnDestroy {

    public analytics: any[] = new Array();
    public employees: any[] = new Array();
    public hoursApproved: any[] = new Array();
    public hoursPending: any[] = new Array();
    public bankHours: any[] = new Array();

    getEmployeesSubscrip: Subscription;
    getAnalyticsSubscrip: Subscription;
    searchSubscrip: Subscription;

    public analyticId = 0;
    public employeeId = 0;
    public comments: string;
    public rejectComments: string;

    indexToRemove: number;

    public isMultipleSelection = false;
    public filterByDates = false;

    @ViewChild('commentsModal') commentsModal;

    @ViewChild('statusApprove') statusApprove;
    @ViewChild('statusReject') statusReject;

    @ViewChild('dateRangePicker') dateRangePicker:DateRangePickerComponent;

    public commentsModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "comments",
        "commentsModal",
        false,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.close"
    );

    @ViewChild('bankHoursModal') bankHoursModal;
    public bankHoursModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "workTimeManagement.bankHoursTitle",
        "bankHoursModal",
        false,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.close"
    );

    @ViewChild('rejectAllModal') rejectAllModal;
    public rejectAllModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "rejectAllModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    constructor(private analyticService: AnalyticService,
        private worktimeService: WorktimeService,
        private datatableService: DataTableService,
        private messageService: MessageService){
    }

    ngOnInit(): void {
        this.getAnalytics();

        const data = JSON.parse(sessionStorage.getItem('lastWorktimeQuery'));

        if(data){
            this.analyticId = data.analyticId;
            this.employeeId = data.employeeId;
            this.filterByDates = data.filterByDates;

            if(this.filterByDates){
                this.dateRangePicker.start = moment(data.startDate);
                this.dateRangePicker.start = moment(data.endDate);
            }
            
            if(this.employeeId > 0){
                this.getEmployees();
            }
        }
    }

    ngOnDestroy(): void {
        if(this.getEmployeesSubscrip) this.getEmployeesSubscrip.unsubscribe();
        if(this.getAnalyticsSubscrip) this.getAnalyticsSubscrip.unsubscribe();
        if(this.searchSubscrip) this.searchSubscrip.unsubscribe();
    }

    getAnalytics() {
        this.getAnalyticsSubscrip = this.worktimeService.getAnalytics().subscribe(data => {
            this.analytics = data;
        });
    }

    getEmployees() {
        this.getAnalyticsSubscrip = this.analyticService.getResources(this.analyticId).subscribe(data => {
            this.employees = data;
        });
    }

    clean() {
        sessionStorage.removeItem('lastWorktimeQuery');
        this.employees = [];
        this.analyticId = 0;
        this.employeeId = 0;
        this.filterByDates = false;
    }

    searchPending(){
        var json = {
            analyticId : this.analyticId,
            employeeId : this.employeeId
        }

        this.messageService.showLoading();

        this.searchSubscrip = this.worktimeService.getWorkTimePending(json).subscribe(response => {
            this.messageService.closeLoading();

            this.hoursPending = response.data.map(item => {
                item.selected = false;
                return item;
            });

            var options = { selector: "#hoursPending", 
                            scrollX: true, 
                            columns: [1, 2, 3, 4, 5, 6],
                            title: `Horas-Pendientes-${moment(new Date()).format("YYYYMMDD")}`,
                            withExport: true,
                            columnDefs: [ {'aTargets': [3], "sType": "date-uk"} ] 
                          };

            this.initGrid(options);

            sessionStorage.setItem('lastWorktimeQuery', JSON.stringify(json));
        },
        error => {
            this.messageService.closeLoading();
        });
    }

    search() {
        var json = {
            analyticId : this.analyticId,
            employeeId : this.employeeId,
            startDate :  this.filterByDates ? this.dateRangePicker.start.toDate() : null,
            endDate:  this.filterByDates ? this.dateRangePicker.end.toDate() : null,
            filterByDates: this.filterByDates
        }

        this.messageService.showLoading();

        this.searchSubscrip = this.worktimeService.getWorkTimeApproved(json).subscribe(response => {
            this.messageService.closeLoading();

            this.hoursApproved = response.data;

            var options = { selector: "#hoursApproved", 
                scrollX: true, 
                columns: [0, 1, 2, 3, 4, 5, 7],
                title: `Horas-Aprobadas-${moment(new Date()).format("YYYYMMDD")}`,
                withExport: true,
                columnDefs: [ {'aTargets': [3], "sType": "date-uk"} ] 
            };

            this.initGrid(options);

            sessionStorage.setItem('lastWorktimeQuery', JSON.stringify(json));
        },
        error => {
            this.messageService.closeLoading();
        });
    }

    initGrid(options){
        this.datatableService.destroy(options.selector);
        this.datatableService.initialize(options);
    }

    showComments(item){
        this.comments = item.comments;
        this.commentsModal.show();
    }

    showApproveModal(worktime, index){
        this.indexToRemove = index;
        this.statusApprove.approve(worktime);
    }

    showRejectModal(worktime, index){
        this.indexToRemove = index;
        this.statusReject.reject(worktime);
    }

    removeItem(){
        this.hoursPending.splice(this.indexToRemove, 1);
        var options = { selector: "#hoursPending", 
            scrollX: true, 
            columns: [1, 2, 3, 4, 5, 6],
            title: `Horas-Pendientes-${moment(new Date()).format("YYYYMMDD")}`,
            withExport: true,
            columnDefs: [ {'aTargets': [4], "sType": "date-uk"} ]
        };

        this.initGrid(options);
    }

    approveAllDisabled(){
        return this.hoursPending.filter(x => x.selected == true).length == 0;
    }

    areAllSelected(){
        return this.hoursPending.every(item => {
            return item.selected == true;
        });
    }

    areAllUnselected(){
        return this.hoursPending.every(item => {
            return item.selected == false;
        });
    }

    selectAll(){
        this.hoursPending.forEach((item, index) => {
            item.selected = true;
        });
    }

    unselectAll(){
        this.hoursPending.forEach((item, index) => {
            item.selected = false;
        });
    }

    approveAll(){
        var hoursSelected = this.hoursPending.filter(x => x.selected == true).map(item => 
        { 
            return { id: item.id, hoursSplitteds: item.hoursSplitteds }
        });

        if(hoursSelected.length == 0) return;

        this.messageService.showLoading();

        this.searchSubscrip = this.worktimeService.approveAll(hoursSelected).subscribe(response => {
            this.messageService.closeLoading();
            this.searchPending();
        },
        error => {
            this.messageService.closeLoading();
        });
    }

    rejectAll(){
        if(!this.rejectComments || this.rejectComments == ""){
            this.messageService.showError("billing.solfac.rejectCommentRequired");
            this.rejectAllModal.resetButtons();
            return;
        }

        var hoursSelected = this.hoursPending.filter(x => x.selected == true).map(item => item.id);

        if(hoursSelected.length == 0) return;

        var json = {
            hourIds: hoursSelected,
            comments: this.rejectComments
        }

        this.searchSubscrip = this.worktimeService.rejectAll(json).subscribe(response => {
            this.rejectAllModal.hide();
            this.searchPending();
        },
        error => {
            this.rejectAllModal.hide();
        });
    }

    showBankHoursModal(item){
        this.bankHours = item.hoursSplitteds;
        this.bankHoursModal.show();
    }
} 