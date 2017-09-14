import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { ProjectService } from "app/services/billing/project.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { DataTableService } from "app/services/common/datatable.service";

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html'
})
export class ProjectsComponent implements OnInit, OnDestroy {

    projects: any[];
    getAllSubscrip: Subscription;
    paramsSubscrip: Subscription;
    customerId: string;
    serviceId: string;
    serviceName: string;
    customerName: string;
    public loading:  boolean = true;

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private service: ProjectService,
        private datatableService: DataTableService,
        private errorHandlerService: ErrorHandlerService) { }

    ngOnInit() {
      this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
        this.customerId = params['customerId'];
        this.serviceId = params['serviceId'];
        this.customerName = sessionStorage.getItem('customerName');
        this.serviceName = sessionStorage.getItem('serviceName');
        this.getAll();
      });
    }

    ngOnDestroy(){
      if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
      if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
    }

    getAll(){
      this.loading = true;

      this.getAllSubscrip = this.service.getAll(this.serviceId).subscribe(d => {
        this.loading = false;
        this.projects = d;

        this.datatableService.init('#projectTable');
      },
      err => {
        this.loading = false;
        this.errorHandlerService.handleErrors(err)
      });
    }

    goToServices(){
      this.router.navigate([`/billing/customers/${this.customerId}/services`]);
    }

    goToProjectDetail(project){
      sessionStorage.setItem("customerId", this.customerId);
      sessionStorage.setItem("serviceId", this.serviceId);
      sessionStorage.setItem("projectDetail", JSON.stringify(project));
      
      this.router.navigate([`/billing/project/${project.id}`]);
    }

    getCurrencySymbol(currency){
      switch(currency){
        case "Peso": { return "$"; }
        case "Dolar": { return "U$D"; }
        case "Euro": { return "€"; }
      }
    }
}
