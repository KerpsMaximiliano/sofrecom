import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { Solfac } from 'app/models/billing/solfac/solfac';
import { HitoDetail } from "app/models/billing/solfac/hitoDetail";
import { SolfacService } from "app/services/billing/solfac.service";
import { Option } from "app/models/option";
import { Router } from '@angular/router';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { Cookie } from "ng2-cookies/ng2-cookies";
import { MessageService } from "app/services/common/message.service";
import { InvoiceService } from "app/services/billing/invoice.service";
import { SolfacStatus } from "app/models/enums/solfacStatus";
import { MenuService } from "app/services/admin/menu.service";
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { CustomerService } from 'app/services/billing/customer.service';
import { Hito } from 'app/models/billing/solfac/hito';
import { ServiceService } from 'app/services/billing/service.service';
import { CertificatesService } from 'app/services/billing/certificates.service';

declare var $:any;

@Component({
  selector: 'app-solfac',
  templateUrl: './solfac.component.html',
  styleUrls: ['./solfac.component.scss']
})
export class SolfacComponent implements OnInit, OnDestroy {

    public model: Solfac = <Solfac>{};
    public provinces: Option[] = new Array<Option>();
    public documentTypes: any[] = new Array();
    public imputationNumbers: Option[] = new Array<Option>();
    public currencies: Option[] = new Array<Option>();
    public invoices: Option[] = new Array<Option>();
    public paymentTerms: Option[] = new Array<Option>();
    public certificates: Option[] = new Array<Option>();
    public currencySymbol: string = "$";
    private projectId: string = "";
    public integratorProject: any;

    public solfacId: number = 0;
    public multipleProjects: boolean = false;
 
    getOptionsSubs: Subscription;
    getInvoiceOptionsSubs: Subscription;
    paramsSubscrip: Subscription;
    getDetailSubscrip: Subscription;
    changeStatusSubscrip: Subscription;
    getCertificateAvailableSubscrip: Subscription;

    public test;

    isCreditNoteSolfacType:boolean = false;
    isDebitNoteSolfacType:boolean = false;
    documentTypeDicts:Object = {
      "default": [1, 3, 6, 7],
      "creditNote": [2, 4],
      "debitNote": [5]
    }
    documentTypeKey:string = "documentTypeName";

    @ViewChild('solfacAttachments') solfacAttachments;

    constructor(private messageService: MessageService,
                private solfacService: SolfacService,
                private menuService: MenuService,
                private certificateService: CertificatesService,
                private customerService: CustomerService,
                private serviceService: ServiceService,
                private invoiceService: InvoiceService,
                private errorHandlerService: ErrorHandlerService,
                private router: Router) { }

    ngOnInit() {
      this.getOptions();
      this.setNewModel();
    }

    ngOnDestroy(){
       if(this.getOptionsSubs) this.getOptionsSubs.unsubscribe();
       if(this.getInvoiceOptionsSubs) this.getInvoiceOptionsSubs.unsubscribe();
       if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
       if(this.getDetailSubscrip) this.getDetailSubscrip.unsubscribe();
       if(this.changeStatusSubscrip) this.changeStatusSubscrip.unsubscribe();
       if(this.getCertificateAvailableSubscrip) this.getCertificateAvailableSubscrip.unsubscribe();
    }

    setDataForMultipleProjects(multipleProjects){
      this.multipleProjects = true;
      this.projectId = multipleProjects.ids;
      this.model.contractNumber = multipleProjects.purchaseOrders;
      this.model.project = multipleProjects.names;
      this.model.projectId = multipleProjects.ids;
      this.model.imputationNumber1 = multipleProjects.analytics; 
      this.model.currencyId = this.getCurrencyId(multipleProjects.currency);
      this.model.analytic = multipleProjects.analytics;
      this.model.remito = multipleProjects.remito;

      this.model.hitos = new Array<Hito>();
      this.model.details = new Array<HitoDetail>();

      multipleProjects.hitos.forEach(hito => {
        var hitoNew = new Hito(0, hito.name, hito.ammount, hito.projectId, hito.id, hito.money, hito.month, 0, hito.money, hito.opportunityId, hito.managerId);
        this.model.hitos.push(hitoNew);

        var detail = new HitoDetail(0, hito.name, 0, 1, hito.ammount, 0, hito.id);
        this.model.details.push(detail);

        this.calculateDetail(detail);
      });

      this.setSolfacType(multipleProjects.hitos);

      this.calculateAmounts();
    }

