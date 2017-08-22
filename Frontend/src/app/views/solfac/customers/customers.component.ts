import { CustomerService } from './../../../services/customer.service';
import { MessageService } from 'app/services/message.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs/Subscription";

@Component({
  selector: 'app-customers',
  templateUrl: './customers.component.html'
})
export class CustomersComponent implements OnInit, OnDestroy {

    getAllSubscrip: Subscription;
    customers: any[];

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private service: CustomerService,
        private messageService: MessageService) { }

    ngOnInit() {
      this.getAll();
    }

    ngOnDestroy(){
      if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
    }

    getAll(){
      this.getAllSubscrip = this.service.getAll(localStorage.getItem("currentUserMail")).subscribe(d => {
        this.customers = d;
      },
      err => {
        console.log(err);
      });
    }

    goToServices(customer){
        this.router.navigate([`/solfac/customers/${customer.Id}/services`, {customerName: customer.Nombre}]);
    }
}
