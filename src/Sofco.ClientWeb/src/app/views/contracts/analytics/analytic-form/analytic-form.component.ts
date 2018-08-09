import { Component, OnDestroy, Input } from "@angular/core";
import { OnInit } from "@angular/core/src/metadata/lifecycle_hooks";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { MessageService } from "app/services/common/message.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs";
import { CostCenterService } from "app/services/allocation-management/cost-center.service";
import { CustomerService } from "../../../../services/billing/customer.service";
import { ServiceService } from "../../../../services/billing/service.service";
import { UserService } from "../../../../services/admin/user.service";

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
    public users: any[] = new Array();

    public customerId: string = "0";
    public serviceId: string = "0";

    getOptionsSubscrip: Subscription;
    getCostCenterOptionsSubscrip: Subscription;
    getNewTitleSubscrip: Subscription;

    @Input() mode: string;

    constructor(private analyticService: AnalyticService,
                private costCenter: CostCenterService,
                private customerService: CustomerService,
                private userService: UserService,
                private serviceService: ServiceService,
                private messageService: MessageService,
                private errorHandlerService: ErrorHandlerService){}

    ngOnInit(): void {
        let service = JSON.parse(sessionStorage.getItem('serviceDetail'));
        let analyticWithProject = sessionStorage.getItem('analyticWithProject');

        if(this.mode == 'new'){
            this.getCustomers();
        }
        
        this.getUsers();

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

            if(analyticWithProject == 'yes'){
                this.model.clientExternalId = this.customerId = sessionStorage.getItem('customerId');
                this.model.clientExternalName = sessionStorage.getItem('customerName');
                this.model.serviceId = this.serviceId = sessionStorage.getItem('serviceId');
                this.model.service = sessionStorage.getItem('serviceName');
                this.model.solutionId = service.solutionTypeId;
                this.model.technologyId = service.technologyTypeId;
                this.model.serviceTypeId = service.serviceTypeId;

                this.getServices();
            }
            else{
                this.model.clientExternalName = 'No Aplica';
                this.model.service = 'No Aplica';
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

    getUsers(){
        this.userService.getOptions().subscribe(data => {
            this.users = data;
        },
        err => {
            this.errorHandlerService.handleErrors(err)
        });
    }

    getCustomers() {
        this.messageService.showLoading();

        this.customerService.getAllOptions().subscribe(d => {
            this.messageService.closeLoading();
            this.customers = d.data;
        },
        err => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(err)
        });
    }

    customerChange(){
        this.model.clientExternalId = this.customerId;

        var customers = this.customers.filter(x => x.id == this.customerId);

        if(customers.length > 0){
            this.model.clientExternalName = customers[0].text;
        }

        this.serviceId = "0";
        this.services = [];

        if(this.customerId != "0"){
            this.getServices();
        }
    }

    getServices(){
        this.messageService.showLoading();
            
        this.serviceService.getAllNotRelated(this.customerId).subscribe(d => {
            this.messageService.closeLoading();
            this.services = d.data;    
        },
        err => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(err);
        });
    }

    serviceChange(){
        this.model.serviceId = this.serviceId;

        var services = this.services.filter(x => x.id == this.serviceId);

        if(services.length > 0){
            this.model.service = services[0].text;

            this.messageService.showLoading();

            this.serviceService.getById(this.customerId, this.serviceId).subscribe(response => {
                this.messageService.closeLoading();

                this.model.solutionId = response.data.solutionTypeId;
                this.model.technologyId = response.data.technologyTypeId;
                this.model.serviceTypeId = response.data.serviceTypeId;
            }, 
            error => {
                this.messageService.closeLoading();
            });
        }
    }
}