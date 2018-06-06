import { Component, OnDestroy, Input, ViewChild } from "@angular/core";
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
import { UtilsService } from "../../../../services/common/utils.service";

@Component({
    selector: 'purchase-order-form',
    templateUrl: './purchaseOrder-form.component.html'
})
export class PurchaseOrderFormComponent implements OnInit, OnDestroy {

    public options: any;
    public model: any = { ammountDetails: new Array() };
    public customers: Option[] = new Array<Option>();
    public analytics: any[] = new Array();
    public projects: any[] = new Array();
    public opportunities: any[] = new Array();
    public currencies: any[] = new Array();

    @Input() mode: string;

    getOptionsSubscrip: Subscription;
    getAnalyticSubscrip: Subscription;
    getCurrenciesSubscrip: Subscription;

    constructor(private purchaseOrderService: PurchaseOrderService,
                private router: Router,
                private analyticService: AnalyticService,
                private utilsService: UtilsService,
                private menuService: MenuService,
                private customerService: CustomerService,
                private messageService: MessageService,
                private errorHandlerService: ErrorHandlerService){}

    ngOnInit(): void {
        this.getCustomers();
        this.getCurrencies();
    }

    ngOnDestroy(): void {
        if(this.getOptionsSubscrip) this.getOptionsSubscrip.unsubscribe();
        if(this.getAnalyticSubscrip) this.getAnalyticSubscrip.unsubscribe();
        if(this.getCurrenciesSubscrip) this.getCurrenciesSubscrip.unsubscribe();
    }

    getCurrencies(){
        this.getCurrenciesSubscrip = this.utilsService.getCurrencies().subscribe(d => {
            this.currencies = d;

            this.currencies.forEach((item, index) => {
                this.model.ammountDetails.push({ currencyId: item.id, text: item.text, ammount: 0, balance: 0 });
            });
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