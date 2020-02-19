import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { UtilsService } from "app/services/common/utils.service";
import { I18nService } from "app/services/common/i18n.service";
import { MessageService } from "app/services/common/message.service";
import { ContractService } from "app/services/allocation-management/contract.service";
import { DataTableService } from "app/services/common/datatable.service";

@Component({
    selector: 'account-info',
    templateUrl: './account-info.html'
})
export class AccountInfoComponent implements OnInit, OnDestroy {
    getYearsSubscrip: Subscription;
    getMonthsSubscrip: Subscription;
    getDataSubscrip: Subscription;

    public yearId: number;
    public monthId: number;

    public years: any[] = new Array();
    public months: any[] = new Array();
    public items: any[] = new Array();

    constructor(private utilsService: UtilsService,
                private i18nService: I18nService,
                private datatableService: DataTableService,
                private contractService: ContractService,
                private messageService: MessageService){
    }

    ngOnInit(): void {
        this.getYearsSubscrip = this.utilsService.getYears().subscribe(data => {
            this.years = data;
        });

        this.getMonthsSubscrip = this.utilsService.getMonths().subscribe(data => {
            this.months = data.map(item => {
                item.text = this.i18nService.translateByKey(item.text);
                return item;
            });
        });
    }

    ngOnDestroy(): void {
        if(this.getYearsSubscrip) this.getYearsSubscrip.unsubscribe();
        if(this.getMonthsSubscrip) this.getMonthsSubscrip.unsubscribe();
        if(this.getDataSubscrip) this.getDataSubscrip.unsubscribe();
    }

    search(){
        this.items = [];
        this.datatableService.destroy("#accountInfoTable");

        if(!this.yearId || !this.monthId || this.yearId <= 0 || this.monthId <= 0) return;

        this.messageService.showLoading();

        this.getDataSubscrip = this.contractService.getAccountInfo(this.yearId, this.monthId).subscribe(response => {
            this.messageService.closeLoading();

            this.items = response.data;

            this.initGrid();
        }, 
        () => this.messageService.closeLoading());
    }

    initGrid(){
        var columns = [0, 1, 2, 3, 4, 5, 6];
        var title = `Informacion de cuentas ${this.yearId}-${this.monthId}`;

        var params = {
          selector: '#accountInfoTable',
          columns: columns,
          title: title,
          withExport: true,
        }
  
        this.datatableService.destroy(params.selector);
        this.datatableService.initialize(params);
    }
}