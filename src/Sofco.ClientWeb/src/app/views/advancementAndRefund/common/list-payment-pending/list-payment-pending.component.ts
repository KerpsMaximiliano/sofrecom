import { Component } from "@angular/core";
import { AdvancementService } from "app/services/advancement-and-refund/advancement.service";
import { DataTableService } from "app/services/common/datatable.service";
import { WorkflowService } from "app/services/workflow/workflow.service";
import { Subscription } from "rxjs";
import { Router } from "@angular/router";
import { MessageService } from "app/services/common/message.service";
import { RefundService } from "app/services/advancement-and-refund/refund.service";
import { MenuService } from "app/services/admin/menu.service";

@Component({
    selector: 'list-payment-pending',
    templateUrl: './list-payment-pending.component.html'
})
export class ListPaymentPendingComponent  {
    getAdvancementsSubscrip: Subscription;
    getRefundsSubscrip: Subscription;
    postSubscrip: Subscription;

    public model: any[] = new Array();
    public advancements: any[] = new Array();
    public refunds: any[] = new Array();
    public totals: any[] = new Array();

    public banks: any[] = new Array();
    public users: any[] = new Array();
    public currencies: any[] = new Array();
    public types: any[] = new Array();

    public bankId: number;
    public userId: number;
    public currencyId: number;
    public typeId: number;

    constructor(private advancementService: AdvancementService,
                public menuService: MenuService,
                private refundService: RefundService,
                private datatableService: DataTableService,
                private workflowService: WorkflowService,
                private router: Router,
                private messageService: MessageService){}

    ngOnInit(): void {
        this.getAll();
    }

    ngOnDestroy(): void {
        if(this.getAdvancementsSubscrip) this.getAdvancementsSubscrip.unsubscribe();
        if(this.getRefundsSubscrip) this.getRefundsSubscrip.unsubscribe();
        if(this.postSubscrip) this.postSubscrip.unsubscribe();
    }

    initGrid(){
        var columns = [1, 2, 3, 4, 5];
        var title = `Adelantos y reitegros pendientes deposito`;

        var params = {
          selector: '#payment-pending',
          columns: columns,
          title: title,
          withExport: true
        }
  
        this.datatableService.destroy(params.selector);
        this.datatableService.initialize(params);
    }

    goToDetail(item){
        this.router.navigate([`/advancementAndRefund/${item.entityController}/${item.id}`]);
    }

