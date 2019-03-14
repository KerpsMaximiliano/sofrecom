import { Component, OnInit, Input, ViewChild } from "@angular/core";
import { DataTableService } from "app/services/common/datatable.service";
import { I18nService } from "app/services/common/i18n.service";
import { RefundService } from "app/services/advancement-and-refund/refund.service";
import { Subscription } from "rxjs";
import { WorkflowStateType } from "app/models/enums/workflowStateType";
import { Router } from "@angular/router";
import { ItemsList } from "@ng-select/ng-select/ng-select/items-list";

@Component({
    selector: 'refund-list-grid',
    templateUrl: './refund-list-grid.component.html',
    styleUrls: ['./refund-list-grid.component.scss']
})
export class RefundListGridComponent implements OnInit {
    @Input()
    public controlId = 'grid1';
    @Input()
    public inWorkflowProcess = true;
    public loading = false;
    subscrip: Subscription;
    public data:any[] = new Array<any>();
    //@ViewChild('gridFilter') gridFilter;

    constructor(private refundService: RefundService,
        private datatableService: DataTableService,
        private router: Router,
        private i18nService: I18nService){}

    ngOnInit(): void {
      //  this.getData();
    }

    goToDetail(id){
        this.router.navigate(['advancementAndRefund/refund/' + id]);
    }

    getData(model) {
       // const model = this.getParameterModel();
        this.loading = true;
        this.subscrip = this.refundService.getAll(model).subscribe(res => {
            this.loading = false;
            this.data = res.data;
            this.initGrid();
        });
    }

    // getParameterModel() {
    //     return this.gridFilter.model !== undefined
    //         ? this.gridFilter.model
    //         : {
    //             inWorkflowProcess: this.inWorkflowProcess
    //         };
    // }

    initGrid(){
        const gridSelector = "#gridTable-" + this.controlId;

        const columns = [1, 2, 3, 4, 5, 6, 7];

        const params = {
            selector: gridSelector,
            columns: columns,
            title: this.i18nService.translateByKey('refund.listTitle'),
            withExport: true,
            columnDefs: []
        }

        if(this.inWorkflowProcess){
            params.columnDefs = [ {"aTargets": [5], "sType": "date-uk"} ]
        }
        else{
            params.columnDefs = [ {"aTargets": [4], "sType": "date-uk"} ]
        }
        
        this.datatableService.destroy(gridSelector);

        this.datatableService.initialize(params);

        setTimeout(() => {
            $(gridSelector + "_wrapper").css("float","left");
            $(gridSelector + "_wrapper").css("padding-bottom","50px");
            $(gridSelector + "_filter label").addClass('search-filter');
            $(".html5buttons").addClass('export-buttons');
            $(gridSelector + "_paginate").addClass('table-pagination');
            $(gridSelector + "_length").css("margin-right","10px");
            $(gridSelector + "_info").css("padding-top","4px");
        }, 500);
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
