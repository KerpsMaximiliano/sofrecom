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
import { UserService } from "app/services/admin/user.service";
import { InvoiceService } from "app/services/billing/invoice.service";
import { SolfacStatus } from "app/models/enums/solfacStatus";
import { MenuService } from "app/services/admin/menu.service";
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { CustomerService } from 'app/services/billing/customer.service';
import { Hito } from 'app/models/billing/solfac/hito';

declare var $:any;

@Component({
  selector: 'app-solfac',
  templateUrl: './solfac.component.html',
  styleUrls: ['./solfac.component.scss']
})
export class SolfacComponent implements OnInit, OnDestroy {

    public model: Solfac = <Solfac>{};
    public provinces: Option[] = new Array<Option>();
    public documentTypes: Option[] = new Array<Option>();
    public imputationNumbers: Option[] = new Array<Option>();
    public currencies: Option[] = new Array<Option>();
    public invoices: Option[] = new Array<Option>();
    public paymentTerms: Option[] = new Array<Option>();
    public users: any[] = new Array();
    public currencySymbol: string = "$";
    private projectId: string = "";

    public solfacId: number = 0;
 
    getOptionsSubs: Subscription;
    getInvoiceOptionsSubs: Subscription;
    paramsSubscrip: Subscription;
    getDetailSubscrip: Subscription;
    changeStatusSubscrip: Subscription;

    public test;

    isCreditNoteSolfacType:boolean = false;
    isDebitNoteSolfacType:boolean = false;
    documentTypeDicts:Object = {
      "default": ["1", "3", "6", "7"],
      "creditNote": ["2", "4"],
      "debitNote": ["5"]
    }
    documentTypeKey:string = "documentTypeName";

    @ViewChild('solfacAttachments') solfacAttachments;

    @ViewChild('saveModal') saveModal;
    public saveModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "saveModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );
    
    constructor(private messageService: MessageService,
                private solfacService: SolfacService,
                private userService: UserService,
                private menuService: MenuService,
                private customerService: CustomerService,
                private invoiceService: InvoiceService,
                private errorHandlerService: ErrorHandlerService,
                private router: Router) { }

    ngOnInit() {
      this.getOptions();
      this.getUserOptions();
      this.setNewModel();
    }

    ngOnDestroy(){
       if(this.getOptionsSubs) this.getOptionsSubs.unsubscribe();
       if(this.getInvoiceOptionsSubs) this.getInvoiceOptionsSubs.unsubscribe();
       if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
       if(this.getDetailSubscrip) this.getDetailSubscrip.unsubscribe();
       if(this.changeStatusSubscrip) this.changeStatusSubscrip.unsubscribe();
    }

    setNewModel(){
      var project = JSON.parse(sessionStorage.getItem('projectDetail'));
      var customer = JSON.parse(sessionStorage.getItem("customer"));

      this.getInvoicesOptions(project.id);

      this.projectId = project.id;

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
      this.model.contractNumber = project.purchaseOrder;
      this.model.project = project.nombre;
      this.model.projectId = project.id;
      this.model.documentType = 1;
      this.model.totalAmount = 0;
      this.model.imputationNumber1 = project.analytic; 
      this.model.imputationNumber3 = 1;
      this.model.currencyId = this.getCurrencyId(project.currency);
      this.model.analytic = project.analytic;
      this.model.customerId = sessionStorage.getItem("customerId");
      this.model.serviceId = sessionStorage.getItem("serviceId");
      this.model.service = sessionStorage.getItem("serviceName");
      this.model.remito = project.remito;

      this.model.hitos = new Array<Hito>();
      this.model.details = new Array<HitoDetail>();

      var hitos = JSON.parse(sessionStorage.getItem('hitosSelected'));

      hitos.forEach(hito => {
        var hitoNew = new Hito(0, hito.name, hito.ammount, project.id, hito.id, hito.money, hito.month, 0, hito.currencyId, hito.opportunityId, hito.managerId);
        this.model.hitos.push(hitoNew);

        var detail = new HitoDetail(0, hito.name, 0, 1, hito.ammount, 0, hito.id);
        this.model.details.push(detail);

        this.calculateDetail(detail);
      });

      this.setSolfacType(hitos);

      this.calculateAmounts();
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

    getOptions(){
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

    addDetail(){
      var externalHitoId = "";

      if(this.model.hitos.length > 0){
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
      this.model.invoicesId = <any>$('#invoices').val();

      this.solfacService.add(this.model).subscribe(
        data => {
          if(data.messages) this.messageService.showMessages(data.messages);

          setTimeout(() => {
            this.router.navigate([`/billing/solfac/${data.data.id}/edit`])
          }, 1000);
          
        },
        err => { 
          this.saveModal.hide();
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
      this.router.navigate([`/billing/customers/${this.model.customerId}/services/${this.model.serviceId}/projects/${this.projectId}`]);
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
      
      this.documentTypes = this.documentTypes.filter(s => allowedValues.includes(s.value));

      if(!allowedValues.includes(this.model.documentType.toString()))
      {
        this.model.documentType = Number(allowedValues[0]);
      }
    }
}