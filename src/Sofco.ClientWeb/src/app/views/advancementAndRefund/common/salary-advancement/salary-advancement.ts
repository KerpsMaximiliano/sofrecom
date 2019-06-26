import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { DataTableService } from "app/services/common/datatable.service";
import { SalaryAdvancementService } from "app/services/advancement-and-refund/salary-advancement";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";

@Component({
    selector: 'salary-advancement',
    templateUrl: './salary-advancement.html'
})
export class SalaryAdvancementComponent implements OnInit, OnDestroy  {
    public model: any[] = new Array();
    public advancementSelecteds: any[] = new Array();

    date: Date;
    amount: number;

    userIdSelected: number;
    itemSelected: any;

    getSubscrip: Subscription;
    updateSubscrip: Subscription;
    deleteSubscrip: Subscription;

    @ViewChild('addModal') addModal;
    public addModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "advancement.salaryDiscountAdd",
        "addModal",
        true,
        true,
        "ACTIONS.save",
        "ACTIONS.cancel"
    );

    constructor(private messageService: MessageService,
                private datatableService: DataTableService,
                private salaryAdvancementService: SalaryAdvancementService){}

    ngOnInit(): void {
        this.getData();
    }

    ngOnDestroy(): void {
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
        if(this.updateSubscrip) this.updateSubscrip.unsubscribe();
        if(this.deleteSubscrip) this.deleteSubscrip.unsubscribe();
    }

    getData(){
        this.messageService.showLoading();

        this.getSubscrip = this.salaryAdvancementService.get().subscribe(response => {
            this.messageService.closeLoading();
            this.model = response.data;

            if(this.userIdSelected){
                var row = this.model.find(x => x.userId == this.userIdSelected);

                if(row != null){
                    this.advancementSelecteds = row.advancements;
                }
                else{
                    this.userIdSelected = null;
                    this.advancementSelecteds = null;
                }
            }

            this.initGrid();
        },
        () => this.messageService.closeLoading());
    }

    initGrid(){
        var columns = [0, 1, 2, 3];
        var title = `Devoluciones de adelantos de sueldo`;

        var params = {
          selector: '#salary-advancement',
          columns: columns,
          title: title,
          withExport: true
        }
  
        this.datatableService.destroy(params.selector);
        this.datatableService.initialize(params);
    }

    selectItem(item){
        this.userIdSelected = item.userId;
        this.advancementSelecteds = item.advancements;
    }

    openModal(item){
        this.itemSelected = item;

        this.addModal.show();
    }

    save(){
        var json = {
            AdvancementId: this.itemSelected.advancementId,
            date: this.date,
            amount: this.amount
        }

        this.updateSubscrip = this.salaryAdvancementService.add(json).subscribe(response => {
            this.addModal.hide();
            this.itemSelected = null;
            this.getData();
        },
        () => this.addModal.resetButtons());
    }

    delete(discount){
        this.messageService.showConfirm(() => {

            this.updateSubscrip = this.salaryAdvancementService.delete(discount.id).subscribe(response => {
                this.getData();
            });
        });
    }
}