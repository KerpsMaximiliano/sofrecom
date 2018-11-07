import { Component, OnDestroy, ViewChild } from "@angular/core";
import { AdvancementService } from "app/services/advancement-and-refund/advancement.service";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";

@Component({
    selector: 'advancement-add',
    templateUrl: './advancement-add.component.html'
})
export class AdvancementAddComponent implements OnDestroy {

    @ViewChild('form') form;

    addSubscrip: Subscription;

    constructor(private advancementService: AdvancementService, 
                private messageService: MessageService){}

    ngOnDestroy(): void {
        if(this.addSubscrip) this.addSubscrip.unsubscribe();
    }

    add(){
        var model = this.form.getModel();

        this.messageService.showLoading();

        this.addSubscrip = this.advancementService.add(model).subscribe(response => {
            this.messageService.closeLoading();
        },
        error => {
            this.messageService.closeLoading();
        });
    }
}