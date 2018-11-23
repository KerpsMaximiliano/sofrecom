import { Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { AdvancementService } from "app/services/advancement-and-refund/advancement.service";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { DataTableService } from "app/services/common/datatable.service";
import { Router } from "@angular/router";
import { WorkflowStateType } from "app/models/enums/workflowStateType";

@Component({
    selector: 'list-in-process',
    templateUrl: './list-in-process.component.html'
})
export class AdvancementListInProcessComponent implements OnInit, OnDestroy {
  
    getSubscrip: Subscription;

    public model: any[] = new Array();

    constructor(private advancementService: AdvancementService,
                private datatableService: DataTableService,
                private router: Router,
                private messageService: MessageService){}

    ngOnInit(): void {
        this.messageService.showLoading();

        this.getSubscrip = this.advancementService.getAllInProcess().subscribe(response => {
            this.messageService.closeLoading();

            this.model = response.data;
            this.initGrid();
        }, 
        error => this.messageService.closeLoading());
    }
                
    ngOnDestroy(): void {
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
    }

    initGrid(){
        var columns = [0, 1, 2, 3, 4, 5];
        var title = `Adelantos-en-proceso`;

        var params = {
          selector: '#advancements',
          columns: columns,
          title: title,
          withExport: true,
          columnDefs: [ {"aTargets": [4], "sType": "date-uk"} ]
        }
  
        this.datatableService.destroy(params.selector);
        this.datatableService.initialize(params);
    }

    goToDetail(item){
        this.router.navigate(['/advancementAndRefund/advancement/' + item.id])
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