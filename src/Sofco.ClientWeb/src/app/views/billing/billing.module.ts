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
import { NgDatepickerModule } from 'ng2-datepicker';
import { StatusDeleteComponent } from 'app/views/billing/solfac/workflow/delete/status-delete.component';
import { StatusCashComponent } from 'app/views/billing/solfac/workflow/cash/status-cash.component';
import { StatusBillComponent } from 'app/views/billing/solfac/workflow/bill/status-bill.component';
import { StatusSendToCdgComponent } from 'app/views/billing/solfac/workflow/sendToCdg/status-sendToCdg.component';
import { StatusSendToDafComponent } from 'app/views/billing/solfac/workflow/sendToDaf/status-sendToDaf.component';
import { StatusRejectComponent } from 'app/views/billing/solfac/workflow/reject/status-reject.component';
import { CloneInvoiceComponent } from 'app/views/billing/invoice/workflow/clone/clone.component';
import { InvoiceHistoryComponent } from 'app/views/billing/invoice/history/invoice-history.component';
import { StatusApproveComponent } from 'app/views/billing/invoice/workflow/approve/status-approve.component';
import { InvoiceStatusSendToDafComponent } from 'app/views/billing/invoice/workflow/sendToDaf/status-sendToDaf.component';
import { InvoiceStatusRejectComponent } from 'app/views/billing/invoice/workflow/reject/invoice-status-reject.component';
import { InvoiceStatusAnnulmentComponent } from 'app/views/billing/invoice/workflow/annulment/status-annulment.component';
import { UpdateSolfacBillComponent } from 'app/views/billing/solfac/workflow/update-bill/update-solfac-bill.component';
import { UpdateSolfacCashComponent } from 'app/views/billing/solfac/workflow/update-cash/update-solfac-cash.component';
import { WidgetModule } from 'app/components/widget/widget.module';
import { Select2Module } from 'app/components/select2/select2';
import { SplitHitoComponent } from 'app/views/billing/hitos/split-hito.component';
import { SolfacPdfViewerComponent } from 'app/views/billing/solfac/pdf-viewer/solfac-pdf-viewer.component';
import { LayoutsModule } from 'app/components/common/layouts/layouts.module';
import { StatusRejectDafComponent } from 'app/views/billing/solfac/workflow/reject-by-daf/status-reject-daf.component';
import { ProjectRightBarComponent } from 'app/views/billing/projects/project-rightbar/project-rightbar.component';
import { BillMultipleProjectsComponent } from 'app/views/billing/projects/bill-multiple-projects/bill-multiple-projects.component';
import { HitosByProjectComponent } from 'app/views/billing/projects/bill-multiple-projects/hitos-by-project/hitos-by-project.component';
import { SolfacAccountControlComponent } from 'app/views/billing/solfac/solfac-account-control/solfac-account-control.component';
import { NewPurchaseOrderComponent } from 'app/views/billing/purchaseOrder/add/add-purchaseOrder.component';
import { PurchaseOrderFormComponent } from 'app/views/billing/purchaseOrder/form/purchaseOrder-form.component';
import { PurchaseOrderService } from 'app/services/billing/purchaseOrder.service';
import { EditPurchaseOrderComponent } from 'app/views/billing/purchaseOrder/edit/edit-purchaseOrder.component';
import { PurchaseOrderSearchComponent } from 'app/views/billing/purchaseOrder/search/search-purchaseOrder.component';
import { PurchaseOrdersByServiceComponent } from 'app/views/billing/projects/purchaseOrders/purchaseOrders-service.component';
import { SolfacDelegateComponent } from 'app/views/billing/solfac/solfac-delegate/solfac-delegate.component'

@NgModule({
  declarations: [CustomersComponent, ServicesComponent, ProjectsComponent, SolfacComponent, SolfacSearchComponent, ProjectDetailComponent, 
                 SolfacDetailComponent, InvoiceComponent, InvoiceDetailComponent, SolfacHistoryComponent, SolfacEditComponent, InvoiceSearchComponent,
                 SolfacAttachmentsComponent, StatusDeleteComponent, StatusCashComponent, StatusBillComponent, StatusSendToCdgComponent, 
                 StatusSendToDafComponent, StatusRejectComponent, CloneInvoiceComponent, InvoiceHistoryComponent, StatusApproveComponent,
                 InvoiceStatusSendToDafComponent, InvoiceStatusRejectComponent, InvoiceStatusAnnulmentComponent, UpdateSolfacBillComponent,
                 UpdateSolfacCashComponent, SplitHitoComponent, SolfacPdfViewerComponent, ProjectRightBarComponent, StatusRejectDafComponent, 
                 BillMultipleProjectsComponent, HitosByProjectComponent, SolfacAccountControlComponent, NewPurchaseOrderComponent, PurchaseOrderFormComponent, 
                 EditPurchaseOrderComponent, PurchaseOrderSearchComponent, PurchaseOrdersByServiceComponent, SolfacDelegateComponent],

  imports     : [CommonModule, Ng2DatatablesModule, NgDatepickerModule, RouterModule, FormsModule, ICheckModule, Ng2ModalModule,
                 TranslateModule, SpinnerModule, FileUploadModule, WidgetModule, Select2Module, LayoutsModule],

  providers   : [CustomerService, ServiceService, ProjectService, SolfacService, InvoiceService, PurchaseOrderService],
  
  exports     : []
})

export class BillingModule {} 