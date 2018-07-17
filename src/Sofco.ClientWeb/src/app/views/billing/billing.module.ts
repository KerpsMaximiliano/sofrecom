import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Ng2DatatablesModule } from 'app/components/datatables/ng2-datatables.module';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ICheckModule } from 'app/components/icheck/icheck.module';
import { Ng2ModalModule } from 'app/components/modal/ng2modal.module';
import { TranslateModule } from '@ngx-translate/core';
import { FileUploadModule } from 'ng2-file-upload';
import { PCheckModule } from 'app/components/pcheck/pcheck.module';
import { DateRangePickerModule } from 'app/components/date-range-picker/date-range.picker.module';

import { CustomerService } from 'app/services/billing/customer.service';
import { ProjectService } from 'app/services/billing/project.service';
import { ServiceService } from 'app/services/billing/service.service';
import { SolfacService } from 'app/services/billing/solfac.service';
import { SolfacDelegateService } from 'app/services/billing/solfac-delegate.service';
import { PurchaseOrderDelegateService } from 'app/services/billing/purchase-order-delegate.service';

import { CustomersComponent } from './customers/customers.component';
import { ServicesComponent } from './services/services.component';
import { ProjectsComponent } from './projects/project-list/projects.component';
import { SolfacComponent } from './solfac/new/solfac.component';
import { ProjectDetailComponent } from 'app/views/billing/projects/project-detail/project-detail.component';
import { SolfacDetailComponent } from 'app/views/billing/solfac/detail/solfac-detail.component';
import { SpinnerModule } from 'app/components/spinner/spinner.module';
import { SolfacSearchComponent } from 'app/views/billing/solfac/search/solfac-search.component';
import { InvoiceComponent } from 'app/views/billing/invoice/new/invoice.component';
import { InvoiceService } from 'app/services/billing/invoice.service';
import { InvoiceDetailComponent } from 'app/views/billing/invoice/detail/invoice-detail.component';
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
import { CertificateSearchComponent } from 'app/views/billing/certificates/search/search-certificate.component';
import { CertificateFormComponent } from 'app/views/billing/certificates/form/certificate-form.component';
import { EditCertificateComponent } from 'app/views/billing/certificates/edit/edit-certificate.component';
import { NewCertificateComponent } from 'app/views/billing/certificates/add/add-certificate.component';
import { CertificatesService } from 'app/services/billing/certificates.service';
import { SolfacDelegateEditComponent } from 'app/views/billing/solfac/solfac-delegate/edit/solfac-delegate-edit.component'
import { PdfViewerModule } from 'app/components/pdf-viewer/pdf-viewer.module';
import { DatePickerModule } from 'app/components/date-picker/date-picker.module';
import { NewHitoComponent } from 'app/views/billing/hitos/new/new-hito.component';
import { ProjectPurchaseOrdersComponent } from 'app/views/billing/projects/project-detail/purchase-order-detail/purchase-order-detail.component';
import { AmountFormatPipe } from 'app/pipes/amount-format.pipe';
import { BillingRouter } from 'app/views/billing/billing.router';
import { ResourceByServiceComponent } from 'app/views/allocation-management/resources/by-service/resource-by-service.component';
import { AnalyticService } from '../../services/allocation-management/analytic.service';
import { EmployeeService } from '../../services/allocation-management/employee.service';
import { UtilsService } from '../../services/common/utils.service';
import { AllocationService } from '../../services/allocation-management/allocation.service';
import { CategoryService } from 'app/services/admin/category.service';
import { PurchaseOrderAdjustmentComponent } from 'app/views/billing/purchaseOrder/adjustment/oc-adjustment.component';
import { PurchaseOrderApprovalDelegateComponent } from './purchaseOrder/approval-delegate/purchase-order-approval-delegate.component';
import { OcStatusDraftComponent } from 'app/views/billing/purchaseOrder/workflow/draft/oc-draft.componet';
import { OcStatusComplianceComponent } from 'app/views/billing/purchaseOrder/workflow/compliance/oc-compliance.component';
import { OcStatusComercialComponent } from 'app/views/billing/purchaseOrder/workflow/comercial/oc-comercial.component';
import { OcStatusOperativeComponent } from 'app/views/billing/purchaseOrder/workflow/operative/oc-operative.component';
import { OcStatusRejectComponent } from 'app/views/billing/purchaseOrder/workflow/reject/oc-reject.component';
import { OcStatusDafComponent } from 'app/views/billing/purchaseOrder/workflow/daf/oc-daf.component';
import { PurchaseOrderHistoryComponent } from 'app/views/billing/purchaseOrder/history/oc-history.component';
import { PurchaseOrderApprovalDelegateEditComponent } from './purchaseOrder/approval-delegate/edit/purchase-order-approval-delegate-edit.component';
import { OcStatusCloseComponent } from 'app/views/billing/purchaseOrder/workflow/close/oc-close.component';
import { PurchaseOrderPendingsComponent } from 'app/views/billing/purchaseOrder/pendings/oc-pendings.component';
import { OcStatusDeleteComponent } from 'app/views/billing/purchaseOrder/workflow/delete/oc-delete.component';

