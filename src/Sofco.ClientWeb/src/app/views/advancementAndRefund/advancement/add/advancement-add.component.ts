import { Component, OnDestroy, ViewChild, OnInit } from "@angular/core";
import { AdvancementService } from "app/services/advancement-and-refund/advancement.service";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { Router } from "@angular/router";
import { I18nService } from "app/services/common/i18n.service";

@Component({
    selector: 'advancement-add',
    templateUrl: './advancement-add.component.html'
})
export class AdvancementAddComponent implements OnInit, OnDestroy {


    @ViewChild('form') form;

    canLoad: boolean = true;

    addSubscrip: Subscription;
    canLoadSubscrip: Subscription;

    constructor(private advancementService: AdvancementService, 
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

        this.addSubscrip = this.advancementService.canLoad().subscribe(response => {
            this.messageService.closeLoading();
            this.canLoad = response.data;
        }, 
        error => {
            this.messageService.closeLoading();
            this.canLoad = false;
        });
    }

    add(){
        var model = this.form.getModel();

        this.messageService.showLoading();

        this.addSubscrip = this.advancementService.add(model).subscribe(response => {
            this.messageService.closeLoading();

            setTimeout(() => {
                this.router.navigate(['/advancementAndRefund/advancement/' + response.data]);
            }, 500);
        },
        error => {
            this.messageService.closeLoading();
        });
    }
}