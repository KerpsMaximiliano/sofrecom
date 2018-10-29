import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Ng2DatatablesModule } from '../../components/datatables/ng2-datatables.module';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ICheckModule } from '../../components/icheck/icheck.module';
import { Ng2ModalModule } from '../../components/modal/ng2modal.module';
import { TranslateModule } from '@ngx-translate/core';
import { FileUploadModule } from 'ng2-file-upload';
import { PCheckModule } from '../../components/pcheck/pcheck.module';
import { DateRangePickerModule } from '../../components/date-range-picker/date-range.picker.module';

import { CustomerService } from '../../services/billing/customer.service';
import { ProjectService } from '../../services/billing/project.service';
import { ServiceService } from '../../services/billing/service.service';
import { SolfacService } from '../../services/billing/solfac.service';
import { SolfacDelegateService } from '../../services/billing/solfac-delegate.service';
import { PurchaseOrderApprovalDelegateService } from '../../services/billing/purchase-order-approval-delegate.service';
import { PurchaseOrderActiveDelegateService } from '../../services/billing/purchase-order-active-delegate.service';

import { CustomersComponent } from './customers/customers.component';
import { ServicesComponent } from './services/services.component';
import { ProjectsComponent } from './projects/project-list/projects.component';
import { SolfacComponent } from './solfac/new/solfac.component';
import { ProjectDetailComponent } from './projects/project-detail/project-detail.component';
import { SolfacDetailComponent } from './solfac/detail/solfac-detail.component';
import { SpinnerModule } from '../../components/spinner/spinner.module';
import { SolfacSearchComponent } from './solfac/search/solfac-search.component';
import { InvoiceComponent } from './invoice/new/invoice.component';
import { InvoiceService } from '../../services/billing/invoice.service';
import { InvoiceDetailComponent } from './invoice/detail/invoice-detail.component';
import { SolfacHistoryComponent } from './solfac/history/solfac-history.component';
import { SolfacEditComponent } from './solfac/edit/solfac-edit.component';
import { InvoiceSearchComponent } from './invoice/search/invoice-search.component';
import { SolfacAttachmentsComponent } from './solfac/attachments/solfac-attachments.component';
import { NgDatepickerModule } from 'ng2-datepicker';
import { StatusDeleteComponent } from './solfac/workflow/delete/status-delete.component';
import { StatusCashComponent } from './solfac/workflow/cash/status-cash.component';
import { StatusBillComponent } from './solfac/workflow/bill/status-bill.component';
import { StatusSendToCdgComponent } from './solfac/workflow/sendToCdg/status-sendToCdg.component';
import { StatusSendToDafComponent } from './solfac/workflow/sendToDaf/status-sendToDaf.component';
import { StatusRejectComponent } from './solfac/workflow/reject/status-reject.component';
import { CloneInvoiceComponent } from './invoice/workflow/clone/clone.component';
import { InvoiceHistoryComponent } from './invoice/history/invoice-history.component';
import { StatusApproveComponent } from './invoice/workflow/approve/status-approve.component';
import { InvoiceStatusSendToDafComponent } from './invoice/workflow/sendToDaf/status-sendToDaf.component';
import { InvoiceStatusRejectComponent } from './invoice/workflow/reject/invoice-status-reject.component';
import { InvoiceStatusAnnulmentComponent } from './invoice/workflow/annulment/status-annulment.component';
import { UpdateSolfacBillComponent } from './solfac/workflow/update-bill/update-solfac-bill.component';
import { UpdateSolfacCashComponent } from './solfac/workflow/update-cash/update-solfac-cash.component';
import { WidgetModule } from '../../components/widget/widget.module';
import { Select2Module } from '../../components/select2/select2';
import { SplitHitoComponent } from './hitos/split-hito.component';
import { SolfacPdfViewerComponent } from './solfac/pdf-viewer/solfac-pdf-viewer.component';
import { LayoutsModule } from '../../components/common/layouts/layouts.module';
import { StatusRejectDafComponent } from './solfac/workflow/reject-by-daf/status-reject-daf.component';
import { BillMultipleProjectsComponent } from './projects/bill-multiple-projects/bill-multiple-projects.component';
import { HitosByProjectComponent } from './projects/bill-multiple-projects/hitos-by-project/hitos-by-project.component';
import { SolfacAccountControlComponent } from './solfac/solfac-account-control/solfac-account-control.component';
import { NewPurchaseOrderComponent } from './purchaseOrder/add/add-purchaseOrder.component';
import { PurchaseOrderFormComponent } from './purchaseOrder/form/purchaseOrder-form.component';
import { PurchaseOrderService } from '../../services/billing/purchaseOrder.service';
import { EditPurchaseOrderComponent } from './purchaseOrder/edit/edit-purchaseOrder.component';
import { PurchaseOrderSearchComponent } from './purchaseOrder/search/search-purchaseOrder.component';
import { PurchaseOrdersByServiceComponent } from './projects/purchaseOrders/purchaseOrders-service.component';
import { SolfacDelegateComponent } from './solfac/solfac-delegate/solfac-delegate.component'
import { CertificateSearchComponent } from './certificates/search/search-certificate.component';
import { CertificateFormComponent } from './certificates/form/certificate-form.component';
import { EditCertificateComponent } from './certificates/edit/edit-certificate.component';
import { NewCertificateComponent } from './certificates/add/add-certificate.component';
import { CertificatesService } from '../../services/billing/certificates.service';
import { SolfacDelegateEditComponent } from './solfac/solfac-delegate/edit/solfac-delegate-edit.component'
import { PdfViewerModule } from '../../components/pdf-viewer/pdf-viewer.module';
import { DatePickerModule } from '../../components/date-picker/date-picker.module';
import { NewHitoComponent } from './hitos/new/new-hito.component';
import { ProjectPurchaseOrdersComponent } from './projects/project-detail/purchase-order-detail/purchase-order-detail.component';
import { AmountFormatPipe } from '../../pipes/amount-format.pipe';
import { BillingRouter } from './billing.router';
import { ResourceByServiceComponent } from '../allocation-management/resources/by-service/resource-by-service.component';
import { AnalyticService } from '../../services/allocation-management/analytic.service';
import { EmployeeService } from '../../services/allocation-management/employee.service';
import { UtilsService } from '../../services/common/utils.service';
import { AllocationService } from '../../services/allocation-management/allocation.service';
import { CategoryService } from '../../services/admin/category.service';
import { PurchaseOrderAdjustmentComponent } from './purchaseOrder/adjustment/oc-adjustment.component';
import { PurchaseOrderApprovalDelegateComponent } from './purchaseOrder/approval-delegate/purchase-order-approval-delegate.component';
import { OcStatusDraftComponent } from './purchaseOrder/workflow/draft/oc-draft.componet';
import { OcStatusComplianceComponent } from './purchaseOrder/workflow/compliance/oc-compliance.component';
import { OcStatusComercialComponent } from './purchaseOrder/workflow/comercial/oc-comercial.component';
import { OcStatusOperativeComponent } from './purchaseOrder/workflow/operative/oc-operative.component';
import { OcStatusRejectComponent } from './purchaseOrder/workflow/reject/oc-reject.component';
import { OcStatusDafComponent } from './purchaseOrder/workflow/daf/oc-daf.component';
import { PurchaseOrderHistoryComponent } from './purchaseOrder/history/oc-history.component';
import { PurchaseOrderApprovalDelegateEditComponent } from './purchaseOrder/approval-delegate/edit/purchase-order-approval-delegate-edit.component';
import { OcStatusCloseComponent } from './purchaseOrder/workflow/close/oc-close.component';
import { PurchaseOrderPendingsComponent } from './purchaseOrder/pendings/oc-pendings.component';
import { OcStatusDeleteComponent } from './purchaseOrder/workflow/delete/oc-delete.component';
import { PurchaseOrderActiveDelegateComponent } from './purchaseOrder/active-view/active-delegate/purchase-order-active-delegate.component';
import { PurchaseOrderActiveDelegateEditComponent } from './purchaseOrder/active-view/active-delegate/edit/purchase-order-active-delegate-edit.component';
import { PurchaseOrderViewComponent } from './purchaseOrder/common/purchase-order-view.component';
import { PurchaseOrderViewFilterComponent } from './purchaseOrder/common/purchase-order-view-filter.component';
import { PurchaseOrderActiveViewComponent } from './purchaseOrder/active-view/purchase-order-active-view.component';
import { ButtonsModule } from '../../components/buttons/buttons.module';
import { DecimalFormatModule } from 'app/components/decimalFormat/decimal-format.directive';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';

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
                 OcStatusCloseComponent, PurchaseOrderPendingsComponent, OcStatusDeleteComponent, PurchaseOrderActiveDelegateComponent, PurchaseOrderActiveDelegateEditComponent,
                 PurchaseOrderViewComponent, PurchaseOrderViewFilterComponent, PurchaseOrderActiveViewComponent],

  imports     : [CommonModule, Ng2DatatablesModule, NgDatepickerModule, RouterModule, FormsModule, ICheckModule, Ng2ModalModule,
                 TranslateModule, SpinnerModule, FileUploadModule, WidgetModule, Select2Module, LayoutsModule, PdfViewerModule, 
                 DatePickerModule, PCheckModule, DateRangePickerModule, BillingRouter, ButtonsModule, DecimalFormatModule],

  providers   : [CustomerService, ServiceService, ProjectService, SolfacService, InvoiceService, PurchaseOrderService, CertificatesService,
                SolfacDelegateService, CategoryService, AnalyticService, EmployeeService, UtilsService, AllocationService, PurchaseOrderApprovalDelegateService, 
                PurchaseOrderActiveDelegateService],

  exports     : []
})

export class BillingModule {}
