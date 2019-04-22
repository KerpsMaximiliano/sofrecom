import { Component, OnInit, ViewChild } from "@angular/core";

declare var $: any;

@Component({
    selector: 'refund-list',
    templateUrl: './refund-list.component.html',
    styleUrls: ['./refund-list.component.scss']
})
export class RefundListComponent implements OnInit {

    public tabInProcess = true;

    public inWorkflowProcess = true;
    @ViewChild('gridFilter') gridFilter;
    @ViewChild('inProcessGrid') inProcess;
    @ViewChild('finalizedGrid') finalized;

    constructor() { }

    ngOnInit(): void {
        const data = JSON.parse(sessionStorage.getItem('lastRefundQuery'));

        if(data){
            this.gridFilter.resourceId = data.userApplicantId;
            this.gridFilter.stateId = data.stateId;
            this.gridFilter.dateSince = data.dateSince;
            this.gridFilter.dateTo = data.dateTo;
            this.gridFilter.inWorkflowProcess = data.inWorkflowProcess;
            this.gridFilter.bankId = data.bank;
            this.gridFilter.setModel();
        }

        this.getData();
    }

    getData() {
        const model = this.getParameterModel();

        if (this.tabInProcess){
            model.inWorkflowProcess = true;
            this.inProcess.getData(model);
        }
        else {
            model.inWorkflowProcess = false;
            this.finalized.getData(model);
        }

    }

    getParameterModel() {
        return this.gridFilter.model !== undefined
            ? this.gridFilter.model
            : {
                inWorkflowProcess: this.inWorkflowProcess
            };
    }
}
