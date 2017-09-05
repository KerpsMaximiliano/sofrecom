import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs/Subscription";
import { Invoice } from "models/billing/invoice/invoice";
import { InvoiceService } from "app/services/billing/invoice.service";
import { MessageService } from "app/services/common/message.service";

@Component({
  selector: 'app-invoice-detail',
  templateUrl: './invoice-detail.component.html',
  styleUrls: ['./invoice-detail.component.scss']
})
export class InvoiceDetailComponent implements OnInit, OnDestroy {

    public model: Invoice = new Invoice();
    paramsSubscrip: Subscription;
    getSubscrip: Subscription;
    projectId;

    constructor(private router: Router,
                private activatedRoute: ActivatedRoute,
                private service: InvoiceService,
                private messageService: MessageService,
                private errorHandlerService: ErrorHandlerService) {}

    ngOnInit() {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {

            this.projectId = params['projectId'];

            this.getSubscrip = this.service.getById(params['id']).subscribe(d => {
                this.model = d;
            },
            err => this.errorHandlerService.handleErrors(err));

        });
    }

    ngOnDestroy() {
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.getSubscrip) this.paramsSubscrip.unsubscribe();
    }

    goBack(){
        this.router.navigate([`/billing/project/${this.projectId}`]);
    }
}