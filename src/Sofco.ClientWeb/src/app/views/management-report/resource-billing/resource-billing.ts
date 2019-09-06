import { Component, OnInit, OnDestroy, ViewChild, Input } from "@angular/core";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { Subscription } from "rxjs";
import { GenericOptionService } from "app/services/admin/generic-option.service";
import { ServiceService } from "app/services/billing/service.service";
import { ResourceBillingItem } from "app/models/management-report/resourceBillingItem";
import { ManagementReportService } from "app/services/management-report/management-report.service";

@Component({
    selector: 'resource-billing-modal',
    templateUrl: './resource-billing.html'
})
export class ResourceBillingModalComponent implements OnInit, OnDestroy {
   
    @ViewChild('resourceBillingModal') resourceBillingModal;
    public resourceBillingModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Cantidad recursos facturados",
        "resourceBillingModal",
        true,
        true,
        "ACTIONS.save",
        "ACTIONS.cancel"
    );

    getProfilesSubscrip: Subscription;
    getSenioritiesSubscrip: Subscription;
    getPurchaseOrdersSubscrip: Subscription;
    getSubscrip: Subscription;

    profiles: any[] = new Array();
    seniorities: any[] = new Array();
    purchaseOrders: any[] = new Array();
    types: any[] = new Array();
    items: ResourceBillingItem[] = new Array();

    monthDisplay: string;
    resourceQuantity: number;
    total: number = 0;
    billingMonthId: number;

    isLoading: boolean = false;

    month: any;
    months: any;

    constructor(private genericOptionsService: GenericOptionService,
        private managementReportBillingService: ManagementReportService,
        private serviceService: ServiceService){
    }

    ngOnInit(): void {
        this.getProfiles();
        this.getSeniorities();

        this.types.push({ id: 1, text: "Mes" });
        this.types.push({ id: 2, text: "Horas" });
    }

    ngOnDestroy(): void {
        if (this.getProfilesSubscrip) this.getProfilesSubscrip.unsubscribe();
        if (this.getSenioritiesSubscrip) this.getSenioritiesSubscrip.unsubscribe();
        if (this.getPurchaseOrdersSubscrip) this.getPurchaseOrdersSubscrip.unsubscribe();
        if (this.getSubscrip) this.getSubscrip.unsubscribe();
    }

    getData(billingMonthId){
        this.isLoading = true;

        this.getSubscrip = this.managementReportBillingService.getResources(billingMonthId).subscribe(response => {
            this.isLoading = false;

            if(response.data && response.data.length > 0){
                response.data.forEach(item => {
                    this.items.push(new ResourceBillingItem(item));
                });

                this.calculateTotal();
            }
        },
        () => this.isLoading = false);
    }

    getProfiles(){
        this.genericOptionsService.controller = "profile";
        this.getProfilesSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
            this.profiles = response.data;
        },
        () => {});
    }

    getPurchaseOrders(serviceId){
        this.getPurchaseOrdersSubscrip = this.serviceService.getPurchaseOrders(serviceId).subscribe(response => {
            this.purchaseOrders = response.map(item => {
                return {
                    id: item.id,
                    text: item.number
                }
            });
        },
        () => {});
    }

    getSeniorities(){
        this.genericOptionsService.controller = "seniority";
        this.getProfilesSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
            this.seniorities = response.data;
        },
        () => {});
    }

    open(month, months){
        this.items = [];
        this.month = month;
        this.months = months;
        
        this.monthDisplay = month.display;
        this.resourceQuantity = month.resourceQuantity;
        this.billingMonthId = month.billingMonthId;

        this.getData(this.billingMonthId);

        this.resourceBillingModal.show();
    }

    addItem(){
        this.items.push(new ResourceBillingItem(null));

        this.resourceQuantity = this.items.filter(x => !x.deleted).length;
    }

    monthHourChange(item){
        if(item.monthHour == 1){
            item.quantity = 1;
        }

        this.calculateTotal();
    }

    removeItem(item, index){
        if(item.id > 0){
            item.deleted = true;
        }
        else{
            this.items.splice(index, 1);
        }

        this.resourceQuantity = this.items.filter(x => !x.deleted).length;
        this.calculateTotal();
    }

    save(){
        this.getProfilesSubscrip = this.managementReportBillingService.saveResources(this.billingMonthId, this.items).subscribe(response => {
            this.resourceBillingModal.hide();
            this.month.resourceQuantity = this.items.filter(x => !x.deleted).length;

            if(response.data && response.data.length > 0){
                response.data.forEach(item => {
                    var month = this.months.find(x => x.billingMonthId == item.id);

                    if(month){
                        month.resourceQuantity = item.quantity;
                    }
                });
            }
        },
        () => this.resourceBillingModal.resetButtons());
    }

    calculateTotal(){
        this.total = 0;

        this.items.forEach(item => {
            item.subTotal = item.amount * item.quantity;
            
            if(!isNaN(item.subTotal)){
                if(!item.deleted){
                    this.total += item.subTotal;
                }
            }
            else{
                item.subTotal = 0;
            }
        });
    }
}