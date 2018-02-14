import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { ServiceService } from "app/services/billing/service.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { DataTableService } from "app/services/common/datatable.service";

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
        private datatableService: DataTableService,
        private errorHandlerService: ErrorHandlerService) { }

    ngOnInit() {
      this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
        this.customerId = params['customerId'];
        this.customerName = sessionStorage.getItem("customerName");
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

        this.initGrid();
      },
      err => {
        this.loading = false;
        this.datatableService.init('#serviceTable', true);
        this.errorHandlerService.handleErrors(err)
      });
    }

    initGrid(){
      var params = {
        selector: '#serviceTable',
        columnDefs: [ {"aTargets": [1, 2], "sType": "date-uk"} ]
      }

      this.datatableService.init2(params);
    }

    goToProjects(service){
      sessionStorage.setItem("serviceName", service.nombre);
      sessionStorage.setItem("serviceId", service.id);
      sessionStorage.setItem("serviceDetail", JSON.stringify(service));
      
      this.router.navigate([`/billing/customers/${this.customerId}/services/${service.id}/projects`]);
    }
}
