import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { ProjectService } from "app/services/billing/project.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { MenuService } from "app/services/admin/menu.service";
import { DataTableService } from "app/services/common/datatable.service";
import { SolfacStatus } from 'app/models/enums/solfacStatus';
import { forEach } from '@angular/router/src/utils/collection';
import { DocumentTypes } from 'app/models/enums/documentTypes';
import { MessageService } from 'app/services/common/message.service';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { Configuration } from 'app/services/common/configuration';

@Component({
  selector: 'app-project-detail',
  templateUrl: './project-detail.component.html',
  styleUrls: ['./project-detail.component.scss']
})
export class ProjectDetailComponent implements OnInit, OnDestroy {

    paramsSubscrip: Subscription;
    getHitosSubscrip: Subscription;
    getSolfacSubscrip: Subscription;
    getInvoicesSubscrip: Subscription;
    getProjectSubscrip: Subscription;
    projectId: string;
    customerId: string;
    serviceId: string;
    customerName: string;
    serviceName: string;
    project: any;
    hitos: any[] = new Array();
    solfacs: any[] = new Array();
    invoices: any[] = new Array();
    public loading:  boolean = true;
    
    incomesBilled: number = 0;
    incomesCashed: number = 0;
    incomesPending: number = 0;

    billedHitoStatus:string = "Facturado";
    pendingHitoStatus:string = "Pendiente";
    projectedHitoStatus:string = "Proyectado";
    paidHitoStatus:string = "Pagado";
    toBeBilledHitoStatus:string = "ToBeBilled";
    documentTypeKey:string = "documentTypeName";

    @ViewChild('hito') hito;
    @ViewChild('splitHito') splitHito;

