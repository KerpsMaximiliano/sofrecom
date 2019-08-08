import { UserService } from "app/services/admin/user.service";
import { Component, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { I18nService } from "app/services/common/i18n.service";
import { DataTableService } from "app/services/common/datatable.service";
import { DelegationType } from "app/models/enums/delegationType";
import { DelegationService } from "app/services/admin/delegation.service";
import { AnalyticService } from "app/services/allocation-management/analytic.service";

@Component({
    selector: 'delegation',
    templateUrl: './delegation.html'
})
export class DelegationComponent implements OnInit, OnDestroy {

    public typeId: number;
    public grantedUserId: number;
    public analyticSourceId: number;
    public userSourceId: number;
    public sourceType = "2";

    public types: any[] = new Array();
    public users: any[] = new Array();
    public resources: any[] = new Array();
    public analytics: any[] = new Array();
    public data: any[] = new Array();

    public addSubscript: Subscription;
    public usersSubscript: Subscription;
    public analyticsSubscript: Subscription;
    public deleteSubscript: Subscription;
    public getSubscript: Subscription;
 
    constructor(private messageService: MessageService,
                private i18nService: I18nService,
                private datatableService: DataTableService,
                private userDelegateService: DelegationService,
                private analyticService: AnalyticService,
                private userService: UserService) { }

    ngOnInit(): void {
        this.usersSubscript = this.userService.getOptions().subscribe(response => {
            this.users = response;
        });

        this.analyticsSubscript = this.userDelegateService.getAnalytics().subscribe(response => {
            this.analytics = response.data;
        });

        this.types.push({ id: DelegationType.ManagementReport, text: this.i18nService.translateByKey(DelegationType[DelegationType.ManagementReport]) });

        this.get();
    }

    ngOnDestroy(): void {
        if(this.addSubscript) this.addSubscript.unsubscribe();          
        if(this.usersSubscript) this.usersSubscript.unsubscribe();          
        if(this.analyticsSubscript) this.analyticsSubscript.unsubscribe();          
        if(this.deleteSubscript) this.deleteSubscript.unsubscribe();          
        if(this.getSubscript) this.getSubscript.unsubscribe();          
    }

    get(){
        this.messageService.showLoading();

        this.getSubscript = this.userDelegateService.get().subscribe(response => {
            this.messageService.closeLoading();
            this.data = response.data;
        },
        error => this.messageService.closeLoading());
    }

    setEmployees() {
        this.userSourceId = null;
        var analytic = this.analytics.find(x => x.id == this.analyticSourceId);

        if(analytic != null){
            this.resources = analytic.resources.map(user => {
                return { id: user.userId, text: user.text };
            });
        }
        else{
            this.resources = [];
        }
    }

    initGrid(){
        var columns = [0, 1, 2, 3];
        var title = `usuarios delegados`;

        var params = {
            selector: '#dataTable',
            columns: columns,
            title: title,
            withExport: true,
            columnDefs: [ {'aTargets': [3], "sType": "date-uk"} ]
          }

          this.datatableService.destroy(params.selector);
          this.datatableService.initialize(params);
    }

    saveEnabled(){
        if(!this.grantedUserId || this.grantedUserId <= 0 || 
           !this.typeId || this.typeId <= 0 || 
           !this.analyticSourceId || this.analyticSourceId <= 0){
               return false;
           }

        if(this.sourceType == "3" && (!this.userSourceId || this.userSourceId <= 0)){
            return false;
        }

        return true;
    }

    save(){
        if(!this.saveEnabled()) return;

        // if(this.sourceType == "1"){
        //     this.sourceId = null;
        // }

        if(this.sourceType == "2"){
            this.userSourceId = null;
        }

        var model = {
            grantedUserId: this.grantedUserId,
            type: this.typeId,
            sourceType: this.sourceType,
            analyticSourceId: this.analyticSourceId,
            userSourceId: this.userSourceId,
        }

        this.messageService.showLoading();

        this.addSubscript = this.userDelegateService.post(model).subscribe(response => {
            this.messageService.closeLoading();
            
            this.get();
        },
        error => this.messageService.closeLoading());
    }

    delete(userDelegate){
        this.messageService.showConfirm(() => {
            this.messageService.showLoading();

            this.deleteSubscript = this.userDelegateService.delete(userDelegate.id).subscribe(response => {
                this.messageService.closeLoading();

                var index = this.data.indexOf(x => x.id == userDelegate.id);
                this.data.splice(index, 1);
            },
            error => this.messageService.closeLoading());
        });
    }
}