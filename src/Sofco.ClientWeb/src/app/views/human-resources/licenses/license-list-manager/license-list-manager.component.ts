import { OnInit, OnDestroy, Component } from "@angular/core";
import { LicenseService } from "../../../../services/human-resources/licenses.service";
import { Router } from "@angular/router";
import { LicenseStatus } from "../../../../models/enums/licenseStatus";
import { Subscription } from "rxjs";
import { DataTableService } from "../../../../services/common/datatable.service";
import { UserInfoService } from "../../../../services/common/user-info.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { MessageService } from "app/services/common/message.service";

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
        private datatableService: DataTableService,
        private messageService: MessageService){
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

    newSearchItem(){
        if(this.dateTo == null || this.dateSince == null) {
            if(this.dateSince == null) {
                this.messageService.showMessage("La Fecha Inicio es requerida", 1);
            }
            if(this.dateTo == null) {
                this.messageService.showMessage("La Fecha Fin es requerida", 1);
            }
            return;
        };
        
        this.dataFiltered = [];

        const newDateSince = moment(this.dateSince).format('YYYY-MM-DD');
        const newDateto = moment(this.dateTo).format('YYYY-MM-DD');

        function verifyFilterDates(filterStart, filterEnd, license) {
            if((filterStart <= license.startDate && license.startDate <= filterEnd) || (filterStart <= license.endDate && license.endDate <= filterEnd)) {
                return true;
            } else {
                return false;
            }
        }

        if(this.dateSince != null && this.dateTo != null && this.employeeId != null && this.licensesTypeId != null){
            for(let i = 0; i < this.data.length; i++){
                if(verifyFilterDates(newDateSince, newDateto, this.data[i]) && this.data[i].employeeId == this.employeeId && this.data[i].licenseTypeId == this.licensesTypeId){
                    this.dataFiltered.push(this.data[i]);
                }    
            }

            this.initGrid();
            return;
        }

        if(this.dateSince != null && this.dateTo != null && this.employeeId != null){
            for(let i = 0; i < this.data.length; i++){
                if(verifyFilterDates(newDateSince, newDateto, this.data[i]) && this.data[i].employeeId == this.employeeId){
                    this.dataFiltered.push(this.data[i]);
                }    
            }

            this.initGrid();
            return;
        }

        if(this.dateSince != null && this.dateTo != null && this.licensesTypeId != null){
            for(let i = 0; i < this.data.length; i++){
                if(verifyFilterDates(newDateSince, newDateto, this.data[i]) && this.licensesTypeId == this.data[i].licenseTypeId){
                    this.dataFiltered.push(this.data[i]);
                }
            }

            this.initGrid();
            return;
        }

        if(this.dateSince != null && this.dateTo != null){
            for(let i = 0; i < this.data.length; i++){           
                if(verifyFilterDates(newDateSince, newDateto, this.data[i])){
                    
                    if(this.licensesTypeId == 0 || this.licensesTypeId == undefined){
                        this.dataFiltered.push(this.data[i]);
                    }else{
                        if(this.licensesTypeId != 0 && this.licensesTypeId == this.data[i].licenseTypeId){
                            this.dataFiltered.push(this.data[i]);
                        }
                    }
                    
                }
            }

            this.dataFiltered.push(this.data);
            this.initGrid();

        }else if(this.dateSince != null && this.dateTo == null){
            for(let i = 0 ; i < this.data.length; i++){
                
                if(this.data[i].endDate >= newDateSince){
                    
                    if(this.licensesTypeId == 0 || this.licensesTypeId == undefined){
                        this.dataFiltered.push(this.data[i]);
                    }else{

                        if(this.licensesTypeId != 0 && this.licensesTypeId == this.data[i].licenseTypeId){
                            this.dataFiltered.push(this.data[i]);
                        }
                    }

                }
            }
            
            this.dataFiltered.push(this.data);
            this.initGrid();
        }else if(this.dateSince == null && this.dateTo != null) {
            for(let i = 0 ; i < this.data.length; i++){
                if(this.data[i].endDate <= newDateto){
                    if(this.licensesTypeId == 0 || this.licensesTypeId == undefined){
                        this.dataFiltered.push(this.data[i]);
                    }else{
                        if(this.licensesTypeId != 0 && this.licensesTypeId == this.data[i].licenseTypeId){
                            this.dataFiltered.push(this.data[i]);
                        }
                    }
                }
            }
            this.dataFiltered.push(this.data);
            this.initGrid();
        }else{
            this.newSearch();
        }

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

        this.newSearch();
    }
}