    getAll(){
        var promises = new Array();

        var promise1 = new Promise((resolve, reject) => {

            this.getAdvancementsSubscrip = this.advancementService.getAllPaymentPending().subscribe(response => {
                this.advancements = [];

                this.advancements = response.data.map(item => {
                    item.selected = false;
                    item.entityController = "advancement";
                    return item;
                });

                resolve();
            },
            error => resolve());
        });

        var promise2 = new Promise((resolve, reject) => {

            this.getRefundsSubscrip = this.refundService.getAllPaymentPending().subscribe(response => {
                this.refunds = [];

                this.refunds = response.data.map(item => {
                    item.selected = false;
                    item.entityController = "refund";
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

            this.model = [];

            this.advancements.forEach(x => {
                this.model.push(x);
            });

            this.refunds.forEach(x => {
                this.model.push(x);
            });

            this.calculateTotals();
            this.fillFilters();
            this.initGrid();
        });
        
    }

    fillFilters(){
        this.banks = [];
        this.users = [];
        this.currencies = [];
        this.types = [];

        this.model.forEach(x => {
        
            if(this.banks.filter(bank => bank.id == x.bank).length == 0){
                this.banks.push({ id: x.bank, text: x.bank });
            }    

            if(this.users.filter(user => user.id == x.userApplicantId).length == 0){
                this.users.push({ id: x.userApplicantId, text: x.userApplicantDesc });
            }   
            
            if(this.currencies.filter(currency => currency.id == x.currencyId).length == 0){
                this.currencies.push({ id: x.currencyId, text: x.currencyDesc });
            }  

            if(this.types.filter(type => type.id == x.type).length == 0){
                this.types.push({ id: x.type, text: x.type });
            }  
        });
    }

    areAllSelected(){
        return this.model.every(item => {
            return item.selected == true;
        });
    }

    areAllUnselected(){
        return this.model.every(item => {
            return item.selected == false;
        });
    }

    selectAll(){
        this.model.forEach((item, index) => {
            item.selected = true;
        });

        this.calculateTotals();
    }

    unselectAll(){
        this.model.forEach((item, index) => {
            item.selected = false;
        });

        this.calculateTotals();
    }

    noneResourseSelected(){
        return this.model.filter(x => x.selected == true).length == 0;
    }

    calculateTotals(){
        this.totals = new Array();

        this.model.forEach(item => {

            if(item.selected){
                var itemTotal = this.totals.filter(x => x.text == item.currencyDesc);

                if(itemTotal.length > 0){
                    itemTotal[0].value = item.ammount + itemTotal[0].value;
                }
                else{
                    this.totals.push({
                        text: item.currencyDesc,
                        value: item.ammount
                    });
                }
            }
        });
    }

    approveAll(){
        this.messageService.showConfirm(x => {

            var list = this.model.filter(x => x.selected).map(x => {
                return {
                    workflowId: x.workflowId,
                    entityId: x.id,
                    nextStateId: x.nextWorkflowStateId,
                    entityController: x.entityController
                }
            });

            if(list.length > 0){
                this.messageService.showLoading();

                var entityIds = [];
                var promises = new Array();

                list.forEach(item => {
                    var promise = new Promise((resolve, reject) => {

                        this.postSubscrip = this.workflowService.post(item).subscribe(response => {
                            resolve();
                            entityIds.push(item.entityId);
                        },
                        error => resolve());
                    });

                    promises.push(promise);
                })

                Promise.all(promises).then(data => { 
                    this.messageService.closeLoading();

                    entityIds.forEach(x => {
                        var indexItemToRemove = this.model.map(x => x.id).indexOf(x);
                        this.model.splice(indexItemToRemove, 1);
                    });

                    this.calculateTotals();
                    this.initGrid();
                });
            }
        })
    }

    clean(){
        this.typeId = null;
        this.userId = null;
        this.bankId = null;
        this.currencyId = null;

        this.search();
    }

    search(){
        this.advancements.forEach(x => {
            x.selected = false;
        });

        this.refunds.forEach(x => {
            x.selected = false;
        });

        this.model = [];

        if(!this.userId && !this.typeId && !this.bankId && !this.currencyId){
            this.advancements.forEach(x => {
                this.model.push(x);
            });

            this.refunds.forEach(x => {
                this.model.push(x);
            });
        }
        else{
            for(var i = 0; i < this.advancements.length; i++){
                var addItem = true;
                var item = this.advancements[i];

                if(this.userId  && this.userId > 0 && this.userId != item.userApplicantId){
                    addItem = false;
                }

                if(this.bankId && this.bankId  != item.bank){
                    addItem = false;
                }

                if(this.typeId && this.typeId  != item.type){
                    addItem = false;
                }

                if(this.currencyId && this.currencyId > 0 && this.currencyId  != item.currencyId){
                    addItem = false;
                }

                if(addItem){
                    this.model.push(item);
                }
            }

            for(var i = 0; i < this.refunds.length; i++){
                var addItem = true;
                var item = this.refunds[i];

                if(this.userId  && this.userId > 0 && this.userId != item.userApplicantId){
                    addItem = false;
                }

                if(this.bankId && this.bankId  != item.bank){
                    addItem = false;
                }

                if(this.typeId && this.typeId  != item.type){
                    addItem = false;
                }

                if(this.currencyId && this.currencyId > 0 && this.currencyId  != item.currencyId){
                    addItem = false;
                }

                if(addItem){
                    this.model.push(item);
                }
            }
        }

        this.initGrid();
        this.calculateTotals();
    }
}