    setDataForSingleProject(){
      var project = JSON.parse(sessionStorage.getItem('projectDetail'));
      
      this.integratorProject = project;

      this.getInvoicesOptions(project.id);
      this.projectId = project.id;
      this.model.contractNumber = project.purchaseOrder;
      this.model.project = project.nombre;
      this.model.projectId = project.id;
      this.model.integrator = project.integrator;
      this.model.integratorId = project.integratorId;
      
      this.model.remito = project.remito;
      this.model.analytic = project.analytic;
      this.model.imputationNumber1 = project.analytic;

      this.model.hitos = new Array<Hito>();
      this.model.details = new Array<HitoDetail>();

      var hitos = JSON.parse(sessionStorage.getItem('hitosSelected'));

      hitos.forEach(hito => {
        var hitoNew = new Hito(0, hito.name, hito.ammount, hito.projectId, hito.id, hito.money, hito.month, 0, hito.money, hito.opportunityId, hito.managerId);
        this.model.hitos.push(hitoNew);

        var detail = new HitoDetail(0, hito.name, 0, 1, hito.ammount, 0, hito.id);
        this.model.details.push(detail);

        this.model.currencyId = this.getCurrencyId(hito.money);

        this.calculateDetail(detail);
      });

      this.setSolfacType(hitos);

      this.calculateAmounts();
    }

    setNewModel(){
      var multipleProjects = JSON.parse(sessionStorage.getItem('multipleProjects'));
      var service = JSON.parse(sessionStorage.getItem('serviceDetail'));
      this.model.totalAmount = 0;
      this.model.documentType = 1;

      if(multipleProjects){
        this.setDataForMultipleProjects(multipleProjects)
      }
      else{
        this.setDataForSingleProject();
      }
    
      var customer = JSON.parse(sessionStorage.getItem("customer"));
      var service = JSON.parse(sessionStorage.getItem('serviceDetail'));

      if(service){
        this.model.imputationNumber1 = service.analytic; 
        this.model.analytic = service.analytic;
        this.model.manager = service.manager;
        this.model.managerId = service.managerId;
      }
      else{
        this.serviceService.getById(sessionStorage.getItem("customerId"), sessionStorage.getItem("serviceId")).subscribe(data => {
          this.model.imputationNumber1 = data.analytic; 
          this.model.analytic = data.analytic;
          this.model.manager = data.manager;
          this.model.managerId = data.managerId;
        },
        err => this.errorHandlerService.handleErrors(err));
      }

      if(customer){
        this.model.businessName = customer.nombre;
        this.model.clientName = customer.contact;
        this.model.celphone = customer.telephone;

        if(customer.paymentTermCode == 0){
          customer.paymentTermCode = 1;
        }
        
        this.model.paymentTermId = customer.paymentTermCode;
      }
      else{
        this.customerService.getById(sessionStorage.getItem("customerId")).subscribe(data => {
          this.model.businessName = data.nombre;
          this.model.clientName = data.contact;
          this.model.celphone = data.telephone;

          if(customer.paymentTermCode == 0){
            customer.paymentTermCode = 1;
          }

          this.model.paymentTermId = data.paymentTermCode;
        },
        err => this.errorHandlerService.handleErrors(err));
      }

      this.model.statusName = SolfacStatus[SolfacStatus.SendPending];
      this.model.statusId = SolfacStatus[SolfacStatus.SendPending];
      this.model.imputationNumber3 = 1;
      this.model.customerId = sessionStorage.getItem("customerId");
      this.model.serviceId = sessionStorage.getItem("serviceId");
      this.model.service = sessionStorage.getItem("serviceName");

      if(Cookie.get('userInfo')){
        var userApplicant = JSON.parse(Cookie.get('userInfo'));

        if(userApplicant && userApplicant.id && userApplicant.name){
          this.model.userApplicantId = userApplicant.id;
          this.model.userApplicantName = userApplicant.name;
        }
      }
      
      this.setCurrencySymbol(this.model.currencyId.toString());
      this.getCertificatesAvailable();
    }

