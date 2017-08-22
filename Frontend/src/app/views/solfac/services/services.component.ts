import { MessageService } from 'app/services/message.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { ServiceService } from "app/services/service.service";

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

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private service: ServiceService,
        private messageService: MessageService) { }

    ngOnInit() {
      this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
        this.customerId = params['customerId'];
        this.customerName = this.activatedRoute.snapshot.params['customerName']
        this.getAll(this.customerId);
      });
      
    }

    ngOnDestroy(){
      if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
      if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
    }

    getAll(customerId){
      this.getAllSubscrip = this.service.getAll(customerId).subscribe(d => {
        this.services = d;
      },
      err => {
        console.log(err);
      });
    }

    goToProjects(service){
      this.router.navigate([`/solfac/customers/${this.customerId}/services/${service.Id}/projects`, {serviceName: service.Nombre}]);
    }
}
