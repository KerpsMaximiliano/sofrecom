import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Router, ActivatedRoute } from "@angular/router";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from "app/services/common/message.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { DataTableService } from "app/services/common/datatable.service";
import { LicenseService } from "../../../../services/human-resources/licenses.service";
import { Cookie } from "ng2-cookies/ng2-cookies";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";

@Component({
    selector: 'resource-detail',
    templateUrl: './resource-detail.component.html',
    styleUrls: ['./resource-detail.component.scss']
})
export class ResourceDetailComponent implements OnInit, OnDestroy {

    @ViewChild('businessHoursModal') businessHoursModal;
    public businessHoursModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "allocationManagement.resources.businessHoursModal",
        "businessHoursModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    ); 

    resourceId: number;
    public model: any;

    public isRrhh: boolean = false;
    public isLoading: boolean = false;
    public businessHours: number;
    public businessHoursDescription: string;

    public licenses: any[] = new Array();
    public tasks: any[] = new Array();

    getSubscrip: Subscription;
    paramsSubscrip: Subscription;
    finalizeExtraHolidaysSubscrip: Subscription;
    getDataSubscrip: Subscription;
    getTasksSubscrip: Subscription;

    constructor(private router: Router,
                private menuService: MenuService,
                private messageService: MessageService,
                private licenseService: LicenseService,
                private dataTableService: DataTableService,
                private activatedRoute: ActivatedRoute,
                private employeeService: EmployeeService,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.resourceId = params['id'];

            this.isRrhh = this.menuService.userIsRrhh && this.menuService.user.employeeId != this.resourceId;

            var data = <any>JSON.stringify(this.activatedRoute.snapshot.data);
            var dataJson = JSON.parse(data);

            if(!dataJson.fromRrhh){
                if(Cookie.get('userInfo')){
                    var userApplicant = JSON.parse(Cookie.get('userInfo'));
            
                    if(userApplicant && userApplicant.employeeId){
                        if(userApplicant.employeeId != this.resourceId){
                            this.router.navigate([`/403`]);
                            return;
                        }
                    }
                }
            }

            this.getProfile();
            this.getLicenses();
            this.getTasks();
        });
    }

    ngOnDestroy(): void {
        if (this.getSubscrip) { this.getSubscrip.unsubscribe(); }
        if (this.paramsSubscrip) { this.paramsSubscrip.unsubscribe(); }
        if (this.getDataSubscrip) { this.getDataSubscrip.unsubscribe(); }
        if (this.finalizeExtraHolidaysSubscrip) { this.finalizeExtraHolidaysSubscrip.unsubscribe(); }
        if (this.getTasksSubscrip) { this.getTasksSubscrip.unsubscribe(); }
    }

    getProfile(){
        this.messageService.showLoading();

        this.getSubscrip = this.employeeService.getProfile(this.resourceId).subscribe(response => {
            this.model = response.data;

            this.businessHours = response.data.businessHours;
            this.businessHoursDescription = response.data.businessHoursDescription;

            this.messageService.closeLoading();
            this.initGrid();
        },
        error => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(error);
        });
    }

    initGrid(){
        const options = { selector: '#analyticsTable', columnDefs: [ {'aTargets': [4, 5], "sType": "date-uk"} ], order: [[ 5, "desc" ]] };
        this.dataTableService.init2(options);
    }

    goToLicenses(){
        this.router.navigate([`/profile/licenses/add`]);
    }

    getLicenses(){
        this.getDataSubscrip = this.licenseService.getByEmployee(this.resourceId).subscribe(data => {
            this.licenses = data;

            var params = {
                selector: "#licenses"
            };
    
            this.dataTableService.init2(params);
        },
        error => {});
    }

    goToDetail(item){
        this.router.navigate([`/allocationManagement/licenses/${item.id}/detail`])
    }

    finalizeExtraHolidays(){
        this.messageService.showLoading();

        this.finalizeExtraHolidaysSubscrip = this.employeeService.finalizeExtraHolidays(this.resourceId).subscribe(data => {
            if(data.messages) this.messageService.showMessages(data.messages);
        },
        error => this.errorHandlerService.handleErrors(error),
        () => this.messageService.closeLoading());
    }

    getTasks(){
        this.getDataSubscrip = this.employeeService.getTasks(this.resourceId).subscribe(response => {
            this.tasks = response.data;

            var params = { selector: "#tasksTable" };
    
            this.dataTableService.init2(params);
        },
        error => { this.errorHandlerService.handleErrors(error) });
    }

    updateBusinessHours(){
        var json = {
            businessHours: this.businessHours,
            businessHoursDescription: this.businessHoursDescription
        };

        this.isLoading = true;

        this.finalizeExtraHolidaysSubscrip = this.employeeService.updateBusinessHours(this.resourceId, json).subscribe(data => {
            this.isLoading = false
            if(data.messages) this.messageService.showMessages(data.messages);
            this.businessHoursModal.hide();

            this.model.businessHours = this.businessHours;
            this.model.businessHoursDescription = this.businessHoursDescription;
        },
        error => { 
            this.isLoading = false;
            this.errorHandlerService.handleErrors(error)
        });
    }

    canUpdateBusinessHours(){
        if(this.businessHours > 0 && this.businessHoursDescription && this.businessHoursDescription != ''){
            return true;
        }

        return false;
    }
} 