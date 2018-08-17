import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from "rxjs";
import { ProjectService } from "../../../../services/billing/project.service";
import { DataTableService } from "../../../../services/common/datatable.service";
import { MenuService } from '../../../../services/admin/menu.service';
import { MessageService } from '../../../../services/common/message.service';

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html'
})
export class ProjectsComponent implements OnInit, OnDestroy {

    projects: any[] = new Array();
    getAllSubscrip: Subscription;
    paramsSubscrip: Subscription;
    updateSubscrip: Subscription;
    customerId: string;
    serviceId: string;
    serviceName: string;
    customerName: string;
    public loading = true;
    public analytic: any;

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private service: ProjectService,
        public menuService: MenuService,
        private messageService: MessageService,
        private datatableService: DataTableService) { }

    ngOnInit() {
      this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
        this.customerId = params['customerId'];
        this.serviceId = params['serviceId'];
        this.customerName = sessionStorage.getItem('customerName');
        this.serviceName = sessionStorage.getItem('serviceName');
        sessionStorage.setItem('customerId', this.customerId);
        sessionStorage.setItem('serviceId', this.serviceId);
        this.getAll(); 
        this.getIfIsRelated();
      });
    }

    ngOnDestroy(){
      if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
      if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
      if(this.updateSubscrip) this.updateSubscrip.unsubscribe();
    }

    getIfIsRelated(){
      this.getAllSubscrip = this.service.getIfIsRelated(this.serviceId).subscribe(response => {
        this.analytic = response;
      });
    }

    getAll(){
      this.messageService.showLoading();

      this.getAllSubscrip = this.service.getAll(this.serviceId).subscribe(d => {
        this.projects = d.data;

        this.initGrid();

        this.messageService.closeLoading();
      },
      err => {
        this.initGrid();
        this.messageService.closeLoading();
      });
    }

    initGrid(){
      var params = {
        selector: '#projectTable',
        columnDefs: [ {"aTargets": [3, 4], "sType": "date-uk"} ]
      }

      this.datatableService.initialize(params);
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
      
      this.router.navigate([`/billing/customers/${this.customerId}/services/${this.serviceId}/projects/${project.crmId}`]);
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

    back(){
      window.history.back();
    }

    goToCreateAnalytic(){
      sessionStorage.setItem('analyticWithProject', 'yes');
      this.router.navigate(['/contracts/analytics/new']);
    }
  
    goToEditAnalytic(){
      this.router.navigate([`/contracts/analytics/${this.analytic.id}/edit/`]);
    }
  
    goToResources(){
      this.router.navigate([`/billing/customers/${this.customerId}/services/${this.serviceId}/resources`]);
    }
  
    goToPurchaseOrders(){
      this.router.navigate([`/billing/customers/${this.customerId}/services/${this.serviceId}/purchaseOrders`]);
    }
    
    update(){
      this.messageService.showLoading();

      this.updateSubscrip = this.service.update().subscribe(data => {
        this.messageService.closeLoading();
        this.getAll();
      });
    }
}
