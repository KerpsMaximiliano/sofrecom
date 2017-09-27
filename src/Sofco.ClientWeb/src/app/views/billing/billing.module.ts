import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Ng2DatatablesModule } from 'app/components/datatables/ng2-datatables.module';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ICheckModule } from "app/components/icheck/icheck.module";
import { Ng2ModalModule } from "app/components/modal/ng2modal.module";
import { TranslateModule } from "@ngx-translate/core";
import { FileUploadModule } from 'ng2-file-upload';

import { CustomerService } from "app/services/billing/customer.service";
import { ProjectService } from 'app/services/billing/project.service';
import { ServiceService } from 'app/services/billing/service.service';
import { SolfacService } from 'app/services/billing/solfac.service';

import { CustomersComponent } from './customers/customers.component';
import { ServicesComponent } from './services/services.component';
import { ProjectsComponent } from './projects/project-list/projects.component';
import { SolfacComponent } from "./solfac/new/solfac.component";
import { ProjectDetailComponent } from "app/views/billing/projects/project-detail/project-detail.component";
import { SolfacDetailComponent } from "app/views/billing/solfac/detail/solfac-detail.component";
import { SpinnerModule } from "app/components/spinner/spinner.module";
import { SolfacSearchComponent } from "app/views/billing/solfac/search/solfac-search.component";
import { InvoiceComponent } from "app/views/billing/invoice/new/invoice.component";
import { InvoiceService } from "app/services/billing/invoice.service";
import { InvoiceDetailComponent } from "app/views/billing/invoice/detail/invoice-detail.component";
import { SolfacHistoryComponent } from 'app/views/billing/solfac/history/solfac-history.component';
import { SolfacEditComponent } from 'app/views/billing/solfac/edit/solfac-edit.component';
import { InvoiceSearchComponent } from 'app/views/billing/invoice/search/invoice-search.component';
import { SolfacAttachmentsComponent } from 'app/views/billing/solfac/attachments/solfac-attachments.component';

@NgModule({
  declarations: [CustomersComponent, ServicesComponent, ProjectsComponent, SolfacComponent, SolfacSearchComponent, ProjectDetailComponent, 
                 SolfacDetailComponent, InvoiceComponent, InvoiceDetailComponent, SolfacHistoryComponent, SolfacEditComponent, InvoiceSearchComponent,
                 SolfacAttachmentsComponent],

  imports     : [CommonModule, Ng2DatatablesModule, RouterModule, FormsModule, ICheckModule, Ng2ModalModule, TranslateModule, SpinnerModule, FileUploadModule],

  providers   : [CustomerService, ServiceService, ProjectService, SolfacService, InvoiceService],
  
  exports     : []
})

export class BillingModule {}