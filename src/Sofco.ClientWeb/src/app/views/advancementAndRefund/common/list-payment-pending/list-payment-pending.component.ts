import { Component, ViewChild } from "@angular/core";
import { WorkflowService } from "app/services/workflow/workflow.service";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { MenuService } from "app/services/admin/menu.service";
import { PaymentPendingService } from "app/services/advancement-and-refund/paymentPending.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";

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

    public totalAmount: number = 0;
    currencyName: string = "";

    currencyWarning: boolean = false;
    refundsInProcessWarning: boolean = false;

    @ViewChild('detailModal') detailModal;
    public detailModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Detalle de adelantos y reintegros",
        "detailModal",
        true,
        true,
        "Pay",
        "ACTIONS.cancel"
    );

    constructor(private paymentPendingService: PaymentPendingService,
                public menuService: MenuService,
                private workflowService: WorkflowService,
                private messageService: MessageService){}

    ngOnInit(): void {
        this.getAll();
    }

    ngOnDestroy(): void {
        if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
        if(this.postSubscrip) this.postSubscrip.unsubscribe();
    }

    getAll(){
        this.messageService.showLoading();

        this.getAllSubscrip = this.paymentPendingService.get().subscribe(response => {
            this.messageService.closeLoading();

            this.model = [];
            this.totalAmount = 0;
            this.currencyName = "";
            this.modelFiltered = [];

            this.model = response.data.map(item => {
                item.selected = false;
                return item;
            });

            this.model.forEach(x => {
                this.fillData(x);
            });
       
            this.fillFilters();
        },
        error => this.messageService.closeLoading());
    }

    private fillData(x) {
        this.modelFiltered.push({ type: "item", data: x, id: x.id, show: false });
        if (x.entities && x.entities.length > 0) {
            x.entities.forEach(detail => {
                detail.selected = false;
            });
            this.modelFiltered.push({ type: "detail", data: x.entities, id: x.id, show: false });
        }
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
        this.modelFiltered.forEach((item, index) => {

            if(item.type == 'item'){
                if(item.data.canPayAll && item.data.ammount > 0){
                    item.data.selected = true;
                    this.onRowPrincipalChange(item);
                }
            }
        });

        this.calculateTotals();
    }

    unselectAll(){
        this.modelFiltered.forEach((item, index) => {

            if(item.type == 'item'){
                item.data.selected = false;
                this.onRowPrincipalChange(item);
            }
        });

        this.calculateTotals();
    }

    canApprove(){
        var rows = this.modelFiltered.filter(x => x.type == 'detail');
        var allUnselected = true;

        if(rows){
            rows.forEach(detail => {
                detail.data.forEach(entity => {
                    if(entity.selected){
                        allUnselected = false;
                    }
                })
            });
        }

        return allUnselected || this.currencyWarning || this.refundsInProcessWarning || this.totalAmount <= 0;
    }

    calculateTotals(){
        this.totalAmount = 0;
        var currencies = [];

        this.modelFiltered.forEach(item => {
            if(item.type == 'detail'){
                item.data.forEach(x => {
                    if(x.selected) {
                        if(x.hasRefundsInProcess){
                            this.refundsInProcessWarning = true;
                        }
        
                        if(!currencies.includes(x.currencyName)){
                            currencies.push(x.currencyName);
                        }
        
                        if(x.type == 'refund' || x.type == 'advancement-accounted'){
                            this.totalAmount += x.ammount;
                        }
        
                        if(x.type == 'advancement'){
                            this.totalAmount -= x.ammount;
                        }
                    }
                });
            }
        });

        if(currencies.length > 1){
            this.currencyWarning = true;
            this.totalAmount = 0;
            this.currencyName = "";
        }
        else{
            if(currencies.length == 1){
                this.currencyName = currencies[0];
            }
        }
    }

    approveAll(){
        this.messageService.showConfirm(x => {
            var list = [];
            
            this.model.forEach(x => {

                x.entities.forEach(e => {

                    if(e.selected) {
                        var item = {
                            workflowId: e.workflowId,
                            entityId: e.id,
                            nextStateId: e.nextWorkflowStateId,
                            entityController: e.type == 'advancement-accounted' ? 'advancement' : e.type,
                            type: e.type == 'advancement-accounted' ? 'advancement' : e.type,
                            userApplicantName: x.userApplicantDesc,
                            id: x.id
                        };
    
                        list.push(item);
                    }
                });
            });

            if(list.length > 0){
                this.messageService.showLoading();

                this.postSubscrip = this.workflowService.doMassiveTransitions(list).subscribe(response => {
                    this.messageService.closeLoading();
            
                    this.getAll();
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
        this.currencyWarning = false;
        this.refundsInProcessWarning = false;

        this.model.forEach(x => {
            x.selected = false;
        });

        this.modelFiltered = [];

        if(!this.userId && !this.bankId){
            this.model.forEach(x => {
                this.fillData(x);
            });
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
                    this.fillData(item);
                }
            }
        }
    }

    onRowPrincipalChange(item){
        this.currencyWarning = false;
        this.refundsInProcessWarning = false;
        item.data.entities.forEach(detail => {
            detail.selected = item.data.selected;
        });

        this.calculateTotals();
    }

    onRowDetailChange(entity, item){
        this.currencyWarning = false;
        this.refundsInProcessWarning = false;
        var entities = item.data;

        if(entity.type == 'refund' && entity.entitiesRelatedIds){
            entity.entitiesRelatedIds.forEach(advancementId => {

                var advancementRow = entities.find(x => x.id == advancementId && x.type == 'advancement');

                if(advancementRow) {
                    advancementRow.selected = entity.selected;

                    if(advancementRow.entitiesRelatedIds){
                        advancementRow.entitiesRelatedIds.forEach(refundId => {
                            var refundRow = entities.find(x => x.id == refundId && x.type == 'refund');

                            if(refundRow) refundRow.selected = entity.selected;
                        });
                    }
                }
            });
        }

        if(entity.type == 'advancement' && entity.entitiesRelatedIds){
            entity.entitiesRelatedIds.forEach(refundId => {

                var refundRow = entities.find(x => x.id == refundId && x.type == 'refund');

                if(refundRow) {
                    refundRow.selected = entity.selected;

                    if(refundRow.entitiesRelatedIds){
                        refundRow.entitiesRelatedIds.forEach(advancementId => {
                            var advancementRow = entities.find(x => x.id == advancementId && x.type == 'advancement');

                            if(advancementRow) advancementRow.selected = entity.selected;
                        });
                    }
                }
            });
        }

        this.calculateTotals();

        var areAllDetailsSelected = entities.every(x => x.selected);
        var row = this.modelFiltered.find(x => x.id == item.id && x.type == 'item');

        if(row) row.data.selected = areAllDetailsSelected;
    }

    expand(item){
        var row = this.modelFiltered.find(x => x.id == item.id && x.type == 'detail');
        if(row){
            row.show = !row.show;
            item.show = row.show;
        }
    }
}