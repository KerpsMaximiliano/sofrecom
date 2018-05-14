import { OnInit, OnDestroy, Component } from "@angular/core";
import { LicenseService } from "app/services/human-resources/licenses.service";
import { Router } from "@angular/router";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from "app/services/common/message.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { LicenseStatus } from "../../../../models/enums/licenseStatus";
import { Subscription } from "rxjs";
import { DataTableService } from "app/services/common/datatable.service";
import { Cookie } from "ng2-cookies/ng2-cookies";
import { UserInfoService } from "../../../../services/common/user-info.service";

@Component({
    selector: 'license-list-manager',
    templateUrl: './license-list-manager.component.html',
    styleUrls: ['./license-list-manager.component.scss']
})
export class LicenseListManager implements OnInit, OnDestroy {

    public authPending: number = LicenseStatus.AuthPending;

    getDataSubscrip: Subscription;

    public data: any[] = new Array();
    public managerId: number = 0;

    constructor(private licenseService: LicenseService,
        private router: Router,
        private datatableService: DataTableService,
        private menuService: MenuService,
        private messageService: MessageService,
        private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void {
        const userInfo = UserInfoService.getUserInfo();

        if(userInfo && userInfo.id){
            this.managerId = userInfo.id;
        }

        this.getAllLicenses();
    }

    ngOnDestroy(): void {
        if(this.getDataSubscrip) this.getDataSubscrip.unsubscribe();
    }

    getAllLicenses(){
        this.getDataSubscrip = this.licenseService.getByManager(this.managerId).subscribe(data => {
            this.data = data;
            this.initGrid();
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    initGrid(){
        var params = {
            selector: "#allLicenses"
        };

        this.datatableService.destroy(params.selector);
        this.datatableService.init2(params);
    }

    goToDetail(item){
        this.router.navigate([`/allocationManagement/licenses/${item.id}/detail`])
    }
}