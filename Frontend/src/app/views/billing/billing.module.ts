import { ProjectService } from './../../services/project.service';
import { ServiceService } from 'app/services/service.service';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Ng2DatatablesModule } from 'app/components/datatables/ng2-datatables.module';
import { CommonModule } from '@angular/common';
import { CustomersComponent } from './customers/customers.component';
import { NgModule } from '@angular/core';
import { ICheckModule } from "app/components/icheck/icheck.module";
import { Ng2ModalModule } from "app/components/modal/ng2modal.module";
import { TranslateModule } from "@ngx-translate/core";
import { CustomerService } from "app/services/customer.service";
import { ServicesComponent } from './services/services.component';
import { ProjectsComponent } from './projects/projects.component';


@NgModule({
  declarations: [CustomersComponent, ServicesComponent, ProjectsComponent],
  imports     : [CommonModule, Ng2DatatablesModule, RouterModule, FormsModule, ICheckModule, Ng2ModalModule, TranslateModule],
  providers   : [CustomerService, ServiceService, ProjectService],
  exports     : []
})

export class BillingModule {}