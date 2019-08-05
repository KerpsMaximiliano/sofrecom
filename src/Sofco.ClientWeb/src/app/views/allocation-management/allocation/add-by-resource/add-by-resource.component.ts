import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { EmployeeService } from "../../../../services/allocation-management/employee.service";
import { MenuService } from "../../../../services/admin/menu.service";
import { ActivatedRoute } from "@angular/router";
import { AnalyticService } from "../../../../services/allocation-management/analytic.service";
import { AppSetting } from '../../../../services/common/app-setting'
import { MessageService } from "app/services/common/message.service";

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
    commentsSubscrip: Subscription;

    analytics: any = new Array<any>();

    resource: any;
    resourceId: number;
    comments: string;
    commentsVisible: boolean = false;

    public monthQuantity: number = 12;

    @ViewChild('allocations') allocations: any;

    dateSince: Date = new Date();

    pmoUser: boolean;
  
    constructor(private analyticService: AnalyticService,
        private menuService: MenuService,
        private messageService: MessageService,
        private activatedRoute: ActivatedRoute,
        private employeeService: EmployeeService,
        private appSetting: AppSetting){}

    ngOnDestroy(): void {
        if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.getByIdSubscrip) this.getByIdSubscrip.unsubscribe();
        if(this.commentsSubscrip) this.commentsSubscrip.unsubscribe();
    }

    ngOnInit(): void {
        this.monthQuantity = this.appSetting.AllocationManagement_Months;
        this.pmoUser = this.menuService.hasFunctionality('CONTR', 'QARDD');
        var resource = JSON.parse(sessionStorage.getItem("resource"));
        
        if(resource){
            this.resource = resource;
            sessionStorage.removeItem("resource");
            this.resourceId = this.resource.id;

            this.comments = resource.assignComments;
            if(this.resource.assignComments) this.commentsVisible = true;
        }
        else{
            this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
                this.resourceId = params['id'];

                this.getByIdSubscrip = this.employeeService.getById(params['id']).subscribe(data => {
                    this.resource = data.data;
                    this.comments = data.data.assignComments;

                    if(this.resource.assignComments) this.commentsVisible = true;
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

    addComment(){
        this.commentsVisible = true;   
    }

    deleteComment(){
        var json = {
            employeeId: this.resourceId,
            comment: ""
        };

        this.saveComments(json, () => { 
            this.commentsVisible = false;
            this.comments = null; 
        });
    }

    editComments(){
        var json = {
            employeeId: this.resourceId,
            comment: this.comments
        };

        this.saveComments(json, null);
    }

    saveComments(json, callback){
        this.messageService.showLoading();

        this.getAllSubscrip = this.employeeService.updateComments(json).subscribe(data => {
            this.messageService.closeLoading();
            this.resource.assignComments = json.comments;

            if(callback){
                callback();
            }
        },
        error => this.messageService.closeLoading());
    }
}