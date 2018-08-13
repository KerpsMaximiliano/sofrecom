import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { EmployeeService } from "../../../../services/allocation-management/employee.service";
import { MenuService } from "../../../../services/admin/menu.service";
import { ActivatedRoute } from "@angular/router";
import { AnalyticService } from "../../../../services/allocation-management/analytic.service";
import { AppSetting } from '../../../../services/common/app-setting'

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

    pmoUser: boolean;
  
    constructor(private analyticService: AnalyticService,
        private menuService: MenuService,
        private activatedRoute: ActivatedRoute,
        private employeeService: EmployeeService,
        private appSetting: AppSetting){}

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
                });
            });
        }

        this.getAllSubscrip = this.analyticService.getOptions().subscribe(data => {
            this.analytics = data;
        });
    }

    add(){
        var analyticId = $('#analyticId').val();

        if(analyticId == 0) return

        var analytic = this.analytics.find(x => x.id == analyticId);

        this.allocations.add(analytic);
    }
 
    search(){
        if(this.pmoUser){
            this.allocations.getAllocations(this.resourceId, this.dateSince, true);
        }
        else{
            this.allocations.getAllocations(this.resourceId, new Date(), true);
        }
    }
}