import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { Solfac } from 'app/models/billing/solfac/solfac';
import { HitoDetail } from "app/models/billing/solfac/hitoDetail";
import { SolfacService } from "app/services/billing/solfac.service";
import { Option } from "app/models/option";
import { Router, ActivatedRoute } from '@angular/router';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { Cookie } from "ng2-cookies/ng2-cookies";
import { MessageService } from "app/services/common/message.service";
import { UserService } from "app/services/admin/user.service";
import { InvoiceService } from "app/services/billing/invoice.service";
import { SolfacStatus } from "app/models/enums/solfacStatus";
import { MenuService } from "app/services/admin/menu.service";
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import * as FileSaver from "file-saver";

declare var $:any;

@Component({
  selector: 'app-solfac-edit',
  templateUrl: './solfac-edit.component.html',
  styleUrls: ['./solfac-edit.component.scss']
})
export class SolfacEditComponent implements OnInit, OnDestroy {

    public model: any = {};
    public provinces: Option[] = new Array<Option>();
    public documentTypes: Option[] = new Array<Option>();
    public imputationNumbers: Option[] = new Array<Option>();
    public currencies: Option[] = new Array<Option>();
    public paymentTerms: Option[] = new Array<Option>();
    public users: any[] = new Array();
    public currencySymbol: string = "$";
    private projectId: string = "";

    public solfacId: number;
    public invoices: any[] = new Array<any>();
    public invoicesRelated: any[] = new Array<any>();

    public updateComments: string;

    private detailSelected: any;
 
    getOptionsSubs: Subscription;
    getInvoiceOptionsSubs: Subscription;
    paramsSubscrip: Subscription;
    getDetailSubscrip: Subscription;
    changeStatusSubscrip: Subscription;
    deleteSubscrip: Subscription;

    @ViewChild('history') history: any;

