import { Component, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { Router } from "@angular/router";
import { MessageService } from "../../../../services/common/message.service";
import { CostCenterService } from "../../../../services/allocation-management/cost-center.service";
import { CostCenter } from "../../../../models/allocation-management/costCenter";

@Component({
    selector: 'add-cost-center',
    templateUrl: './add-cost-center.component.html'
})
export class AddCostCenterComponent implements OnDestroy {

    public model: CostCenter = new CostCenter();

    addSubscrip: Subscription;

    constructor(private costCenterService: CostCenterService,
                private router: Router,
                private messageService: MessageService){
    }

    ngOnDestroy(): void {
        if(this.addSubscrip) this.addSubscrip.unsubscribe();
    }

    add(){
        this.messageService.showLoading();

        this.addSubscrip = this.costCenterService.add(this.model).subscribe(
            () => {
                this.messageService.closeLoading();
                this.router.navigate(['/contracts/costCenter']);
            },
            () => {
                this.messageService.closeLoading();
            });
    }
}