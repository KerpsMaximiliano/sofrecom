import { Component, OnDestroy, OnInit } from "@angular/core";
import { AdvancementService } from "app/services/advancement-and-refund/advancement.service";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { DataTableService } from "app/services/common/datatable.service";
import { WorkflowStateType } from "app/models/enums/workflowStateType";
import { Router } from "@angular/router";

@Component({
    selector: 'list-finalized',
    templateUrl: './list-finalized.component.html',
    styleUrls: ['./list-finalized.component.scss']
})
export class AdvancementListFinalizedComponent implements OnInit, OnDestroy {

    getSubscrip: Subscription;

    public model: any[] = new Array();
    public visible: boolean = false;

    constructor(private advancementService: AdvancementService,
                private datatableService: DataTableService,
                private router: Router,
                private messageService: MessageService){}

    ngOnInit(): void {
        
    }

    ngOnDestroy(): void {
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
    }
    
    initGrid(){
        var columns = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        var title = `Adelantos-finalizados`;

        var params = {
          selector: '#advancements-finalized',
          columns: columns,
          title: title,
          withExport: true,
          columnDefs: [ {"aTargets": [4], "sType": "date-uk"} ],
          currencyColumns: [6]
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
        this.router.navigate(['/advancementAndRefund/advancement/' + item.id]);
    }

    search(parameters){
        this.visible = true;
        this.messageService.showLoading();

        this.getSubscrip = this.advancementService.getAllFinalized(parameters).subscribe(response => {
            this.messageService.closeLoading();

            this.model = [];
            this.model = response.data;
            this.initGrid();
        }, 
        () => this.messageService.closeLoading());
    }

    getStatusClass(type){
        switch(type){
            case WorkflowStateType.Info: return "label-success";
            case WorkflowStateType.Warning: return "label-warning";
            case WorkflowStateType.Success: return "label-primary";
            case WorkflowStateType.Danger: return "label-danger";
        }
    }

}