import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Ng2DatatablesModule } from 'app/components/datatables/ng2-datatables.module';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ICheckModule } from "app/components/icheck/icheck.module";
import { Ng2ModalModule } from "app/components/modal/ng2modal.module";
import { TranslateModule } from "@ngx-translate/core";

import { CustomerService } from "app/services/billing/customer.service";
import { ProjectService } from 'app/services/billing/project.service';
import { ServiceService } from 'app/services/billing/service.service';
import { SolfacService } from 'app/services/billing/solfac.service';

import { CustomersComponent } from './customers/customers.component';
import { ServicesComponent } from './services/services.component';
import { ProjectsComponent } from './projects/project-list/projects.component';
import { SolfacComponent } from "./solfac/new/solfac.component";
import { SolfacSearchComponent } from "app/views/billing/solfacSearch/solfacSearch.component";
import { ProjectDetailComponent } from "app/views/billing/projects/project-detail/project-detail.component";
import { SolfacDetailComponent } from "app/views/billing/solfac/detail/solfac-detail.component";

@NgModule({
  declarations: [CustomersComponent, ServicesComponent, ProjectsComponent, SolfacComponent, SolfacSearchComponent, ProjectDetailComponent, SolfacDetailComponent],
  imports     : [CommonModule, Ng2DatatablesModule, RouterModule, FormsModule, ICheckModule, Ng2ModalModule, TranslateModule],
  providers   : [CustomerService, ServiceService, ProjectService, SolfacService],
  exports     : []
})

export class BillingModule {}