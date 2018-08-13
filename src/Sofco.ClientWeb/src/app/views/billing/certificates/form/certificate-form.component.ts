import { Component, OnDestroy, Input } from "@angular/core";
import { OnInit } from "@angular/core/src/metadata/lifecycle_hooks";
import { Subscription } from "rxjs";
import { CustomerService } from "../../../../services/billing/customer.service";
import { Option } from "../../../../models/option";

@Component({
    selector: 'certificate-form',
    templateUrl: './certificate-form.component.html'
})
export class CertificateFormComponent implements OnInit, OnDestroy {

    public model: any = {};
    public customers: Option[] = new Array<Option>();

    @Input() mode: string;

    getOptionsSubscrip: Subscription;

    constructor(private customerService: CustomerService){
    }

    ngOnInit(): void {
        this.getCustomers();
    }

    ngOnDestroy(): void {
        if (this.getOptionsSubscrip) this.getOptionsSubscrip.unsubscribe();
    }

    getCustomers() {
        this.getOptionsSubscrip = this.customerService.getOptions().subscribe(res => {
            this.customers = res.data;
        });
    }
}