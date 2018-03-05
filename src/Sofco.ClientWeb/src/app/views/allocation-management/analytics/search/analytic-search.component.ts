import { Component, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Router } from "@angular/router";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { DataTableService } from "app/services/common/datatable.service";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from "app/services/common/message.service";
import { I18nService } from "app/services/common/i18n.service";

@Component({
    selector: 'analytic-search',
    templateUrl: './analytic-search.component.html'
})

export class AnalyticSearchComponent implements OnInit, OnDestroy {

    public model: any[] = new Array<any>();
    public resources: any[] = new Array<any>();
    public loading:  boolean = true;
    public loadingResources: boolean = true;
    public resource: any;

    getAllSubscrip: Subscription;
    getAllEmployeesSubscrip: Subscription;

    constructor(private analyticService: AnalyticService,
                private router: Router,
                private i18nService: I18nService,
                public menuService: MenuService,
                private messageService: MessageService,
                private dataTableService: DataTableService,
                private errorHandlerService: ErrorHandlerService){
    }
 
    ngOnInit(): void {
        this.getAllSubscrip = this.analyticService.getAll().subscribe(data => {
            this.model = data;
            this.loading = false;
            this.dataTableService.init('#analyticsTable', false);
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    ngOnDestroy(): void {
        if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
    }

    gotToEdit(analytic){
        this.router.navigate([`/contracts/analytics/${analytic.id}/edit`]);
    }

    gotToView(analytic){
        this.router.navigate([`/contracts/analytics/${analytic.id}/view`]);
    }

    goToAssignResource(analytic){
        if(this.menuService.hasFunctionality('ALLOC', 'ADRES')){
            sessionStorage.setItem("analytic", JSON.stringify(analytic));
            this.router.navigate([`/contracts/analytics/${analytic.id}/allocations`]);
        }
        else{
            this.messageService.showError("allocationManagement.allocation.forbidden");
        }
    }

    goToAdd(){
        sessionStorage.setItem('analyticWithProject', 'no');
        this.router.navigate(['/contracts/analytics/new']);
    }

    getStatus(analytic){
        switch(analytic.status){
            case 1: return this.i18nService.translateByKey("allocationManagement.analytics.status.open");
            case 2: return this.i18nService.translateByKey("allocationManagement.analytics.status.close");
            case 3: return this.i18nService.translateByKey("allocationManagement.analytics.status.closeForExpenses");
        }
    }
}