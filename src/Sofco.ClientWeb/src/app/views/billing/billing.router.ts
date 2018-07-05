import { Routes, RouterModule } from "@angular/router";
import { CustomersComponent } from "app/views/billing/customers/customers.component";
import { AuthGuard } from "app/guards/auth.guard";
import { ResourceByServiceComponent } from "app/views/allocation-management/resources/by-service/resource-by-service.component";
import { PurchaseOrdersByServiceComponent } from "app/views/billing/projects/purchaseOrders/purchaseOrders-service.component";
import { BillMultipleProjectsComponent } from "app/views/billing/projects/bill-multiple-projects/bill-multiple-projects.component";
import { ProjectsComponent } from "app/views/billing/projects/project-list/projects.component";
import { ProjectDetailComponent } from "app/views/billing/projects/project-detail/project-detail.component";
import { SolfacComponent } from "app/views/billing/solfac/new/solfac.component";
import { SolfacDelegateComponent } from "app/views/billing/solfac/solfac-delegate/solfac-delegate.component";
import { SolfacDelegateEditComponent } from "app/views/billing/solfac/solfac-delegate/edit/solfac-delegate-edit.component";
import { SolfacEditComponent } from "app/views/billing/solfac/edit/solfac-edit.component";
import { SolfacSearchComponent } from "app/views/billing/solfac/search/solfac-search.component";
import { SolfacDetailComponent } from "app/views/billing/solfac/detail/solfac-detail.component";
import { PurchaseOrderSearchComponent } from "app/views/billing/purchaseOrder/search/search-purchaseOrder.component";
import { EditPurchaseOrderComponent } from "app/views/billing/purchaseOrder/edit/edit-purchaseOrder.component";
import { NewPurchaseOrderComponent } from "app/views/billing/purchaseOrder/add/add-purchaseOrder.component";
import { CertificateSearchComponent } from "app/views/billing/certificates/search/search-certificate.component";
import { NewCertificateComponent } from "app/views/billing/certificates/add/add-certificate.component";
import { EditCertificateComponent } from "app/views/billing/certificates/edit/edit-certificate.component";
import { InvoiceComponent } from "app/views/billing/invoice/new/invoice.component";
import { InvoiceDetailComponent } from "app/views/billing/invoice/detail/invoice-detail.component";
import { InvoiceSearchComponent } from "app/views/billing/invoice/search/invoice-search.component";
import { ServicesComponent } from "app/views/billing/services/services.component";
import { PurchaseOrderDelegateComponent } from "./purchaseOrder/purchase-order-delegate/purchase-order-delegate.component";
import { PurchaseOrderPendingsComponent } from "app/views/billing/purchaseOrder/pendings/oc-pendings.component";

const BILLING_ROUTER: Routes = [
    { path: 'customers', children:[
        { path: "", component: CustomersComponent, canActivate: [AuthGuard], data: { module: "SOLFA", functionality: "ALTA" } },
        { path: ":customerId/services", children: [
          { path: "", component: ServicesComponent, canActivate: [AuthGuard] },
          { path: ":serviceId/resources", component: ResourceByServiceComponent, canActivate: [AuthGuard] },
          { path: ":serviceId/purchaseOrders", component: PurchaseOrdersByServiceComponent, canActivate: [AuthGuard] },
          { path: ":serviceId/projects", children: [
            { path: "", component: ProjectsComponent, canActivate: [AuthGuard] },
            { path: "billMultiple", component: BillMultipleProjectsComponent, canActivate: [AuthGuard] },
            { path: ":projectId", component: ProjectDetailComponent, canActivate: [AuthGuard] },
          ]}, 
        ]}
      ]},

      { path: "solfac",
        children: [
         { path: "", component: SolfacComponent, canActivate: [AuthGuard], data: { module: "SOLFA", functionality: "ALTA" } },
         { path: "delegate", component: SolfacDelegateComponent, canActivate: [AuthGuard], data: { module: "SOLFA", functionality: "SOLDE" } },
         { path: "delegate/edit", component: SolfacDelegateEditComponent, canActivate: [AuthGuard], data: { module: "SOLFA", functionality: "SOLDE" } },
         { path: ":solfacId/edit", component: SolfacEditComponent, canActivate: [AuthGuard], data: { module: "SOLFA", functionality: "ALTA" } },
         { path: "search", component: SolfacSearchComponent, canActivate: [AuthGuard], data: { module: "SOLFA", functionality: "QUERY" } },
         { path: ":solfacId", component: SolfacDetailComponent, canActivate: [AuthGuard], data: { module: "SOLFA", functionality: "QUERY" } }
      ]},

      { path: "purchaseOrders",
          children: [
          { path: "query", component: PurchaseOrderSearchComponent, canActivate: [AuthGuard], data: { module: "PUROR", functionality: "QUERY" } },
          { path: "new", component: NewPurchaseOrderComponent, canActivate: [AuthGuard], data: { module: "PUROR", functionality: "ALTA" } },
          { path: "delegate", component: PurchaseOrderDelegateComponent, canActivate: [AuthGuard], data: { module: "PUROR", functionality: "PODE" } },
          { path: "pendings", component: PurchaseOrderPendingsComponent, canActivate: [AuthGuard], data: { module: "PUROR", functionality: "PEND" } },
          { path: ":id", component: EditPurchaseOrderComponent, canActivate: [AuthGuard], data: { module: "PUROR", functionality: "VIEW" } },
      ]},

      { path: "certificates",
          children: [
          { path: "", component: CertificateSearchComponent, canActivate: [AuthGuard], data: { module: "CERT", functionality: "QUERY" } },
          { path: "new", component: NewCertificateComponent, canActivate: [AuthGuard], data: { module: "CERT", functionality: "ALTA" } },
          { path: ":id", component: EditCertificateComponent, canActivate: [AuthGuard], data: { module: "CERT", functionality: "ALTA" } },
      ]},

      { path: "invoice",
        children: [
        { path: "new/project/:projectId", component: InvoiceComponent, canActivate: [AuthGuard], data: { module: "REM", functionality: "ALTA" } },
        { path: ":id/project/:projectId", component: InvoiceDetailComponent, canActivate: [AuthGuard], data: { module: "REM", functionality: "QUERY" } },
        { path: "search", component: InvoiceSearchComponent, canActivate: [AuthGuard], data: { module: "REM", functionality: "QUERY" } },
      ]},
];

export const BillingRouter = RouterModule.forChild(BILLING_ROUTER);