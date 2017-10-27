import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { MessageService } from "app/services/common/message.service";
import { I18nService } from "app/services/common/i18n.service";
import { MenuService } from "app/services/admin/menu.service";
import { Router, ActivatedRoute } from "@angular/router";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { DateRangePickerComponent } from "app/components/datepicker/date-range-picker.component";
import { AllocationService } from "app/services/allocation-management/allocation.service";
import { AllocationModel, Allocation } from "app/models/allocation-management/allocation";

declare var $:any;

@Component({
    selector: 'add-allocation-by-resource',
    templateUrl: './add-by-resource.component.html',
    styleUrls: ['./add-by-resource.component.scss']
})

export class AddAllocationByResourceComponent implements OnInit, OnDestroy {

    getAllSubscrip: Subscription;
    getAllAllocationsSubscrip: Subscription;
    paramsSubscrip: Subscription;
    getByIdSubscrip: Subscription;

    analytics: any = new Array<any>();

    @ViewChild('dateRangePicker') dateRangePicker: DateRangePickerComponent;
    public datePickerOptionRange: string = "next";

    model: AllocationModel;
    resource: any;

    // model: any[] = [
    //     { analytic: "1111-3333", percentages: [
    //         { date: "Ene. 17", value: 0 },
    //         { date: "Feb. 17", value: 0 },
    //         { date: "Mar. 17", value: 0 },
    //         { date: "Abr. 17", value: 0 },
    //         { date: "May. 17", value: 0 },
    //         { date: "Jun. 17", value: 0 },
    //         { date: "Jul. 17", value: 0 },
    //         { date: "Ago. 17", value: 0 },
    //         { date: "Sep. 17", value: 0 },
    //         { date: "Oct. 17", value: 0 },
    //         { date: "Nov. 17", value: 0 },
    //         { date: "Dic. 17", value: 0 }
    //     ]},
    //     { analytic: "2222-4444", percentages: [
    //         { date: "Ene. 17", value: 0 },
    //         { date: "Feb. 17", value: 0 },
    //         { date: "Mar. 17", value: 0 },
    //         { date: "Abr. 17", value: 0 },
    //         { date: "May. 17", value: 0 },
    //         { date: "Jun. 17", value: 0 },
    //         { date: "Jul. 17", value: 0 },
    //         { date: "Ago. 17", value: 0 },
    //         { date: "Sep. 17", value: 0 },
    //         { date: "Oct. 17", value: 0 },
    //         { date: "Nov. 17", value: 0 },
    //         { date: "Dic. 17", value: 0 }
    //     ]},
    // ]

    totals: any[] = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];

    constructor(private analyticService: AnalyticService,
        private router: Router,
        private menuService: MenuService,
        private i18nService: I18nService,
        private allocationsService: AllocationService,
        private messageService: MessageService,
        private activatedRoute: ActivatedRoute,
        private employeeService: EmployeeService,
        private errorHandlerService: ErrorHandlerService){
    }

    ngOnDestroy(): void {
        if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
        if(this.getAllAllocationsSubscrip) this.getAllAllocationsSubscrip.unsubscribe();
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.getByIdSubscrip) this.getByIdSubscrip.unsubscribe();
    }

    ngOnInit(): void {
        var resource = JSON.parse(sessionStorage.getItem("resource"));
        
        if(resource){
            this.resource = resource;
            sessionStorage.removeItem("resource");

            this.getAllocations();
        }
        else{
            this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {

                this.getByIdSubscrip = this.employeeService.getById(params['id']).subscribe(data => {
                    this.resource = data;

                    this.getAllocations();
                },
                error => this.errorHandlerService.handleErrors(error));
            });
        }

        this.getAllSubscrip = this.analyticService.getAll().subscribe(data => {
            this.analytics = data;
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    getAllocations(){
        this.getAllAllocationsSubscrip = this.allocationsService.getAllocations(this.resource.id, new Date(2017, 0, 1).toUTCString(), new Date(2017, 11, 1).toUTCString()).subscribe(data => {
            this.model = data;

            this.model.monthsHeader.forEach((item, index) => {
                this.updateTotal(0, index);
            });
        },
        error => this.errorHandlerService.handleErrors(error));
    }

    updateTotal(value, monthIndex){
        var total = 0;

        this.model.allocations.forEach(function(allocation, index){
            total += allocation.months[monthIndex].percentage;
        })

        this.totals[monthIndex] = total;
    }

    add(){
        var analyticId = $('#analyticId').val();

        var analytic = this.analytics.find(x => x.id == analyticId);

        var months = [];

        this.model.monthsHeader.forEach((item, index) => {
            months.push({ 
                allocationId: 0,
                date: new Date(2017, index, 1),
                percentage: 0
            })
        });

        var row = new Allocation();
        row.analyticId = 0;
        row.analyticTitle = analytic.title;
        row.employeeId = this.resource.id;
        row.months = months;

        this.model.allocations.push(row);
    }
}