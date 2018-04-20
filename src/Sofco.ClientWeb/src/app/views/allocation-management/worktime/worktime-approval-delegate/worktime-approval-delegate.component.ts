import { Component, Input, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { UserService } from 'app/services/admin/user.service';
import { I18nService } from 'app/services/common/i18n.service';
import { DataTableService } from 'app/services/common/datatable.service';
import { MessageService } from 'app/services/common/message.service';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { WorkTimeApprovalDelegateService } from 'app/services/allocation-management/worktime-approval-delegate.service';
import { Ng2DatatablesModule } from 'app/components/datatables/ng2-datatables.module';
import { AnalyticService } from "app/services/allocation-management/analytic.service";
declare var $: any;

@Component({
    selector: 'app-worktime-approval-delegate',
    templateUrl: './worktime-approval-delegate.component.html'
  })

export class WorkTimeApprovalDelegateComponent implements OnInit, OnDestroy {

    private nullId = '';

    private subscription: Subscription;

    @Input()
    public model: any;

    private idKey = 'id';
    private textKey = 'text';

    public workTimeApprovals: any[] = new Array<any>();
    public workTimeApprovalId: string = null;

    public users: any[] = new Array<any>();
    public userId: string = null;

    public approvers: any[] = new Array<any>();
    public approversUerId: string = null;

    public analytics: any[] = new Array();
    public analyticId = 0;

    private itemSelected: any;

    public loading = false;

    public modalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        'ACTIONS.confirmTitle',
        'confirmModal',
        true,
        true,
        'ACTIONS.DELETE',
        'ACTIONS.cancel');

    @ViewChild('confirmModal') confirmModal;

    constructor(private workTimeApprovalDelegateService: WorkTimeApprovalDelegateService,
        private analyticService: AnalyticService,
        private usersService: UserService,
        private errorHandlerService: ErrorHandlerService,
        private dataTableService: DataTableService,
        private messageService: MessageService,
        private i18nService: I18nService,
        private router: Router) {
    }

    ngOnInit() {
        this.getAnalytics();
        this.initApproverUserControl();
        this.getUsers();
        this.getWorkTimeApprovals();
    }

    ngOnDestroy() {
        if (this.subscription) {
            this.subscription.unsubscribe();
        }
    }

    getWorkTimeApprovals() {
        this.loading = true;
        const query = {
            analyticId: this.analyticId,
            approvalId: this.approversUerId
        };
        this.subscription = this.workTimeApprovalDelegateService.getCurrentEmployees(query).subscribe(res => {
            this.loading = false;
            this.workTimeApprovals = res.data;
            this.initTable();
        },
        err => {
            this.loading = false;
            this.errorHandlerService.handleErrors(err);
        });
    }

    getAnalytics() {
        this.subscription = this.analyticService.getByCurrentUser().subscribe(res => {
            this.analytics = res.data;
            this.setAnalyticSelect();
        },
        error => {
            this.errorHandlerService.handleErrors(error);
        });
    }

    setAnalyticSelect() {
        const data = this.mapToSelect(this.analytics, this.model);
        const self = this;

        $('#analyticsControl').select2({
            data: data,
            allowClear: true,
            placeholder: this.i18nService.translateByKey('selectAnOption')
        });
        $('#analyticsControl').on('select2:unselecting', function(){
            self.analyticId = 0;
            self.getWorkTimeApprovals();
        });
        $('#analyticsControl').on('select2:select', function(evt){
            const item = evt.params.data;
            self.analyticId = item.id === this.nullId ? 0 : item.id;
            self.getWorkTimeApprovals();
            self.getApprovers();
        });
    }

    mapToSelect(data: Array<any>, selectedOption: string): Array<any> {
        const result = new Array<any>();
        result.push({id: this.nullId, text: ''});
        data.forEach(s => {
            const text = s[this.textKey];
            result.push({
                id: s[this.idKey],
                text: text,
                selected: false
            });
        });
        return result;
      }

    updateApproverUserControl() {
        const options = $('#userApprovalControl').data('select2').options.options;
        options.data = this.mapToSelect(this.approvers, this.model);
        $('#userApprovalControl').empty().select2(options);
    }

    getUsers(): void {
        this.subscription = this.usersService.getOptions().subscribe(res => {
            this.users = res;
            this.initUserControl();
        },
        err => {
            this.errorHandlerService.handleErrors(err);
        });
    }

    getApprovers(): void {
        this.subscription = this.workTimeApprovalDelegateService.getApprovals(this.analyticId).subscribe(res => {
            this.approvers = res.data;
            this.updateApproverUserControl();
        },
        err => {
            this.errorHandlerService.handleErrors(err);
        });
    }

    initApproverUserControl() {
        const self = this;
        $('#userApprovalControl').select2({
            data: [],
            allowClear: true,
            placeholder: this.i18nService.translateByKey('selectAnOption')
        });
        $('#userApprovalControl').on('select2:unselecting', function(){
            self.approversUerId = null;
            self.getWorkTimeApprovals();
        });
        $('#userApprovalControl').on('select2:select', function(evt){
            const item = evt.params.data;
            self.approversUerId = item.id === this.nullId ? null : item.id;
            self.getWorkTimeApprovals();
        });
    }

    initUserControl() {
        const data = this.mapToSelect(this.users, this.model);
        const self = this;

        $('#userControl').select2({
            data: data,
            allowClear: true,
            placeholder: this.i18nService.translateByKey('selectAnOption'),
            width: '100%'
        });
        $('#userControl').on('select2:unselecting', function(){
            self.userId = null;
        });
        $('#userControl').on('select2:select', function(evt){
            const item = evt.params.data;
            self.userId = item.id === this.nullId ? null : item.id;
        });
    }

    showUser(): boolean {
        return this.analyticId > 0;
    }

    showSave(): boolean {
        const isValid = this.workTimeApprovals.filter(x => x.checked).length > 0;
        if (!isValid) {
            return false;
        }
        return this.userId !== null;
    }

    showDelete(item): boolean {
        return item.workTimeApproval != null;
    }

    initTable() {
        const options = { selector: "#resourcesTable" };
        this.dataTableService.destroy(options.selector);
        this.dataTableService.init2(options);
    }

    goToAdd() {
        this.router.navigate(['/allocationManagement/workTimeApproval/delegate/edit']);
    }

    save() {
        this.setAddApprovers();
        this.messageService.showLoading();
        const data = this.workTimeApprovals.filter(x => x.checked);

        this.subscription = this.workTimeApprovalDelegateService.save(data).subscribe(response => {
            this.messageService.closeLoading();
            if(response.messages) this.messageService.showMessages(response.messages);
            this.getWorkTimeApprovals();
        },
        err => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(err);
        });
    }

    setAddApprovers() {
        const adds = this.workTimeApprovals.filter(x => x.checked);
        const self = this;
        adds.forEach(x => {
            x.approvalUserId = self.userId;
        });
    }

    showConfirmDelete(item: any) {
        this.itemSelected = item;
        this.confirmModal.show();
    }

    processDelete() {
        const id = this.itemSelected.workTimeApproval.id;
        this.confirmModal.hide();
        this.subscription = this.workTimeApprovalDelegateService.delete(id).subscribe(response => {
            if(response.messages) this.messageService.showMessages(response.messages);
            this.getWorkTimeApprovals();
        },
        err => {
            this.errorHandlerService.handleErrors(err);
        });
    }

    collapse() {
        if ($("#collapseOne").hasClass('in')) {
            $("#collapseOne").removeClass('in');
        } else {
            $("#collapseOne").addClass('in');
        }
        this.changeIcon();
    }

    changeIcon() {
        if ($("#collapseOne").hasClass('in')) {
            $("#search-icon").toggleClass('fa-minus').toggleClass('fa-plus');
        } else {
            $("#search-icon").toggleClass('fa-plus').toggleClass('fa-minus');
        }
    }
}
