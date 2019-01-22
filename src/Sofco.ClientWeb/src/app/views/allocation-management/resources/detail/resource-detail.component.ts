import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { Router, ActivatedRoute } from "@angular/router";
import { MenuService } from "../../../../services/admin/menu.service";
import { MessageService } from "../../../../services/common/message.service";
import { EmployeeService } from "../../../../services/allocation-management/employee.service";
import { DataTableService } from "../../../../services/common/datatable.service";
import { LicenseService } from "../../../../services/human-resources/licenses.service";
import { Ng2ModalConfig } from "../../../../components/modal/ng2modal-config";
import { UserInfoService } from "../../../../services/common/user-info.service";
import { UserService } from "../../../../services/admin/user.service";
import { EmployeeProfileHistoryService } from "../../../../services/allocation-management/employee-profile-history.service";
import { I18nService } from "../../../../services/common/i18n.service";
import { WorkflowStateType } from "app/models/enums/workflowStateType";

@Component({
    selector: 'resource-detail',
    templateUrl: './resource-detail.component.html',
    styleUrls: ['./resource-detail.component.scss']
})
export class ResourceDetailComponent implements OnInit, OnDestroy {

    @ViewChild('businessHoursModal') businessHoursModal;
    public businessHoursModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "allocationManagement.resources.hoursByContract",
        "businessHoursModal",
        false,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    ); 

    resourceId: number;
    userLoggedId: number;
    public model: any;

    public isRrhh: boolean = false;
    public isManager: boolean = false;

    public editModel = {
        businessHours: 0,
        businessHoursDescription: "",
        office: "",
        managerId: null,
        billingPercentage: 0,
        holidaysPending: null,
        hasCreditCard: false
    }

    public licenses: any[] = new Array();
    public tasks: any[] = new Array();
    public managers: any[] = new Array();
    public profileHistories: any[] = new Array();
    public advancements: any[] = new Array();
    public refunds: any[] = new Array();

    getSubscrip: Subscription;
    paramsSubscrip: Subscription;
    finalizeExtraHolidaysSubscrip: Subscription;
    getDataSubscrip: Subscription;
    getTasksSubscrip: Subscription;
    getRefundDataSubscrip: Subscription;
    getManagersSubscript: Subscription;

    constructor(private router: Router,
                private i18nService: I18nService,
                private menuService: MenuService,
                private messageService: MessageService,
                private licenseService: LicenseService,
                private userService: UserService,
                private dataTableService: DataTableService,
                private activatedRoute: ActivatedRoute,
                private employeeService: EmployeeService,
                private employeeProfileHistoryService: EmployeeProfileHistoryService){

        this.businessHoursModalConfig.acceptInlineButton = true;
    }

    ngOnInit(): void {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.resourceId = params['id'];

            this.isRrhh = this.menuService.userIsRrhh && this.menuService.user.employeeId != this.resourceId;

            var data = <any>JSON.stringify(this.activatedRoute.snapshot.data);
            var dataJson = JSON.parse(data);

            if (!dataJson.fromRrhh) {
                const userInfo = UserInfoService.getUserInfo();

                if (userInfo && userInfo.employeeId && userInfo.employeeId != this.resourceId) {
                    this.router.navigate([`/403`]);
                    return;
                }

                this.userLoggedId = userInfo.id;
            }

            this.getUsers();
            this.getAdvancements();
            this.getProfile();
            this.getLicenses();
            this.getTasks();
            this.getRefunds();
        });
    }

    ngOnDestroy(): void {
        if (this.getSubscrip) { this.getSubscrip.unsubscribe(); }
        if (this.paramsSubscrip) { this.paramsSubscrip.unsubscribe(); }
        if (this.getDataSubscrip) { this.getDataSubscrip.unsubscribe(); }
        if (this.finalizeExtraHolidaysSubscrip) { this.finalizeExtraHolidaysSubscrip.unsubscribe(); }
        if (this.getTasksSubscrip) { this.getTasksSubscrip.unsubscribe(); }
        if (this.getRefundDataSubscrip) { this.getRefundDataSubscrip.unsubscribe(); }
    }

    getProfile(){
        this.messageService.showLoading();

        this.getSubscrip = this.employeeService.getProfile(this.resourceId).subscribe(response => {
            this.model = response.data;

            this.editModel.businessHours = response.data.businessHours;
            this.editModel.businessHoursDescription = response.data.businessHoursDescription;
            this.editModel.office = response.data.officeAddress;
            this.editModel.holidaysPending = response.data.holidaysPendingByLaw;
            this.editModel.managerId = response.data.managerId.toString();
            this.editModel.billingPercentage = response.data.percentage;
            this.editModel.hasCreditCard = response.data.hasCreditCard;

            this.messageService.closeLoading();
            this.initGrid();
            this.getProfileHistories();
        },
        () => this.messageService.closeLoading());
    }

    initGrid(){
        const options = { selector: '#analyticsTable', columnDefs: [ {'aTargets': [5], "sType": "date-uk"} ], order: [[ 5, "desc" ]] };
        this.dataTableService.initialize(options);
    }

    goToLicenses(){
        this.router.navigate([`/profile/licenses/add`]);
    }

    goToTimeSheet(){
        this.router.navigate([`/profile/workTime`]);
    }

    getAdvancements(){
        this.getDataSubscrip = this.employeeService.getAdvancements(this.resourceId).subscribe(response => {
            this.advancements = response.data;

            var params = {
                selector: "#advancementTable",
                columnDefs: [ {'aTargets': [4], "sType": "date-uk"} ]
            };
    
            this.dataTableService.initialize(params);
        });
    }

    getRefunds(){
        this.getRefundDataSubscrip = this.employeeService.getRefunds(this.resourceId).subscribe(response => {
            this.refunds = response.data;

            var params = {
                selector: "#refundTable",
                columnDefs: [ {'aTargets': [1], "sType": "date-uk"} ]
            };
    
            this.dataTableService.initialize(params);
        });
    }

    getLicenses(){
        this.getDataSubscrip = this.licenseService.getByEmployee(this.resourceId).subscribe(data => {
            this.licenses = data;

            var params = {
                selector: "#licenses",
                order: [[ 2, "desc" ]],
                columnDefs: [ {'aTargets': [2, 4, 5], "sType": "date-uk"} ]
            };
    
            this.dataTableService.initialize(params);
        });
    }

    goToDetail(item){
        this.router.navigate([`/rrhh/licenses/${item.id}/detail`])
    }

    finalizeExtraHolidays(){
        this.messageService.showLoading();

        this.finalizeExtraHolidaysSubscrip = this.employeeService.finalizeExtraHolidays(this.resourceId).subscribe(data => {
            this.messageService.closeLoading()
        },
        () => this.messageService.closeLoading());
    }

    getTasks(){
        this.getDataSubscrip = this.employeeService.getTasks(this.resourceId).subscribe(response => {
            this.tasks = response.data;

            var params = { selector: "#tasksTable" };
    
            this.dataTableService.initialize(params);
        });
    }

    update(){
        var json = {
            businessHours: this.editModel.businessHours,
            businessHoursDescription: this.editModel.businessHoursDescription,
            holidaysPending: this.editModel.holidaysPending,
            office: this.editModel.office,
            billingPercentage: this.editModel.billingPercentage,
            managerId: this.editModel.managerId,
            hasCreditCard: this.editModel.hasCreditCard
        };

        this.finalizeExtraHolidaysSubscrip = this.employeeService.put(this.resourceId, json).subscribe(data => {
            this.businessHoursModal.hide();

            setTimeout(() => {
                window.location.reload();
            }, 750);
        },
        () => this.businessHoursModal.resetButtons());
    }

    canAddLicense(){
        if(!this.isRrhh && this.menuService.hasFunctionality('PROFI', 'ALTA')) return true;

        return false;
    }

    canAddWorkTime(){
        if(!this.isRrhh && this.menuService.hasFunctionality('PROFI', 'WORKT')) return true;

        return false;
    }

    getUsers(){
        this.getManagersSubscript = this.userService.getManagersAndDirectors().subscribe(response => {
            this.managers = response;
        });
    }

    getFormattedPhone() {
        if(this.model == null) return "";

        const phoneCountryCode = this.model.phoneCountryCode;
        const phoneAreaCode = this.model.phoneAreaCode;
        const phoneNumber = this.model.phoneNumber;

        return "+"+phoneCountryCode+" "+phoneAreaCode+" "+phoneNumber;
    }

    getProfileHistories() {
        this.getSubscrip = this.employeeProfileHistoryService.getByEmployeeNumber(this.model.employeeNumber).subscribe(response => {
            this.profileHistories = this.mapProfileHistories(response.data);

            var params = { selector: "#profileHistoryTable" };
            this.dataTableService.initialize(params);
        });
    }

    mapProfileHistories(data:any[]) {
        const result = [];
        const self = this;

        this.resolveReference(data);

        data.forEach(x => {
            const fields = JSON.parse(x.fields);
            fields.forEach(item => {
                const key = self.lowerizeFirstLetter(item);
                result.push({
                    "field": self.i18nService.translateByKey('allocationManagement.resources.grid.' + key),
                    "oldValue": x.employeePrevious[key],
                    "newValue": x.employee[key],
                    "dateTime": x.dateTime
                });
            });
        });

        return result;
    }

    lowerizeFirstLetter(txt) {
        return txt.charAt(0).toLowerCase() + txt.slice(1);
    }

    resolveReference(data:any[]) {
        const self = this;

        data.forEach(x => {
            const fields = JSON.parse(x.fields);
            fields.forEach(item => {
                const key = self.lowerizeFirstLetter(item);
                if(key === 'managerId')
                {
                    self.setManagerReference(x);
                }
            });
        });
    }

    setManagerReference(item)
    {
        item.employee.managerId = item.employee.manager.name;
        item.employeePrevious.managerId = item.employeePrevious.manager.name;
    }

    getStatusClass(type){
        switch(type){
            case WorkflowStateType.Info: return "label-success";
            case WorkflowStateType.Warning: return "label-warning";
            case WorkflowStateType.Success: return "label-primary";
            case WorkflowStateType.Danger: return "label-danger";
        }
    }
    
    goToAdvancementDetail(item){
        this.router.navigate(['/advancementAndRefund/advancement/' + item.id])
    }

    goToRefundDetail(item){
        this.router.navigate(['/advancementAndRefund/refund/' + item.id])
    }

    userLoggedIsManager(){
        return this.editModel.managerId == this.userLoggedId;
    }
}
