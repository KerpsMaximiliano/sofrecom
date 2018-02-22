import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { ProjectService } from "app/services/billing/project.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { DataTableService } from "app/services/common/datatable.service";
import { MenuService } from 'app/services/admin/menu.service';

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html'
})
export class ProjectsComponent implements OnInit, OnDestroy {

    projects: any[] = new Array();
    getAllSubscrip: Subscription;
    paramsSubscrip: Subscription;
    customerId: string;
    serviceId: string;
    serviceName: string;
    customerName: string;
    public loading:  boolean = true;

    @ViewChild('rightbar') rightbar;
 
    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private service: ProjectService,
        private menuService: MenuService,
        private datatableService: DataTableService,
        private errorHandlerService: ErrorHandlerService) { }

    ngOnInit() {
      this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
        this.customerId = params['customerId'];
        this.serviceId = params['serviceId'];
        this.customerName = sessionStorage.getItem('customerName');
        this.serviceName = sessionStorage.getItem('serviceName');
        this.getAll();
        this.getIfIsRelated();
      });
    }

    ngOnDestroy(){
      if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
      if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
    }

    getIfIsRelated(){
      this.getAllSubscrip = this.service.getIfIsRelated(this.serviceId).subscribe(data => {
        this.rightbar.hasAnalytic = data;
      },
      err => this.errorHandlerService.handleErrors(err));
    }

    getAll(){
      this.loading = true;

      this.getAllSubscrip = this.service.getAll(this.serviceId).subscribe(d => {
        this.loading = false;
        this.projects = d;

        this.initGrid();
      },
      err => {
        this.loading = false;
        this.initGrid();
        this.errorHandlerService.handleErrors(err)
      });
    }

    initGrid(){
      var params = {
        selector: '#projectTable',
        columnDefs: [ {"aTargets": [5, 6], "sType": "date-uk"} ]
      }

      this.datatableService.init2(params);
    }

    goToServices(){
      this.router.navigate([`/billing/customers/${this.customerId}/services`]);
    }

    goToBillMultiple(){
      sessionStorage.setItem('projectsToBillMultiple', JSON.stringify(this.projects));
      this.router.navigate([`/billing/customers/${this.customerId}/services/${this.serviceId}/projects/billMultiple`]);
    }

    goToProjectDetail(project){
      sessionStorage.setItem("customerId", this.customerId);
      sessionStorage.setItem("serviceId", this.serviceId);
      sessionStorage.setItem("projectDetail", JSON.stringify(project));
      
      this.router.navigate([`/billing/customers/${this.customerId}/services/${this.serviceId}/projects/${project.id}`]);
    }

    getCurrencySymbol(currency){
      switch(currency){
        case "Peso": { return "$"; }
        case "Dolar": { return "U$D"; }
        case "Euro": { return "€"; }
      }
    }

    canBillMultiple(){
      return this.menuService.hasFunctionality('SOLFA', 'MUPRO') && this.projects.length > 1;
    }
}
