import { Component, OnDestroy } from '@angular/core';
import { Subscription } from "rxjs";
import { DataTableService } from "../../../../../services/common/datatable.service";
import { ProjectService } from '../../../../../services/billing/project.service';

@Component({
  selector: 'project-purchase-orders',
  templateUrl: './purchase-order-detail.component.html'
})
export class ProjectPurchaseOrdersComponent implements OnDestroy {

    purchaseOrders: any[] = new Array();
    getAllSubscrip: Subscription;

    constructor(
        private projectService: ProjectService,
        private datatableService: DataTableService) { }

    ngOnDestroy(){
      if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
    }

    getAll(projectId){
      this.getAllSubscrip = this.projectService.getPurchaseOrders(projectId).subscribe(d => {
        this.purchaseOrders = d;

        var columns = [0, 1, 2, 3, 4, 5];
        var title = `Resumen ordenes de compra`;

        var params = {
            selector: '#purchaseOrderTable',
            columns: columns,
            title: title,
            withExport: true
        }

        this.datatableService.destroy(params.selector);
        this.datatableService.initialize(params);
      });
    }
}
