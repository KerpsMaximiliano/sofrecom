import { MessageService } from 'app/services/message.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { ProjectService } from "app/services/billing/project.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html'
})
export class ProjectsComponent implements OnInit, OnDestroy {

    projects: any[];
    getAllSubscrip: Subscription;
    paramsSubscrip: Subscription;
    customerId: number;
    serviceId: number;
    serviceName: string;
    customerName: string;

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private service: ProjectService,
        private messageService: MessageService,
        private errorHandlerService: ErrorHandlerService) { }

    ngOnInit() {
      this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
        this.customerId = params['customerId'];
        this.serviceId = params['serviceId'];
        this.customerName = JSON.parse(sessionStorage.getItem('customer')).nombre;
        this.serviceName = sessionStorage.getItem('serviceName');
        this.getAll();
      });
    }

    ngOnDestroy(){
      if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
      if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
    }

    getAll(){
      this.getAllSubscrip = this.service.getAll(this.serviceId).subscribe(d => {
        this.projects = d;
      },
      err => this.errorHandlerService.handleErrors(err));
    }

    goToServices(){
      this.router.navigate([`/billing/customers/${this.customerId}/services`]);
    }

    goToProjectDetail(project){
      sessionStorage.setItem("customerId", this.customerId.toString());
      sessionStorage.setItem("serviceId", this.serviceId.toString());
      sessionStorage.setItem("projectDetail", JSON.stringify(project));
      this.router.navigate([`/billing/project/${project.id}`]);
    }
}
