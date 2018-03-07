import { Component, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Router, ActivatedRoute } from "@angular/router";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from "app/services/common/message.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { DataTableService } from "app/services/common/datatable.service";

@Component({
    selector: 'resource-detail',
    templateUrl: './resource-detail.component.html',
    styleUrls: ['./resource-detail.component.scss']
})
export class ResourceDetailComponent implements OnInit, OnDestroy {

    resourceId: number;
    public model: any;

    getSubscrip: Subscription;
    paramsSubscrip: Subscription;

    constructor(private router: Router,
                private menuService: MenuService,
                private messageService: MessageService,
                private dataTableService: DataTableService,
                private activatedRoute: ActivatedRoute,
                private employeeService: EmployeeService,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.resourceId = params['id'];

            this.messageService.showLoading();

            this.getSubscrip = this.employeeService.getProfile(params['id']).subscribe(response => {
                this.model = response.data;

                this.messageService.closeLoading();
                this.initGrid();
            },
            error => {
                this.messageService.closeLoading();
                this.errorHandlerService.handleErrors(error);
            });
        });
    }

    ngOnDestroy(): void {
        if (this.getSubscrip) { this.getSubscrip.unsubscribe(); }
        if (this.paramsSubscrip) { this.paramsSubscrip.unsubscribe(); }
    }

    initGrid(){
        const options = { selector: '#analyticsTable', columnDefs: [ {'aTargets': [4, 5], "sType": "date-uk"} ], order: [[ 5, "desc" ]] };
        this.dataTableService.init2(options);
    }

    goToLicenses(){
        this.router.navigate([`/profile/licenses/add`]);
    }
}