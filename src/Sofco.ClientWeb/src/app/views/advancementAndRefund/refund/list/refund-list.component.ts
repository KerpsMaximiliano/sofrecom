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
        this.getData();
    }

    getData() {
      //  debugger;
        const model = this.getParameterModel();

        if (this.tabInProcess){
            this.inProcess.getData(model);
        }
        else {
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
