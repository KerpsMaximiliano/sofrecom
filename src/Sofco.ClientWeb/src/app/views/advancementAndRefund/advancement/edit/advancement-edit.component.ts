import { Component, OnDestroy, ViewChild, OnInit } from "@angular/core";
import { AdvancementService } from "app/services/advancement-and-refund/advancement.service";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { ActivatedRoute } from "@angular/router";

@Component({
    selector: 'advancement-edit',
    templateUrl: './advancement-edit.component.html'
})
export class AdvancementEditComponent implements OnInit, OnDestroy {
  
    @ViewChild('form') form;

    editSubscrip: Subscription;
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

                this.form.setModel(response.data);
            }, 
            error => this.messageService.closeLoading());
        }
    }
                
    ngOnDestroy(): void {
        if(this.editSubscrip) this.editSubscrip.unsubscribe();
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
    }

    update(){
        var model = this.form.getModel();

        this.messageService.showLoading();

        this.editSubscrip = this.advancementService.edit(model).subscribe(response => {
            this.messageService.closeLoading();
        },
        error => {
            this.messageService.closeLoading();
        });
    }
}