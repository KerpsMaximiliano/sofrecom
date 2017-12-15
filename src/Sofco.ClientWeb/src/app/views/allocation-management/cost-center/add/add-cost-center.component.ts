import { Component, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Router } from "@angular/router";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from "app/services/common/message.service";
import { CostCenterService } from "app/services/allocation-management/cost-center.service";
import { CostCenter } from "app/models/allocation-management/costCenter";

@Component({
    selector: 'add-cost-center',
    templateUrl: './add-cost-center.component.html'
})
export class AddCostCenterComponent implements OnDestroy {

    public model: CostCenter = new CostCenter();

    addSubscrip: Subscription;

    constructor(private costCenterService: CostCenterService,
                private router: Router,
                private menuService: MenuService,
                private messageService: MessageService,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnDestroy(): void {
        if(this.addSubscrip) this.addSubscrip.unsubscribe();
    }

    add(){
        this.addSubscrip = this.costCenterService.add(this.model).subscribe(
            data => {
              if(data.messages) this.messageService.showMessages(data.messages);
            },
            err => this.errorHandlerService.handleErrors(err));
    }
}