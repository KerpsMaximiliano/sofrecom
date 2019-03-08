import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { MessageService } from "../../../../services/common/message.service";
import { Router } from "@angular/router";
import { Subscription } from "rxjs";
import { DataTableService } from "../../../../services/common/datatable.service";
import { WorkflowService } from "app/services/workflow/workflow.service";
declare var moment: any;

@Component({
    selector: 'workflow-list',
    templateUrl: './workflow-list.component.html'
})
  export class WorkflowListComponent implements OnInit, OnDestroy {

    public items: any[] = new Array(); 
 
    public getSubscrip: Subscription;

    @ViewChild('newModal') newModal;

    constructor(private messageService: MessageService,
                private router: Router,
                private dataTableService: DataTableService,
                private workflowService: WorkflowService) { }

    ngOnInit(): void {
        this.getAll();
    }

    ngOnDestroy(): void {
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
    }

    getAll(){
        this.messageService.showLoading();

        this.getSubscrip = this.workflowService.getWorkflows().subscribe(response => {
            this.messageService.closeLoading();
            this.items = response.data;
            this.initGrid();
        }, 
        () => this.messageService.closeLoading());
    }

    goToDetail(item){
        this.router.navigate([`/admin/workflows/${item.id}`]);
    }

    initGrid(){
        var columns = [0, 1, 2, 3];
        var title = `Workflows-${moment(new Date()).format("YYYYMMDD")}`;

        var params = {
            selector: '#workflowsTable',
            columns: columns,
            title: title, 
            order: [[ 3, "desc" ]],
            withExport: true,
            columnDefs: [ {'aTargets': [2], "sType": "date-uk"} ]
        }

        this.dataTableService.destroy(params.selector);
        this.dataTableService.initialize(params);
    }

    addItem(){
        this.newModal.show();
    }

    onAddSuccess(newItem){
        this.items.push(newItem);

        this.initGrid();
    }
}