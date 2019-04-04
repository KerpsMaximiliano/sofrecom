import { Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { ManagementReportService } from "app/services/management-report/management-report.service";
import { Subscription } from "rxjs";
import { MenuService } from "app/services/admin/menu.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { MessageService } from "app/services/common/message.service";


@Component({
    selector: 'cost-detail',
    templateUrl: './cost-detail.component.html',
    styleUrls: ['./cost-detail.component.scss']
})
export class CostDetailComponent implements OnInit, OnDestroy {

    paramsSubscrip: Subscription;
    getCostSubscrip: Subscription;

    //Propiedades
    serviceId: string;
    months: any[] = new Array();
    employees: any[] = new Array();
    fundedResourses: any[] = new Array();
    
    @ViewChild('editItemModal') editItemModal;
    public editItemModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "billing.project.detail.milestone.splitTitle",
        "editItemModal",
        true,
        true,
        "ACTIONS.save",
        "ACTIONS.cancel"
    );

    //Constructor
    constructor(private managementReportService: ManagementReportService,
        private activatedRoute: ActivatedRoute,
        private messageService: MessageService,
        private menuService: MenuService){}

    ngOnInit(): void {
       this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
        this.serviceId = params['serviceId'];

        this.getCost();
      });
    }

    ngOnDestroy(): void {
        
    }

    getCost(){
        this.messageService.showLoading();

        this.getCostSubscrip = this.managementReportService.getCostDetail(this.serviceId).subscribe(response => {
            this.messageService.closeLoading();
            
            this.months = response.data.monthsHeader;
            this.employees = response.data.costEmployees;
            this.fundedResourses = response.data.fundedResources;
        }, 
        error => {
            this.messageService.closeLoading();
        });
    }

    openEditItemModal(item){

        if(this.menuService.hasFunctionality('MANRE', 'EDIT-COST-DETAIL')){
            this.editItemModal.show();
        }
    }



}

