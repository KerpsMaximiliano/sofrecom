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

declare var $:any;

@Component({
    selector: 'add-allocation',
    templateUrl: './add-allocation.component.html'
})

export class AddAllocationComponent implements OnInit, OnDestroy {

    public analytic;
    public resources: any[] = new Array<any>();
    public allocationToSearch: AllocationSearch = new AllocationSearch();

    getByIdSubscrip: Subscription;
    paramsSubscrip: Subscription;
    getAllocationResourcesSubscrip: Subscription;
    addSubscrip: Subscription;

    showPanelAllocation: boolean = false;

    @ViewChild('allocationList') allocationList: any;

    public dateOptions;

    constructor(private analyticService: AnalyticService,
                private router: Router,
                private menuService: MenuService,
                private messageService: MessageService,
                private allocationService: AllocationService,
                private employeeService: EmployeeService,
                private activatedRoute: ActivatedRoute,
                private errorHandlerService: ErrorHandlerService){

                this.dateOptions = this.menuService.getDatePickerOptions();
    }

    ngOnInit(): void {
        var analytic = JSON.parse(sessionStorage.getItem("analytic"));

        if(analytic){
            this.analytic = analytic;
            sessionStorage.removeItem("analytic");
        }
        else{
            this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {

                this.getByIdSubscrip = this.analyticService.getById(params['id']).subscribe(data => {
                    this.analytic = data;
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
        this.getAllocationResourcesSubscrip = this.employeeService.getOptions().subscribe(data => {
            this.resources = data;
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    search(){
        var employeeId = $('#employeeId').val();

        this.showPanelAllocation = true;
        this.allocationList.model = new Array<any>();
        this.allocationList.getAllocations(employeeId, this.analytic.startDate, this.analytic.endDate);
    }

    assign(){
        var employeeId = $('#employeeId').val();

        var employee = this.resources.find(x => x.id == employeeId);

        var json = {
            analyticId: this.analytic.id,
            employeeId: employeeId,
            billingPercentage: employee ? employee.billingPercentage : 0,
            percentage: this.allocationToSearch.percentage,
            dateSince: this.allocationToSearch.dateSince,
            dateTo: this.allocationToSearch.dateTo
        }

        this.addSubscrip = this.allocationService.add(json).subscribe(data => {
            if(data.messages) this.messageService.showMessages(data.messages);

            this.allocationList.getAllocations(employeeId, this.analytic.startDate, this.analytic.endDate);
        },
        error => this.errorHandlerService.handleErrors(error));
    }
}