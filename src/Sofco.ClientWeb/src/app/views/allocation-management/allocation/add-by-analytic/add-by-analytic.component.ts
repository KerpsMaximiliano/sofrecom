import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { ActivatedRoute } from "@angular/router";
import { AnalyticService } from "../../../../services/allocation-management/analytic.service";
import { MenuService } from "../../../../services/admin/menu.service";
import { AllocationSearch } from "../../../../models/allocation-management/allocationSearch";
import { EmployeeService } from "../../../../services/allocation-management/employee.service";
import { AppSetting } from '../../../../services/common/app-setting'

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

    public resourceId: any; 

    dateSince: Date = new Date();

    pmoUser: boolean;

    constructor(private analyticService: AnalyticService,
                private menuService: MenuService,
                private employeeService: EmployeeService,
                private activatedRoute: ActivatedRoute,
                private appSetting: AppSetting){}

    ngOnInit(): void {
        this.monthQuantity = this.appSetting.AllocationManagement_Months;
        this.pmoUser = this.menuService.hasFunctionality('CONTR', 'QARDD');

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
                });
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
        });
    }

    getTimeline(analyticId){
        this.resourceTimeline.model = new Array<any>();
        this.resourceTimeline.getAllocations(analyticId, this.dateSince, this.monthQuantity);
    }

    search(){
        if(this.resourceId == undefined || this.resourceId == '0') return;

        this.resourceTimeline.getAllocations(this.analytic.id, this.dateSince, this.monthQuantity);

        if(this.pmoUser){
            this.allocations.getAllocations(this.resourceId, this.dateSince, true);
        }
        else{
            this.allocations.getAllocations(this.resourceId, new Date(), true);
        }
    }
}