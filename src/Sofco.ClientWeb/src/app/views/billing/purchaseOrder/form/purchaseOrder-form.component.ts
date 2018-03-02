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

@Component({
    selector: 'purchase-order-form',
    templateUrl: './purchaseOrder-form.component.html'
})
export class PurchaseOrderFormComponent implements OnInit, OnDestroy {

    public options: any;
    public model: any = {};
    public customers: Option[] = new Array<Option>();

    @Input() mode: string;

    public datePickerOptions;
    getOptionsSubscrip: Subscription;

    constructor(private purchaseOrderService: PurchaseOrderService,
                private router: Router,
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
            data => {
                this.options = data;
            },
            err => this.errorHandlerService.handleErrors(err));
    }

    ngOnDestroy(): void {
        if(this.getOptionsSubscrip) this.getOptionsSubscrip.unsubscribe();
    }

    getCustomers(){
        this.customerService.getOptions().subscribe(data => {
            this.customers = data;
        },
        err => {
            this.errorHandlerService.handleErrors(err)
        });
    }
}