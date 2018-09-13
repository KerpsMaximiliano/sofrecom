import { Component, Input, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { UserService } from '../../../services/admin/user.service';
import { I18nService } from '../../../services/common/i18n.service';
import { DataTableService } from '../../../services/common/datatable.service';
import { MessageService } from '../../../services/common/message.service';
import { Ng2ModalConfig } from '../../../components/modal/ng2modal-config';
import { UserApproverService } from '../../../services/allocation-management/user-approver.service';
import { AnalyticService } from "../../../services/allocation-management/analytic.service";
import { UserApproverType } from 'app/models/enums/userApproverType';
import { MenuService } from '../../../services/admin/menu.service';
declare var $: any;

@Component({
    selector: 'app-approvers',
    templateUrl: './approvers.component.html'
  })

export class ApproversComponent implements OnInit, OnDestroy {

    private nullId = '';

    private subscription: Subscription;

    private directorMode = false;

    @Input()
    public type: UserApproverType;

    private idKey = 'id';
    private textKey = 'text';

    public data: any[] = new Array<any>();

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

    public modalConfig2: Ng2ModalConfig = new Ng2ModalConfig(
        'ACTIONS.confirmTitle',
        'confirmModal2',
        true,
        true,
        'ACTIONS.DELETE',
        'ACTIONS.cancel');

    @ViewChild('confirmModal2') confirmModal2;

    constructor(private approversService: UserApproverService,
        private analyticService: AnalyticService,
        private usersService: UserService,
        private dataTableService: DataTableService,
        private messageService: MessageService,
        private menuService: MenuService,
        private i18nService: I18nService,
        private router: Router) {
    }

    ngOnInit() {
        this.directorMode = this.menuService.hasSectors();
        this.setApproversService();
        this.getAnalytics();
        this.initApproverUserControl();
        this.getUsers();
        this.getData();
    }

    setApproversService() {
        this.approversService.setType(this.type);
    }

    ngOnDestroy() {
        if (this.subscription) {
            this.subscription.unsubscribe();
        }
    }

    getData() {
        if((this.type == UserApproverType.WorkTime || !this.directorMode)
            && this.analyticId == 0){
            return;
        }
        this.loading = true;
        const query = {
            analyticId: this.analyticId,
            approvalId: this.approversUerId
        };
        this.subscription = this.approversService.getCurrentEmployees(query).subscribe(res => {
            this.loading = false;
            this.data = res.data.map(item => {
                item.checked = false;
                return item;
            });

            this.initTable();
        },
        () => this.loading = false);
    }

    getAnalytics() {
        this.subscription = this.analyticService.getByManager().subscribe(res => {
            this.analytics = res.data;
            this.setAnalyticSelect();
        });
    }

    setAnalyticSelect() {
        const data = this.mapToSelect(this.analytics);
        const self = this;

        $('#analyticsControl').select2({
            data: data,
            allowClear: true,
            placeholder: this.i18nService.translateByKey('selectAnOption')
        });
        $('#analyticsControl').on('select2:unselecting', function(){
            self.analyticId = 0;
            self.getData();
        });
        $('#analyticsControl').on('select2:select', function(evt){
            const item = evt.params.data;
            self.analyticId = item.id === this.nullId ? 0 : item.id;
            self.getData();
            self.getApprovers();
        });
    }

    mapToSelect(data: Array<any>): Array<any> {
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
        options.data = this.mapToSelect(this.approvers);
        $('#userApprovalControl').empty().select2(options);
    }

    updateUserControl() {
        const options = $('#userControl').data('select2').options.options;
        options.data = this.mapToSelect(this.users);
        $('#userControl').empty().select2(options);
    }

    getUsers(): void {
        this.subscription = this.usersService.getOptions().subscribe(res => {
            this.users = res;
            this.initUserControl();
        });
    }

    getApprovers(): void {
        this.subscription = this.approversService.getApprovals(this.analyticId).subscribe(res => {
            this.approvers = res.data;
            this.updateApproverUserControl();
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
            self.getData();
        });
        $('#userApprovalControl').on('select2:select', function(evt){
            const item = evt.params.data;
            self.approversUerId = item.id === this.nullId ? null : item.id;
            self.getData();
        });
    }

    initUserControl() {
        const data = this.mapToSelect(this.users);
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
        const isValid = this.data.filter(x => x.checked).length > 0;
        if (!isValid) {
            return false;
        }
        return this.userId !== null;
    }

    showSelectApprovers(): boolean {
        return this.data.filter(x => x.checked).length > 0;
    }

    showGrid(): boolean {
        return this.analyticId > 0 || this.data.length > 0;
    }

    showFilter(): boolean {
        if(this.type == UserApproverType.License){
            return this.directorMode;
        }
        return false;
    }

    showDelete(item): boolean {
        return item.userApprover != null;
    }

    initTable() {
        const options = { selector: "#resourcesTable" };
        this.dataTableService.destroy(options.selector);
        this.dataTableService.initialize(options);
    }

    goToAdd() {
        this.router.navigate(['/allocationManagement/workTimeApproval/delegate/edit']);
    }

    save() {
        this.setAddApprovers();
        this.messageService.showLoading();
        const data = this.data.filter(x => x.checked);

        this.subscription = this.approversService.save(data).subscribe(response => {
            this.messageService.closeLoading();
            this.updateUserControl();
            this.getData();
            this.getApprovers();
        },
        () => this.messageService.closeLoading());
    }

    setAddApprovers() {
        const adds = this.data.filter(x => x.checked);
        const self = this;
        adds.forEach(x => {
            x.approverUserId = self.userId;
        });
    }

    showConfirmDelete(item: any) {
        this.itemSelected = item;
        this.confirmModal.show();
    }

    processDelete() {
        const id = this.itemSelected.userApprover.id;
        this.confirmModal.hide();

        this.subscription = this.approversService.delete(id).subscribe(response => {
            this.getData();
        }); 
    }

    processDeleteAll(){
        const selecteds = this.data.filter(item => item.checked);
        const ids = selecteds.map(x => x.userApprover.id);

        this.subscription = this.approversService.deleteAll(ids).subscribe(response => {
            this.confirmModal2.hide();
            this.getData();
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
            $("#search-icon").toggleClass('fa-caret-down').toggleClass('fa-caret-up');
        } else {
            $("#search-icon").toggleClass('fa-caret-up').toggleClass('fa-caret-down');
        }
    }

    deleteAllEnable(){
        const selecteds = this.data.filter(item => item.checked);

        if(selecteds.length == 0) return false;

        return selecteds.every(item => item.userApprover != null);
    }
}