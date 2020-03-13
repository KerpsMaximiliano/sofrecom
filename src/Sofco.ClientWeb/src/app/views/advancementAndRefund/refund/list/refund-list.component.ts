import { Component, OnInit, ViewChild } from "@angular/core";
import { environment } from "environments/environment";
import { MenuService } from "app/services/admin/menu.service";

declare var $: any;

@Component({
    selector: 'refund-list',
    templateUrl: './refund-list.component.html',
    styleUrls: ['./refund-list.component.scss']
})
export class RefundListComponent implements OnInit {

    @ViewChild('gridFilter') gridFilter;
    @ViewChild('finalizedGrid') finalized;

    constructor(private menuService: MenuService) { }

    ngOnInit(): void {
        const data = JSON.parse(sessionStorage.getItem('lastRefundQuery'));

        this.menuService.userIsRrhh = true;

        if(data){
            this.gridFilter.resourceId = data.userApplicantId;
            this.gridFilter.stateIds = data.stateIds;
            this.gridFilter.dateSince = data.dateSince;
            this.gridFilter.dateTo = data.dateTo;
            this.gridFilter.bankId = data.bank;
            this.gridFilter.analyticIds = data.analyticIds;
            this.gridFilter.setModel();

            this.getData();
        }
        else {
            if(this.menuService.userIsDaf) this.gridFilter.stateIds.push(environment.dafWorkflowStateId);
            if(this.menuService.userIsGaf) this.gridFilter.stateIds.push(environment.gafWorkflowStateId);
            if(this.menuService.userIsDirector) this.gridFilter.stateIds.push(environment.pendingDirectorWorkflowStateId);
            if(this.menuService.userIsManager) this.gridFilter.stateIds.push(environment.pendingManagerWorkflowStateId);
            
            if(this.menuService.userIsCompliance) {
                this.gridFilter.stateIds.push(environment.pendingComplianceWorkflowStateId);
                this.gridFilter.stateIds.push(environment.pendingComplianceDGWorkflowStateId);
            }

            if(this.gridFilter.stateIds.length > 0){
                this.getData();
            }
        }

        this.gridFilter.userIsCompliance = this.menuService.userIsCompliance;
    }

    getData() {
        this.finalized.getData(this.gridFilter.getModel());
    }
}
