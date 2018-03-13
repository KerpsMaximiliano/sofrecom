import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { MessageService } from "app/services/common/message.service";
import { MenuService } from "app/services/admin/menu.service";
import { Router, ActivatedRoute } from "@angular/router";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { DateRangePickerComponent } from "app/components/datepicker/date-range-picker.component";
import { AllocationService } from "app/services/allocation-management/allocation.service";
import { AllocationModel, Allocation } from "app/models/allocation-management/allocation";
import { AppSetting } from 'app/services/common/app-setting'

declare var $:any;

@Component({
    selector: 'add-allocation-by-resource',
    templateUrl: './add-by-resource.component.html',
    styleUrls: ['./add-by-resource.component.scss']
})

export class AddAllocationByResourceComponent implements OnInit, OnDestroy {

    getAllSubscrip: Subscription;
    paramsSubscrip: Subscription;
    getByIdSubscrip: Subscription;

    analytics: any = new Array<any>();

    resource: any;
    resourceId: number;

    public monthQuantity: number = 12;

    @ViewChild('allocations') allocations: any;

    dateSince: Date = new Date();
    public dateOptions;

    pmoUser: boolean;
  
    constructor(private analyticService: AnalyticService,
        private router: Router,
        private menuService: MenuService,
        private allocationsService: AllocationService,
        private messageService: MessageService,
        private activatedRoute: ActivatedRoute,
        private employeeService: EmployeeService,
        private errorHandlerService: ErrorHandlerService,
        private appSetting: AppSetting){
            this.dateOptions = this.menuService.getDatePickerOptions();
    }

    ngOnDestroy(): void {
        if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.getByIdSubscrip) this.getByIdSubscrip.unsubscribe();
    }

    ngOnInit(): void {
        this.monthQuantity = this.appSetting.AllocationManagement_Months;
        this.pmoUser = this.menuService.hasFunctionality('ALLOC', 'QARDD');
        var resource = JSON.parse(sessionStorage.getItem("resource"));
        
        if(resource){
            this.resource = resource;
            sessionStorage.removeItem("resource");
            this.resourceId = this.resource.id;
        }
        else{
            this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
                this.resourceId = params['id'];

                this.getByIdSubscrip = this.employeeService.getById(params['id']).subscribe(data => {
                    this.resource = data.data;
                },
                error => this.errorHandlerService.handleErrors(error));
            });
        }

        this.getAllSubscrip = this.analyticService.getOptions().subscribe(data => {
            this.analytics = data;
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    add(){
        var analyticId = $('#analyticId').val();

        var analytic = this.analytics.find(x => x.id == analyticId);

        this.allocations.add(analytic);
    }

    search(){
        if(this.pmoUser){
            this.allocations.getAllocations(this.resourceId, this.dateSince);
        }
        else{
            this.allocations.getAllocations(this.resourceId, new Date());
        }
    }
}