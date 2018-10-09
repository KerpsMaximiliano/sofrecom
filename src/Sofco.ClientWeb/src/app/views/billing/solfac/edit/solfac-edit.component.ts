import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs";
import { HitoDetail } from "../../../../models/billing/solfac/hitoDetail";
import { SolfacService } from "../../../../services/billing/solfac.service";
import { Option } from "../../../../models/option";
import { Router, ActivatedRoute } from '@angular/router';
import { Cookie } from "ng2-cookies/ng2-cookies";
import { MessageService } from "../../../../services/common/message.service";
import { UserService } from "../../../../services/admin/user.service";
import { InvoiceService } from "../../../../services/billing/invoice.service";
import { SolfacStatus } from "../../../../models/enums/solfacStatus";
import { MenuService } from "../../../../services/admin/menu.service";
import { Ng2ModalConfig } from '../../../../components/modal/ng2modal-config';
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
    public purchaseOrders: any[] = new Array();
    public users: any[] = new Array();
    public currencySymbol: string = "$";

    public solfacId: number;
    public invoices: any[] = new Array<any>();
    public invoicesRelated: any[] = new Array<any>();

    public updateComments: string;

    private detailSelected: any;
    private integratorProject: any;
    public showAccountControl: boolean = false;

    @ViewChild('pdfViewer') pdfViewer: any;
 
    getOptionsSubs: Subscription;
    getInvoiceOptionsSubs: Subscription;
    paramsSubscrip: Subscription;
    getDetailSubscrip: Subscription;
    changeStatusSubscrip: Subscription;
    deleteSubscrip: Subscription;

    @ViewChild('history') history: any;
    @ViewChild('solfacAttachments') solfacAttachments: any;

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
                private router: Router) { }

    ngOnInit() {
      this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
        this.solfacId = params['solfacId'];

        this.getSolfac(this.solfacId);
        this.getUserOptions();
        this.getInvoices();
      });

      $('#currency-select select').attr('disabled', 'disabled');
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
          (d.statusName == SolfacStatus[SolfacStatus.SendPending] || d.statusName == SolfacStatus[SolfacStatus.ManagementControlRejected] ||
            d.statusName == SolfacStatus[SolfacStatus.RejectedByDaf])){

            this.model = d;
            this.setCurrencySymbol(this.model.currencyId.toString());

            this.getInvoicesOptions(this.model.projectId);

            sessionStorage.setItem('customerName', this.model.businessName);
            sessionStorage.setItem('serviceName', this.model.serviceName);

            this.getOptions();
        }
        else{
            this.messageService.showError("billing.solfac.cannotUpdateSolfac");
            this.router.navigate([`/billing/customers/${d.customerId}/services/${d.serviceId}/projects/${d.projectId}`]);
        }
        this.setIntegrator();
        this.solfacAttachments.getCertificatesAvailable(this.model.customerId);
      });
    }

    setIntegrator(){
      this.integratorProject = {
        integrator:this.model.integrator,
        integratorId:this.model.integratorId
      }
      this.showAccountControl = true;
    }

    getUserOptions(){
        this.userService.getOptions().subscribe(data => {
          this.users = data;

          var userapplicant = this.users.find(x => x.userName == Cookie.get('currentUser'));
          this.model.userApplicantId = userapplicant.id;
          this.model.userApplicantName = userapplicant.text;
        });
    }

    getInvoicesOptions(projectId){
      this.getInvoiceOptionsSubs = this.invoiceService.getOptions(projectId).subscribe(data => {
        this.invoices = data;
      });
    } 

    getInvoices(){
      this.getInvoiceOptionsSubs = this.solfacService.getInvoices(this.solfacId).subscribe(data => {
        this.invoicesRelated = data;
      });
    }

    getOptions(){
      this.getOptionsSubs = this.solfacService.getOptions(this.model.serviceId, this.model.opportunityNumber).subscribe(data => {
        this.currencies = data.currencies;
        this.provinces = data.provinces;
        this.documentTypes = data.documentTypes;
        this.purchaseOrders = data.purchaseOrders;
        this.imputationNumbers = data.imputationNumbers; 
      });
    }

    purchaseOrderChange(purchaseOrderId){
      var ocOption = this.purchaseOrders.find(x => x.id == purchaseOrderId);

      if(ocOption){
        this.model.paymentTerm = ocOption.extraValue;
      }
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

          this.removeDetail();
        },
        err => {
          this.deleteDetailModal.hide();
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
          this.updateModal.show();
          this.update = this.justUpdate;
        });
    }

    validateSend(){
      this.solfacService.validate(this.model).subscribe(
        data => {
          this.updateModal.show();
          this.update = this.updateAndSend;
        });
    }

    updateAndSend(){
      this.model.comments = this.updateComments;

      this.solfacService.updateAndSend(this.model).subscribe(
          data => {
            this.updateModal.hide();
            this.goBack();
          },
          err => {
            this.updateModal.hide();
          });
    }

    update(){}

    justUpdate(){
      this.model.comments = this.updateComments;

      this.solfacService.update(this.model).subscribe(
          data => {
            this.updateModal.hide();
            this.history.getHistories();
          },
          err => {
            this.updateModal.hide();
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

    goBack(){
      if(this.model.isMultiple){
        this.router.navigate([`/billing/customers/${this.model.customerId}/services/${this.model.serviceId}/projects`]);
      }
      else{
        this.router.navigate([`/billing/customers/${this.model.customerId}/services/${this.model.serviceId}/projects/${this.model.projectId}`]);
      }
    }

    exportPdf(invoice){
      this.invoiceService.exportPdfFile(invoice.pdfFileId).subscribe(file => {
          FileSaver.saveAs(file, invoice.pdfFileName);
      });
    } 

    deleteInvoiceOfSolfac(invoiceId, index){
      this.solfacService.deleteInvoiceOfSolfac(this.solfacId, invoiceId).subscribe(data => {
        if(data.messages) this.messageService.showMessages(data.messages);
        this.invoicesRelated.splice(index, 1);
      });
    }

    addInvoice(){
      var invoices = <any>$('#invoices').val();

      if(invoices && invoices.length == 0) return;

      this.solfacService.addInvoices(this.solfacId, invoices).subscribe(data => {
        if(data.data && data.data.length > 0){

          data.data.forEach(element => {
            this.invoicesRelated.push({ id: element.id, invoiceNumber: element.invoiceNumber, pdfFileName: element.pDfFileData.fileName });
          });

          this.invoices = this.invoices.filter(item => {
            if(!invoices.includes(item.id)) return item;

            return null;
          })
        }
      });
    }

    viewPdf(invoice){
      if(invoice.pdfFileName.endsWith('.pdf')){
        this.invoiceService.getPdfFile(invoice.pdfFileId).subscribe(response => {
            this.pdfViewer.renderFile(response.data);
        });
      }
    }
}