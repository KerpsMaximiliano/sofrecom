import { OnInit, OnDestroy, Component } from "@angular/core";
import { LicenseService } from "../../../../services/human-resources/licenses.service";
import { Router } from "@angular/router";
import { LicenseStatus } from "../../../../models/enums/licenseStatus";
import { Subscription } from "rxjs";
import { DataTableService } from "../../../../services/common/datatable.service";
import { UserInfoService } from "../../../../services/common/user-info.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";

declare var moment: any;

@Component({
    selector: 'license-list-manager',
    templateUrl: './license-list-manager.component.html',
    styleUrls: ['./license-list-manager.component.scss']
})
export class LicenseListManager implements OnInit, OnDestroy {

    public authPending: number = LicenseStatus.AuthPending;

    getDataSubscrip: Subscription;
    getLicenseTypeSubscrip: Subscription;
    getEmployeesSubscrip: Subscription;

    public data: any[] = new Array();
    public dataFiltered: any[] = new Array();
    public resources: any[] = new Array();
    public licensesTypes: any[] = new Array();

    public licensesTypeId: any;
    public employeeId: any;
    public managerId: number = 0;

    public dateSince: Date = null;
    public dateTo: Date = null;

    constructor(private licenseService: LicenseService,
        private router: Router,
        private employeeService: EmployeeService,
        private datatableService: DataTableService){
    }

    ngOnInit(): void {
        const userInfo = UserInfoService.getUserInfo();

        this.getEmployees();
        this.getLicenceTypes();

        if(userInfo && userInfo.id){
            this.managerId = userInfo.id;
        }

        this.getAllLicenses();
    }

    ngOnDestroy(): void {
        if(this.getDataSubscrip)
            this.getDataSubscrip.unsubscribe();

        if(this.getLicenseTypeSubscrip) 
            this.getLicenseTypeSubscrip.unsubscribe();
    
        if(this.getEmployeesSubscrip)
            this.getEmployeesSubscrip.unsubscribe();
    }

    getEmployees(){
        this.getEmployeesSubscrip = this.employeeService.getAll().subscribe(data => {
            this.resources = data;
        });
    }

    getLicenceTypes(){
        this.getLicenseTypeSubscrip = this.licenseService.getLicenceTypes().subscribe(data => {

            var list = [];
            data.optionsWithPayment.forEach(element => {
                list.push(element);
            });

            data.optionsWithoutPayment.forEach(element => {
                list.push(element);
            });

            this.licensesTypes = list;
        });
    }

    getAllLicenses(){
        this.getDataSubscrip = this.licenseService.getByManager(this.managerId).subscribe(data => {
            this.data = data;
            this.newSearch();
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

    newSearch(){
        this.dataFiltered = [];

        this.data.forEach(x => {
            var addItem = true;

            if(this.employeeId  && this.employeeId > 0 && this.employeeId != x.employeeId){
                addItem = false;
            }

            if(this.licensesTypeId && this.licensesTypeId > 0 && this.licensesTypeId  != x.licenseTypeId){
                addItem = false;
            }

            if(this.dateSince >= x.startDate){
                addItem = false;
            }

            if(this.dateTo <= x.toDate){
                addItem = false;
            }

            if(addItem){
                this.dataFiltered.push(x);
            }
        });

        this.initGrid();
    }

    clean(){
        this.employeeId = null;
        this.licensesTypeId = null;
        this.dateSince = null;
        this.dateTo = null;
    }
}