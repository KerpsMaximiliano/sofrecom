import { Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { AdvancementService } from "app/services/advancement-and-refund/advancement.service";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { DataTableService } from "app/services/common/datatable.service";
import { Router } from "@angular/router";

@Component({
    selector: 'list-finalized',
    templateUrl: './list-finalized.component.html'
})
export class AdvancementListFinalizedComponent implements OnInit, OnDestroy {

    getSubscrip: Subscription;

    public model: any[] = new Array();

    constructor(private advancementService: AdvancementService,
                private datatableService: DataTableService,
                private router: Router,
                private messageService: MessageService){}

    ngOnInit(): void {
        this.initGrid();
    }

    ngOnDestroy(): void {
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
    }
    
    initGrid(){
        var columns = [0, 1, 2, 3, 4, 5];
        var title = `Adelantos-finalizados`;

        var params = {
          selector: '#advancements-finalized',
          columns: columns,
          title: title,
          withExport: true,
          columnDefs: [ {"aTargets": [4], "sType": "date-uk"} ]
        }
  
        this.datatableService.destroy(params.selector);
        this.datatableService.initialize(params);
    }

    goToDetail(item){
        this.router.navigate(['/advancementAndRefund/advancement/' + item.id])
    }

    search(parameters){
        this.messageService.showLoading();

        this.getSubscrip = this.advancementService.getAllFinalized(parameters).subscribe(response => {
            this.messageService.closeLoading();

            this.model = [];
            this.model = response.data;
            this.initGrid();
        }, 
        error => this.messageService.closeLoading());
    }
}