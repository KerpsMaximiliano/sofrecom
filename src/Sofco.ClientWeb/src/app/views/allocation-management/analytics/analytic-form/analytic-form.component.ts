import { Component, OnDestroy, Input } from "@angular/core";
import { OnInit } from "@angular/core/src/metadata/lifecycle_hooks";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { Router } from "@angular/router";
import { MessageService } from "app/services/common/message.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs/Subscription";
import { CostCenterService } from "app/services/allocation-management/cost-center.service";
import { MenuService } from "app/services/admin/menu.service";

@Component({
    selector: 'analytic-form',
    templateUrl: './analytic-form.component.html'
})
export class AnalyticFormComponent implements OnInit, OnDestroy {

    public options: any;
    public costCenters: any;
 
    public model: any = {};

    getOptionsSubscrip: Subscription;
    getCostCenterOptionsSubscrip: Subscription;
    getNewTitleSubscrip: Subscription;

    @Input() mode: string;

    public datePickerOptions;

    constructor(private analyticService: AnalyticService,
                private costCenter: CostCenterService,
                private router: Router,
                private menuService: MenuService,
                private messageService: MessageService,
                private errorHandlerService: ErrorHandlerService){

        this.datePickerOptions = this.menuService.getDatePickerOptions();
    }

    ngOnInit(): void {
        this.getOptionsSubscrip = this.analyticService.getFormOptions().subscribe(
            data => {
              this.options = data;
            },
            err => this.errorHandlerService.handleErrors(err));

        this.getCostCenterOptionsSubscrip = this.costCenter.getOptions().subscribe(
            data => {
                this.costCenters = data;
            },
            err => this.errorHandlerService.handleErrors(err));

        let analyticWithProject = sessionStorage.getItem('analyticWithProject');
        let project = JSON.parse(sessionStorage.getItem('projectDetail'));

        if(analyticWithProject == 'yes'){
            this.model.clientExternalId = sessionStorage.getItem('customerId');
            this.model.clientExternalName = sessionStorage.getItem('customerName');
            this.model.serviceId = sessionStorage.getItem('serviceId');
            this.model.service = sessionStorage.getItem('serviceName');
            this.model.contractNumber = project.purchaseOrder;
            this.model.amountEarned = project.totalAmmount; 
            this.model.currencyId = this.getCurrencyId(project.currency);
            this.model.solutionId = project.solutionTypeId;
            this.model.technologyId = project.technologyTypeId;
            this.model.serviceTypeId = project.serviceTypeId;
        }
        else{
            this.model.clientExternalName = 'No Aplica';
            this.model.service = 'No Aplica';
            this.model.contractNumber = 'No Aplica';
            this.model.currencyId = 1;
        }
        
        this.model.startDateContract = new Date();
        this.model.endDateContract = new Date();
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
}