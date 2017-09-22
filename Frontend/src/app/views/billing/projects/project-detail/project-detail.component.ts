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

    @ViewChild('hito') hito;

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

            this.getProject(params['projectId']);
            this.getSolfacs(this.projectId);
            this.getHitos(this.projectId);
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

    getHitos(projectId){
        this.getHitosSubscrip = this.service.getHitos(projectId).subscribe(d => {
            this.hitos = d;

            this.datatableService.init('#hitoTable');
            this.datatableService.adjustColumns();
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    getInvoices(projectId){
        this.getInvoicesSubscrip = this.service.getInvoices(projectId).subscribe(d => {
            this.invoices = d;

            this.datatableService.init('#invoiceTable');
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    getSolfacs(projectId){
        this.getSolfacSubscrip = this.service.getSolfacs(projectId).subscribe(d => {
            this.solfacs = d;

            this.datatableService.init('#solfacTable');
            this.datatableService.adjustColumns();
        },
        err => this.errorHandlerService.handleErrors(err));
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
}
