import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Router } from "@angular/router";
import { DataTableService } from "app/services/common/datatable.service";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from "app/services/common/message.service";
import { CostCenterService } from "app/services/allocation-management/cost-center.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";

declare var moment: any;

@Component({
    selector: 'list-cost-center',
    templateUrl: './list-cost-center.component.html'
})

export class ListCostCenterComponent implements OnInit, OnDestroy {

    public model: any[] = new Array<any>();
    public costCentersFiltered: any[] = new Array<any>();
    public actives: boolean = true;

    getAllSubscrip: Subscription;

    private costCenterSelected: any;
    private index;
    public modalMessage: string;
    
    public modalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle", //title
        "confirmModal", //id
        true,          //Accept Button
        true,          //Cancel Button
        "ACTIONS.DEACTIVATE",     //Accept Button Text
        "ACTIONS.cancel");   //Cancel Button Text

    @ViewChild("confirmModal") confirmModal;

    constructor(private costCenterService: CostCenterService,
                private router: Router,
                public menuService: MenuService,
                private messageService: MessageService,
                private dataTableService: DataTableService,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void {
        this.messageService.showLoading();

        this.getAllSubscrip = this.costCenterService.getAll().subscribe(data => {
            this.messageService.closeLoading();
            this.model = data;
            this.filterActives(true);
        },
        error => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(error);
        });
    }

    ngOnDestroy(): void {
        if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
    }

    initGrid(){
        var columns = [0, 1, 2];
        var title = `Centro_de_Costos-${moment(new Date()).format("YYYYMMDD")}`;

        var params = {
            selector: '#costCentersTable',
            columns: columns,
            title: title,
            withExport: true
          }

          this.dataTableService.destroy(params.selector);
          this.dataTableService.initialize(params);
    }

    goToAdd(){
        this.router.navigate(['/contracts/costCenter/add']);
    }

    confirm(){
        this.costCenterService.changeStatus(this.costCenterSelected.id, !this.costCenterSelected.active).subscribe(data => {
            this.confirmModal.hide();
            if(data.messages) this.messageService.showMessages(data.messages);
            this.costCenterSelected.active = !this.costCenterSelected.active;
            this.filterActives(this.actives);
        },
        error => {
            this.confirmModal.hide();
            this.errorHandlerService.handleErrors(error);
        });
    }

    habInhabClick(costCenter, index){
        this.costCenterSelected = costCenter;
        this.index = index;

        if(costCenter.active){
            this.modalConfig.acceptButtonText = "ACTIONS.DEACTIVATE";
            this.modalMessage = "ACTIONS.areYouSureConfirmDelete";
        }
        else{
            this.modalConfig.acceptButtonText = "ACTIONS.ACTIVATE";
            this.modalMessage = "ACTIONS.areYouSureConfirmAdd";
        }

        this.confirmModal.show();
    }

    filterActives(active){
        this.actives = active;

        if(active){
            this.costCentersFiltered = this.model.filter((element) => {
                return element.active == true;
            });
        }
        else{
            this.costCentersFiltered = this.model.filter((element) => {
                return element;
            });
        }

        this.initGrid();
    }

    gotToEdit(costCenter){
        this.router.navigate(['/contracts/costCenter/' + costCenter.id + '/edit']);
    }
} 