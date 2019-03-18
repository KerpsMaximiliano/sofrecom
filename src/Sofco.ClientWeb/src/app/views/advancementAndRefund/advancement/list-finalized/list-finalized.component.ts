import { Component, OnDestroy, OnInit, ViewChild, Output, EventEmitter } from "@angular/core";
import { AdvancementService } from "app/services/advancement-and-refund/advancement.service";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { DataTableService } from "app/services/common/datatable.service";
import { Router } from "@angular/router";
import { WorkflowStateType } from "app/models/enums/workflowStateType";

@Component({
    selector: 'list-finalized',
    templateUrl: './list-finalized.component.html',
    styleUrls: ['./list-finalized.component.scss']
})
export class AdvancementListFinalizedComponent implements OnInit, OnDestroy {

    getSubscrip: Subscription;

    public model: any[] = new Array();
    public visible: boolean = false;

    public banks: any[] = new Array();
    @Output()
    valueChange = new EventEmitter<any>();

    constructor(private advancementService: AdvancementService,
                private datatableService: DataTableService,
                private router: Router,
                private messageService: MessageService){}

    ngOnInit(): void {
        this.filterBanks();
    }

    ngOnDestroy(): void {
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
    }
    
    initGrid(){
        var columns = [0, 1, 2, 3, 4, 5];
        var title = `Adelantos-finalizados`;

        var params = {
          selector: '#advancements-finalized',
          columns: columns,
          title: title,
          withExport: true,
          columnDefs: [ {"aTargets": [4], "sType": "date-uk"} ]
        }
  
        this.datatableService.destroy(params.selector);
        this.datatableService.initialize(params);

        setTimeout(() => {
            $("#advancements-finalized_wrapper").css("float","left");
            $("#advancements-finalized_wrapper").css("padding-bottom","50px");
            $("#advancements-finalized_filter label").addClass('search-filter');
            $(".html5buttons").addClass('export-buttons');
            $("#advancements-finalized_paginate").addClass('table-pagination');
            $("#advancements-finalized_length").css("margin-right","10px");
            $("#advancements-finalized_info").css("padding-top","4px");
        }, 500);
    }

    goToDetail(item){
        this.router.navigate(['/advancementAndRefund/advancement/' + item.id])
    }

    search(parameters){
        this.visible = true;
        this.messageService.showLoading();

        this.getSubscrip = this.advancementService.getAllFinalized(parameters).subscribe(response => {
            this.messageService.closeLoading();

            this.model = [];
            this.model = response.data;
            this.initGrid();
            this.filterBanks();
        }, 
        error => this.messageService.closeLoading());
    }

    getStatusClass(type){
        switch(type){
            case WorkflowStateType.Info: return "label-success";
            case WorkflowStateType.Warning: return "label-warning";
            case WorkflowStateType.Success: return "label-primary";
            case WorkflowStateType.Danger: return "label-danger";
        }
    }

    filterBanks() {
        
        this.model.forEach(x => {

            if (this.banks.filter(bank => bank.id == x.bank).length == 0) {
                this.banks.push({ id: x.bank, text: x.bank });
            }
        });

        this.valueChange.emit(this.banks);
        debugger;
    }
}