    @ViewChild('updateModal') updateModal;
    public updateModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      "billing.solfac.addUpdateComments",
      "updateModal",
      true,
      true,
      "ACTIONS.ACCEPT",
      "ACTIONS.cancel"
    );

    @ViewChild('deleteDetailModal') deleteDetailModal;
    public deleteDetailModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "deleteDetailModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    constructor(private messageService: MessageService,
                private solfacService: SolfacService,
                private userService: UserService,
                private activatedRoute: ActivatedRoute,
                private menuService: MenuService,
                private invoiceService: InvoiceService,
                private errorHandlerService: ErrorHandlerService,
                private router: Router) { }

    ngOnInit() {
      this.getOptions();
      this.getUserOptions();

      this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
        this.solfacId = params['solfacId'];
        this.getSolfac(this.solfacId);
        this.getInvoices();
      });
    }
 
    ngOnDestroy(){
       if(this.getOptionsSubs) this.getOptionsSubs.unsubscribe();
       if(this.getInvoiceOptionsSubs) this.getInvoiceOptionsSubs.unsubscribe();
       if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
       if(this.getDetailSubscrip) this.getDetailSubscrip.unsubscribe();
       if(this.changeStatusSubscrip) this.getDetailSubscrip.unsubscribe();
       if(this.deleteSubscrip) this.deleteSubscrip.unsubscribe();
    }

    getSolfac(solfacId){
      this.getDetailSubscrip = this.solfacService.get(solfacId).subscribe(d => {

        if(this.menuService.hasFunctionality('SOLFA', 'ALTA') && 
          (d.statusName == SolfacStatus[SolfacStatus.SendPending] || d.statusName == SolfacStatus[SolfacStatus.ManagementControlRejected])){

            this.model = d;
            this.setCurrencySymbol(this.model.currencyId);
            this.getInvoicesOptions(this.model.projectId);

            sessionStorage.setItem('customerName', this.model.businessName);
            sessionStorage.setItem('serviceName', this.model.serviceName);
        }
        else{
            this.messageService.showError("billing.solfac.cannotUpdateSolfac");
            this.router.navigate([`/billing/customers/${d.customerId}/services/${d.serviceId}/projects/${d.projectId}`]);
        }
      },
      err => this.errorHandlerService.handleErrors(err));
    }

    getUserOptions(){
        this.userService.getOptions().subscribe(data => {
          this.users = data;

          var userapplicant = this.users.find(x => x.userName == Cookie.get('currentUser'));
          this.model.userApplicantId = userapplicant.value;
          this.model.userApplicantName = userapplicant.text;
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    getInvoicesOptions(projectId){
      this.getInvoiceOptionsSubs = this.invoiceService.getOptions(projectId).subscribe(data => {
        this.invoices = data;
      },
      err => this.errorHandlerService.handleErrors(err));
    } 

    getInvoices(){
      this.getInvoiceOptionsSubs = this.solfacService.getInvoices(this.solfacId).subscribe(data => {
        this.invoicesRelated = data;
      },
      err => this.errorHandlerService.handleErrors(err));
    }

    getOptions(){
      this.getOptionsSubs = this.solfacService.getOptions().subscribe(data => {
        this.currencies = data.currencies;
        this.provinces = data.provinces;
        this.documentTypes = data.documentTypes;
        this.imputationNumbers = data.imputationNumbers; 
        this.paymentTerms = data.paymentTerms;
      },
      err => this.errorHandlerService.handleErrors(err));
    }

    addDetail(){
      var externalHitoId = "";

      if(this.model.hitos.length > 0){
        externalHitoId = this.model.hitos[0].externalHitoId;
      }

      var detail = new HitoDetail(0, "", 1, 1, 1, 0, externalHitoId);
      this.model.details.push(detail);

      this.calculateAmounts();
    }

    openDeleteDetail(detail, index){
      this.detailSelected = { detail, index }; 
      this.deleteDetailModal.show();
    }

    deleteDetail(){
      if(this.detailSelected.detail.id == 0){
        this.removeDetail();
        this.deleteDetailModal.hide();
        return;
      }

      this.solfacService.deleteDetail(this.detailSelected.detail.id).subscribe(
        data => {
          this.deleteDetailModal.hide();

          if(data.messages) this.messageService.showMessages(data.messages);
          this.removeDetail();
        },
        err => {
          this.deleteDetailModal.hide();
          this.errorHandlerService.handleErrors(err)
        });
    }

    removeDetail(){
      this.model.details.splice(this.detailSelected.index, 1);
      this.calculateAmounts();
    }

    calculateDetail(detail: HitoDetail){
      if(detail.quantity > 0 && detail.unitPrice > 0){
          detail.total = detail.quantity * detail.unitPrice;
      }
      else{
        detail.total = 0;
      }
    }

    calculateTotal(detail: HitoDetail){
      this.calculateDetail(detail);
      this.calculateAmounts();
    }

    calculateAmounts(){
      this.model.totalAmount = 0;
      
      this.model.details.forEach(detail => {
        this.model.totalAmount += detail.total;
      });
    }

    canUpdate(){
        return this.menuService.hasFunctionality("SOLFA", "ALTA");
    }

    validate(){
      this.solfacService.validate(this.model).subscribe(
        data => {
          if(data.messages) this.messageService.showMessages(data.messages);
          this.updateModal.show();
        },
        err => {
          this.errorHandlerService.handleErrors(err)
        });
    }

    update(){
      this.model.comments = this.updateComments;

      this.solfacService.update(this.model).subscribe(
          data => {
            this.updateModal.hide();
            if(data.messages) this.messageService.showMessages(data.messages);
            this.history.getHistories();
          },
          err => {
            this.updateModal.hide();
            this.errorHandlerService.handleErrors(err)
          });
    }

    goToSearch(){
      this.router.navigate([`/billing/solfac/search`]);
    }

    setCurrencySymbol(currencyId){
      switch(currencyId){
        case "1": { this.currencySymbol = "$"; break; }
        case "2": { this.currencySymbol = "U$D"; break; }
        case "3": { this.currencySymbol = "€"; break; }
      }
    }

    getCurrencyId(currency){
      switch(currency){
        case "Peso": { return 1; }
        case "Dolar": { return 2; }
        case "Euro": { return 3; }
      }
    }

    goToProject(){
      this.router.navigate([`/billing/customers/${this.model.customerId}/services/${this.model.serviceId}/projects/${this.model.projectId}`]);
    }

    exportPdf(invoice){
      this.invoiceService.getPdf(invoice.id).subscribe(file => {
          FileSaver.saveAs(file, invoice.pdfFileName);
      },
      err => this.errorHandlerService.handleErrors(err));
    } 

    deleteInvoiceOfSolfac(invoiceId, index){
      this.solfacService.deleteInvoiceOfSolfac(this.solfacId, invoiceId).subscribe(data => {
        if(data.messages) this.messageService.showMessages(data.messages);
        this.invoicesRelated.splice(index, 1);
      },
      err => this.errorHandlerService.handleErrors(err));
    }

    addInvoice(){
      var invoices = <any>$('#invoices').val();

      if(invoices && invoices.length == 0) return;

      this.solfacService.addInvoices(this.solfacId, invoices).subscribe(data => {
        if(data.messages) this.messageService.showMessages(data.messages);
    
        if(data.data && data.data.length > 0){

          data.data.forEach(element => {
            this.invoicesRelated.push({ id: element.id, invoiceNumber: element.invoiceNumber });
          });

          this.invoices = this.invoices.filter(item => {
            if(!invoices.includes(item.value)) return item;

            return null;
          })
        }
      },
      err => this.errorHandlerService.handleErrors(err));
    }
}