    @ViewChild('closeHitoModal') closeHitoModal;
    public closeHitoModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "closeHitoModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private service: ProjectService,
        private datatableService: DataTableService,
        private messageService: MessageService,
        public menuService: MenuService,
        private errorHandlerService: ErrorHandlerService,
        private config: Configuration) {}

    ngOnInit() {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.projectId = params['projectId'];
            this.customerId = params['customerId'];
            this.serviceId = params['serviceId'];

            this.customerName = sessionStorage.getItem('customerName');
            this.serviceName = sessionStorage.getItem('serviceName');

            sessionStorage.setItem('customerId', params['customerId']);
            sessionStorage.setItem('serviceId', params['serviceId']);
            sessionStorage.setItem('projectId', params['projectId']);

            this.getProject(params['projectId']);
            this.getSolfacs(this.projectId);
            this.getHitos();
            this.getInvoices(this.projectId);
        });
    }

    ngOnDestroy(){
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.getHitosSubscrip) this.getHitosSubscrip.unsubscribe();
        if(this.getSolfacSubscrip) this.getHitosSubscrip.unsubscribe();
        if(this.getInvoicesSubscrip) this.getInvoicesSubscrip.unsubscribe();
        if(this.getProjectSubscrip) this.getProjectSubscrip.unsubscribe();
    }

    goToProjects(){
        this.router.navigate([`/billing/customers/${this.customerId}/services/${this.serviceId}/projects`]);
    }

    goToServices(){
      this.router.navigate([`/billing/customers/${this.customerId}/services`]);
    }

    getProject(projectId){
        var project = sessionStorage.getItem("projectDetail");

        if(project){
            this.project = JSON.parse(project);
            this.loading = false;
        }
        else{
            this.getProjectSubscrip = this.service.getById(projectId).subscribe(data => {
                this.project = data;
                sessionStorage.setItem("projectDetail", JSON.stringify(data));
                this.loading = false;
            },
            err => {
                this.loading = false;
                this.errorHandlerService.handleErrors(err);
            });
        }
    }

    getHitos(){
        this.getHitosSubscrip = this.service.getHitos(this.projectId).subscribe(d => {
            this.hitos = d;

            this.datatableService.destroy('#hitoTable');
            this.datatableService.init('#hitoTable', false);
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    getInvoices(projectId){
        this.getInvoicesSubscrip = this.service.getInvoices(projectId).subscribe(d => {
            this.invoices = d;

            this.datatableService.init('#invoiceTable', false);
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    getSolfacs(projectId){
        this.getSolfacSubscrip = this.service.getSolfacs(projectId).subscribe(d => {
            this.solfacs = d;

            this.datatableService.init('#solfacTable', false);

            //this.calculateIncomes();
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    calculateIncomes() {
        var incomesPending = this.project.incomes;

        this.solfacs.forEach((item, index) => {

            if(item.statusName == SolfacStatus[SolfacStatus.Invoiced]){

                if(item.documentTypeId == DocumentTypes.CreditNoteA || item.documentTypeId == DocumentTypes.CreditNoteB){
                    this.incomesBilled -= item.totalAmount;
                    incomesPending += item.totalAmount;
                }
                else{
                    this.incomesBilled += item.totalAmount;

                    if(item.documentTypeId != DocumentTypes.DebitNote){
                        incomesPending -= item.totalAmount;
                    }
                }
            }

            if(item.statusName == SolfacStatus[SolfacStatus.AmountCashed]){

                if(item.documentTypeId == DocumentTypes.CreditNoteA || item.documentTypeId == DocumentTypes.CreditNoteB){
                    this.incomesCashed -= item.totalAmount;
                    incomesPending += item.totalAmount;
                }
                else{
                    this.incomesCashed += item.totalAmount;

                    if(item.documentTypeId != DocumentTypes.DebitNote){
                        incomesPending -= item.totalAmount;
                    }
                }
            }
        });

        this.incomesPending = incomesPending;
    }

    generateSolfac() {
        var hitos = this.getHitosSelected();
        sessionStorage.setItem("hitosSelected", JSON.stringify(hitos));
        this.router.navigate(["/billing/solfac"]);
    }

    getHitosSelected(){
        var hitos = this.hitos.filter(hito => {
            if(hito.included && hito.included == true){
             return hito;
            }

            return null;
        });

        return hitos;
    }

    setCurrencySymbol(currencyId){
      switch(currencyId){
        case 1: { return "$";  }
        case 2: { return "U$D"; }
        case 3: { return "€"; }
      }
    }

    getCurrencySymbol(currency){
      switch(currency){
        case "Peso": { return "$"; }
        case "Dolar": { return "U$D"; }
        case "Euro": { return "€"; }
      }
    }
 
    goToSolfacDetail(solfac){
        if(this.menuService.hasFunctionality('SOLFA', 'ALTA') && 
          (solfac.statusName == SolfacStatus[SolfacStatus.SendPending] || solfac.statusName == SolfacStatus[SolfacStatus.ManagementControlRejected]))
        {
            this.router.navigate(["/billing/solfac/" + solfac.id + "/edit"]);
        }
        else{
            this.router.navigate(["/billing/solfac/" + solfac.id]);
        }
    }

    goToInvoiceDetail(invoice){
        this.router.navigate(["/billing/invoice/" + invoice.id + "/project/" + this.projectId]);
    }

    goToCreateInvoice(){
        this.router.navigate(["/billing/invoice/new/project/" + this.projectId]);
    } 

    canCreateInvoice(){
        return this.menuService.hasFunctionality('REM', 'ALTA');
    }

    canCreateSolfac(){
        return this.menuService.hasFunctionality('SOLFA', 'ALTA');
    }

    canSeeInvoices(){
        return this.menuService.hasFunctionality('REM', 'QUERY') && this.project.remito == true;
    }

    canSeeSolfacs(){
        return this.menuService.hasFunctionality('SOLFA', 'QUERY');
    }

    generateSolfacVisible(){
        var hitos = this.getHitosSelected();

        let isValid = hitos.length > 0;
    
        hitos.forEach(item => {
            if(item.billed || item.status == "Cerrado"){
                isValid = false;
            }
        });

        return isValid;
    }

    canSplit(){
        if(!this.menuService.hasFunctionality('SOLFA', 'SPLIH')) return false;

        let hitos = this.getHitosSelected();
        let isValid = hitos.length == 1;
        
        if(!isValid) return false;

        var hito = hitos[0];

        if(hito.status == "Cerrado") return true;

        if(hito.billed) isValid = false;

        return isValid;
    }

    canClose(){
        let hitos = this.getHitosSelected();
        let isValid = hitos.length == 1;
        
        if(!isValid) return false;

        var hito = hitos[0];

        if(hito.status == "Cerrado") {
            return false;
        }

        if(!hito.billed){
            isValid = true;
        }
        else{
            isValid = false;
        }

        return isValid;
    }

    closeHito(){
        var hito = this.getHitosSelected()[0];
    
        this.service.closeHito(hito.id).subscribe(data => {
            if(data.messages) this.messageService.showMessages(data.messages);
            hito.status = "Cerrado";
            hito.statusCode = this.config.crmCloseStatusCode;
            this.closeHitoModal.hide();
        },
        err => {
            this.closeHitoModal.hide();
            this.errorHandlerService.handleErrors(err);
        });
    }

    canCreateCreditNote():boolean {
        if(!this.canCreateSolfac()) return false;

        if(!this.isValidCreditNote()) return false;

        return true;
    }

    canCreateDebitNote():boolean {
        if(!this.canCreateSolfac()) return false;

        if(!this.isValidDebitNote()) return false;

        return true;
    }

    isValidCreditNote():boolean {
        if(this.solfacs.length == 0) return false;
        var hitos = this.getHitosSelected();
        if(hitos.length != 1) return false;
        var hito = hitos[0];
        if(hito.solfacId == 0) return false;
        if(hito.status != this.billedHitoStatus 
            && hito.status != this.paidHitoStatus) return false;
        return true;
    }

    isValidDebitNote():boolean {
        var hitos = this.getHitosSelected();
        if(hitos.length != 1) return false;
        var hito = hitos[0];
        if(hito.status != this.billedHitoStatus 
            && hito.status != this.paidHitoStatus
            && hito.status != this.toBeBilledHitoStatus) return false;
        return true;
    }

    split(){
        var hito = this.getHitosSelected()[0];

        hito.projectId = this.projectId;
        hito.managerId = this.project.managerId;
        hito.opportunityId = this.project.opportunityId;
        hito.currencyId = this.project.currencyId;

        this.splitHito.openModal(hito);
    }

    createSolfac() {
        sessionStorage.removeItem(this.documentTypeKey);
        
        this.generateSolfac();
    }

    createCreditNote() {
        var hito = this.getHitosSelected()[0];
        
        hito.projectId = this.projectId;
        hito.managerId = this.project.managerId;
        hito.opportunityId = this.project.opportunityId;
        hito.currencyId = this.project.currencyId;

        sessionStorage.setItem(this.documentTypeKey, "creditNote");

        this.generateSolfac();
    }

    createDebitNote() {
        var hito = this.getHitosSelected()[0];
        
        hito.projectId = this.projectId;
        hito.managerId = this.project.managerId;
        hito.opportunityId = this.project.opportunityId;
        hito.currencyId = this.project.currencyId;

        sessionStorage.setItem(this.documentTypeKey, "debitNote");

        this.generateSolfac();
    }
}
