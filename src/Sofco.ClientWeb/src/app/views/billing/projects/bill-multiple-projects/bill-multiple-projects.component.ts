import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs";
import { ProjectService } from "app/services/billing/project.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { MessageService } from 'app/services/common/message.service';

@Component({
  selector: 'bill-multiple-projects',
  templateUrl: './bill-multiple-projects.component.html'
})
export class BillMultipleProjectsComponent implements OnInit, OnDestroy {

    projects: any[];
    getAllSubscrip: Subscription;
    paramsSubscrip: Subscription;
    customerId: string;
    serviceId: string;
    serviceName: string;
    customerName: string;

    hitosSelected: any = {};
 
    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private messageService: MessageService,
        private service: ProjectService,
        private errorHandlerService: ErrorHandlerService) { }

    ngOnInit() {
      this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
          this.customerId = params['customerId'];
          this.serviceId = params['serviceId'];
          this.customerName = sessionStorage.getItem('customerName');
          this.serviceName = sessionStorage.getItem('serviceName');

          var projects = JSON.parse(sessionStorage.getItem('projectsToBillMultiple'));

          if(projects && projects.length > 0){
            this.projects = projects;
          }
          else{
            this.getAll();
          }
      });
    }

    ngOnDestroy(){
      if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
      if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
    }

    getAll(){
      this.messageService.showLoading();

      this.getAllSubscrip = this.service.getAll(this.serviceId).subscribe(d => {
        this.messageService.closeLoading();
        this.projects = d;
      },
      err => {
        this.messageService.closeLoading();
        this.errorHandlerService.handleErrors(err)
      });
    }

    goToServices(){
      this.router.navigate([`/billing/customers/${this.customerId}/services`]);
    }

    goBack(){
      this.router.navigate([`/billing/customers/${this.customerId}/services/${this.serviceId}/projects`]);
    }

    selectHito(event){
      this.hitosSelected[event.id] = event.hitos;
    }

    canCreate(){
      var countSelected = 0;
      var currencies = [];

      for(let i = 0; i < this.projects.length; i++){

        if(this.hitosSelected['hito'+i]){
          if(this.hitosSelected['hito'+i].length > 0){
            countSelected++;
          }
        }
      }

      if(countSelected > 1){

        for(let i = 0; i < this.projects.length; i++){

          this.hitosSelected['hito'+i].forEach(hito => {
            if(!currencies.includes(hito.moneyId)){
              currencies.push(hito.moneyId);
            }
          });
        }
        
        if(currencies.length > 1){
          return false;
        }
        else{
          return true;
        }
      } 

      return false;
    }

    createSolfac(){
      var json = { 
        currency: "",
        remito: false,
        hitos: [],
        names: "",
        ids: "",
      }

      var projectsSelected = {
        names: [],
        ids: [],
      };

      for(let i = 0; i < this.projects.length; i++){

        if(this.hitosSelected['hito'+i]) {

          projectsSelected.names.push(this.projects[i].name);
          projectsSelected.ids.push(this.projects[i].crmId);

          if(this.projects[i].remito){
            json.remito = this.projects[i].remito;
          }

          this.hitosSelected['hito'+i].forEach(hito => {
            json.currency = hito.money;

            json.hitos.push({
              name: hito.name,
              projectId: hito.projectId,
              id: hito.id,
              money: hito.money,
              ammount: hito.ammount,
              month: hito.month,
              opportunityId: this.projects[i].opportunityId,
              managerId: this.projects[i].managerId,
            });
          });
        }
      }

      json.names = projectsSelected.names.join(';');
      json.ids = projectsSelected.ids.join(';');

      sessionStorage.setItem('multipleProjects', JSON.stringify(json));

      this.router.navigate(["/billing/solfac"]);
    }
}
