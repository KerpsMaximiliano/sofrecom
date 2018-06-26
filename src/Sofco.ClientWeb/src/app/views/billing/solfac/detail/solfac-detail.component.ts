import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { SolfacService } from "app/services/billing/solfac.service";
import { Router, ActivatedRoute } from '@angular/router';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import * as FileSaver from "file-saver";
import { InvoiceService } from "app/services/billing/invoice.service";
import { MessageService } from "app/services/common/message.service";
import { Option } from 'app/models/option';

@Component({
  selector: 'app-solfac-detail',
  templateUrl: './solfac-detail.component.html',
  styleUrls: ['./solfac-detail.component.scss']
})
export class SolfacDetailComponent implements OnInit, OnDestroy {

    @ViewChild('history') history: any;
    @ViewChild('pdfViewer') pdfViewer: any;

    public model: any = {};
    public solfacId: any;
    public currencySymbol = "$";

    paramsSubscrip: Subscription;
    getDetailSubscrip: Subscription;
    changeStatusSubscrip: Subscription;
    updateOcSubs: Subscription;
    getOptionsSubs: Subscription;

    public purchaseOrders: Option[] = new Array<Option>();
    public invoicesRelated: any[] = new Array<any>();

    constructor(private solfacService: SolfacService,
                private activatedRoute: ActivatedRoute,
                private invoiceService: InvoiceService,
                private messageService: MessageService,
                private errorHandlerService: ErrorHandlerService,
                private router: Router) { }

    ngOnInit() {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.solfacId = params['solfacId'];
            this.getSolfac();
            this.getInvoices();
        });
    }
 
    ngOnDestroy(){
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.getDetailSubscrip) this.getDetailSubscrip.unsubscribe();
        if(this.changeStatusSubscrip) this.changeStatusSubscrip.unsubscribe();
        if(this.getOptionsSubs) this.getOptionsSubs.unsubscribe();
        if(this.updateOcSubs) this.updateOcSubs.unsubscribe();
    }
 
    getOptions() {
        this.getOptionsSubs = this.solfacService.getOptions(this.model.serviceId).subscribe(data => {
          this.purchaseOrders = data.purchaseOrders;
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    updateOc(){
        this.updateOcSubs = this.solfacService.updateOc(this.model.id, this.model.purchaseOrderId).subscribe(response => {
            if(response.messages) this.messageService.showMessages(response.messages);
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    getSolfac(){
        this.messageService.showLoading();

        this.getDetailSubscrip = this.solfacService.get(this.solfacId).subscribe(d => {
            this.messageService.closeLoading();

            this.model = d;
            this.setCurrencySymbol(this.model.currencyId);

            this.getOptions();

            sessionStorage.setItem('customerName', this.model.businessName);
            sessionStorage.setItem('serviceName', this.model.serviceName);
        },
        err => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(err);
        });
    }

    setCurrencySymbol(currencyId){
      switch(currencyId){
        case 1: { this.currencySymbol = "$"; break; }
        case 2: { this.currencySymbol = "U$D"; break; }
        case 3: { this.currencySymbol = "€"; break; }
      }
    }

    goToProject(){
      this.router.navigate([`/billing/customers/${this.model.customerId}/services/${this.model.serviceId}/projects/${this.model.projectId}`]);
    }

    goToSearch(){
      this.router.navigate([`/billing/solfac/search`]);
    }

    exportPdf(invoice){
        this.invoiceService.exportPdfFile(invoice.pdfFileId).subscribe(file => {
            FileSaver.saveAs(file, invoice.pdfFileName);
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    getInvoices(){
        this.solfacService.getInvoices(this.solfacId).subscribe(data => {
            this.invoicesRelated = data;
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    updateStatus(event){
        if(event.statusName) this.model.statusName = event.statusName;
        if(event.invoiceCode) this.model.invoiceCode = event.invoiceCode;
        if(event.invoiceDate) this.model.invoiceDate = event.invoiceDate;
        if(event.cashedDate) this.model.cashedDate = event.cashedDate;
    }

    printSolfac() {
        window.print();
    }

    viewPdf(invoice){
        if(invoice.pdfFileName.endsWith('.pdf')){
            this.invoiceService.getPdfFile(invoice.pdfFileId).subscribe(response => {
                this.pdfViewer.renderFile(response.data);
            },
            err => this.errorHandlerService.handleErrors(err));
        }
    }
}