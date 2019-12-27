import { UserService } from "app/services/admin/user.service";
import { Component, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { I18nService } from "app/services/common/i18n.service";
import { DataTableService } from "app/services/common/datatable.service";
import { DelegationType } from "app/models/enums/delegationType";
import { DelegationService } from "app/services/admin/delegation.service";
import { MenuService } from "app/services/admin/menu.service";

@Component({
    selector: 'delegation',
    templateUrl: './delegation.html'
})
export class DelegationComponent implements OnInit, OnDestroy {

    typeId: number;
    grantedUserId: number;
    analyticSourceId: number;
    userSourceId: number;
    sourceType = "2";

    sourceTypeDisabled: boolean;
    analyticDisabled: boolean;
    userSourceDisabled: boolean;

    types: any[] = new Array();
    users: any[] = new Array();
    resources: any[] = new Array();
    analytics: any[] = new Array();
    data: any[] = new Array();

    addSubscript: Subscription;
    usersSubscript: Subscription;
    analyticsSubscript: Subscription;
    deleteSubscript: Subscription;
    getSubscript: Subscription;
 
    constructor(private messageService: MessageService,
                private i18nService: I18nService,
                private menuService: MenuService,
                private datatableService: DataTableService,
                private userDelegateService: DelegationService,
                private userService: UserService) { }

    ngOnInit(): void {
        this.usersSubscript = this.userService.getOptions().subscribe(response => {
            this.users = response;
        });

        this.analyticsSubscript = this.userDelegateService.getAnalytics().subscribe(response => {
            this.analytics = response.data;
        });

        if(this.menuService.hasFunctionality("ALLOC", "MAN-REPORT-DELEGATE")){
            this.types.push({ id: DelegationType.ManagementReport, text: this.i18nService.translateByKey(DelegationType[DelegationType.ManagementReport]) });
        }

        if(this.menuService.hasFunctionality("ALLOC", "ADVANCEMENT-DELEGATE")){
            this.types.push({ id: DelegationType.Advancement, text: "Adelantos" });
        }

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
        if(!this.grantedUserId || this.grantedUserId <= 0 ||  !this.typeId || this.typeId <= 0){
            return false;
        }

        if(this.typeId == DelegationType.ManagementReport){
            if(!this.analyticSourceId || this.analyticSourceId <= 0) return false;

            if(this.sourceType == "3" && (!this.userSourceId || this.userSourceId <= 0))return false;
        }

        return true;
    }

    save(){
        if(!this.saveEnabled()) return;

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

    typeChanged(){
        if(this.typeId == DelegationType.ManagementReport){
            this.sourceTypeDisabled = false;
            this.analyticDisabled = false;
            this.sourceType = "2";
        }

        if(this.typeId == DelegationType.Advancement){
            this.sourceTypeDisabled = true;
            this.analyticDisabled = true;
            this.userSourceDisabled = false;
            this.sourceType = "3";
            this.analyticSourceId = null;

            this.resources = [];
            this.analytics.forEach(x => {
                x.resources.forEach(user => {
                    if(this.resources.indexOf(u => u.id == user.userId) == -1){
                        this.resources.push({ id: user.userId, text: user.text });
                    }
                });
            })
        }
    }
}