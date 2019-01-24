import { Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { AdvancementService } from "app/services/advancement-and-refund/advancement.service";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { DataTableService } from "app/services/common/datatable.service";
import { Router } from "@angular/router";
import { WorkflowService } from "app/services/workflow/workflow.service";

@Component({
    selector: 'list-payment-pending',
    templateUrl: './list-payment-pending.component.html',
    styleUrls: ['./list-payment-pending.component.scss']
})
export class AdvancementListPaymentPendingComponent implements OnInit, OnDestroy {

    getSubscrip: Subscription;
    postSubscrip: Subscription;

    public model: any[] = new Array();
    public totals: any[] = new Array();

    constructor(private advancementService: AdvancementService,
                private datatableService: DataTableService,
                private workflowService: WorkflowService,
                private router: Router,
                private messageService: MessageService){}

    ngOnInit(): void {
        this.search({});
    }

    ngOnDestroy(): void {
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
        if(this.postSubscrip) this.postSubscrip.unsubscribe();
    }
    
    initGrid(){
        var columns = [0, 1, 2, 3, 4, 5, 6, 7, 8];
        var title = `Adelantos-pendientes-deposito`;

        var params = {
          selector: '#payment-pending',
          columns: columns,
          title: title,
          withExport: true,
          columnDefs: [ {"aTargets": [3], "sType": "date-uk"} ]
        }
  
        this.datatableService.destroy(params.selector);
        this.datatableService.initialize(params);

        setTimeout(() => {
            $("#payment-pending_wrapper").css("float","left");
            $("#payment-pending_wrapper").css("padding-bottom","50px");
            $("#payment-pending_filter label").addClass('search-filter');
            $(".html5buttons").addClass('export-buttons');
            $("#payment-pending_paginate").addClass('table-pagination');
            $("#payment-pending_length").css("margin-right","10px");
            $("#payment-pending_info").css("padding-top","4px");
        }, 500);
    }

    goToDetail(item){
        this.router.navigate(['/advancementAndRefund/advancement/' + item.id])
    }

    search(parameters){
        this.messageService.showLoading();

        this.getSubscrip = this.advancementService.getAllPaymentPending(parameters).subscribe(response => {
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
    }

    unselectAll(){
        this.model.forEach((item, index) => {
            item.selected = false;
        });
    }

    noneResourseSelected(){
        return this.model.filter(x => x.selected == true).length == 0;
    }

    calculateTotals(){
        this.totals = new Array();

        this.model.forEach(item => {

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
        });
    }

    approveAll(){
        this.messageService.showConfirm(x => {

            var list = this.model.filter(x => x.selected).map(x => {
                return {
                    workflowId: x.workflowId,
                    entityId: x.id,
                    nextStateId: x.nextWorkflowStateId,
                    entityController: 'advancement'
                }
            });

            if(list.length > 0){
                this.messageService.showLoading();

                var promises = new Array();

                list.forEach(item => {
                    var promise = new Promise((resolve, reject) => {

                        this.postSubscrip = this.workflowService.post(item).subscribe(response => {
                            resolve();
                            var indexItemToRemove = this.model.map(x => x.id).indexOf(item.entityId);
                            this.model.splice(indexItemToRemove, 1);
                        },
                        error => resolve());
                    });

                    promises.push(promise);
                })

                Promise.all(promises).then(data => { 
                    this.messageService.closeLoading();
                    this.calculateTotals();
                    this.initGrid();
                });
            }
        })
    }
}