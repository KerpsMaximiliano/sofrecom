import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs";
import { ServiceService } from "../../../services/billing/service.service";
import { DataTableService } from "../../../services/common/datatable.service";
import { MessageService } from '../../../services/common/message.service';

@Component({
  selector: 'app-services',
  templateUrl: './services.component.html'
})
export class ServicesComponent implements OnInit, OnDestroy {
    getAllSubscrip: Subscription;
    paramsSubscrip: Subscription;
    updateSubscrip: Subscription;
    services: any[];
    public customerId: any;
    public customerName: string;
    public loading = true;

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private service: ServiceService,
        private messageService: MessageService,
        private datatableService: DataTableService) { }

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
      if(this.updateSubscrip) this.updateSubscrip.unsubscribe();
    }

    getAll(customerId){
      this.messageService.showLoading();

      this.getAllSubscrip = this.service.getAll(customerId).subscribe(d => {
        this.services = d.data;

        this.initGrid();

        this.messageService.closeLoading();
      },
      err => {
        this.messageService.closeLoading();

        var options = { selector: "#serviceTable" }
        this.datatableService.initialize(options);
      });
    }

    initGrid(){
      var params = {
        selector: '#serviceTable',
        columnDefs: [ {"aTargets": [3, 4], "sType": "date-uk"} ]
      }

      this.datatableService.initialize(params);
    }

    goToProjects(service){
      sessionStorage.setItem("serviceName", service.name);
      sessionStorage.setItem("serviceId", service.crmId);
      sessionStorage.setItem("serviceDetail", JSON.stringify(service));

      this.router.navigate([`/billing/customers/${this.customerId}/services/${service.crmId}/projects`]);
    }

    update(){
      this.messageService.showLoading();

      this.updateSubscrip = this.service.update().subscribe(data => {
        this.messageService.closeLoading();
        this.getAll(this.customerId);
      });
    }
}
