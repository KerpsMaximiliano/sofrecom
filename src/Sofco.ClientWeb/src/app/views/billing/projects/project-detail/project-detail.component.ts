import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { ProjectService } from "app/services/billing/project.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { MenuService } from "app/services/admin/menu.service";
import { DataTableService } from "app/services/common/datatable.service";
import { SolfacStatus } from 'app/models/enums/solfacStatus';

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

    @ViewChild('hito') hito;
    @ViewChild('splitHito') splitHito;

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private service: ProjectService,
        private datatableService: DataTableService,
        public menuService: MenuService,
        private errorHandlerService: ErrorHandlerService) {}

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

            this.calculateIncomes();
            //this.datatableService.adjustColumns();
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
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    calculateIncomes() {
        this.hitos.forEach((item, index) => {
            if(item.status == "Facturado") this.incomesBilled += item.ammount;
            if(item.status == "Pagado") this.incomesCashed += item.ammount;
            if(item.status != "Facturado" && item.status != "Pagado") this.incomesPending += item.ammount;
        });
    }

    generateSolfac() {
        var hitos = this.getHitosSelected();
        sessionStorage.setItem("hitosSelected", JSON.stringify(hitos));
        this.router.navigate(["/billing/solfac"]);
    }

    generateSolfacVisible(){
        var hitos = this.getHitosSelected();

        if(hitos.length > 0){
            return true;
        } 

        return false;
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
        return this.menuService.hasFunctionality('REM', 'QUERY');
    }

    canSeeSolfacs(){
        return this.menuService.hasFunctionality('SOLFA', 'QUERY');
    }

    canSplit(){
        var hitos = this.getHitosSelected();

        if(hitos.length == 1){
            return true;
        } 

        return false;
    }

    split(){
        var hito = this.getHitosSelected()[0];

        hito.projectId = this.projectId;
        hito.managerId = this.project.managerId;
        hito.opportunityId = this.project.opportunityId;

        this.splitHito.openModal(hito);
    }
}
