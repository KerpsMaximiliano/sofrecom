import { Component, OnDestroy, OnInit } from "@angular/core";
import { Subscription } from "rxjs";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Router, ActivatedRoute } from "@angular/router";
import { MenuService } from "app/services/admin/menu.service";
import { MessageService } from "app/services/common/message.service";
import { CostCenterService } from "app/services/allocation-management/cost-center.service";
import { CostCenter } from "app/models/allocation-management/costCenter";

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
                private menuService: MenuService,
                private activatedRoute: ActivatedRoute,
                private messageService: MessageService,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnInit(): void {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.messageService.showLoading();

            this.getByIdSubscrip = this.costCenterService.getById(params['id']).subscribe(data => {
                this.messageService.closeLoading();
                this.model = data;
            },
            error => {
                this.messageService.closeLoading();
                this.errorHandlerService.handleErrors(error);
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
            data => {
              this.messageService.closeLoading();
              if(data.messages) this.messageService.showMessages(data.messages);
              this.router.navigate(['/contracts/costCenter']);
            },
            err => {
                this.messageService.closeLoading();
                this.errorHandlerService.handleErrors(err);
            });
    }
}