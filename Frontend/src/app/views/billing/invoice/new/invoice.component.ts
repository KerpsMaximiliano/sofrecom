import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs/Subscription";
import { Invoice } from "app/models/billing/invoice/invoice";
import { InvoiceService } from "app/services/billing/invoice.service";
import { MessageService } from "app/services/common/message.service";
import * as FileSaver from "file-saver";
import { FileUploader } from 'ng2-file-upload';
import { MenuService } from "app/services/admin/menu.service";
import { InvoiceStatus } from "app/models/enums/invoiceStatus";
declare var $: any;

@Component({
  selector: 'app-invoice',
  templateUrl: './invoice.component.html',
  styleUrls: ['./invoice.component.scss']
})
export class InvoiceComponent implements OnInit, OnDestroy {

    public model: Invoice = new Invoice();
    paramsSubscrip: Subscription;
    sendToDafSubscrip: Subscription;
    projectId: string;
    project: any;
    customer: any;
    public uploader: FileUploader;
    excelUploaded: boolean = false;

    constructor(private router: Router,
                private activatedRoute: ActivatedRoute,
                private service: InvoiceService,
                public menuService: MenuService,
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
        this.model.invoiceStatus = InvoiceStatus[InvoiceStatus.SendPending];
        this.model.service = sessionStorage.getItem("serviceName");
    }

    ngOnDestroy() {
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.sendToDafSubscrip) this.sendToDafSubscrip.unsubscribe();
    }

    save(){
      this.service.add(this.model).subscribe(
        data => {
          if(data.messages) this.messageService.showMessages(data.messages);

          this.configUploader(data.data.id);

          this.model.id = data.data.id;

          this.exportToExcel();
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    cancel(){
      this.router.navigate([`/billing/project/${this.projectId}`]);
    }

    exportToExcel(){
        this.service.export(this.model).subscribe(file => {
            FileSaver.saveAs(file, `remito_${new Date().toLocaleString()}.xlsx`);
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    sendToDaf(){
        this.service.sendToDaf(this.model.id).subscribe(data => {
            if(data.messages) this.messageService.showMessages(data.messages);

            setTimeout(() => {
                this.cancel();
            }, 1500)
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    private configUploader(id){
        this.uploader = new FileUploader({url: this.service.getUrlForImportExcel(id), authToken: Cookie.get('access_token') });
        this.uploader.onAfterAddingFile = (file)=> { file.withCredentials = false; };

        this.uploader.onCompleteItem = (item:any, response:any, status:any, headers:any) => {
            this.excelUploaded = true;
        };
    }

    canSendInvoice(){
        if(this.menuService.hasFunctionality('REM', 'SEND') && this.model.id > 0 && this.excelUploaded){
            return true;
        }

        return false;
    }

    delete(){
        this.service.delete(this.model.id).subscribe(data => {
            if(data.messages) this.messageService.showMessages(data.messages);

            setTimeout(() => { this.cancel(); }, 1500)
        },
        err => this.errorHandlerService.handleErrors(err));
    }
}