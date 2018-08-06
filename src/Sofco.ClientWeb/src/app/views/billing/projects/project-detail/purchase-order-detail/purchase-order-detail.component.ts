import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { DataTableService } from "app/services/common/datatable.service";
import { MessageService } from 'app/services/common/message.service';
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
        private messageService: MessageService,
        private datatableService: DataTableService,
        private errorHandlerService: ErrorHandlerService) { }

    ngOnDestroy(){
      if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
    }

    getAll(projectId){
      this.getAllSubscrip = this.projectService.getPurchaseOrders(projectId).subscribe(d => {
        this.purchaseOrders = d;

        var params = {
            selector: '#purchaseOrderTable',
        }

        this.datatableService.destroy(params.selector);
        this.datatableService.initialize(params);
      },
      err => {
        this.errorHandlerService.handleErrors(err);
      });
    }
}
