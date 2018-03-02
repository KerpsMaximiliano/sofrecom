import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Router, ActivatedRoute } from "@angular/router";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { AllocationService } from "app/services/allocation-management/allocation.service";
import { MenuService } from "app/services/admin/menu.service";
import { AllocationSearch } from "app/models/allocation-management/allocationSearch";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { MessageService } from "app/services/common/message.service";
import { DateRangePickerComponent } from "app/components/datepicker/date-range-picker.component";
import { AppSetting } from 'app/services/common/app-setting'

declare var $:any;

@Component({
    selector: 'add-by-analytic',
    templateUrl: './add-by-analytic.component.html'
})

export class AddAllocationComponent implements OnInit, OnDestroy {

    public analytic;
    public resources: any[] = new Array<any>();
    public allocationToSearch: AllocationSearch = new AllocationSearch();
    public monthQuantity: number = 12;

    getByIdSubscrip: Subscription;
    paramsSubscrip: Subscription;
    getAllocationResourcesSubscrip: Subscription;
    addSubscrip: Subscription;

    @ViewChild('resourceTimeline') resourceTimeline: any;
    @ViewChild('allocations') allocations: any;

    public resourceId: number = 0; 

    dateSince: Date = new Date();
    public dateOptions;

    pmoUser: boolean;

    constructor(private analyticService: AnalyticService,
                private router: Router,
                private menuService: MenuService,
                private messageService: MessageService,
                private allocationService: AllocationService,
                private employeeService: EmployeeService,
                private activatedRoute: ActivatedRoute,
                private errorHandlerService: ErrorHandlerService,
                private appSetting: AppSetting){
                    
                this.dateOptions = this.menuService.getDatePickerOptions();
    }

    ngOnInit(): void {
        this.monthQuantity = this.appSetting.AllocationManagement_Months;
        this.pmoUser = this.menuService.hasFunctionality('ALLOC', 'QARDD');

        var analytic = JSON.parse(sessionStorage.getItem("analytic"));

        if(analytic){
            this.analytic = analytic;
            sessionStorage.removeItem("analytic");
            this.getTimeline(this.analytic.id);
        }
        else{
            this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {

                this.getByIdSubscrip = this.analyticService.getById(params['id']).subscribe(data => {
                    this.analytic = data;

                    this.getTimeline(params['id']);
                },
                error => this.errorHandlerService.handleErrors(error));
            });
        } 

        this.getAllocationResources();
    }

    ngOnDestroy(): void {
        if(this.getByIdSubscrip) this.getByIdSubscrip.unsubscribe();
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.getAllocationResourcesSubscrip) this.getAllocationResourcesSubscrip.unsubscribe();
        if(this.addSubscrip) this.addSubscrip.unsubscribe();
    }

    getAllocationResources(){
        this.getAllocationResourcesSubscrip = this.employeeService.getAll().subscribe(data => {
            this.resources = data;
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    getTimeline(analyticId){
        this.resourceTimeline.model = new Array<any>();
        this.resourceTimeline.getAllocations(analyticId);
    }

    search(){
        var employeeId = $('#employeeId').val();
        this.resourceId = employeeId;

        if(this.pmoUser){
            this.allocations.getAllocations(employeeId, this.dateSince);
        }
        else{
            this.allocations.getAllocations(employeeId, new Date());
        }
    }
}