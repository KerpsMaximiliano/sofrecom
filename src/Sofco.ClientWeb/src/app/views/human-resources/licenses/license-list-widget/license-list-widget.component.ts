import { OnInit, OnDestroy, Component, Input } from "@angular/core";
import { LicenseService } from "app/services/human-resources/licenses.service";
import { Subscription } from "rxjs";
import { DataTableService } from "app/services/common/datatable.service";
import { Router } from "@angular/router";

declare var moment: any;

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
        private router: Router,
        private datatableService: DataTableService){
    }

    ngOnInit(): void {
        if(this.managerId > 0){
            this.getDataSubscrip = this.licenseService.getByManagerAndStatus(this.managerId, this.statusId).subscribe(data => {
                this.data = data;
                this.initGrid()
            },
            () => { });
        }
        else{
            this.getDataSubscrip = this.licenseService.getByStatus(this.statusId).subscribe(data => {
                this.data = data;
                this.initGrid()
            },
            () => { });
        }
    }

    ngOnDestroy(): void {
        if(this.getDataSubscrip) this.getDataSubscrip.unsubscribe();
    }
 
    initGrid(){
        var columns = [0, 1, 2, 3, 4, 5, 6, 7];
        var title = `Licencias-${moment(new Date()).format("YYYYMMDD")}`;

        var params = {
            selector: "#licenseStatus-" + this.statusId,
            columns: columns,
            title: title,
            order: [[ 3, "desc" ]],
            withExport: true,
            columnDefs: [ {'aTargets': [3, 4, 5], "sType": "date-uk"} ]
        };

        this.datatableService.initialize(params);
    }

    goToDetail(item){
        this.router.navigate([`/rrhh/licenses/${item.id}/detail`])
    }
}