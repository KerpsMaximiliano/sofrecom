import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs";
import { ProjectService } from "../../../../services/billing/project.service";
import { MenuService } from "../../../../services/admin/menu.service";
import { DataTableService } from "../../../../services/common/datatable.service";
import { SolfacStatus } from '../../../../models/enums/solfacStatus';
import { MessageService } from '../../../../services/common/message.service';
import { Ng2ModalConfig } from '../../../../components/modal/ng2modal-config';
import { Configuration } from '../../../../services/common/configuration';
import { ServiceService } from '../../../../services/billing/service.service';
import { NewHito } from '../../../../models/billing/solfac/newHito';
import { InvoiceStatus } from '../../../../models/enums/invoiceStatus';
import { InvoiceService } from '../../../../services/billing/invoice.service';

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
    public service: any = { analytic: "", manager: "" }

    public incomes: any[] = new Array({ currency: 1, symbol: "$", value: 0 });
    public incomesPendingArray: any[] = new Array({ currency: 1, symbol: "$", value: 0 }, { currency: 2, symbol: "U$D", value: 0 }, { currency: 2, symbol: "€", value: 0 });
    public incomesBilledArray: any[] = new Array({ currency: 1, symbol: "$", value: 0 }, { currency: 2, symbol: "U$D", value: 0 }, { currency: 2, symbol: "€", value: 0 });
    public incomesCashedArray: any[] = new Array({ currency: 1, symbol: "$", value: 0 }, { currency: 2, symbol: "U$D", value: 0 }, { currency: 2, symbol: "€", value: 0 });

    incomesBilled: number = 0;
    incomesCashed: number = 0;
    incomesPending: number = 0;

    public annulmentComments: string;

    billedHitoStatus:string = "Facturado";
    pendingHitoStatus:string = "Pendiente";
    projectedHitoStatus:string = "Proyectado";
    paidHitoStatus:string = "Pagado";
    toBeBilledHitoStatus:string = "ToBeBilled";
    documentTypeKey:string = "documentTypeName";

    @ViewChild('hito') hito;
    @ViewChild('splitHito') splitHito;
    @ViewChild('newHito') newHito;
    @ViewChild('ocs') ocs;

    @ViewChild('closeHitoModal') closeHitoModal;
    public closeHitoModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "closeHitoModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    @ViewChild('invoiceAnnulmentModal') invoiceAnnulmentModal;
    public invoiceAnnulmentModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "invoiceAnnulmentModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private projectService: ProjectService,
        private invoiceService: InvoiceService,
        private datatableService: DataTableService,
        private messageService: MessageService,
        public menuService: MenuService,
        private serviceService: ServiceService,
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

            this.setProject(params['projectId']);
            this.getService();
            this.getSolfacs(this.projectId);
            this.getHitos();
    
            this.ocs.getAll(this.projectId);
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

    getService(){
        var service = JSON.parse(sessionStorage.getItem('serviceDetail'));
 
        if(service){
          this.service.analytic = service.analytic;
          this.service.manager = service.manager;
          this.service.managerId = service.managerId;
          this.service.serviceType = service.serviceType;
          this.service.solutionType = service.solutionType;
          this.service.technologyType = service.technologyType;
        }
        else{
          this.serviceService.getById(sessionStorage.getItem("customerId"), sessionStorage.getItem("serviceId")).subscribe(response => {
            this.service.analytic = response.data.analytic;
            this.service.manager = response.data.manager;
            this.service.serviceType = response.data.serviceType;
            this.service.solutionType = response.data.solutionType;
            this.service.technologyType = response.data.technologyType;
          });
        }
    }

    setProject(projectId){
        var projectData = sessionStorage.getItem("projectDetail");

        if(projectData){
            var project = JSON.parse(projectData);

            if(projectId != project.id){
                this.getProject(projectId);
            }
            else{
                this.project = project;
                this.loading = false;
            }
        }
        else{
            this.getProject(projectId);
        }
    }

    getProject(projectId){
        this.getProjectSubscrip = this.projectService.getById(projectId).subscribe(data => {
            this.project = data;
            sessionStorage.setItem("projectDetail", JSON.stringify(data));
            this.loading = false;

            if(this.canSeeInvoices()){
                this.getInvoices(this.projectId);
            }
        },
        err => {
            this.loading = false;
        });
    }
 
    resolveHitoLabel(hito){
        if(hito.status == this.pendingHitoStatus && hito.solfacId && hito.solfacId > 0){
            return 'label-pending-related';
        }
        
        return `label-${hito.status}`
    }

    resolveHitoCheckClass(hito){
        if(hito.status == this.pendingHitoStatus && hito.solfacId && hito.solfacId > 0){
            return 'pending-related';
        }
        
        return `${hito.status}`
    }

    getHitos(){
        this.getHitosSubscrip = this.projectService.getHitos(this.projectId).subscribe(d => {
            this.hitos = d.map(item => {
                item.projectId = this.projectId;
                return item;
            });

            this.initHitosGrid();
        });
    }

    initHitosGrid(){ 
        var columns = [1, 2, 3, 4, 5];
        var title = `Hitos`;

        var params = {
          selector: '#hitoTable',
          columns: columns,
          title: title,
          withExport: true,
          columnDefs: [ {"aTargets": [3], "sType": "date-uk"} ]
        }
  
        this.datatableService.destroy(params.selector);
        this.datatableService.initialize(params);
    }

    getInvoices(projectId){
        this.getInvoicesSubscrip = this.projectService.getInvoices(projectId).subscribe(d => {
            this.invoices = d;

            this.initInvoicesGrid();
        });
    }

    initInvoicesGrid(){
        var columns = [1, 2, 3, 4];
        var title = `Remitos`;

        var params = {
          selector: '#invoiceGrid',
          columns: columns,
          title: title, 
          withExport: true,
          order: [[ 3, "desc" ]],
          columnDefs: [ {"aTargets": [3], "sType": "date-uk"} ]
        }
  
        this.datatableService.destroy(params.selector);
        this.datatableService.initialize(params);
    }

    getSolfacs(projectId){
        this.getSolfacSubscrip = this.projectService.getSolfacs(projectId).subscribe(d => {
            this.solfacs = d;

            this.initSolfacGrid();
        });
    }

    initSolfacGrid(){
        var columns = [0, 1, 2, 3, 4, 5];
        var title = `Solicitudes`;

        var params = {
          selector: '#solfacTable',
          columns: columns,
          title: title,
          withExport: true,
          columnDefs: [ {"aTargets": [2], "sType": "date-uk"} ]
        }
  
        this.datatableService.initialize(params);
    }


    generateSolfac() { 
        var hitos = this.getHitosSelected();
        sessionStorage.setItem("hitosSelected", JSON.stringify(hitos));
        sessionStorage.removeItem('multipleProjects');
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

    getCurrencyId(currency){
        switch(currency){
          case "Peso": { return 1; }
          case "Dolar": { return 2; }
          case "Euro": { return 3; }
        }
    }
 
    goToSolfacDetail(solfac){
        if(this.menuService.hasFunctionality('SOLFA', 'ALTA') && 
          (solfac.statusName == SolfacStatus[SolfacStatus.SendPending] || solfac.statusName == SolfacStatus[SolfacStatus.ManagementControlRejected] ||
           solfac.statusName == SolfacStatus[SolfacStatus.RejectedByDaf]))
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
        if(this.project && this.project.remito){
            return this.menuService.hasFunctionality('REM', 'QUERY') && this.project.remito == true;
        }
        
        return false;
    }

    canSeeSolfacs(){
        return this.menuService.hasFunctionality('SOLFA', 'QUERY');
    }
 
    generateSolfacVisible(){
        var currencies = [];
        var hitos = this.getHitosSelected();

        let isValid = hitos.length > 0;
    
        hitos.forEach(item => {
            if(!currencies.includes(item.moneyId)){
                currencies.push(item.moneyId);
            }

            if(item.billed || item.status == "Cerrado"){
                isValid = false;
            }
        });

        if(currencies.length > 1){
            isValid = false;
        }

        return isValid;
    }

    canCreateHito(){
        if(!this.menuService.hasFunctionality('SOLFA', 'NEW-HITO')) return false;

        let hitos = this.getHitosSelected();
        return hitos.length == 0;
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
    
        this.projectService.closeHito(hito.id).subscribe(data => {
            hito.status = "Cerrado";
            hito.statusCode = this.config.crmCloseStatusCode;
            this.closeHitoModal.hide();
        },
        err => {
            this.closeHitoModal.hide();
        });
    }

    canCreateCreditNote():boolean {
        if(!this.menuService.hasFunctionality('SOLFA', 'CREDIT-DEBIT-NOTE')) return false;

        if(!this.canCreateSolfac()) return false;

        if(!this.isValidCreditNote()) return false;

        return true;
    }

    canCreateDebitNote():boolean {
        if(!this.menuService.hasFunctionality('SOLFA', 'CREDIT-DEBIT-NOTE')) return false;
        
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
        var hito = this.translateHito(this.getHitosSelected()[0]);

        this.splitHito.openModal(hito);
    }

    createHito(){
        var hito = new NewHito();
        hito = this.translateHito(hito);

        this.newHito.openModal(hito);
    }

    createSolfac() {
        sessionStorage.removeItem(this.documentTypeKey);
        
        this.generateSolfac();
    }

    createCreditNote() {
        var hito = this.translateHito(this.getHitosSelected()[0]);
        
        sessionStorage.setItem(this.documentTypeKey, "creditNote");

        this.generateSolfac();
    }

    createDebitNote() {
        var hito = this.translateHito(this.getHitosSelected()[0]);
        
        sessionStorage.setItem(this.documentTypeKey, "debitNote");

        this.generateSolfac();
    }
 
    translateHito(hito:any) {
        hito.projectId = this.projectId;
        hito.managerId = this.service.managerId;
        hito.opportunityId = this.project.opportunityId;
        return hito;
    }

    canAskForAnnulment(){
        var invoicesSelectedCount = 0;

        var invoices = this.invoices.filter(invoice => {

            if(invoice.selected && invoice.selected == true){
              invoicesSelectedCount++;

              if(invoice.invoiceStatus == InvoiceStatus[InvoiceStatus.Sent] || invoice.invoiceStatus == InvoiceStatus[InvoiceStatus.Approved]){
                    return invoice;
                }
            }

            return null;
        });

        if(invoices.length == 0) return false;
        if(invoices.length != invoicesSelectedCount) return false;

        return true;
    }

    askForAnnulment(){
        if(!this.annulmentComments || this.annulmentComments == ""){
            this.messageService.showError("billing.solfac.rejectCommentRequired");
            this.invoiceAnnulmentModal.resetButtons();
            return;
        }

        var invoicesIds = this.invoices.filter(x => x.selected).map(x => x.id);

        var json = {
            invoices: invoicesIds,
            comments: this.annulmentComments
        }

        this.invoiceService.askForAnnulment(json).subscribe(data => {
            this.invoiceAnnulmentModal.hide();

            this.invoices.forEach(item => {
                if(invoicesIds.includes(item.id)){
                    item.invoiceStatus = InvoiceStatus[InvoiceStatus.RequestAnnulment];
                }
            });
        },
        err => {
            this.invoiceAnnulmentModal.hide();
        });
    }
}
