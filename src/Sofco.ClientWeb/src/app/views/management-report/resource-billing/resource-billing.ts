import { Component, OnInit, OnDestroy, ViewChild, Input } from "@angular/core";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { Subscription } from "rxjs";
import { GenericOptionService } from "app/services/admin/generic-option.service";
import { ServiceService } from "app/services/billing/service.service";
import { ResourceBillingItem } from "app/models/management-report/resourceBillingItem";

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

    profiles: any[] = new Array();
    seniorities: any[] = new Array();
    purchaseOrders: any[] = new Array();
    types: any[] = new Array();
    items: ResourceBillingItem[] = new Array();

    month: string;
    resourceQuantity: number;
    total: number;

    constructor(private genericOptionsService: GenericOptionService,
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

    open(month){
        this.month = month.display;
        this.resourceQuantity = month.resourceQuantity;
        this.total = 0;

        this.resourceBillingModal.show();
    }

    addItem(){
        this.items.push(new ResourceBillingItem(null));
    }

    monthHourChange(item){
        if(item.monthHour == 1){
            item.quantity = 1;
        }
    }

    removeItem(item){
        
    }
}