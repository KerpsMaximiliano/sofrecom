import { OnInit, OnDestroy, Component, Input } from "@angular/core";
import { LicenseService } from "app/services/human-resources/licenses.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from "app/services/common/message.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs";
import { DataTableService } from "app/services/common/datatable.service";
import { Cookie } from "ng2-cookies/ng2-cookies";
import { Router } from "@angular/router";

@Component({
    selector: 'license-list-widget',
    templateUrl: './license-list-widget.component.html'
})
export class LicenseListWidget implements OnInit, OnDestroy {

    public data: any[] = new Array();

    @Input() statusId: number;
    @Input() managerId: number;
    @Input() label: string;
    @Input() color: string;

    getDataSubscrip: Subscription

    public loading: boolean = false;

    constructor(private licenseService: LicenseService,
        private employeeService: EmployeeService,
        private router: Router,
        private menuService: MenuService,
        private datatableService: DataTableService,
        private messageService: MessageService,
        private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void {
        if(this.managerId > 0){
            this.getDataSubscrip = this.licenseService.getByManagerAndStatus(this.managerId, this.statusId).subscribe(data => {
                this.data = data;
                this.initGrid()
            },
            error => {});
        }
        else{
            this.getDataSubscrip = this.licenseService.getByStatus(this.statusId).subscribe(data => {
                this.data = data;
                this.initGrid()
            },
            error => {});
        }
    }

    ngOnDestroy(): void {
        if(this.getDataSubscrip) this.getDataSubscrip.unsubscribe();
    }

    initGrid(){
        var params = {
            selector: "#licenseStatus-" + this.statusId,
            scrollX: true
        };

        this.datatableService.init2(params);
    }

    goToDetail(item){
        this.router.navigate([`/allocationManagement/licenses/${item.id}/detail`])
    }
}