import { Component, OnInit, Input, ViewChild } from "@angular/core";
import { DataTableService } from "app/services/common/datatable.service";
import { I18nService } from "app/services/common/i18n.service";
import { RefundService } from "app/services/advancement-and-refund/refund.service";
import { Subscription } from "rxjs";
import { WorkflowStateType } from "app/models/enums/workflowStateType";
import { Router } from "@angular/router";
import { MessageService } from "app/services/common/message.service";

@Component({
    selector: 'refund-list-grid',
    templateUrl: './refund-list-grid.component.html',
    styleUrls: ['./refund-list-grid.component.scss']
})
export class RefundListGridComponent implements OnInit {
    subscrip: Subscription;
    public data:any[] = new Array<any>();

    constructor(private refundService: RefundService,
        private router: Router,
        private messageService: MessageService,
        private datatableService: DataTableService,
        private i18nService: I18nService){}

    ngOnInit(): void {
    }

    goToDetail(id){
        this.router.navigate(['/advancementAndRefund/refund/' + id]);
    }

    getData(model) {
        this.messageService.showLoading();
        
        this.subscrip = this.refundService.getAll(model).subscribe(res => {
            this.messageService.closeLoading();
            
            sessionStorage.setItem('lastRefundQuery', JSON.stringify(model));

            this.data = res.data;
            this.initGrid();
        });
    }

    initGrid(){
        const gridSelector = "#refundTable";

        const columns = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 13];

        const params = {
            selector: gridSelector,
            columns: columns,
            title: this.i18nService.translateByKey('refund.listTitle'),
            withExport: true,
            columnDefs: [ {"aTargets": [9, 10], "sType": "date-uk"} ],
            currencyColumns: [5, 6, 7, 8]
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
