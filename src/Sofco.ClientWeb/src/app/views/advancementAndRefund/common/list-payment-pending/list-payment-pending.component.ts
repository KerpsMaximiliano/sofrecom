import { Component } from "@angular/core";
import { AdvancementService } from "app/services/advancement-and-refund/advancement.service";
import { DataTableService } from "app/services/common/datatable.service";
import { WorkflowService } from "app/services/workflow/workflow.service";
import { Subscription } from "rxjs";
import { Router } from "@angular/router";
import { MessageService } from "app/services/common/message.service";
import { RefundService } from "app/services/advancement-and-refund/refund.service";
import { MenuService } from "app/services/admin/menu.service";
import { PaymentPendingService } from "app/services/advancement-and-refund/paymentPending.service";

@Component({
    selector: 'list-payment-pending',
    templateUrl: './list-payment-pending.component.html'
})
export class ListPaymentPendingComponent  {
    getAllSubscrip: Subscription;
    postSubscrip: Subscription;

    public model: any[] = new Array();
    public modelFiltered: any[] = new Array();

    public banks: any[] = new Array();
    public users: any[] = new Array();
    public currencies: any[] = new Array();
    public types: any[] = new Array();

    public bankId: number;
    public userId: number;

    public totalAmount: number;

    constructor(private paymentPendingService: PaymentPendingService,
                public menuService: MenuService,
                private datatableService: DataTableService,
                private workflowService: WorkflowService,
                private router: Router,
                private messageService: MessageService){}

    ngOnInit(): void {
        this.getAll();
    }

    ngOnDestroy(): void {
        if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
        if(this.postSubscrip) this.postSubscrip.unsubscribe();
    }

    initGrid(){
        var columns = [1, 2, 3, 4];
        var title = `Adelantos y reitegros pendientes deposito`;

        var params = {
          selector: '#payment-pending',
          columns: columns,
          title: title,
          withExport: true,
          currencyColumns: [4]
        }
  
        this.datatableService.destroy(params.selector);
        this.datatableService.initialize(params);
    }

    goToDetail(item){
        this.router.navigate([`/advancementAndRefund/${item.entityController}/${item.id}`]);
    }

    getAll(){
        this.messageService.showLoading();

        this.getAllSubscrip = this.paymentPendingService.get().subscribe(response => {
            this.messageService.closeLoading();

            this.model = [];

            this.model = response.data.map(item => {
                item.selected = false;
                return item;
            });

            this.modelFiltered = this.model;

            this.calculateTotals();
            this.fillFilters();
            this.initGrid();
        },
        error => this.messageService.closeLoading());
    }

    fillFilters(){
        this.banks = [];
        this.users = [];

        this.model.forEach(x => {
        
            if(this.banks.filter(bank => bank.id == x.bank).length == 0){
                this.banks.push({ id: x.bank, text: x.bank });
            }    

            if(this.users.filter(user => user.id == x.userApplicantId).length == 0){
                this.users.push({ id: x.userApplicantId, text: x.userApplicantDesc });
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
        this.totalAmount = 0;
        this.model.forEach(item => {
            if(item.selected){
                this.totalAmount += item.ammount;
            }
        });
    }

    approveAll(){
        this.messageService.showConfirm(x => {
            var list = [];
            
            this.model.filter(x => x.selected).forEach(x => {

                x.entities.forEach(e => {

                    var item = {
                        workflowId: e.workflowId,
                        entityId: e.id,
                        nextStateId: e.nextWorkflowStateId,
                        entityController: e.type,
                        type: e.type,
                        userApplicantName: x.userApplicantDesc,
                        id: x.id
                    };

                    list.push(item);
                });
            });

            if(list.length > 0){
                this.messageService.showLoading();

                this.postSubscrip = this.workflowService.doMassiveTransitions(list).subscribe(response => {
                    this.messageService.closeLoading();

                    list.forEach(item => {
                        var itemToDelete = this.model.find(x => x.id == item.id);

                        if(itemToDelete){
                            var index = this.model.indexOf(itemToDelete);
                            this.model.splice(index, 1);
                        }
                    });

                    this.search();
                },
                error => {
                    this.messageService.closeLoading();
                    this.getAll();
                });
            }
        });
    }

    clean(){
        this.userId = null;
        this.bankId = null;

        this.search();
    }

    search(){
        this.model.forEach(x => {
            x.selected = false;
        });

        this.modelFiltered = [];

        if(!this.userId && !this.bankId){
            this.modelFiltered = this.model;
        }
        else{
            for(var i = 0; i < this.model.length; i++){
                var addItem = true;
                var item = this.model[i];

                if(this.userId  && this.userId > 0 && this.userId != item.userApplicantId){
                    addItem = false;
                }

                if(this.bankId && this.bankId  != item.bank){
                    addItem = false;
                }

                if(addItem){
                    this.modelFiltered.push(item);
                }
            }
        }

        this.initGrid();
        this.calculateTotals();
    }
}