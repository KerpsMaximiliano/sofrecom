import { Component, OnDestroy, ViewChild, OnInit } from "@angular/core";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { Router } from "@angular/router";
import { I18nService } from "app/services/common/i18n.service";
import { RefundService } from "app/services/advancement-and-refund/refund.service";

@Component({
    selector: 'refund-add',
    templateUrl: './refund-add.component.html',
    styleUrls: ['./refund-add.component.scss']
})
export class RefundAddComponent implements OnInit, OnDestroy {
    @ViewChild('form') form;

    canLoad = true;

    addSubscrip: Subscription;
    canLoadSubscrip: Subscription;

    constructor(private refundService: RefundService,
                private router: Router,
                public i18nService: I18nService,
                private messageService: MessageService){}

    ngOnInit(): void {
        this.checkIfCanLoad();
    }

    ngOnDestroy(): void {
        if(this.addSubscrip) this.addSubscrip.unsubscribe();
        if(this.canLoadSubscrip) this.canLoadSubscrip.unsubscribe();
    }

    checkIfCanLoad(){
        this.messageService.showLoading();

        this.addSubscrip = this.refundService.canLoad().subscribe(response => {
            this.messageService.closeLoading();
            this.canLoad = response.data;
            this.removeAnimated();
        },
        error => {
            this.messageService.closeLoading();
            this.canLoad = false;
        });
    }

    add(){
        const model = this.form.getModel();
        model.hasCreditCard = this.form.hasCreditCard;

        this.messageService.showLoading();

        this.addSubscrip = this.refundService.add(model).subscribe(response => {
            this.messageService.closeLoading();

            setTimeout(() => {
                this.router.navigate(['/advancementAndRefund/refund/' + response.data]);
            }, 500);
        },
        error => {
            this.messageService.closeLoading();
        });
    }

    removeAnimated() {
        window.setTimeout(function(){
            $("#contentWrapper").removeClass('fadeInRight').removeClass('animated')
          }, 1000);
    }
}
