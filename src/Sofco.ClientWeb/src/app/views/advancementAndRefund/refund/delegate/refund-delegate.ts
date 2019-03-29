import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { RefundService } from "app/services/advancement-and-refund/refund.service";
import { UserService } from "app/services/admin/user.service";
import { MessageService } from "app/services/common/message.service";
import { DataTableService } from "app/services/common/datatable.service";
import { Subscription } from "rxjs";
import { UserApproverService } from "app/services/allocation-management/user-approver.service";
import { UserApproverType } from "app/models/enums/userApproverType";
import { MenuService } from "app/services/admin/menu.service";

@Component({
    selector: 'refund-delegate',
    templateUrl: './refund-delegate.html'
})

export class RefundDelegateComponent implements OnInit, OnDestroy {

    public data: any[] = new Array<any>();

    public users: any[] = new Array<any>();
    public userId: number;

    public analytics: any[] = new Array();
    public analyticId: number;

    private itemSelected: any;

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

    private getAnalyticsSubscription: Subscription;
    private getUsersSubscription: Subscription;
    private getDataSubscription: Subscription;
    private postSubscription: Subscription;
    private deleteSubscription: Subscription;

    constructor(private refundService: RefundService,
                private userService: UserService,
                private messageService: MessageService,
                private menuService: MenuService,
                private approversService: UserApproverService,
                private dataTableService: DataTableService){
    }

    ngOnInit(): void {
        this.approversService.setType(UserApproverType.Refund);

        this.getUsers();
        this.getAnalytics();
        this.getData();
    }

    ngOnDestroy(): void {
        if (this.getAnalyticsSubscription) this.getAnalyticsSubscription.unsubscribe();
        if (this.getUsersSubscription) this.getUsersSubscription.unsubscribe();
        if (this.getDataSubscription) this.getDataSubscription.unsubscribe();
        if (this.postSubscription) this.postSubscription.unsubscribe();
        if (this.deleteSubscription) this.deleteSubscription.unsubscribe();
    }

    getAnalytics() {
        this.getAnalytics = this.refundService.getAnalytics().subscribe(res => {
            this.analytics = res.data;
        });
    }

    getUsers() {
        this.getUsersSubscription = this.userService.getOptions().subscribe(res => {
            this.users = res;
        });
    }

    getData() {
        var userId = this.menuService.user.id;

        this.messageService.showLoading();

        this.getDataSubscription = this.approversService.getByUserId(userId).subscribe(res => {
            this.messageService.closeLoading();

            this.data = res.data.map(item => {
                item.checked = false;
                return item;
            });

            this.initTable();
        },
        error => this.messageService.closeLoading());
    }

    initTable() {
        const options = { selector: "#resourcesTable" };
        this.dataTableService.destroy(options.selector);
        this.dataTableService.initialize(options);
    }

    save() {
        var json ={
            id: 0,
            analyticId: this.analyticId,
            ApproverUserId: this.userId,
            userId: this.menuService.user.id,
            employeeId: this.menuService.user.employeeId
        }

        this.messageService.showLoading();

        this.postSubscription = this.approversService.save([json]).subscribe(response => {
            this.messageService.closeLoading();

            this.getData();
        },
        () => this.messageService.closeLoading());
    }

    showSave(): boolean {
        if(this.analyticId > 0 && this.userId > 0) return true;

        return false;
    }

    showConfirmDelete(item: any) {
        this.itemSelected = item;
        this.confirmModal.show();
    }

    processDelete() {
        const id = this.itemSelected.id;
        this.confirmModal.hide();

        this.deleteSubscription = this.approversService.delete(id).subscribe(response => {
            this.getData();
        }); 
    }

    processDeleteAll(){
        const selecteds = this.data.filter(item => item.checked);
        const ids = selecteds.map(x => x.id);

        this.deleteSubscription = this.approversService.deleteAll(ids).subscribe(response => {
            this.confirmModal2.hide();
            this.getData();
        });
    }

    deleteAllEnable(){
        const selecteds = this.data.filter(item => item.checked);

        if(selecteds.length == 0) return false;

        return true;
    }

    areAllSelected(){
        return this.data.every(item => {
            return item.checked == true;
        });
    }

    areAllUnselected(){
        return this.data.every(item => {
            return item.checked == false;
        });
    }

    selectAll(){
        this.data.forEach((item) => {
            item.checked = true;
        });
    }

    unselectAll(){
        this.data.forEach((item) => {
            item.checked = false;
        });
    }
}