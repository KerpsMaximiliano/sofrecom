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

export const ROUTES:Routes = [
  // Main redirect
  {path: '', redirectTo: 'inicio', pathMatch: 'full', canActivate: [AuthGuard]},

  // App views
  {
    path: 'admin', component: BasicLayoutComponent,
    children: [
      { path: 'roles', children:[
        { path: '', component: RolesComponent, canActivate: [AuthGuard], data: { module: "ROL", functionality: "QUERY" } },
        { path: 'add', component: RolAddComponent, data: { module: "ROL", functionality: "ALTA" } },
        { path: 'edit/:id', component: RolEditComponent, data: { module: "ROL", functionality: "UPDAT" } }
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
    ]
  },
  {
    path: 'billing', component: BasicLayoutComponent,
    children: [
      { path: 'customers', children:[
        { path:"", component: CustomersComponent, canActivate: [AuthGuard], data: { module: "SOLFA", functionality: "ALTA" } },
        { path:":customerId/services", children: [
          { path: "", component: ServicesComponent, canActivate: [AuthGuard] },
          { path: ":serviceId/projects", children: [
            { path: "", component: ProjectsComponent, canActivate: [AuthGuard] },
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

      { path: "invoice",
        children: [
        { path: "new/project/:projectId", component: InvoiceComponent, canActivate: [AuthGuard], data: { module: "REM", functionality: "ALTA" } },
        { path: ":id/project/:projectId", component: InvoiceDetailComponent, canActivate: [AuthGuard], data: { module: "REM", functionality: "QUERY" } },
        { path: "search", component: InvoiceSearchComponent, canActivate: [AuthGuard], data: { module: "REM", functionality: "QUERY" } },
      ]},
    ]
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
