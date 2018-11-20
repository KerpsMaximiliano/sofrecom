import { Component, OnDestroy, OnInit } from "@angular/core";
import { AdvancementService } from "app/services/advancement-and-refund/advancement.service";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { ActivatedRoute } from "@angular/router";

@Component({
    selector: 'advancement-detail',
    templateUrl: './advancement-detail.component.html',
    styleUrls: ['./advancement-detail.component.scss']
})
export class AdvancementDetailComponent implements OnInit, OnDestroy {
  
    public model: any;

    getSubscrip: Subscription;

    constructor(private advancementService: AdvancementService,
                private activateRoute: ActivatedRoute ,
                private messageService: MessageService){}

    ngOnInit(): void {
        const routeParams = this.activateRoute.snapshot.params;

        if (routeParams.id) {
            this.messageService.showLoading();

            this.getSubscrip = this.advancementService.get(routeParams.id).subscribe(response => {
                this.messageService.closeLoading();

                this.model = response.data;
            }, 
            error => this.messageService.closeLoading());
        }
    }
                
    ngOnDestroy(): void {
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
    }
}