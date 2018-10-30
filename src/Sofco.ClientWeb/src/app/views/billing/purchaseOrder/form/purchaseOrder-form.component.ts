import { Component, OnDestroy, Input } from "@angular/core";
import { OnInit } from "@angular/core/src/metadata/lifecycle_hooks";
import { Subscription } from "rxjs";
import { CustomerService } from "../../../../services/billing/customer.service";
import { Option } from "../../../../models/option";
import { AnalyticService } from "../../../../services/allocation-management/analytic.service";
import { UtilsService } from "../../../../services/common/utils.service";
import { PurchaseOrderStatus } from "../../../../models/enums/purchaseOrderStatus";
import { MessageService } from "../../../../services/common/message.service";

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
    public opportunitiesAux: any[] = new Array();
    public areas: any[] = new Array();
    public isReadOnly = false;

    @Input() mode: string;

    getOptionsSubscrip: Subscription;
    getAnalyticSubscrip: Subscription;
    getAreasSubscrip: Subscription;

    constructor(private analyticService: AnalyticService,
                private utilsService: UtilsService,
                private messageService: MessageService,
                private customerService: CustomerService){}

    ngOnInit(): void {
        this.getAreas();

        if(this.mode == 'new'){
            this.getCurrencies();
        }

        // var self = this;
        // $('#analytics').on('change', function(){ 
        //     self.searchOpportunities(); 
        //     $('#opportunity-select').val(null).trigger('change');
        // });
    }

    ngOnDestroy(): void {
        if(this.getOptionsSubscrip) this.getOptionsSubscrip.unsubscribe();
        if(this.getAnalyticSubscrip) this.getAnalyticSubscrip.unsubscribe();
        if(this.getAreasSubscrip) this.getAreasSubscrip.unsubscribe();
    }

    getAreas(){
        this.getAreasSubscrip = this.utilsService.getAreas().subscribe(d => {
            this.areas = d;
        });
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
        });
    }
 
    customerChanged(){
        this.model.analyticIds = [];
        this.model.proposalIds = [];

        this.getAnalytics();
    }

    getAnalytics(){
        // $('#opportunity-select').val(null).trigger('change');

        this.getAnalyticSubscrip = this.analyticService.getActivesByClientId(this.model.clientExternalId).subscribe(
            data => {
                this.analytics = data;
            });
    }

    getCustomers(showLoading: boolean){
        if(showLoading) this.messageService.showLoading();

        this.customerService.getAllOptions().subscribe(res => {
            if(showLoading) this.messageService.closeLoading();         
            this.customers = res.data;
        },
        err => {
            if(showLoading) this.messageService.closeLoading();    
        });
    }

    getOpportunities(analyticid, resolve){
        this.analyticService.getOpportunities(analyticid).subscribe(res => {
            resolve();

            if(res.data && res.data.length > 0){
                res.data.forEach(item => {
                    this.opportunitiesAux.push(item);
                });
            }
        },
        err => {
            resolve();
            this.messageService.closeLoading();
        });
    }

    searchOpportunities(){
    //    var analytics = $('#analytics').val();
    //    var analytics = this.model.analyticIds
       this.opportunities = [];
       this.opportunitiesAux = [];
       
        if(this.model.analyticIds.length > 0) {

            this.messageService.showLoading();

            var promises = new Array();

            this.model.analyticIds.forEach(item => {

                var promise = new Promise((resolve, reject) => {
                    this.getOpportunities(item, resolve);
                });

                promises.push(promise);
            });

            Promise.all(promises).then(data => { 
                this.messageService.closeLoading();
                this.opportunities = this.opportunitiesAux;
                
                // setTimeout(() => {
                //     $('#opportunity-select').val(this.model.proposalIds).trigger('change');
                // }, 300);
             });
       }
    }
}
