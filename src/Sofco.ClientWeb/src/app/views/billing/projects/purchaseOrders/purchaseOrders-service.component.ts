import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs";
import { ServiceService } from '../../../../services/billing/service.service';
import { PurchaseOrderService } from '../../../../services/billing/purchaseOrder.service';
import * as FileSaver from "file-saver";
import { MessageService } from '../../../../services/common/message.service';

@Component({
  selector: 'service-purchase-orders',
  templateUrl: './purchaseOrders-service.component.html',
  styleUrls: ['./purchaseOrders-service.component.scss']
})
export class PurchaseOrdersByServiceComponent implements OnInit, OnDestroy {

    purchaseOrders: any[] = new Array();
    getAllSubscrip: Subscription;
    paramsSubscrip: Subscription;
    customerId: string;
    serviceId: string;
    serviceName: string;
    customerName: string;

    @ViewChild('pdfViewer') pdfViewer;

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private service: ServiceService,
        private purchaseOrderService: PurchaseOrderService,
        private messageService: MessageService) { }

    ngOnInit() {
      this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
        this.serviceId = params['serviceId'];
        this.customerId = params['customerId'];
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
      this.messageService.showLoading();

      this.getAllSubscrip = this.service.getPurchaseOrders(this.serviceId).subscribe(d => {
        this.messageService.closeLoading();
        this.purchaseOrders = d;
      });
    }

    goToProjects(){
      sessionStorage.setItem("customerId", this.customerId);
      sessionStorage.setItem("serviceId", this.serviceId);
      
      this.router.navigate([`/billing/customers/${this.customerId}/services/${this.serviceId}/projects`]);
    }

    exportExcel(item){
        this.purchaseOrderService.exportFile(item.fileId).subscribe(file => {
            FileSaver.saveAs(file, item.fileName);
        });
    }

    goToServices(){
        this.router.navigate([`/billing/customers/${this.customerId}/services`]);
    }

    viewFile(item){
      if(item.fileName.endsWith('.pdf')){
          this.purchaseOrderService.getFile(item.fileId).subscribe(response => {
              this.pdfViewer.renderFile(response.data);
          });
      }
  }

  goBack(){
    window.history.back();
  }
}
