import { OnInit, OnDestroy, Component } from "@angular/core";
import { LicenseService } from "app/services/human-resources/licenses.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { Router, ActivatedRoute } from "@angular/router";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from "app/services/common/message.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { LicenseStatus } from "../../../../models/enums/licenseStatus";
import { Subscription } from "rxjs";
import { DataTableService } from "app/services/common/datatable.service";

declare var $: any;

@Component({
    selector: 'license-list-rrhh',
    templateUrl: './license-list-rrhh.component.html'
})
export class LicenseListRrhh implements OnInit, OnDestroy {

    public pending: number = LicenseStatus.Pending;
    public authPending: number = LicenseStatus.AuthPending;
    public approved: number = LicenseStatus.Approved;

    getDataSubscrip: Subscription;
    getLicenseTypeSubscrip: Subscription;
    getEmployeesSubscrip: Subscription;

    public data: any[] = new Array();
    public resources: any[] = new Array();
    public licensesTypes: any[] = new Array();

    constructor(private licenseService: LicenseService,
        private employeeService: EmployeeService,
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private datatableService: DataTableService,
        private menuService: MenuService,
        private messageService: MessageService,
        private errorHandlerService: ErrorHandlerService){
    }

    toggleNavigation(): void {
        $(".theme-config-box").toggleClass("show");
    }

    ngOnInit(): void {
        this.getEmployees();
        this.getLicenceTypes();
        this.initGrid();
    }

    ngOnDestroy(): void {
        if(this.getDataSubscrip) this.getDataSubscrip.unsubscribe();
        if(this.getLicenseTypeSubscrip) this.getLicenseTypeSubscrip.unsubscribe();
        if(this.getEmployeesSubscrip) this.getEmployeesSubscrip.unsubscribe();
    }

    getEmployees(){
        this.getEmployeesSubscrip = this.employeeService.getAll().subscribe(data => {
            this.resources = data;
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    getLicenceTypes(){
        this.getLicenseTypeSubscrip = this.licenseService.getLicenceTypes().subscribe(data => {

            data.optionsWithPayment.forEach(element => {
                this.licensesTypes.push(element);
            });

            data.optionsWithoutPayment.forEach(element => {
                this.licensesTypes.push(element);
            });
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    search(){
        var params = {
            employeeId: $( "#employeeId" ).val(),
            licenseTypeId: $( "#licensesTypeId" ).val(),
        }

        this.messageService.showLoading();

        this.getDataSubscrip = this.licenseService.search(params).subscribe(data => {
            this.messageService.closeLoading();
            this.data = data;
            this.initGrid()
        },
        error => {});
    }

    initGrid(){
        var params = {
            selector: "#licenseStatusApproved",
            scrollX: true
        };

        this.datatableService.destroy(params.selector);
        this.datatableService.init2(params);
    }

    clean(){
        $( "#employeeId" ).val(0).trigger('change');;
        $( "#licensesTypeId" ).val(0).trigger('change');;
    }
}