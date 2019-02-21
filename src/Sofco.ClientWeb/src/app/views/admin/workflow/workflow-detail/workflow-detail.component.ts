import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { MessageService } from "../../../../services/common/message.service";
import { Router, ActivatedRoute } from "@angular/router";
import { Subscription } from "rxjs";
import { DataTableService } from "../../../../services/common/datatable.service";
import { WorkflowService } from "app/services/workflow/workflow.service";
declare var moment: any;

@Component({
    selector: 'workflow-detail',
    templateUrl: './workflow-detail.component.html'
})
  export class WorkflowDetailComponent implements OnInit, OnDestroy {

    @ViewChild('addTransitionModal') addTransitionModal;

    public model: any = {
        id: 0,
        description: "",
        version: "",
        active: false,
        transitions: []
    } 
 
    public getSubscrip: Subscription;
    public deleteTransitionSubscrip: Subscription;

    constructor(private messageService: MessageService,
                private router: Router,
                private activateRoute: ActivatedRoute,
                private dataTableService: DataTableService,
                private workflowService: WorkflowService) { }

    ngOnInit(): void {
        const routeParams = this.activateRoute.snapshot.params;

        this.getSubscrip = this.workflowService.getWorkflowById(routeParams.id).subscribe(response => {
            this.messageService.closeLoading();
            this.model = response.data;
            this.initGrid();
        }, 
        () => this.messageService.closeLoading());
    }

    ngOnDestroy(): void {
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
    }

    goToDetail(item){
        this.router.navigate([`/admin/workflows/${this.model.id}/transition/${item.id}`]);
    }

    initGrid(){
        var columns = [0, 1, 2, 3];
        var title = `Workflow ${this.model.description}-transiciones-${moment(new Date()).format("YYYYMMDD")}`;

        var params = {
            selector: '#transitionsTable',
            columns: columns,
            title: title,
            withExport: true
        }

        this.dataTableService.destroy(params.selector);
        this.dataTableService.initialize(params);
    }

    addTransition(){
        this.router.navigate([`/admin/workflows/${this.model.id}/transition/new`]);
    }

    delete(item, index){

        this.messageService.showConfirm(() => {

            this.messageService.showLoading();

            this.deleteTransitionSubscrip = this.workflowService.deleteTransition(item.id).subscribe(response => {
                this.messageService.closeLoading();
                this.model.transitions.splice(index, 1);
                this.initGrid()
            },
            error => {
                this.messageService.closeLoading();
            });

        });
    }
}