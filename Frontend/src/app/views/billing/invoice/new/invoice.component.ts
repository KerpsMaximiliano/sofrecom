import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs/Subscription";
import { Invoice, Detail } from "models/billing/invoice/invoice";
import { InvoiceService } from "app/services/billing/invoice.service";
import { MessageService } from "app/services/common/message.service";

@Component({
  selector: 'app-invoice',
  templateUrl: './invoice.component.html',
  styleUrls: ['./invoice.component.scss']
})
export class InvoiceComponent implements OnInit, OnDestroy {

    public model: Invoice = new Invoice();
    paramsSubscrip: Subscription;
    projectId: string;
    project: any;
    customer: any;

    constructor(private router: Router,
                private activatedRoute: ActivatedRoute,
                private service: InvoiceService,
                private messageService: MessageService,
                private errorHandlerService: ErrorHandlerService) {}

    ngOnInit() {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.projectId = params['projectId'];
        });

        this.project = JSON.parse(sessionStorage.getItem("projectDetail"));
        this.customer = JSON.parse(sessionStorage.getItem("customer"));

        this.model.accountName = this.customer.nombre;
        this.model.address = this.customer.address;
        this.model.zipcode = this.customer.postalCode;
        this.model.city = this.customer.city;
        this.model.province = this.customer.province;
        this.model.country = this.customer.country;
        this.model.cuit = this.customer.cuit;
        this.model.project = this.project.nombre;
        this.model.projectId = this.projectId;
        this.model.analytic = this.project.analytic;
        this.model.service = sessionStorage.getItem("serviceName");

        this.model.details.push(new Detail("", 0));
    }

    ngOnDestroy() {

    }

    addDetail(){
        this.model.details.push(new Detail("", 0));
    }

    deleteDetail(index){
        this.model.details.splice(index, 1);
    }

    save(){
      this.service.add(this.model).subscribe(
        data => {
          if(data.messages) this.messageService.showMessages(data.messages);

          setTimeout(() => {
            this.router.navigate([`/billing/project/${this.projectId}`]);
          }, 1500)
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    cancel(){
      this.router.navigate([`/billing/project/${this.projectId}`]);
    }
}