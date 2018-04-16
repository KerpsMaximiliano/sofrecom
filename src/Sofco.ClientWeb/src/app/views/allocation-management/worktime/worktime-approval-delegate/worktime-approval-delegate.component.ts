import { Component, Input, Output, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { ServiceService } from 'app/services/billing/service.service';
import { CustomerService } from 'app/services/billing/customer.service';
import { UserService } from 'app/services/admin/user.service';
import { I18nService } from 'app/services/common/i18n.service';
import { DataTableService } from 'app/services/common/datatable.service';
import { MessageService } from 'app/services/common/message.service';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { WorkTimeApprovalDelegateService } from 'app/services/allocation-management/worktime-approval-delegate.service';
import { Ng2DatatablesModule } from 'app/components/datatables/ng2-datatables.module';
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

    private customers: any[] = new Array<any>();
    public customerId: string = null;

    private services: any[] = new Array<any>();
    public serviceId: string = null;

    public users: any[] = new Array<any>();
    public userId: string = null;

    public approvers: any[] = new Array<any>();
    public approversUerId: string = null;

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
        private serviceService: ServiceService,
        private customerService: CustomerService,
        private usersService: UserService,
        private errorHandlerService: ErrorHandlerService,
        private dataTableService: DataTableService,
        private messageService: MessageService,
        private i18nService: I18nService,
        private router: Router) {
    }

    ngOnInit() {
        this.getCustomers();
        this.initServiceControl();
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
            customerId: this.customerId,
            serviceId: this.serviceId,
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

    getCustomers() {
        this.messageService.showLoading();
        this.subscription = this.customerService.getOptionsByCurrentManager().subscribe(res => {
            this.messageService.closeLoading();
            this.customers = this.sortCustomers(res.data);
            this.setCustomerSelect();
        },
        err => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(err);
        });
    }

    setCustomerSelect() {
        const data = this.mapToSelect(this.customers, this.model);
        const self = this;

        $('#customerControl').select2({
            data: data,
            allowClear: true,
            placeholder: this.i18nService.translateByKey('selectAnOption')
        });
        $('#customerControl').on('select2:unselecting', function(){
            self.customerId = null;
            self.serviceId = null;
            self.getWorkTimeApprovals();
            self.getApprovers();
        });
        $('#customerControl').on('select2:select', function(evt){
            const item = evt.params.data;
            self.customerId = item.id === this.nullId ? null : item.id;
            self.getServices();
            self.getWorkTimeApprovals();
            self.getApprovers();
        });
    }

    sortCustomers(data: Array<any>) {
        return data.sort(function (a, b) {
            return a.text.localeCompare(b.text);
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

    getServices(): void {
        if (this.customerId === null) { return; }
        this.subscription = this.serviceService.getOptions(this.customerId).subscribe(res => {
            this.services = res.data;
            this.updateServiceControl();
        },
        err => {
            this.errorHandlerService.handleErrors(err);
        });
    }

    updateServiceControl() {
        const options = $('#serviceControl').data('select2').options.options;
        options.data = this.mapToSelect(this.services, this.model);
        $('#serviceControl').empty().select2(options);
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
        const query = {
            customerId: this.customerId,
            serviceId: this.serviceId
        };
        this.subscription = this.workTimeApprovalDelegateService.getApprovals(query).subscribe(res => {
            this.approvers = res.data;
            this.updateApproverUserControl();
        },
        err => {
            this.errorHandlerService.handleErrors(err);
        });
    }

    initServiceControl() {
        const self = this;
        $('#serviceControl').select2({
            data: [],
            allowClear: true,
            placeholder: this.i18nService.translateByKey('selectAnOption')
        });
        $('#serviceControl').on('select2:unselecting', function(){
            self.serviceId = null;
            self.getWorkTimeApprovals();
            self.getApprovers();
        });
        $('#serviceControl').on('select2:select', function(evt){
            const item = evt.params.data;
            self.serviceId = item.id === this.nullId ? null : item.id;
            self.getWorkTimeApprovals();
            self.getApprovers();
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

    showService(): boolean {
        return this.customerId !== null;
    }

    showUser(): boolean {
        return this.serviceId !== null;
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
            $("#search-icon").toggleClass('fa-angle-down').toggleClass('fa-angle-left');
        } else {
            $("#search-icon").toggleClass('fa-angle-left').toggleClass('fa-angle-down');
        }
    }
}
