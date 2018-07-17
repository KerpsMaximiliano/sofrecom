import { OnInit, OnDestroy, Component, ViewChild } from "@angular/core";
import { DataTableService } from "app/services/common/datatable.service";
import { MessageService } from "app/services/common/message.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { WorktimeService } from "app/services/worktime-management/worktime.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";

declare var $: any;

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

    getEmployeesSubscrip: Subscription;
    getAnalyticsSubscrip: Subscription;
    searchSubscrip: Subscription;

    public analyticId = 0;
    public employeeId = 0;
    public comments: string;
    public rejectComments: string;

    indexToRemove: number;

    public isMultipleSelection = false;

    @ViewChild('commentsModal') commentsModal;

    @ViewChild('statusApprove') statusApprove;
    @ViewChild('statusReject') statusReject;

    public commentsModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "comments",
        "commentsModal",
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
        private messageService: MessageService,
        private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void {
        this.getAnalytics();

        const data = JSON.parse(sessionStorage.getItem('lastWorktimeQuery'));

        if(data){
            this.analyticId = data.analyticId;
            this.employeeId = data.employeeId;

            if(data.month > 0 && data.year > 0){
                $('#monthyear').val(data.month + '-' + data.year);
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
        },
        error => {
            this.errorHandlerService.handleErrors(error);
        });
    }

    getEmployees() {
        this.getAnalyticsSubscrip = this.analyticService.getResources(this.analyticId).subscribe(data => {
            this.employees = data;
        },
        error => {
            this.errorHandlerService.handleErrors(error);
        });
    }

    clean() {
        sessionStorage.removeItem('lastWorktimeQuery');
     
        this.analyticId = 0;
        this.employeeId = 0;
        $('#monthyear').val('');
    }

    searchPending(){
        var json = {
            analyticId : this.analyticId,
            employeeId : this.employeeId,
            month : 0,
            year : 0
        }

        this.messageService.showLoading();

        this.searchSubscrip = this.worktimeService.getWorkTimePending(json).subscribe(response => {
            this.messageService.closeLoading();
            if(response.messages) this.messageService.showMessages(response.messages);

            this.hoursPending = response.data.map(item => {
                item.selected = false;
                return item;
            });

            var options = { selector: "#hoursPending", scrollX: true, columnDefs: [ {'aTargets': [3], "sType": "date-uk"} ] };
            this.initGrid(options);

            sessionStorage.setItem('lastWorktimeQuery', JSON.stringify(json));
        },
        error => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(error);
        });
    }

    search() {
        var json = {
            analyticId : this.analyticId,
            employeeId : this.employeeId,
            month : 0,
            year : 0
        }

        var monthYear = $('#monthyear').val();

        if(monthYear && monthYear != ''){
            var monthYearSplitted = monthYear.split('-');
            json.month = monthYearSplitted[0];
            json.year = monthYearSplitted[1];
        }

        this.messageService.showLoading();

        this.searchSubscrip = this.worktimeService.getWorkTimeApproved(json).subscribe(response => {
            this.messageService.closeLoading();
            if(response.messages) this.messageService.showMessages(response.messages);

            this.hoursApproved = response.data;

            var options = { selector: "#hoursApproved", scrollX: true, columnDefs: [ {'aTargets': [3], "sType": "date-uk"} ] };
            this.initGrid(options);

            sessionStorage.setItem('lastWorktimeQuery', JSON.stringify(json));
        },
        error => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(error);
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
        var options = { selector: "#hoursPending", scrollX: true, columnDefs: [ {'aTargets': [3], "sType": "date-uk"} ] };
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
        var hoursSelected = this.hoursPending.filter(x => x.selected == true).map(item => item.id);

        if(hoursSelected.length == 0) return;

        this.messageService.showLoading();

        this.searchSubscrip = this.worktimeService.approveAll(hoursSelected).subscribe(response => {
            this.messageService.closeLoading();
            if(response.messages) this.messageService.showMessages(response.messages);
            this.searchPending();
        },
        error => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(error);
        });
    }

    rejectAll(){
        var hoursSelected = this.hoursPending.filter(x => x.selected == true).map(item => item.id);

        if(hoursSelected.length == 0) return;

        var json = {
            hourIds: hoursSelected,
            comments: this.rejectComments
        }

        this.searchSubscrip = this.worktimeService.rejectAll(json).subscribe(response => {
            this.rejectAllModal.hide();
            if(response.messages) this.messageService.showMessages(response.messages);
            this.searchPending();
        },
        error => {
            this.rejectAllModal.hide();
            this.errorHandlerService.handleErrors(error);
        });
    }
} 