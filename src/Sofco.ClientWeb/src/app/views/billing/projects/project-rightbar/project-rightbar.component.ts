import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { MenuService } from 'app/services/admin/menu.service';

declare var jQuery:any;

@Component({
  selector: 'project-rightbar',
  templateUrl: 'project-rightbar.component.html'
})

export class ProjectRightBarComponent {

  public analytic: any;

  constructor(private router: Router, public menuService: MenuService) {
  }

  toggleNavigation(): void {
    jQuery(".theme-config-box").toggleClass("show");
  }

  goToCreateAnalytic(){
    sessionStorage.setItem('analyticWithProject', 'yes');
    this.router.navigate(['/contracts/analytics/new']);
  }

  goToEditAnalytic(){
    this.router.navigate([`/contracts/analytics/${this.analytic.id}/edit/`]);
  }

  goToResources(){
    var customerId = sessionStorage.getItem('customerId');
    var serviceId = sessionStorage.getItem('serviceId');
    this.router.navigate([`/billing/customers/${customerId}/services/${serviceId}/resources`]);
  }

  goToPurchaseOrders(){
    var customerId = sessionStorage.getItem('customerId');
    var serviceId = sessionStorage.getItem('serviceId');
    this.router.navigate([`/billing/customers/${customerId}/services/${serviceId}/purchaseOrders`]);
  }
}
