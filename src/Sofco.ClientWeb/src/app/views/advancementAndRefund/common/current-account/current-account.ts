import { Component, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { CurrentAccountService } from "app/services/advancement-and-refund/current-account.sevice";
import { DataTableService } from "app/services/common/datatable.service";

@Component({
    selector: 'current-account',
    templateUrl: './current-account.html'
})
export class CurrentAccountComponent implements OnInit, OnDestroy  {

    public model: any[] = new Array();

    getSubscrip: Subscription;

    constructor(private messageService: MessageService,
                private datatableService: DataTableService,
                private currentAccountService: CurrentAccountService){}

    ngOnInit(): void {
        this.messageService.showLoading();

        this.getSubscrip = this.currentAccountService.get().subscribe(response => {
            this.messageService.closeLoading();
            this.model = response.data;

            this.initGrid();
        },
        () => this.messageService.closeLoading());
    }

    ngOnDestroy(): void {
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
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
}