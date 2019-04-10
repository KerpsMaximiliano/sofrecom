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
    public dataApprovers: any[] = new Array<any>();
    public dataDelegates: any[] = new Array<any>();

    public managersAndDirectors: any[] = new Array<any>();
    public users: any[] = new Array<any>();

    public allUsers: any[] = new Array<any>();
    public userId: number;

    public analytics: any[] = new Array();
    public analyticId: number;

    private itemSelected: any;

    public type = "1";
    analyticDisabled: boolean = false;

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
    private getData2Subscription: Subscription;
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
        this.getManagersAndDirectors();
        this.getAnalytics();
        this.getData();
    }

    ngOnDestroy(): void {
        if (this.getAnalyticsSubscription) this.getAnalyticsSubscription.unsubscribe();
        if (this.getUsersSubscription) this.getUsersSubscription.unsubscribe();
        if (this.getDataSubscription) this.getDataSubscription.unsubscribe();
        if (this.postSubscription) this.postSubscription.unsubscribe();
        if (this.deleteSubscription) this.deleteSubscription.unsubscribe();
        if (this.getData2Subscription) this.getData2Subscription.unsubscribe();
    }

    getAnalytics() {
        this.getAnalytics = this.refundService.getAnalytics().subscribe(res => {
            this.analytics = res.data;
        });
    }

    getUsers() {
        this.getUsersSubscription = this.userService.getOptions().subscribe(res => {
            this.users = res;
            this.allUsers = res;
        });
    }

    getManagersAndDirectors() {
        this.getUsersSubscription = this.userService.getManagersAndDirectors().subscribe(res => {
            this.managersAndDirectors = res;
        });
    }

    getData() {
        var promises = new Array();

        var userId = this.menuService.user.id;

        var promise1 = new Promise((resolve, reject) => {
            this.getDataSubscription = this.approversService.getByUserId(userId).subscribe(res => {
                this.dataApprovers = res.data.map(item => {
                    item.checked = false;
                    item.type = 'Aprobación';
                    item.typeId = '1';
                    return item;
                });
    
                resolve();
            },
            error => resolve());
        });

        var promise2 = new Promise((resolve, reject) => {

            this.getData2Subscription = this.refundService.getDelegates().subscribe(res => {
                this.dataDelegates = res.data.map(delegate => {
                    var item = {
                        checked: false,
                        id: delegate.id,
                        approverName: delegate.text,
                        analyticName: '',
                        type: 'Generación',
                        typeId: '2'
                    }
                    
                    return item;
                });
    
                resolve();
            },
            error => resolve());
        });

        promises.push(promise1);
        promises.push(promise2);
  
        this.messageService.showLoading();

        Promise.all(promises).then(data => { 
            this.messageService.closeLoading();

            this.data = [];

            this.dataApprovers.forEach(x => {
                this.data.push(x);
            });

            this.dataDelegates.forEach(x => {
                this.data.push(x);
            });

            this.initTable();
        });
    }

    typeChange(){
        this.userId = null;

        if(this.type == '1'){
            this.analyticDisabled = false;
            this.allUsers = this.users;
        }
        else{
            this.analyticDisabled = true;
            this.analyticId = null;
            this.allUsers = this.managersAndDirectors;
        }
    }

    initTable() {
        const options = { selector: "#resourcesTable" };
        this.dataTableService.destroy(options.selector);
        this.dataTableService.initialize(options);
    }

    save() {
        if(this.type == '1'){
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

        if(this.type == '2'){
            this.messageService.showLoading();

            this.postSubscription = this.refundService.addDelegate(this.userId).subscribe(response => {
                this.messageService.closeLoading();
    
                this.getData();
            },
            () => this.messageService.closeLoading());
        }
    }

    showSave(): boolean {
        if(this.type == '1' && this.analyticId > 0 && this.userId > 0){
            return true;
        }
        
        if(this.type == '2' && this.userId > 0){
            return true;
        }

        return false;
    }

    showConfirmDelete(item: any) {
        this.itemSelected = item;
        this.confirmModal.show();
    }

    processDelete() {
        const id = this.itemSelected.id;

        if(this.itemSelected.typeId == '1'){
            this.confirmModal.hide();

            this.deleteSubscription = this.approversService.delete(id).subscribe(response => {
                this.getData();
            }); 
        }

        if(this.itemSelected.typeId == '2'){
            this.confirmModal.hide();

            this.deleteSubscription = this.refundService.deleteDelegate([id]).subscribe(response => {
                this.getData();
            }); 
        }
    }

    processDeleteAll(){
        var promises = new Array();

        var promise1 = new Promise((resolve, reject) => {
            const approversSelecteds = this.data.filter(item => item.checked && item.typeId == '1');
            const approversIds = approversSelecteds.map(x => x.id);
    
            if(approversIds && approversIds.length > 0){
                this.deleteSubscription = this.approversService.deleteAll(approversIds).subscribe(response => {
                    resolve();
                });
            }
            else{
                resolve();
            }
        });

        var promise2 = new Promise((resolve, reject) => {
            const delegatesSelecteds = this.data.filter(item => item.checked && item.typeId == '2');
            const delegatesIds = delegatesSelecteds.map(x => x.id);
    
            if(delegatesIds && delegatesIds.length > 0){
                this.deleteSubscription = this.refundService.deleteDelegate(delegatesIds).subscribe(response => {
                    resolve();
                });
            }
            else{
                resolve();
            }
        });

        promises.push(promise1);
        promises.push(promise2);
  
        Promise.all(promises).then(data => { 
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