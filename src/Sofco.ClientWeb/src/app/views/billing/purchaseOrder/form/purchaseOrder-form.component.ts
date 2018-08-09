import { Component, OnDestroy, Input } from "@angular/core";
import { OnInit } from "@angular/core/src/metadata/lifecycle_hooks";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs";
import { CustomerService } from "app/services/billing/customer.service";
import { Option } from "app/models/option";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { UtilsService } from "app/services/common/utils.service";
import { PurchaseOrderStatus } from "app/models/enums/purchaseOrderStatus";
import { MessageService } from "app/services/common/message.service";

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
    public currencyDisabled = false;
    public isReadOnly = false;

    @Input() mode: string;

    getOptionsSubscrip: Subscription;
    getAnalyticSubscrip: Subscription;
    getAreasSubscrip: Subscription;

    constructor(private analyticService: AnalyticService,
                private utilsService: UtilsService,
                private messageService: MessageService,
                private customerService: CustomerService,
                private errorHandlerService: ErrorHandlerService){}

    ngOnInit(): void {
        this.getAreas();

        if(this.mode == 'new'){
            this.getCurrencies();
        }

        var self = this;
        $('#analytics').on('change', function(){ 
            self.searchOpportunities();
        });
    }

    ngOnDestroy(): void {
        if(this.getOptionsSubscrip) this.getOptionsSubscrip.unsubscribe();
        if(this.getAnalyticSubscrip) this.getAnalyticSubscrip.unsubscribe();
        if(this.getAreasSubscrip) this.getAreasSubscrip.unsubscribe();
    }

    getAreas(){
        this.getAreasSubscrip = this.utilsService.getAreas().subscribe(d => {
            this.areas = d;
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    getCurrencies(){
        this.getAreasSubscrip = this.utilsService.getCurrencies().subscribe(currencies => {

            if(this.mode == 'new'){
                currencies.forEach((item) => {
                    this.model.ammountDetails.push({ currencyId: item.id, currencyDescription: item.text, ammount: 0, balance: 0, enable: false });
                });
            }
            else{
                if(this.model.status == PurchaseOrderStatus.Draft || this.model.status == PurchaseOrderStatus.Reject){
                    currencies.forEach((item) => {
                        var exist = this.model.ammountDetails.filter(modelItem => {
                            return modelItem.currencyId == item.id;
                        });
    
                        if(!exist || (exist && exist.length == 0)){
                            this.model.ammountDetails.push({ currencyId: item.id, currencyDescription: item.text, ammount: 0, balance: 0, enable: false });
                        }
                    });
                }
            }
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

    getCustomers(showLoading: boolean){
        if(showLoading) this.messageService.showLoading();

        this.customerService.getAllOptions().subscribe(res => {
            if(showLoading) this.messageService.closeLoading();         
            this.customers = res.data;
        },
        err => {
            if(showLoading) this.messageService.closeLoading();    
            this.errorHandlerService.handleErrors(err)
        });
    }

    getOpportunities(analyticid, resolve){
        this.analyticService.getOpportunities(analyticid).subscribe(res => {
            resolve();

            if(res.data && res.data.length > 0){
                res.data.forEach(item => {
                    this.opportunities.push(item);
                });
            }
        },
        err => {
            resolve();
            this.messageService.closeLoading();
        });
    }

    searchOpportunities(){
       var analytics = $('#analytics').val();
       this.opportunities = [];

        if(analytics.length > 0) {

            this.messageService.showLoading();

            var promises = new Array();

            analytics.forEach(item => {

                var promise = new Promise((resolve, reject) => {
                    this.getOpportunities(item, resolve);
                });

                promises.push(promise);
            });

            Promise.all(promises).then(data => { 
                this.messageService.closeLoading();

                setTimeout(() => {
                    $('#opportunity-select select').val(this.model.proposal).trigger('change');
                }, 300);
             });
       }
    }
}
