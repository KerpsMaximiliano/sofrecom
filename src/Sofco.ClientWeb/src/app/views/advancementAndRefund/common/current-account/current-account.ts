import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { CurrentAccountService } from "app/services/advancement-and-refund/current-account.sevice";
import { DataTableService } from "app/services/common/datatable.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";

@Component({
    selector: 'current-account',
    templateUrl: './current-account.html'
})
export class CurrentAccountComponent implements OnInit, OnDestroy  {

    public model: any[] = new Array();
    public advancementsSelected: any[] = new Array();

    public currentAccountDetail: any = {
        currency: "",
        refunds: [],
        advancements: [],
    }

    getSubscrip: Subscription;
    updateSubscrip: Subscription;

    @ViewChild('detailModal') detailModal;
    public detailModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "advancement.userCurrentAccountDetail",
        "detailModal",
        true,
        true,
        "ACTIONS.doRelate",
        "ACTIONS.cancel"
    );

    constructor(private messageService: MessageService,
                private datatableService: DataTableService,
                private currentAccountService: CurrentAccountService){}

    ngOnInit(): void {
        this.getData();
    }

    ngOnDestroy(): void {
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
        if(this.updateSubscrip) this.updateSubscrip.unsubscribe();
    }

    getData(){
        this.messageService.showLoading();

        this.getSubscrip = this.currentAccountService.get().subscribe(response => {
            this.messageService.closeLoading();
            this.model = response.data;

            this.initGrid();
        },
        () => this.messageService.closeLoading());
    }

    initGrid(){
        var columns = [0, 1, 2, 3, 4, 5];
        var title = `cuenta corriente de usuarios`;

        var params = {
          selector: '#current-account',
          columns: columns,
          title: title,
          withExport: true
        }
  
        this.datatableService.destroy(params.selector);
        this.datatableService.initialize(params);
    }

    openDetailModal(item){
        this.currentAccountDetail = item;

        this.detailModal.show();
    }

    saveRefunds(){
        var json = {
            advancements: this.advancementsSelected,
            refunds: this.currentAccountDetail.refunds.filter(x => x.selected && x.selected == true).map(item => {
                return item.id
            }),
        }

        this.getSubscrip = this.currentAccountService.updateMassive(json).subscribe(response => {
            this.getData();
            this.detailModal.hide();
        },
        () => this.detailModal.resetButtons());
    }

    saveEnabled(){
        if(this.advancementsSelected.length == 0 || this.noneRefundSelected()){
            return false;
        }

        return true;
    }

    noneRefundSelected(){
        return this.currentAccountDetail.refunds.filter(x => x.selected && x.selected == true).length == 0;
    }
}