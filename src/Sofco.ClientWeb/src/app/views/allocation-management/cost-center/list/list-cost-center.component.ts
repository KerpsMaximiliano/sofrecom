import { Component, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Router } from "@angular/router";
import { DataTableService } from "app/services/common/datatable.service";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from "app/services/common/message.service";
import { CostCenterService } from "app/services/allocation-management/cost-center.service";

@Component({
    selector: 'list-cost-center',
    templateUrl: './list-cost-center.component.html'
})

export class ListCostCenterComponent implements OnInit, OnDestroy {

    public model: any[] = new Array<any>();
    public loading: boolean = false;

    getAllSubscrip: Subscription;

    constructor(private costCenterService: CostCenterService,
                private router: Router,
                private menuService: MenuService,
                private messageService: MessageService,
                private dataTableService: DataTableService,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void {
        this.loading = true;

        this.getAllSubscrip = this.costCenterService.getAll().subscribe(data => {
            this.model = data;
            this.loading = false;
            this.dataTableService.init('#costCentersTable', false);
        },
        error => {
            this.loading = false;
            this.errorHandlerService.handleErrors(error)
        });
    }

    ngOnDestroy(): void {
        if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
    }
}