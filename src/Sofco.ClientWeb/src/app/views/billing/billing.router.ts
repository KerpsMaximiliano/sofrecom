import { Routes, RouterModule } from "@angular/router";
import { CustomersComponent } from "./customers/customers.component";
import { AuthGuard } from "../../guards/auth.guard";
import { ResourceByServiceComponent } from "../allocation-management/resources/by-service/resource-by-service.component";
import { PurchaseOrdersByServiceComponent } from "./projects/purchaseOrders/purchaseOrders-service.component";
import { BillMultipleProjectsComponent } from "./projects/bill-multiple-projects/bill-multiple-projects.component";
import { ProjectsComponent } from "./projects/project-list/projects.component";
import { ProjectDetailComponent } from "./projects/project-detail/project-detail.component";
import { SolfacComponent } from "./solfac/new/solfac.component";
import { SolfacEditComponent } from "./solfac/edit/solfac-edit.component";
import { SolfacSearchComponent } from "./solfac/search/solfac-search.component";
import { SolfacDetailComponent } from "./solfac/detail/solfac-detail.component";
import { PurchaseOrderSearchComponent } from "./purchaseOrder/search/search-purchaseOrder.component";
import { EditPurchaseOrderComponent } from "./purchaseOrder/edit/edit-purchaseOrder.component";
import { NewPurchaseOrderComponent } from "./purchaseOrder/add/add-purchaseOrder.component";
import { CertificateSearchComponent } from "./certificates/search/search-certificate.component";
import { NewCertificateComponent } from "./certificates/add/add-certificate.component";
import { EditCertificateComponent } from "./certificates/edit/edit-certificate.component";
import { InvoiceComponent } from "./invoice/new/invoice.component";
import { InvoiceDetailComponent } from "./invoice/detail/invoice-detail.component";
import { InvoiceSearchComponent } from "./invoice/search/invoice-search.component";
import { ServicesComponent } from "./services/services.component";
import { PurchaseOrderPendingsComponent } from "./purchaseOrder/pendings/oc-pendings.component";

const BILLING_ROUTER: Routes = [
    { path: 'customers', children:[
        { path: "", component: CustomersComponent, canActivate: [AuthGuard], data: { module: "SOLFA", functionality: "ALTA" } },
        { path: ":customerId/services", children: [
          { path: "", component: ServicesComponent, canActivate: [AuthGuard] },
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
         { path: ":solfacId/edit", component: SolfacEditComponent, canActivate: [AuthGuard], data: { module: "SOLFA", functionality: "ALTA" } },
         { path: "search", component: SolfacSearchComponent, canActivate: [AuthGuard], data: { module: "SOLFA", functionality: "QUERY" } },
         { path: ":solfacId", component: SolfacDetailComponent, canActivate: [AuthGuard], data: { module: "SOLFA", functionality: "QUERY" } }
      ]},

      { path: "purchaseOrders",
          children: [
          { path: "query", component: PurchaseOrderSearchComponent, canActivate: [AuthGuard], data: { module: "PUROR", functionality: "QUERY" } },
          { path: "new", component: NewPurchaseOrderComponent, canActivate: [AuthGuard], data: { module: "PUROR", functionality: "ALTA" } },
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