@NgModule({
  declarations: [CustomersComponent, ServicesComponent, ProjectsComponent, SolfacComponent, SolfacSearchComponent, ProjectDetailComponent,
                 SolfacDetailComponent, InvoiceComponent, InvoiceDetailComponent, SolfacHistoryComponent, SolfacEditComponent, InvoiceSearchComponent,
                 SolfacAttachmentsComponent, StatusDeleteComponent, StatusCashComponent, StatusBillComponent, StatusSendToCdgComponent, 
                 StatusSendToDafComponent, StatusRejectComponent, CloneInvoiceComponent, InvoiceHistoryComponent, StatusApproveComponent,
                 InvoiceStatusSendToDafComponent, InvoiceStatusRejectComponent, InvoiceStatusAnnulmentComponent, UpdateSolfacBillComponent,
                 UpdateSolfacCashComponent, SplitHitoComponent, SolfacPdfViewerComponent, StatusRejectDafComponent,
                 BillMultipleProjectsComponent, HitosByProjectComponent, SolfacAccountControlComponent, NewPurchaseOrderComponent, PurchaseOrderFormComponent, 
                 EditPurchaseOrderComponent, PurchaseOrderSearchComponent, PurchaseOrdersByServiceComponent, SolfacDelegateComponent,
                 CertificateSearchComponent, CertificateFormComponent, EditCertificateComponent, NewCertificateComponent, SolfacDelegateEditComponent,
                 NewHitoComponent, ProjectPurchaseOrdersComponent, AmountFormatPipe, ResourceByServiceComponent, PurchaseOrderAdjustmentComponent,
                 OcStatusDraftComponent, OcStatusComplianceComponent, OcStatusComercialComponent, OcStatusOperativeComponent, OcStatusRejectComponent,
                 OcStatusDafComponent, PurchaseOrderHistoryComponent, PurchaseOrderApprovalDelegateComponent, PurchaseOrderApprovalDelegateEditComponent,
                 OcStatusCloseComponent, PurchaseOrderPendingsComponent, OcStatusDeleteComponent],

  imports     : [CommonModule, Ng2DatatablesModule, NgDatepickerModule, RouterModule, FormsModule, ICheckModule, Ng2ModalModule,
                 TranslateModule, SpinnerModule, FileUploadModule, WidgetModule, Select2Module, LayoutsModule, PdfViewerModule, 
                 DatePickerModule, PCheckModule, DateRangePickerModule, BillingRouter],

  providers   : [CustomerService, ServiceService, ProjectService, SolfacService, InvoiceService, PurchaseOrderService, CertificatesService,
                SolfacDelegateService, CategoryService, AnalyticService, EmployeeService, UtilsService, AllocationService, PurchaseOrderDelegateService],

  exports     : []
})

export class BillingModule {}
