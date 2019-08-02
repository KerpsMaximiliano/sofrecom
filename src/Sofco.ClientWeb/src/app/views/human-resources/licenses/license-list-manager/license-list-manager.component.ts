import { OnInit, OnDestroy, Component } from "@angular/core";
import { LicenseService } from "../../../../services/human-resources/licenses.service";
import { Router } from "@angular/router";
import { LicenseStatus } from "../../../../models/enums/licenseStatus";
import { Subscription } from "rxjs";
import { DataTableService } from "../../../../services/common/datatable.service";
import { UserInfoService } from "../../../../services/common/user-info.service";

declare var moment: any;

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
        private datatableService: DataTableService){
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
        });
    }

    initGrid(){
        var columns = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
        var title = `Licencias-Aprobadas-${moment(new Date()).format("YYYYMMDD")}`;

        var params = {
            selector: "#allLicenses", 
            columns: columns,
            title: title,
            order: [[ 3, "desc" ]],
            withExport: true,
            columnDefs: [ {'aTargets': [4, 5, 6], "sType": "date-uk"} ]
        };

        this.datatableService.destroy(params.selector);
        this.datatableService.initialize(params);
    }

    goToDetail(item){
        this.router.navigate([`/rrhh/licenses/${item.id}/detail`])
    }
}