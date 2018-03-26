import { Component, OnDestroy, Input } from "@angular/core";
import { OnInit } from "@angular/core/src/metadata/lifecycle_hooks";
import { Router } from "@angular/router";
import { MessageService } from "app/services/common/message.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs/Subscription";
import { MenuService } from "app/services/admin/menu.service";
import { PurchaseOrderService } from "app/services/billing/purchaseOrder.service";
import { CustomerService } from "../../../../services/billing/customer.service";
import { Cookie } from "ng2-cookies/ng2-cookies";
import { Option } from "app/models/option";
import { AnalyticService } from "../../../../services/allocation-management/analytic.service";
import { ProjectService } from "app/services/billing/project.service";

@Component({
    selector: 'purchase-order-form',
    templateUrl: './purchaseOrder-form.component.html'
})
export class PurchaseOrderFormComponent implements OnInit, OnDestroy {

    public options: any;
    public model: any = {};
    public customers: Option[] = new Array<Option>();
    public analytics: any[] = new Array();
    public projects: any[] = new Array();

    @Input() mode: string;

    public datePickerOptions;
    getOptionsSubscrip: Subscription;
    getAnalyticSubscrip: Subscription;

    constructor(private purchaseOrderService: PurchaseOrderService,
                private router: Router,
                private analyticService: AnalyticService,
                private projectService: ProjectService,
                private menuService: MenuService,
                private customerService: CustomerService,
                private messageService: MessageService,
                private errorHandlerService: ErrorHandlerService){

        this.datePickerOptions = this.menuService.getDatePickerOptions();
    }

    ngOnInit(): void {
        this.model.receptionDate = new Date();
       
        this.getCustomers();

        this.getOptionsSubscrip = this.purchaseOrderService.getFormOptions().subscribe(
            res => {
                this.options = res;
            },
            err => this.errorHandlerService.handleErrors(err));
    }

    ngOnDestroy(): void {
        if(this.getOptionsSubscrip) this.getOptionsSubscrip.unsubscribe();
        if(this.getAnalyticSubscrip) this.getAnalyticSubscrip.unsubscribe();
    }

    analyticChange(){
        var item = this.analytics.find(x => x.id == this.model.analyticId);
        this.model.commercialManagerId = item.commercialManagerId;
        this.model.managerId = item.managerId;

        this.getProjects(item.serviceId);
    }

    getProjects(serviceId){
        this.projectService.getOptions(serviceId).subscribe(d => {
            this.projects = d.data;
            this.model.projectId = 0;
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    getAnalytics(){
        this.getAnalyticSubscrip = this.analyticService.getClientId(this.model.clientExternalId).subscribe(
            data => {
                this.analytics = data;
            },
            err => this.errorHandlerService.handleErrors(err));
    }

    getCustomers(){
        this.customerService.getOptions().subscribe(res => {
            this.customers = res.data;
        },
        err => {
            this.errorHandlerService.handleErrors(err)
        });
    }
} 