    getInvoicesOptions(projectId) {
      this.getInvoiceOptionsSubs = this.invoiceService.getOptions(projectId).subscribe(data => {
        this.invoices = data;
      },
      err => this.errorHandlerService.handleErrors(err));
    }
 
    getOptions() {
      this.getOptionsSubs = this.solfacService.getOptions().subscribe(data => {
        this.currencies = data.currencies;
        this.provinces = data.provinces;
        this.documentTypes = data.documentTypes;
        this.imputationNumbers = data.imputationNumbers;
        this.paymentTerms = data.paymentTerms;
        this.updateDocumentTypes();
      },
      err => this.errorHandlerService.handleErrors(err));
    }

    addDetail() {
      let externalHitoId = '';

      if (this.model.hitos.length > 0){
        externalHitoId = this.model.hitos[0].externalHitoId;
      }

      var detail = new HitoDetail(0, "", 1, 1, 1, 0, externalHitoId);
      this.model.details.push(detail);

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

    save(){
      this.messageService.showLoading();

      this.model.invoicesId = <any>$('#invoices').val();
      this.model.certificatesId = <any>$('#certificates').val();

      this.solfacService.add(this.model).subscribe(
        data => {
          this.messageService.closeLoading();

          if(data.messages) this.messageService.showMessages(data.messages);
          sessionStorage.removeItem('hitosSelected');
          sessionStorage.removeItem('multipleProjects');

          setTimeout(() => {
            this.router.navigate([`/billing/solfac/${data.data.id}/edit`])
          }, 500);
        },
        err => {
          this.messageService.closeLoading();
          this.errorHandlerService.handleErrors(err);
        });
    }

    deleteDetail(index){
      this.model.details.splice(index, 1);

      this.calculateAmounts();
    }

    canSave(){
      return this.menuService.hasFunctionality("SOLFA", "ALTA") && this.solfacId == 0;
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

    cancel(){
      sessionStorage.removeItem('hitosSelected');
      sessionStorage.removeItem('multipleProjects');

      if(this.multipleProjects){
        this.router.navigate([`/billing/customers/${this.model.customerId}/services/${this.model.serviceId}/projects`]);
      }
      else{
        this.router.navigate([`/billing/customers/${this.model.customerId}/services/${this.model.serviceId}/projects/${this.projectId}`]);
      }
    }

    setSolfacType(hitos:Array<any>) {
      let documentTypeName = sessionStorage.getItem(this.documentTypeKey);
      sessionStorage.removeItem(this.documentTypeKey);
      this.isCreditNoteSolfacType = documentTypeName == "creditNote";
      this.isDebitNoteSolfacType = documentTypeName == "debitNote";

      if(this.isCreditNoteSolfacType || this.isDebitNoteSolfacType)
      {
        let currentHito = hitos[0];
        let hito = this.model.hitos[0];
        hito.solfacId = currentHito.solfacId;
      }

      this.updateDocumentTypes();
    }

    getAllowedDocumentType():Array<string> {
      if(this.isCreditNoteSolfacType)
      {
        return this.documentTypeDicts["creditNote"];
      }
      if(this.isDebitNoteSolfacType)
      {
        return this.documentTypeDicts["debitNote"];
      }
      return this.documentTypeDicts["default"];
    }

    updateDocumentTypes() {
      let allowedValues = this.getAllowedDocumentType();
      
      this.documentTypes = this.documentTypes.filter(s => allowedValues.includes(s.id));

      if(this.documentTypes.length > 0 && !allowedValues.includes(this.model.documentType.toString()))
      {
        this.model.documentType = Number(allowedValues[0]);
      }
    }

    hasIntegrator():Boolean {
      return false;
    }

    getCertificatesAvailable(){
      this.getCertificateAvailableSubscrip = this.certificateService.getByClient(this.model.customerId).subscribe(data => {
        this.certificates = data;
      },
      err => this.errorHandlerService.handleErrors(err));
    }
}