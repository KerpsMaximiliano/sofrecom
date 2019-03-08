import { Component, OnInit, OnDestroy } from "@angular/core";
import { Router } from "@angular/router";
import { Subscription } from "rxjs";
import { MessageService } from "../../../../services/common/message.service";
import { WorkflowService } from "app/services/workflow/workflow.service";
import { MenuService } from "../../../../services/admin/menu.service";
import { DataTableService } from "../../../../services/common/datatable.service";

declare var moment: any;

@Component({
    selector: 'workflow-state-list',
    templateUrl: './state-list.component.html'
})

export class WorkflowStateListComponent implements OnInit, OnDestroy {

    public states: any[] = new Array();

    //RxJS  
    public getSubscrip: Subscription;
    public deactivateSubscrip: Subscription;
    public activateSubscrip: Subscription;

    constructor(private router: Router,
        public menuService: MenuService,
        private workflowService: WorkflowService,
        private dataTableService: DataTableService,
        private messageService: MessageService) { }

    ngOnInit(): void {
        //Called after the constructor, initializing input properties, and the first call to ngOnChanges.
        //Add 'implements OnInit' to the class.
        this.getAll();
    }

    ngOnDestroy(): void {
        //Called once, before the instance is destroyed.
        //Add 'implements OnDestroy' to the class.
        if (this.getSubscrip) this.getSubscrip.unsubscribe();
        if (this.deactivateSubscrip) this.deactivateSubscrip.unsubscribe();
        if (this.activateSubscrip) this.activateSubscrip.unsubscribe();
    }

    getAll() {
        this.messageService.showLoading();

        this.getSubscrip = this.workflowService.getWorkflowStates().subscribe(response => {
            this.messageService.closeLoading();
            this.states = response;
            this.initGrid();
        },
            error => this.messageService.closeLoading());
    }

    habInhabClick(state) {
        this.messageService.showLoading();
        var newState = !state.active;
        
        this.activateSubscrip = this.workflowService.activeWorkflowState(state.id, newState).subscribe(response => {
            this.messageService.closeLoading();
            state.active = newState;
            },
            error => {
                this.messageService.closeLoading();
            });

    }

    goToDetail(task) {
        this.router.navigate([`/admin/states/${task.id}/edit`]);
    }

    initGrid() {
        var columns = [0, 1, 2, 3];
        var title = `Areas-${moment(new Date()).format("YYYYMMDD")}`;

        var params = {
            selector: '#stateTable',
            columns: columns,
            title: title,
            withExport: true,
            columnDefs: [{ 'aTargets': [2, 3], "sType": "date-uk" }]
        }

        this.dataTableService.destroy(params.selector);
        this.dataTableService.initialize(params);
    }

}