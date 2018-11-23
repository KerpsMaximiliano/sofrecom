import { Component, OnDestroy, ViewChild, OnInit } from "@angular/core";
import { AdvancementService } from "app/services/advancement-and-refund/advancement.service";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { Router } from "@angular/router";

@Component({
    selector: 'advancement-add',
    templateUrl: './advancement-add.component.html'
})
export class AdvancementAddComponent implements OnDestroy {

    @ViewChild('form') form;

    addSubscrip: Subscription;

    constructor(private advancementService: AdvancementService, 
                private router: Router,
                private messageService: MessageService){}
               
    ngOnDestroy(): void {
        if(this.addSubscrip) this.addSubscrip.unsubscribe();
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