import { Component, OnDestroy, Input } from "@angular/core";
import { OnInit } from "@angular/core/src/metadata/lifecycle_hooks";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { Router } from "@angular/router";
import { MessageService } from "app/services/common/message.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs/Subscription";
import { CostCenterService } from "app/services/allocation-management/cost-center.service";
import { MenuService } from "app/services/admin/menu.service";
import { CustomerService } from "../../../../services/billing/customer.service";
import { ServiceService } from "../../../../services/billing/service.service";

@Component({
    selector: 'analytic-form',
    templateUrl: './analytic-form.component.html'
})
export class AnalyticFormComponent implements OnInit, OnDestroy {

    public options: any;
    public costCenters: any;
 
    public model: any = {};
    public services: any[] = new Array();
    public customers: any[] = new Array();

    public customerId: string = "0";
    public serviceId: string = "0";

    getOptionsSubscrip: Subscription;
    getCostCenterOptionsSubscrip: Subscription;
    getNewTitleSubscrip: Subscription;

    @Input() mode: string;

    constructor(private analyticService: AnalyticService,
                private costCenter: CostCenterService,
                private router: Router,
                private customerService: CustomerService,
                private serviceService: ServiceService,
                private menuService: MenuService,
                private messageService: MessageService,
                private errorHandlerService: ErrorHandlerService){}

    ngOnInit(): void {
        let service = JSON.parse(sessionStorage.getItem('serviceDetail'));
        let analyticWithProject = sessionStorage.getItem('analyticWithProject');

        this.getCustomers();

        this.getOptionsSubscrip = this.analyticService.getFormOptions().subscribe(
            data => {
                this.options = data;

                if(this.mode == 'new' && analyticWithProject == 'yes'){
                    var manager = this.options.managers.filter(element => {
                        if(element.extraValue == service.managerId) return element;
                    });
    
                    if(manager && manager.length > 0){
                        this.model.managerId = manager[0].id;  
                    }
                }
            },
            err => this.errorHandlerService.handleErrors(err));

        this.getCostCenterOptionsSubscrip = this.costCenter.getOptions().subscribe(
            data => {
                this.costCenters = data;
            },
            err => this.errorHandlerService.handleErrors(err));

        if(this.mode == 'new'){
            this.model.activityId = 1;
            this.model.currencyId = 1;

            if(analyticWithProject == 'yes'){
                this.model.clientExternalId = sessionStorage.getItem('customerId');
                this.model.clientExternalName = sessionStorage.getItem('customerName');
                this.model.serviceId = sessionStorage.getItem('serviceId');
                this.model.service = sessionStorage.getItem('serviceName');
                // this.model.contractNumber = project.purchaseOrder;
                // this.model.amountEarned = project.totalAmmount; 
                // this.model.currencyId = this.getCurrencyId(service.currency);
                this.model.solutionId = service.solutionTypeId;
                this.model.technologyId = service.technologyTypeId;
                this.model.serviceTypeId = service.serviceTypeId;
            }
            else{
                this.model.clientExternalName = 'No Aplica';
                this.model.service = 'No Aplica';
                this.model.contractNumber = 'No Aplica';
            }
           
            this.model.startDateContract = new Date();
            this.model.endDateContract = new Date();
        }
    }

    ngOnDestroy(): void {
        if(this.getOptionsSubscrip) this.getOptionsSubscrip.unsubscribe();
        if(this.getCostCenterOptionsSubscrip) this.getCostCenterOptionsSubscrip.unsubscribe();
        if(this.getNewTitleSubscrip) this.getNewTitleSubscrip.unsubscribe();
    }

    costCenterChange(){
        if(this.mode == 'edit') return;

        this.getNewTitleSubscrip = this.analyticService.getNewTitle(this.model.costCenterId).subscribe(
            response => {
                this.model.title = response.data;
            },
            err => this.errorHandlerService.handleErrors(err));
    }

    getCurrencyId(currency){
        switch(currency){
            case "Peso": { return 1; }
            case "Dolar": { return 2; }
            case "Euro": { return 3; }
        }
    }

    getCustomers() {
        this.messageService.showLoading();

        this.customerService.getOptions().subscribe(d => {
            this.messageService.closeLoading();
            this.customers = d.data;
        },
        err => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(err)
        });
    }

    customerChange(){
        this.model.clientExternalName = this.customerId;

        this.serviceId = "0";
        this.services = [];

        if(this.customerId != "0"){
            this.messageService.showLoading();
            
            this.serviceService.getOptions(this.customerId).subscribe(d => {
                this.messageService.closeLoading();
                this.services = d.data;
            },
            err => {
                this.messageService.closeLoading();
                this.errorHandlerService.handleErrors(err);
            });
        }
    }

    serviceChange(){
        this.model.service = this.serviceId;
    }
}