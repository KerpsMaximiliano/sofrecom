import { OnInit, OnDestroy, Component, ViewChild } from "@angular/core";
import { LicenseService } from "app/services/human-resources/licenses.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { Router } from "@angular/router";
import { MessageService } from "app/services/common/message.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { LicenseStatus } from "../../../../models/enums/licenseStatus";
import { Subscription } from "rxjs";
import { DataTableService } from "app/services/common/datatable.service";
import * as FileSaver from "file-saver";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";

declare var $: any;

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

    public startDate: Date = new Date();
    public endDate: Date = new Date();

    public isLoading: boolean = false;

    constructor(private licenseService: LicenseService,
        private employeeService: EmployeeService,
        private router: Router,
        private datatableService: DataTableService,
        private messageService: MessageService,
        private errorHandlerService: ErrorHandlerService){
    }

    toggleNavigation(): void {
        $(".theme-config-box").toggleClass("show");
    }

    ngOnInit(): void {
        var data = JSON.parse(sessionStorage.getItem('lastLicenseQuery'));

        this.getEmployees(data);
        this.getLicenceTypes(data);

        if(data){
            this.searchLastQuery(data);
        }
        else{
            this.initGrid();
        }
    }

    ngOnDestroy(): void {
        if(this.getDataSubscrip) this.getDataSubscrip.unsubscribe();
        if(this.getLicenseTypeSubscrip) this.getLicenseTypeSubscrip.unsubscribe();
        if(this.getEmployeesSubscrip) this.getEmployeesSubscrip.unsubscribe();
    }

    getEmployees(lastQuery){
        this.getEmployeesSubscrip = this.employeeService.getAll().subscribe(data => {
            this.resources = data;

            setTimeout(() => {
                if(lastQuery){
                    $( "#employeeId" ).val(lastQuery.employeeId).trigger('change');
                }
            }, 0)
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    getLicenceTypes(lastQuery){
        this.getLicenseTypeSubscrip = this.licenseService.getLicenceTypes().subscribe(data => {

            data.optionsWithPayment.forEach(element => {
                this.licensesTypes.push(element);
            });

            data.optionsWithoutPayment.forEach(element => {
                this.licensesTypes.push(element);
            });

            setTimeout(() => {
                if(lastQuery){
                    this.licensesTypeId = lastQuery.licenseTypeId
                }
            }, 0)
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    searchLastQuery(data){
        var params = {
            employeeId: data.employeeId,
            licenseTypeId: data.licenseTypeId
        }

        this.search(params);
    }

    newSearch(){
        var params = {
            employeeId: $( "#employeeId" ).val(),
            licenseTypeId: this.licensesTypeId
        }

        this.search(params);
    }

    search(params){
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
        var params = {
            selector: "#licenseStatusApproved",
            columnDefs: [ {'aTargets': [3, 4, 5], "sType": "date-uk"} ]
        };

        this.datatableService.destroy(params.selector);
        this.datatableService.initialize(params);
    }

    clean(){
        sessionStorage.removeItem('lastLicenseQuery');
        $( "#employeeId" ).val(0).trigger('change');;
        this.licensesTypeId = 0;
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
        this.isLoading = true;
 
        this.licenseService.createReport(json).subscribe(file => {
            this.reportModal.hide();
            this.isLoading = false;
            this.messageService.closeLoading();

            var startDateName = `${this.startDate.getFullYear()}-${this.startDate.getMonth() == 0 ? 12 : this.startDate.getMonth()}`;
            var endDateName = `${this.endDate.getFullYear()}-${this.endDate.getMonth() == 0 ? 12 : this.endDate.getMonth()}`;

            FileSaver.saveAs(file, `${startDateName} - ${endDateName} Licencias.xlsx`);
        },
        err => {
            this.isLoading = false;
            this.reportModal.hide();
            this.errorHandlerService.handleErrors(err);
        });
    }

    goToAdd(){
        this.router.navigate([`/rrhh/licenses/add`])
    }
}