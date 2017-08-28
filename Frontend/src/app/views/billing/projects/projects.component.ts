import { MessageService } from 'app/services/message.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { ProjectService } from "app/services/billing/project.service";

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
        private messageService: MessageService) { }

    ngOnInit() {
      this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
        this.customerId = params['customerId'];
        this.serviceId = params['serviceId'];
        this.customerName = params['customerName'];
        this.serviceName = this.activatedRoute.snapshot.params['serviceName'];
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
      err => {
        console.log(err);
      });
    }

    goToServices(){
      this.router.navigate([`/billing/customers/${this.customerId}/services`, {customerName: this.customerName}]);
    }
}
