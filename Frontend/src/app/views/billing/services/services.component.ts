import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { ServiceService } from "app/services/billing/service.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";

@Component({
  selector: 'app-services',
  templateUrl: './services.component.html'
})
export class ServicesComponent implements OnInit, OnDestroy {
    getAllSubscrip: Subscription;
    paramsSubscrip: Subscription;
    services: any[];
    public customerId: any;
    public customerName: string;
    public loading:  boolean = true;

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private service: ServiceService,
        private errorHandlerService: ErrorHandlerService) { }

    ngOnInit() {
      this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
        this.customerId = params['customerId'];
        var customer = JSON.parse(sessionStorage.getItem("customer"));
        this.customerName = customer.nombre;
        this.getAll(this.customerId);
      });
      
    }

    ngOnDestroy(){
      if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
      if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
    }

    getAll(customerId){
      this.loading = true;

      this.getAllSubscrip = this.service.getAll(customerId).subscribe(d => {
        this.loading = false;
        this.services = d;
      },
      err => {
        this.loading = false;
        this.errorHandlerService.handleErrors(err)
      });
    }

    goToProjects(service){
      sessionStorage.setItem("serviceName", service.nombre);
      this.router.navigate([`/billing/customers/${this.customerId}/services/${service.id}/projects`]);
    }
}
