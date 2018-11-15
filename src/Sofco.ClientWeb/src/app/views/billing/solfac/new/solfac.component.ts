import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs";
import { Solfac } from '../../../../models/billing/solfac/solfac';
import { HitoDetail } from "../../../../models/billing/solfac/hitoDetail";
import { SolfacService } from "../../../../services/billing/solfac.service";
import { Option } from "../../../../models/option";
import { Router } from '@angular/router';
import { MessageService } from "../../../../services/common/message.service";
import { InvoiceService } from "../../../../services/billing/invoice.service";
import { SolfacStatus } from "../../../../models/enums/solfacStatus";
import { MenuService } from "../../../../services/admin/menu.service";
import { CustomerService } from '../../../../services/billing/customer.service';
import { Hito } from '../../../../models/billing/solfac/hito';
import { ServiceService } from '../../../../services/billing/service.service';
import { CertificatesService } from '../../../../services/billing/certificates.service';
import { UserInfoService } from '../../../../services/common/user-info.service';
import { I18nService } from '../../../../services/common/i18n.service';
import { AnalyticService } from 'app/services/allocation-management/analytic.service';

declare var $:any;
declare var swal: any;

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
    public certificates: Option[] = new Array<Option>();
    public purchaseOrders: any[] = new Array();
    public currencySymbol = "$";
    private projectId = "";
    public integratorProject: any;

    public solfacId = 0;
    public multipleProjects = false;
    public activityDisabled = false;
 
    getOptionsSubs: Subscription;
    getInvoiceOptionsSubs: Subscription;
    paramsSubscrip: Subscription;
    getDetailSubscrip: Subscription;
    changeStatusSubscrip: Subscription;
    getCertificateAvailableSubscrip: Subscription;
    getAnalyticSubscrip: Subscription;

    isCreditNoteSolfacType = false;
    isDebitNoteSolfacType = false;
    documentTypeDicts:Object = {
      "default": [1, 3, 6, 7],
      "creditNote": [2, 4],
      "debitNote": [5]
    }
    documentTypeKey = "documentTypeName";

    @ViewChild('solfacAttachments') solfacAttachments;

    constructor(private messageService: MessageService,
                private solfacService: SolfacService,
                private menuService: MenuService,
                private certificateService: CertificatesService,
                private analyticService: AnalyticService,
                private customerService: CustomerService,
                private serviceService: ServiceService,
                private invoiceService: InvoiceService,
                private i18nService: I18nService,
                private router: Router) { }

    ngOnInit() {
      this.getOptions();
      this.setNewModel();

      $('#currency-select select').attr('disabled', 'disabled');
    }

    ngOnDestroy(){
       if(this.getOptionsSubs) this.getOptionsSubs.unsubscribe();
       if(this.getInvoiceOptionsSubs) this.getInvoiceOptionsSubs.unsubscribe();
       if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
       if(this.getDetailSubscrip) this.getDetailSubscrip.unsubscribe();
       if(this.changeStatusSubscrip) this.changeStatusSubscrip.unsubscribe();
       if(this.getCertificateAvailableSubscrip) this.getCertificateAvailableSubscrip.unsubscribe();
       if(this.getAnalyticSubscrip) this.getAnalyticSubscrip.unsubscribe();
    }

    setDataForMultipleProjects(multipleProjects){
      this.multipleProjects = true;
      this.projectId = multipleProjects.ids;
      this.model.project = multipleProjects.names;
      this.model.projectId = multipleProjects.ids;
      this.model.currencyId = this.getCurrencyId(multipleProjects.currency);
      this.model.remito = multipleProjects.remito;

      this.model.hitos = new Array<Hito>();
      this.model.details = new Array<HitoDetail>();

      multipleProjects.hitos.forEach(hito => {
        const hitoNew = new Hito(0, hito.name, hito.ammount, hito.projectId, hito.id, hito.money, hito.month, 0, hito.moneyId, hito.opportunityId, hito.managerId);
        this.model.hitos.push(hitoNew);

        const detail = new HitoDetail(0, hito.name, 0, 1, hito.ammount, 0, hito.id);
        this.model.details.push(detail);

        this.calculateDetail(detail);
      });

      this.setSolfacType(multipleProjects.hitos);

      this.calculateAmounts();
    }

    setDataForSingleProject(){
      const project = JSON.parse(sessionStorage.getItem('projectDetail'));

      this.integratorProject = project;

      this.getInvoicesOptions(project.crmId);
      this.projectId = project.crmId;
      this.model.project = project.name;
      this.model.projectId = project.crmId;
      this.model.integrator = project.integrator;
      this.model.integratorId = project.integratorId;
      this.model.remito = project.remito;
      this.model.opportunityNumber = project.opportunityNumber;
      this.model.clientName = project.principalContactName;

      this.model.hitos = new Array<Hito>();
      this.model.details = new Array<HitoDetail>();

      const hitos = JSON.parse(sessionStorage.getItem('hitosSelected'));

      if(!this.validateHitos(hitos)) return;

      hitos.forEach(hito => {
        const hitoNew = new Hito(0, hito.name, hito.ammount, hito.projectId, hito.id, hito.money, hito.month, 0, hito.moneyId, hito.opportunityId, hito.managerId);
        this.model.hitos.push(hitoNew);

        const detail = new HitoDetail(0, hito.name, 0, 1, hito.ammount, 0, hito.id);
        this.model.details.push(detail);

        this.model.currencyId = this.getCurrencyId(hito.money);

        this.calculateDetail(detail);
      });

      this.setSolfacType(hitos);

      this.calculateAmounts();
    }

    setNewModel(){
      const multipleProjects = JSON.parse(sessionStorage.getItem('multipleProjects'));
      this.model.totalAmount = 0;
      this.model.documentType = 1;

      if(multipleProjects){
        this.setDataForMultipleProjects(multipleProjects)
      } else {
        this.setDataForSingleProject();
      }

      const customer = JSON.parse(sessionStorage.getItem("customer"));
      const service = JSON.parse(sessionStorage.getItem('serviceDetail'));

      if(service){
        this.model.imputationNumber1 = service.analytic; 
        this.model.analytic = service.analytic;
        this.model.manager = service.manager;
        this.model.managerId = service.managerId;

        this.getAnalytic(this.model.analytic);

      } else {
        this.serviceService.getById(sessionStorage.getItem("customerId"), sessionStorage.getItem("serviceId")).subscribe(data => {
          this.model.imputationNumber1 = data.analytic;
          this.model.analytic = data.analytic;
          this.model.manager = data.manager;
          this.model.managerId = data.managerId;

          this.getAnalytic(this.model.analytic);
        });
      }

      if(customer){
        this.model.businessName = customer.name;
        this.model.celphone = customer.telephone;
      } else {
        this.customerService.getById(sessionStorage.getItem("customerId")).subscribe(data => {
          this.model.businessName = data.name;
          this.model.celphone = data.telephone;
        });
      }

      this.model.statusName = SolfacStatus[SolfacStatus.SendPending];
      this.model.statusId = SolfacStatus[SolfacStatus.SendPending];
      this.model.imputationNumber3 = 1;
      this.model.customerId = sessionStorage.getItem("customerId");
      this.model.serviceId = sessionStorage.getItem("serviceId");
      this.model.service = sessionStorage.getItem("serviceName");

      const userInfo = UserInfoService.getUserInfo();

      if (userInfo && userInfo.id && userInfo.name) {
        this.model.userApplicantId = userInfo.id;
        this.model.userApplicantName = userInfo.name;
      }

      this.setCurrencySymbol(this.model.currencyId.toString());
      this.getCertificatesAvailable();
    }

    getAnalytic(title) {
      this.getAnalyticSubscrip = this.analyticService.getByTitle(title).subscribe(data => {
        if(data && data.activityId > 0){
          this.model.imputationNumber3 = data.activityId;
          this.activityDisabled = true;
        }
      });
    }

    getInvoicesOptions(projectId) {
      this.getInvoiceOptionsSubs = this.invoiceService.getOptions(projectId).subscribe(data => {
        this.invoices = data;
      });
    }

    getOptions() {
      const project = JSON.parse(sessionStorage.getItem('projectDetail'));

      this.getOptionsSubs = this.solfacService.getOptions(sessionStorage.getItem("serviceId"), project.opportunityNumber).subscribe(data => {
        this.currencies = data.currencies;
        this.provinces = data.provinces;
        this.documentTypes = data.documentTypes;
        this.imputationNumbers = data.imputationNumbers;
        this.purchaseOrders = data.purchaseOrders;
        this.updateDocumentTypes();
      });
    }

    addDetail() {
      let externalHitoId = '';

      if (this.model.hitos.length > 0){
        externalHitoId = this.model.hitos[0].externalHitoId;
      }

      const detail = new HitoDetail(0, "", 1, 1, 1, 0, externalHitoId);
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

          sessionStorage.removeItem('hitosSelected');
          sessionStorage.removeItem('multipleProjects');

          setTimeout(() => {
            this.router.navigate([`/billing/solfac/${data.data.id}/edit`])
          }, 500);
        },
        err => {
          this.messageService.closeLoading();
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
      const documentTypeName = sessionStorage.getItem(this.documentTypeKey);
      sessionStorage.removeItem(this.documentTypeKey);
      this.isCreditNoteSolfacType = documentTypeName == "creditNote";
      this.isDebitNoteSolfacType = documentTypeName == "debitNote";

      if(this.isCreditNoteSolfacType || this.isDebitNoteSolfacType)
      {
        const currentHito = hitos[0];
        const hito = this.model.hitos[0];
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
      const allowedValues = this.getAllowedDocumentType();

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
      });
    }

    validateHitos(hitos) {
      let isValid = true;

      if(hitos == null) {
        swal({
          type: 'error',
          title: 'Error',
          html: '<h4>' + this.i18nService.translateByKey('billing.solfac.hitosSelectedDataError') + '</h4>',
        }).then((result) => {
          if (result.value) {
            window.location.href = '/';
          }
        });
        isValid = false;
      }

      return isValid;
    }

    purchaseOrderChange(purchaseOrderId){
      const ocOption = this.purchaseOrders.find(x => x.id == purchaseOrderId);

      if(ocOption){
        this.model.paymentTerm = ocOption.extraValue;
      }
    }
}