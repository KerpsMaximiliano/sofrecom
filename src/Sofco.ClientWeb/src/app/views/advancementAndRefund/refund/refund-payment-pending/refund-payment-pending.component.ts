import { Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { DataTableService } from "app/services/common/datatable.service";
import { Router } from "@angular/router";
import { WorkflowService } from "app/services/workflow/workflow.service";
import { RefundService } from "app/services/advancement-and-refund/refund.service";

@Component({
    selector: 'refund-payment-pending',
    templateUrl: './refund-payment-pending.component.html',
    styleUrls: ['./refund-payment-pending.component.scss']
})
export class RefundListPaymentPendingComponent implements OnInit, OnDestroy {

    getSubscrip: Subscription;
    postSubscrip: Subscription;

    public model: any[] = new Array();
    public totals: any[] = new Array();

    constructor(private refundService: RefundService,
                private datatableService: DataTableService,
                private workflowService: WorkflowService,
                private router: Router,
                private messageService: MessageService){}

    ngOnInit(): void {
        this.search();
    }

    ngOnDestroy(): void {
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
        if(this.postSubscrip) this.postSubscrip.unsubscribe();
    }
    
    initGrid(){
        var columns = [1, 2, 3, 4, 5, 6, 7];
        var title = `reintegros-pendientes-deposito`;

        var params = {
          selector: '#refund-payment-pending',
          columns: columns,
          title: title,
          withExport: true,
          columnDefs: [ {"aTargets": [1], "sType": "date-uk"} ]
        }
  
        this.datatableService.destroy(params.selector);
        this.datatableService.initialize(params);

        setTimeout(() => {
            $("#refund-payment-pending_wrapper").css("float","left");
            $("#refund-payment-pending_wrapper").css("padding-bottom","50px");
            $("#refund-payment-pending_filter label").addClass('search-filter');
            $(".html5buttons").addClass('export-buttons');
            $("#refund-payment-pending_paginate").addClass('table-pagination');
            $("#refund-payment-pending_length").css("margin-right","10px");
            $("#refund-payment-pending_info").css("padding-top","4px");
        }, 500);
    }

    goToDetail(item){
        this.router.navigate(['/advancementAndRefund/refund/' + item.id])
    }

    search(){
        this.messageService.showLoading();

        this.getSubscrip = this.refundService.getAllPaymentPending().subscribe(response => {
            this.messageService.closeLoading();

            this.model = [];

            this.model = response.data.map(item => {
                item.selected = false;
                return item;
            });

            this.calculateTotals();
            this.initGrid();
        }, 
        error => this.messageService.closeLoading());
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
                var itemTotal = this.totals.filter(x => x.text == item.currencyName);

                if(itemTotal.length > 0){
                    itemTotal[0].value = item.userRefund + itemTotal[0].value;
                }
                else{
                    this.totals.push({
                        text: item.currencyName,
                        value: item.userRefund
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
                    entityController: 'refund'
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
}