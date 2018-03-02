import { ServicesComponent } from './views/billing/services/services.component';
import { CustomersComponent } from './views/billing/customers/customers.component';
import { AuthGuard } from './guards/auth.guard';
import { RolesComponent } from './views/admin/roles/rol-list/roles.component';
import { UsersComponent } from './views/admin/users/user-list/users.component';
import { UserDetailComponent } from './views/admin/users/user-detail/user-detail.component';
import { GroupsComponent } from './views/admin/groups/group-list/groups.component';
import { FunctionalitiesComponent } from './views/admin/functionalities/functionalities.component';
import {Routes} from "@angular/router";
import {BlankLayoutComponent} from "./components/common/layouts/blankLayout.component";
import {BasicLayoutComponent} from "./components/common/layouts/basicLayout.component";
import { RolEditComponent } from "app/views/admin/roles/rol-edit/rol-edit.component";
import { RolAddComponent } from "app/views/admin/roles/rol-add/rol-add.component";
import { GroupAddComponent } from "app/views/admin/groups/group-add/group-add.component";
import { GroupEditComponent } from "app/views/admin/groups/group-edit/group-edit.component";
import { ModulesComponent } from "app/views/admin/modules/module-list/modules.component";
import { ModuleEditComponent } from "app/views/admin/modules/module-edit/module-edit.component";
import { ProjectsComponent } from "app/views/billing/projects/project-list/projects.component";
import { SolfacComponent } from "app/views/billing/solfac/new/solfac.component";
import { ProjectDetailComponent } from "app/views/billing/projects/project-detail/project-detail.component";
import { SolfacDetailComponent } from "app/views/billing/solfac/detail/solfac-detail.component";
import { SolfacSearchComponent } from "app/views/billing/solfac/search/solfac-search.component";
import { InvoiceComponent } from "app/views/billing/invoice/new/invoice.component";
import { InvoiceDetailComponent } from "app/views/billing/invoice/detail/invoice-detail.component";
import { ForbiddenComponent } from "app/views/appviews/errors/403/forbidden.component";
import { StarterViewComponent } from "app/views/appviews/home/starterview.component";
import { LoginComponent } from "app/views/appviews/login/login.component";
import { SolfacEditComponent } from 'app/views/billing/solfac/edit/solfac-edit.component';
import { InvoiceSearchComponent } from 'app/views/billing/invoice/search/invoice-search.component';
import { UserAddComponent } from 'app/views/admin/users/user-add/user-add.component';
import { SolfacReportComponent } from './views/report/solfac/solfac.component';
import { AnalyticSearchComponent } from 'app/views/allocation-management/analytics/search/analytic-search.component';
import { AddAllocationComponent } from 'app/views/allocation-management/allocation/add-by-analytic/add-by-analytic.component';
import { AddAllocationByResourceComponent } from 'app/views/allocation-management/allocation/add-by-resource/add-by-resource.component';
import { ResourceSearchComponent } from 'app/views/allocation-management/resources/search/resource-search.component';
import { SettingsComponent } from 'app/views/admin/settings/settings.component';
import { AddCostCenterComponent } from 'app/views/allocation-management/cost-center/add/add-cost-center.component';
import { ListCostCenterComponent } from 'app/views/allocation-management/cost-center/list/list-cost-center.component';
import { NewAnalyticComponent } from 'app/views/allocation-management/analytics/new/new-analytic.component';
import { NewsComponent } from 'app/views/allocation-management/news/news.component';
import { EditAnalyticComponent } from 'app/views/allocation-management/analytics/edit/edit-analytic.component';
import { BillMultipleProjectsComponent } from 'app/views/billing/projects/bill-multiple-projects/bill-multiple-projects.component';
import { ResourceByServiceComponent } from 'app/views/allocation-management/resources/by-service/resource-by-service.component';
import { ResourceDetailComponent } from 'app/views/allocation-management/resources/detail/resource-detail.component';
import { AllocationReportComponent } from 'app/views/allocation-management/allocation/report/allocation-report.component';
import { ViewAnalyticComponent } from 'app/views/allocation-management/analytics/view/view-analytic.component';
import { EditCostCenterComponent } from 'app/views/allocation-management/cost-center/edit/edit-cost-center.component';
import { NewPurchaseOrderComponent } from 'app/views/billing/purchaseOrder/add/add-purchaseOrder.component';
import { EditPurchaseOrderComponent } from 'app/views/billing/purchaseOrder/edit/edit-purchaseOrder.component';
import { PurchaseOrderSearchComponent } from 'app/views/billing/purchaseOrder/search/search-purchaseOrder.component';
import { PurchaseOrdersByServiceComponent } from 'app/views/billing/projects/purchaseOrders/purchaseOrders-service.component';
import { SolfacDelegateComponent } from 'app/views/billing/solfac/solfac-delegate/solfac-delegate.component';
import { CertificateSearchComponent } from 'app/views/billing/certificates/search/search-certificate.component';
import { CertificateFormComponent } from 'app/views/billing/certificates/form/certificate-form.component';
import { EditCertificateComponent } from 'app/views/billing/certificates/edit/edit-certificate.component';
import { NewCertificateComponent } from 'app/views/billing/certificates/add/add-certificate.component';
import { AddLicenseComponent } from 'app/views/human-resources/licenses/add/add-license.componente';
import { SolfacDelegateEditComponent } from 'app/views/billing/solfac/solfac-delegate/edit/solfac-delegate-edit.component';

