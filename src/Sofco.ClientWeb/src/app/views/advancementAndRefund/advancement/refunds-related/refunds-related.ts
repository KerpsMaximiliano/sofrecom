import { Component, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { WorkflowStateType } from 'app/models/enums/workflowStateType';
import { AdvancementService } from 'app/services/advancement-and-refund/advancement.service';
import { Router } from '@angular/router';

@Component({
  selector: 'refunds-related',
  templateUrl: './refunds-related.html'
})
export class RefundsRelatedComponent implements OnDestroy {

    public refunds: any[] = new Array<any>();
    public advancements: any[] = new Array<any>();

    public advancementSum: number = 0;
    public refundSum: number = 0;
    public companyRefund: number = 0;
    public userRefund: number = 0;

    getSubscrip: Subscription;

    constructor(private advancementService: AdvancementService,
                private router: Router) {
    }

    ngOnDestroy(){
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
    }

    init(ids){
        this.getSubscrip = this.advancementService.getResume(ids).subscribe(response => {
            this.refunds = response.data.refunds;
            this.advancements = response.data.advancements;

            this.calculateTotals();
        });
    }

    calculateTotals(){
        this.advancements.forEach(x => {
            this.advancementSum += x.total;
        });

        this.refunds.forEach(x => {
            this.refundSum += x.total;
        });

        if(this.advancementSum < this.refundSum){
            this.userRefund = this.refundSum - this.advancementSum;
        }
        else{
            if(this.refundSum < this.advancementSum){
                this.companyRefund = this.advancementSum - this.refundSum;
            }
        }
    }

    goToRefund(refund){
        this.router.navigate(['/advancementAndRefund/refund/' + refund.id]);
    }

    getStatusClass(type){
        switch(type){
            case WorkflowStateType.Info: return "label-success";
            case WorkflowStateType.Warning: return "label-warning";
            case WorkflowStateType.Success: return "label-primary";
            case WorkflowStateType.Danger: return "label-danger";
        }
    }
}