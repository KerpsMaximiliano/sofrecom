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
    public invoices: Option[] = new Array<Option>();
    public users: any[] = new Array();
    public currencySymbol: string = "$";
    private projectId: string = "";

    public solfacId: number;

    public updateComments: string;
 
    getOptionsSubs: Subscription;
    getInvoiceOptionsSubs: Subscription;
    paramsSubscrip: Subscription;
    getDetailSubscrip: Subscription;
    changeStatusSubscrip: Subscription;

    @ViewChild('confirmModal') confirmModal;
    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "confirmModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    @ViewChild('updateModal') updateModal;
    public updateModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      "billing.solfac.addUpdateComments",
      "updateModal",
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
      });
    }

    ngOnDestroy(){
       if(this.getOptionsSubs) this.getOptionsSubs.unsubscribe();
       if(this.getInvoiceOptionsSubs) this.getInvoiceOptionsSubs.unsubscribe();
       if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
       if(this.getDetailSubscrip) this.getDetailSubscrip.unsubscribe();
       if(this.changeStatusSubscrip) this.getDetailSubscrip.unsubscribe();
    }

    getSolfac(solfacId){
      this.getDetailSubscrip = this.solfacService.get(solfacId).subscribe(d => {

        if(this.menuService.hasFunctionality('SOLFA', 'ALTA') && 
          (d.statusName == SolfacStatus[SolfacStatus.SendPending] || d.statusName == SolfacStatus[SolfacStatus.ManagementControlRejected])){

            this.model = d;
            this.setCurrencySymbol(this.model.currencyId);
            this.getInvoicesOptions(this.model.projectId);
        }
        else{
            this.messageService.showError("No se puede modificar la solicitud en el estado actual");
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

    getOptions(){
      this.getOptionsSubs = this.solfacService.getOptions().subscribe(data => {
        this.currencies = data.currencies;
        this.provinces = data.provinces;
        this.documentTypes = data.documentTypes;
        this.imputationNumbers = data.imputationNumbers;
      },
      err => this.errorHandlerService.handleErrors(err));
    }

    calculate(){
      this.model.hitos.forEach(item => {
        this.calculateDetail(item);
      });

      this.calculateAmounts();
    }

    calculateDetail(detail: HitoDetail){
      if(detail.quantity > 0 && detail.unitPrice > 0){

        if(this.model.documentType == 3 || this.model.documentType == 4){
            detail.total = detail.quantity * (detail.unitPrice * 1.21);
        }
        else{
          detail.total = detail.quantity * detail.unitPrice;
        }
      }
      else{
        detail.total = 0;
      }
    }

    calculateTotal(detail: HitoDetail){
      this.calculateDetail(detail);
      this.calculateAmounts();
    }

    changeDocumentType(){
      this.model.hitos.forEach(detail => {
        this.calculateTotal(detail);
      });
    }

    calculateAmounts(){
      this.model.amount = 0;

      this.model.hitos.forEach(detail => {
        this.model.amount += detail.total;
      });

      if(this.model.documentType == 1 || this.model.documentType == 2){
        this.model.iva21 = this.model.amount * 0.21;
      }
      else{
        this.model.iva21 = 0;
      }

      this.model.totalAmount = this.model.amount + this.model.iva21;
    }

    canUpdate(){
        return this.menuService.hasFunctionality("SOLFA", "ALTA");
    }

    update(){
      this.model.comments = this.updateComments;

      this.solfacService.update(this.model).subscribe(
          data => {
            this.updateModal.hide();
            if(data.messages) this.messageService.showMessages(data.messages);
          },
          err => {
            this.updateModal.hide();
            this.errorHandlerService.handleErrors(err)
          });
    }

    canSendToCDG(){
        if((this.model.statusName == SolfacStatus[SolfacStatus.SendPending] || 
           this.model.statusName == SolfacStatus[SolfacStatus.ManagementControlRejected])
           && this.menuService.hasFunctionality("SOLFA", "SCDG")){

            return true;
        }

        return false;
    }

    showConfirmSendToCDG(){
      this.confirm = this.sendToCDG;
      this.confirmModal.show();
    }

    sendToCDG(){
        this.changeStatusSubscrip = this.solfacService.changeStatus(this.model.id, SolfacStatus.PendingByManagementControl, "", "").subscribe(
            data => {
                this.confirmModal.hide();
                if(data.messages) this.messageService.showMessages(data.messages);

                setTimeout(() => {
                    this.router.navigate([`/billing/customers/${this.model.customerId}/services/${this.model.serviceId}/projects/${this.model.projectId}`]); 
                }, 0);
            },
            error => {
                this.confirmModal.hide();
                this.errorHandlerService.handleErrors(error);
            });
    }

    canDelete(){
      if(this.model.statusName == SolfacStatus[SolfacStatus.SendPending] || 
         this.model.statusName == SolfacStatus[SolfacStatus.ManagementControlRejected]){
          return true;
      }

      return false;
    }

    showConfirmDelete(){
      this.confirm = this.delete;
      this.confirmModal.show();
    }

    delete(){
      this.solfacService.delete(this.model.id).subscribe(data => {
          this.confirmModal.hide();
          if(data.messages) this.messageService.showMessages(data.messages);

          setTimeout(() => { this.goToProject() }, 1500)
      },
      err => {
          this.confirmModal.hide();
          this.errorHandlerService.handleErrors(err);
      });
    }

    goToSearch(){
      this.router.navigate([`/billing/solfac/search`]);
    }

    confirm() {}

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

    showUpdateModal(){
      this.updateModal.show();
    }
}