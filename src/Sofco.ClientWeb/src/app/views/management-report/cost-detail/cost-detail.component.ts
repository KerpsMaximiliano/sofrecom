import { Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { ManagementReportService } from "app/services/management-report/management-report.service";
import { Subscription } from "rxjs";
import { MenuService } from "app/services/admin/menu.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { MessageService } from "app/services/common/message.service";
import * as moment from 'moment';


@Component({
    selector: 'cost-detail',
    templateUrl: './cost-detail.component.html',
    styleUrls: ['./cost-detail.component.scss']
})
export class CostDetailComponent implements OnInit, OnDestroy {

    paramsSubscrip: Subscription;
    getCostSubscrip: Subscription;
    updateCostSubscrip: Subscription;

    //Propiedades
    serviceId: string;
    months: any[] = new Array();
    employees: any[] = new Array();
    fundedResourses: any[] = new Array();
    monthSelected:  any = {value: 0};
    ItemSelected:  any;
    model: any;

    
    @ViewChild('editItemModal') editItemModal;
    public editItemModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Editar Monto",
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
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.getCostSubscrip) this.getCostSubscrip.unsubscribe();
        if(this.updateCostSubscrip) this.updateCostSubscrip.unsubscribe();
    }

    getCost(){
        this.messageService.showLoading();

        this.getCostSubscrip = this.managementReportService.getCostDetail(this.serviceId).subscribe(response => {
            this.messageService.closeLoading();
            
            this.model = response.data;
           
            this.months = response.data.monthsHeader;
            this.employees = response.data.costEmployees;
            this.fundedResourses = response.data.fundedResources;

        }, 
        error => {
            this.messageService.closeLoading();
        });
    }

    openEditItemModal(month, item){
       
        if(this.menuService.hasFunctionality('MANRE', 'EDIT-COST-DETAIL')){
            this.editItemModal.show();
            this.monthSelected = month;
            this.ItemSelected = item;
        }
    }

    EditItem(){
        
        this.editItemModal.hide();
         
    }

    save(){
        this.messageService.showLoading();

        this.model.employees = this.employees;
        this.model.fundedResources = this.fundedResourses;

        this.updateCostSubscrip = this.managementReportService.PostCostDetail(this.serviceId, this.model).subscribe(response => {
            this.messageService.closeLoading();
          
        }, 
        error => {
            this.messageService.closeLoading();
        });
    }



}

