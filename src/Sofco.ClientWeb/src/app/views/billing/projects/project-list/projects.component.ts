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

        if(d.data && d.data.length > 0){
          d.data.forEach(x => {
            this.projects.push({ type: "item", data: x, id: x.id, show: false });
  
            if (x.billings && x.billings.length > 0) {
                x.billings.forEach(detail => {
                    detail.selected = false;
                });

                this.projects.push({ type: "detail", data: x.billings, id: x.id, show: false });
            }
          });
        }
 
        // this.initGrid();

        this.messageService.closeLoading();
      },
      err => {
        // this.initGrid();
        this.messageService.closeLoading();
      });
    }

    initGrid(){
      var params = {
        selector: '#projectTable',
        columnDefs: [ {"aTargets": [3, 4], "sType": "date-uk"} ]
      }
 
      this.datatableService.destroy(params.selector);
      this.datatableService.initialize(params);
    }

    goToServices(){
      this.router.navigate([`/billing/customers/${this.customerId}/services`]);
    }

    goToManagementReport(){
      this.router.navigate([`/managementReport/${this.customerId}/service/${this.serviceId}/detail`]);
    }

    goToBillMultiple(){
      var filtered = this.projects.filter(x => x.type == "item");

      sessionStorage.setItem('projectsToBillMultiple', JSON.stringify(filtered.map(x => x.data)));
      this.router.navigate([`/billing/customers/${this.customerId}/services/${this.serviceId}/projects/billMultiple`]);
    }

    goToResources(){
      this.router.navigate([`/contracts/analytics/${this.analytic.id}/resources`]);
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
  
    goToPurchaseOrders(){
      this.router.navigate([`/billing/customers/${this.customerId}/services/${this.serviceId}/purchaseOrders`]);
    }
    
    update(){
      this.messageService.showLoading();

      this.updateSubscrip = this.service.update().subscribe(data => {
        this.messageService.closeLoading();
        this.getAll();
      },
      error => {
        this.messageService.closeLoading();
      });
    }

    expand(item){
        var row = this.projects.find(x => x.id == item.id && x.type == 'detail');

        if(row){
            row.show = !row.show;
            item.show = row.show;
        }
    }

    getIconClass(project){
      if(project.data.billings.length == 0) return "";

      if(project.type == 'item' && project.show == false){
        return "fa fa-plus-square";
      }
      else{
        return "fa fa-minus";
      }
    }
}
