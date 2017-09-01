import { Router} from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { CustomerService } from "app/services/billing/customer.service";
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { ErrorHandlerService } from "app/services/common/errorHandler.service";

@Component({
  selector: 'app-customers',
  templateUrl: './customers.component.html'
})
export class CustomersComponent implements OnInit, OnDestroy {

    getAllSubscrip: Subscription;
    customers: any[];
    public loading:  boolean = true;

    constructor(
        private router: Router,
        private service: CustomerService,
        private errorHandlerService: ErrorHandlerService) { }

    ngOnInit() {
      this.getAll();
    }

    ngOnDestroy(){
      if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
    }

    getAll(){
      this.getAllSubscrip = this.service.getAll(Cookie.get("currentUserMail")).subscribe(d => {
        this.loading = false;
        this.customers = d;
      },
      err => {
        this.loading = false;
        this.errorHandlerService.handleErrors(err)
      });
    }

    goToServices(customer){
      sessionStorage.setItem("customer", JSON.stringify(customer));
      this.router.navigate([`/billing/customers/${customer.id}/services`]);
    }
}
