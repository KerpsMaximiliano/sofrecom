import { Component, OnDestroy, Input } from "@angular/core";
import { OnInit } from "@angular/core/src/metadata/lifecycle_hooks";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs/Subscription";
import { CustomerService } from "../../../../services/billing/customer.service";
import { Option } from "app/models/option";
import { AnalyticService } from "../../../../services/allocation-management/analytic.service";
import { UtilsService } from "../../../../services/common/utils.service";

@Component({
    selector: 'purchase-order-form',
    templateUrl: './purchaseOrder-form.component.html',
    styleUrls: ['./purchaseOrder-form.component.scss']
})
export class PurchaseOrderFormComponent implements OnInit, OnDestroy {

    public model: any = { ammountDetails: new Array() };
    public customers: Option[] = new Array<Option>();
    public analytics: any[] = new Array();
    public projects: any[] = new Array();
    public opportunities: any[] = new Array();
    public areas: any[] = new Array();

    @Input() mode: string;

    getOptionsSubscrip: Subscription;
    getAnalyticSubscrip: Subscription;
    getCurrenciesSubscrip: Subscription;

    constructor(private analyticService: AnalyticService,
                private utilsService: UtilsService,
                private customerService: CustomerService,
                private errorHandlerService: ErrorHandlerService){}

    ngOnInit(): void {
        this.getCustomers();
        this.getAreas();

        if(this.mode == 'new'){
            this.getCurrencies();
        }
    }

    ngOnDestroy(): void {
        if(this.getOptionsSubscrip) this.getOptionsSubscrip.unsubscribe();
        if(this.getAnalyticSubscrip) this.getAnalyticSubscrip.unsubscribe();
        if(this.getCurrenciesSubscrip) this.getCurrenciesSubscrip.unsubscribe();
    }

    getAreas(){
        this.getCurrenciesSubscrip = this.utilsService.getAreas().subscribe(d => {
            this.areas = d;
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    getCurrencies(){
        this.getCurrenciesSubscrip = this.utilsService.getCurrencies().subscribe(currencies => {
            currencies.forEach((item) => {
                this.model.ammountDetails.push({ currencyId: item.id, currencyDescription: item.text, ammount: 0, balance: 0, enable: false });
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
        this.customerService.getAllOptions().subscribe(res => {
            this.customers = res.data;
        },
        err => {
            this.errorHandlerService.handleErrors(err)
        });
    }
} 