import { OnInit, OnDestroy, Component, ViewChild } from "@angular/core";
import { LicenseService } from "../../../../services/human-resources/licenses.service";
import { EmployeeService } from "../../../../services/allocation-management/employee.service";
import { Router } from "@angular/router";
import { MessageService } from "../../../../services/common/message.service";
import { LicenseStatus } from "../../../../models/enums/licenseStatus";
import { Subscription } from "rxjs";
import { DataTableService } from "../../../../services/common/datatable.service";
import * as FileSaver from "file-saver";
import { Ng2ModalConfig } from "../../../../components/modal/ng2modal-config";

declare var $: any;
declare var moment: any;

@Component({
    selector: 'license-list-rrhh',
    templateUrl: './license-list-rrhh.component.html',
    styleUrls: ['./license-list-rrhh.component.scss']
})
export class LicenseListRrhh implements OnInit, OnDestroy {

    @ViewChild('reportModal') reportModal;
    public reportModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "reportModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    public dateSince: Date = null;
    public dateTo: Date = null;

    public pending: number = LicenseStatus.Pending;
    public authPending: number = LicenseStatus.AuthPending;
    public approved: number = LicenseStatus.Approved;

    getDataSubscrip: Subscription;
    getLicenseTypeSubscrip: Subscription;
    getEmployeesSubscrip: Subscription;

    public data: any[] = new Array();
    public resources: any[] = new Array();
    public licensesTypes: any[] = new Array();

    public licensesTypeId: any;
    public employeeId: any;

    public startDate: Date = new Date();
    public endDate: Date = new Date();
    public lblDate: boolean;

    constructor(private licenseService: LicenseService,
        private employeeService: EmployeeService,
        private router: Router,
        private datatableService: DataTableService,
        private messageService: MessageService){
    }

    toggleNavigation(): void {
        $(".theme-config-box").toggleClass("show");
    }

    ngOnInit(): void {
        var data = JSON.parse(sessionStorage.getItem('lastLicenseQuery'));

        this.getEmployees();
        this.getLicenceTypes();

        if(data){
            this.searchLastQuery(data);
        }
        else{
            this.newSearch();
        }
    }

    ngOnDestroy(): void {
        if(this.getDataSubscrip) this.getDataSubscrip.unsubscribe();
        if(this.getLicenseTypeSubscrip) this.getLicenseTypeSubscrip.unsubscribe();
        if(this.getEmployeesSubscrip) this.getEmployeesSubscrip.unsubscribe();
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

    searchLastQuery(data){
        var params = {
            employeeId: data.employeeId,
            licenseTypeId: data.licenseTypeId
        }

        this.search(params);
    }

    newSearch(){
        if(this.dateSince > this.dateTo){
            this.lblDate = true;
            return;
        }

        var params = {
            employeeId: this.employeeId,
            licenseTypeId: this.licensesTypeId,
            dateSince: this.dateSince,
            dateTo: this.dateTo
        }
        this.search(params);
    }

    search(params){
        this.lblDate = false;
        this.messageService.showLoading();

        this.getDataSubscrip = this.licenseService.search(params).subscribe(data => {
            this.messageService.closeLoading();
            this.data = data;
            this.initGrid();
            sessionStorage.setItem('lastLicenseQuery', JSON.stringify(params));
        },
        error => {});

    }

    initGrid(){
        var columns = [0, 1, 2, 3, 4, 5, 6, 7, 8, 10];
        var title = `Licencias-Aprobadas-${moment(new Date()).format("YYYYMMDD")}`;

        var params = {
            selector: "#licenseStatusApproved", 
            columns: columns,
            title: title,
            withExport: true,
            columnDefs: [ {'aTargets': [4, 5, 6], "sType": "date-uk"} ]
        };

        this.datatableService.destroy(params.selector);
        this.datatableService.initialize(params);
    }

    clean(){
        sessionStorage.removeItem('lastLicenseQuery');
        this.employeeId = null;
        this.licensesTypeId = null;
        this.dateSince = null;
        this.dateTo = null;
        this.newSearch();
    }

    goToDetail(item){
        this.router.navigate([`/rrhh/licenses/${item.id}/detail`])
    }

    createReport(){
        var json = {
            startDate: this.startDate.toUTCString(),
            endDate: this.endDate.toUTCString()
        }

        this.messageService.showLoading();
 
        this.licenseService.createReport(json).subscribe(file => {
            this.reportModal.hide();
            this.messageService.closeLoading();

            var startDateName = `${this.startDate.getFullYear()}-${this.startDate.getMonth() == 0 ? 12 : this.startDate.getMonth()}`;
            var endDateName = `${this.endDate.getFullYear()}-${this.endDate.getMonth() == 0 ? 12 : this.endDate.getMonth()}`;

            FileSaver.saveAs(file, `${startDateName} - ${endDateName} Licencias.xlsx`);
        },
        err => {
            this.messageService.showWarningByFolder('rrhh/license', 'reportWithoutEmpty');

            this.reportModal.hide();
            this.messageService.closeLoading();
        });
    }

    goToAdd(){
        this.router.navigate([`/rrhh/licenses/add`])
    }
}
