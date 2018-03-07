import { Component, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Router, ActivatedRoute } from "@angular/router";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from "app/services/common/message.service";
import { AllocationService } from "app/services/allocation-management/allocation.service";

@Component({
    selector: 'resource-by-service',
    templateUrl: './resource-by-service.component.html',
    styleUrls: ['./resource-by-service.component.scss']
})

export class ResourceByServiceComponent implements OnInit, OnDestroy {

    public resources: any[] = new Array<any>();
    public serviceName: string;
    public customerName: string;
    customerId: string;
    serviceId: string;

    getAllSubscrip: Subscription;
    paramsSubscrip: Subscription;

    constructor(private router: Router,
                private menuService: MenuService,
                private messageService: MessageService,
                private activatedRoute: ActivatedRoute,
                private allocationervice: AllocationService,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.customerId = params['customerId'];
            this.serviceId = params['serviceId'];
            this.customerName = sessionStorage.getItem('customerName');
            this.serviceName = sessionStorage.getItem('serviceName');
            this.getAll();
          });
    }

    ngOnDestroy(): void {
        if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
    }

    getAll(){
        this.messageService.showLoading();

        this.getAllSubscrip = this.allocationervice.getAllocationsByService(this.serviceId).subscribe(data => {
            this.resources = data;
            this.messageService.closeLoading();
        },
        error => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(error)
        });
    }
 
    goToServices(){
        this.router.navigate([`/billing/customers/${this.customerId}/services`]);
    }

    goToProjects(){
        sessionStorage.setItem("customerId", this.customerId);
        sessionStorage.setItem("serviceId", this.serviceId);
        
        this.router.navigate([`/billing/customers/${this.customerId}/services/${this.serviceId}/projects`]);
      }
}