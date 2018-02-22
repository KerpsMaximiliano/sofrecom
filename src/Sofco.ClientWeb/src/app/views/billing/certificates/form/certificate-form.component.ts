import { Component, OnDestroy, Input } from "@angular/core";
import { OnInit } from "@angular/core/src/metadata/lifecycle_hooks";
import { Router } from "@angular/router";
import { MessageService } from "app/services/common/message.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs/Subscription";
import { MenuService } from "app/services/admin/menu.service";
import { CustomerService } from "app/services/billing/customer.service";
import { Cookie } from "ng2-cookies/ng2-cookies";
import { Option } from "app/models/option";

@Component({
    selector: 'certificate-form',
    templateUrl: './certificate-form.component.html'
})
export class CertificateFormComponent implements OnInit, OnDestroy {

    public model: any = {};
    public customers: Option[] = new Array<Option>();

    @Input() mode: string;

    getOptionsSubscrip: Subscription;

    constructor(private router: Router,
                private menuService: MenuService,
                private customerService: CustomerService,
                private messageService: MessageService,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void {
        this.getCustomers();
    }

    ngOnDestroy(): void {
        if(this.getOptionsSubscrip) this.getOptionsSubscrip.unsubscribe();
    }

    getCustomers(){
        this.getOptionsSubscrip = this.customerService.getOptions(Cookie.get("currentUserMail")).subscribe(data => {
            this.customers = data;
        },
        err => {
            this.errorHandlerService.handleErrors(err)
        });
    }
}