export const ROUTES:Routes = [
  // Main redirect
  {path: '', redirectTo: 'inicio', pathMatch: 'full', canActivate: [AuthGuard]},

  // App views
  {
    path: 'admin', component: BasicLayoutComponent,
    children: [
      { path: 'roles', children:[
        { path: '', component: RolesComponent, canActivate: [AuthGuard], data: { module: "ROL", functionality: "QUERY" } },
        { path: 'add', component: RolAddComponent, canActivate: [AuthGuard], data: { module: "ROL", functionality: "ALTA" } },
        { path: 'edit/:id', component: RolEditComponent, canActivate: [AuthGuard], data: { module: "ROL", functionality: "UPDAT" } }
      ]},

      { path: 'groups', children:[
        {path: '', component: GroupsComponent, canActivate: [AuthGuard], data: { module: "GRP", functionality: "QUERY" } },
        {path: 'add', component: GroupAddComponent, canActivate: [AuthGuard], data: { module: "GRP", functionality: "ALTA" } },
        {path: 'edit/:id', component: GroupEditComponent, canActivate: [AuthGuard], data: { module: "GRP", functionality: "UPDAT" } }
      ]},

      { path: "users", children: [
         { path: '', component: UsersComponent, canActivate: [AuthGuard], data: { module: "USR", functionality: "QUERY" } },
         { path: 'add', component: UserAddComponent, canActivate: [AuthGuard] },
         { path: 'detail/:id', component: UserDetailComponent, canActivate: [AuthGuard], data: { module: "USR", functionality: "UPDAT" } }
      ]},

      { path: "functionalities", component: FunctionalitiesComponent, canActivate: [AuthGuard], data: { module: "FUNC", functionality: "QUERY" } },

      { path: "entities", children: [
        { path: '', component: ModulesComponent, canActivate: [AuthGuard], data: { module: "MOD", functionality: "QUERY" } },
        { path: 'edit/:id', component: ModuleEditComponent, canActivate: [AuthGuard], data: { module: "MOD", functionality: "UPDAT" } }
      ]},

      { path: 'settings', children:[
        {path: '', component: SettingsComponent, canActivate: [AuthGuard], data: { module: "PARMS", functionality: "UPDAT" } }
      ]},
    ]
  },

  {
    path: 'billing', component: BasicLayoutComponent,
    children: [
      { path: 'customers', children:[
        { path:"", component: CustomersComponent, canActivate: [AuthGuard], data: { module: "SOLFA", functionality: "ALTA" } },
        { path:":customerId/services", children: [
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
          { path: "", component: PurchaseOrderSearchComponent, canActivate: [AuthGuard], data: { module: "PUROR", functionality: "QUERY" } },
          { path: "new", component: NewPurchaseOrderComponent, canActivate: [AuthGuard], data: { module: "PUROR", functionality: "ALTA" } },
          { path: ":id", component: EditPurchaseOrderComponent, canActivate: [AuthGuard], data: { module: "PUROR", functionality: "ALTA" } },
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
    ]
  },

  {
    path: 'contracts', component: BasicLayoutComponent,
    children: [
      { path:"analytics", 
      children: [
        { path: "", component: AnalyticSearchComponent, canActivate: [AuthGuard], data: { module: "ALLOC", functionality: "QUERY" } },
        { path: "new", component: NewAnalyticComponent, canActivate: [AuthGuard], data: { module: "ALLOC", functionality: "ANADD" } },
        { path: ":id/edit", component: EditAnalyticComponent, canActivate: [AuthGuard], data: { module: "ALLOC", functionality: "ANEDT" } },
        { path: ":id/view", component: ViewAnalyticComponent, canActivate: [AuthGuard], data: { module: "ALLOC", functionality: "ANVIW" } },
        { path: ":id/allocations", component: AddAllocationComponent, canActivate: [AuthGuard], data: { module: "ALLOC", functionality: "ADRES" } },
      ]},
      {
        path: "costCenter",
        children: [
          { path: "", component: ListCostCenterComponent, canActivate: [AuthGuard], data: { module: "ALLOC", functionality: "CCLST" } },
          { path: "add", component: AddCostCenterComponent, canActivate: [AuthGuard], data: { module: "ALLOC", functionality: "CCADD" } },
          { path: ":id/edit", component: EditCostCenterComponent, canActivate: [AuthGuard], data: { module: "ALLOC", functionality: "CCADD" } }
        ]
      }]
  },

  {
    path: 'allocationManagement', component: BasicLayoutComponent,
    children: [
      { path: "allocationsReport", component: AllocationReportComponent, canActivate: [AuthGuard], data: { module: "ALLOC", functionality: "PMORP" } },
      {
        path: "resources", 
        children: [
          { path:"", component: ResourceSearchComponent, canActivate: [AuthGuard], data: { module: "ALLOC", functionality: "LSTRE" } },
          { path:":id", component: ResourceDetailComponent, canActivate: [AuthGuard], data: { module: "ALLOC", functionality: "LSTRE" } },
          { path:":id/allocations", component: AddAllocationByResourceComponent, canActivate: [AuthGuard], data: { module: "ALLOC", functionality: "ADRES" } },
        ]
      },
      {
        path: "employees", 
        children: [
          { path:"news", component: NewsComponent, canActivate: [AuthGuard], data: { module: "ALLOC", functionality: "NEWSQ" } } ,
        ]
      },
      {
        path:"licenses",
        children: [
          { path:"add", component: AddLicenseComponent, canActivate: [AuthGuard], data: { fromProfile: false, module: "ALLOC", functionality: "ALTA" } } ,
        ]
      }]
  },

  {
    path: 'profile', component: BasicLayoutComponent,
    children: [
      { 
        path:"licenses",
        children: [
          { path:"add", component: AddLicenseComponent, canActivate: [AuthGuard], data: { fromProfile: true, module: "ALLOC", functionality: "ALTA" } } ,
        ]
      }]
  },

  {
    path: 'reports', component: BasicLayoutComponent,
    children: [
      { 
        path: 'solfacs', children:[
          { path:"", component: SolfacReportComponent, canActivate: [AuthGuard], data: { module: "REPOR", functionality: "REPOR" } }
        ]
      }
    ]
  },
      
  {
    path: '', component: BasicLayoutComponent,
    children: [
      {path: 'inicio', component: StarterViewComponent, canActivate: [AuthGuard]}
    ]
  },
  {
    path: '', component: BlankLayoutComponent,
    children: [
      { path: 'login', component: LoginComponent },
      { path: '403', component: ForbiddenComponent, canActivate: [AuthGuard] }
    ]
  },

  // Handle all other routes
  {path: '**',  redirectTo: 'inicio'}
];
