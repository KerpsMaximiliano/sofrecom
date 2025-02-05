import { Router} from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs";
import { CustomerService } from "../../../services/billing/customer.service";
import { DataTableService } from "../../../services/common/datatable.service";
import { MessageService } from '../../../services/common/message.service';
import { MenuService } from 'app/services/admin/menu.service';

@Component({
  selector: 'app-customers',
  templateUrl: './customers.component.html'
})
export class CustomersComponent implements OnInit, OnDestroy {

    getAllSubscrip: Subscription;
    updateSubscrip: Subscription;
    customers: any[];
    public loading = true;

    constructor(
        private router: Router,
        private service: CustomerService,
        public menuService: MenuService,
        private messageService: MessageService,
        private datatableService: DataTableService) { }

    ngOnInit() {
      this.getAll();
    }

    ngOnDestroy(){
      if (this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
      if (this.updateSubscrip) this.updateSubscrip.unsubscribe();
    }

    getAll(){
      this.messageService.showLoading();

      this.getAllSubscrip = this.service.getAll().subscribe(d => {
        this.customers = d.data;

        var options = { selector: "#customerTable" }
        this.datatableService.destroy(options.selector);
        this.datatableService.initialize(options);

        this.messageService.closeLoading();
      },
      err => this.messageService.closeLoading());
    }

    goToServices(customer){
      sessionStorage.setItem("customer", JSON.stringify(customer));
      sessionStorage.setItem("customerId", customer.crmId);
      sessionStorage.setItem("customerName", customer.name);
      
      this.router.navigate([`/billing/customers/${customer.crmId}/services`]);
    }

    update(){
      this.messageService.showLoading();

      this.updateSubscrip = this.service.update().subscribe(data => {
        this.messageService.closeLoading();
        this.getAll();
      });
    }
}
