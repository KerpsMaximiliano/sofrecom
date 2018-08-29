import { Component, OnDestroy, OnInit } from "@angular/core";
import { Subscription } from "rxjs";
import { Router, ActivatedRoute } from "@angular/router";
import { MessageService } from "../../../../services/common/message.service";
import { CostCenterService } from "../../../../services/allocation-management/cost-center.service";
import { CostCenter } from "../../../../models/allocation-management/costCenter";

@Component({
    selector: 'edit-cost-center',
    templateUrl: './edit-cost-center.component.html'
})
export class EditCostCenterComponent implements OnInit, OnDestroy {
    public model: CostCenter = new CostCenter();

    addSubscrip: Subscription;
    paramsSubscrip: Subscription;
    getByIdSubscrip: Subscription;

    constructor(private costCenterService: CostCenterService,
                private router: Router,
                private activatedRoute: ActivatedRoute,
                private messageService: MessageService){
    }

    ngOnInit(): void {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.messageService.showLoading();

            this.getByIdSubscrip = this.costCenterService.getById(params['id']).subscribe(data => {
                this.messageService.closeLoading();
                this.model = data;
            },
            () => {
                    this.messageService.closeLoading();
                });
        });
    }

    ngOnDestroy(): void {
        if(this.addSubscrip) this.addSubscrip.unsubscribe();
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.getByIdSubscrip) this.getByIdSubscrip.unsubscribe();
    }

    save(){
        this.messageService.showLoading();

        this.addSubscrip = this.costCenterService.edit(this.model).subscribe(
            () => {
                this.messageService.closeLoading();
                this.router.navigate(['/contracts/costCenter']);
            },
            () => {
                this.messageService.closeLoading();
            });
    }
}