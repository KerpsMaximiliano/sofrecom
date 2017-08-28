import { ServicesComponent } from './views/billing/services/services.component';
import { CustomersComponent } from './views/billing/customers/customers.component';
import { AuthGuard } from './guards/auth.guard';
import { RolesComponent } from './views/admin/roles/roles.component';
import { UsersComponent } from './views/admin/users/users.component';
import { UserDetailComponent } from './views/admin/users/user-detail/user-detail.component';
import { GroupsComponent } from './views/admin/groups/groups.component';
import { FunctionalitiesComponent } from './views/admin/functionalities/functionalities.component';
import {Routes} from "@angular/router";

import {StarterViewComponent} from "./views/appviews/starterview.component";
import {LoginComponent} from "./views/appviews/login.component";

import {BlankLayoutComponent} from "./components/common/layouts/blankLayout.component";
import {BasicLayoutComponent} from "./components/common/layouts/basicLayout.component";
import { TopNavigationLayoutComponent } from "./components/common/layouts/topNavigationlayout.component";
import { RolEditComponent } from "app/views/admin/roles/rol-edit/rol-edit.component";
import { RolAddComponent } from "app/views/admin/roles/rol-add/rol-add.component";
import { GroupAddComponent } from "app/views/admin/groups/group-add/group-add.component";
import { GroupEditComponent } from "app/views/admin/groups/group-edit/group-edit.component";
import { ModulesComponent } from "app/views/admin/modules/modules.component";
import { ModuleEditComponent } from "app/views/admin/modules/module-edit/module-edit.component";
import { ProjectsComponent } from "app/views/billing/projects/projects.component";
import { SolfacComponent } from "app/views/billing/solfac/solfac.component";
import { SolfacSearchComponent } from "app/views/billing/solfacSearch/solfacSearch.component";

export const ROUTES:Routes = [
  // Main redirect
  {path: '', redirectTo: 'inicio', pathMatch: 'full', canActivate: [AuthGuard]},

  // App views
  {
    path: 'admin', component: BasicLayoutComponent, canActivate: [AuthGuard],
    children: [
      { path: 'roles', children:[
        { path: '', component: RolesComponent },
        { path: 'add', component: RolAddComponent },
        { path: 'edit/:id', component: RolEditComponent }
      ]},
      { path: 'groups', children:[
        {path: '', component: GroupsComponent},
        {path: 'add', component: GroupAddComponent},
        {path: 'edit/:id', component: GroupEditComponent}
      ]},
      { path: "users", children: [
         { path: '', component: UsersComponent },
         { path: 'detail/:id', component: UserDetailComponent }
      ]},
      { path: "functionalities", component: FunctionalitiesComponent },
      { path: "entities", children: [
        { path: '', component: ModulesComponent },
        { path: 'edit/:id', component: ModuleEditComponent }
      ]},
    ]
  },
  {
    path: 'billing', component: BasicLayoutComponent, canActivate: [AuthGuard],
    children: [
      { path: 'customers', children:[
        { path:"", component: CustomersComponent },
        { path:":customerId/services", children: [
          { path: "", component: ServicesComponent },
          { path: ":serviceId/projects", component: ProjectsComponent }
        ]}
      ]},
      { path: "solfac", component: SolfacComponent },
      { path: "searchSolfac", component: SolfacSearchComponent },
      { path: "remitos", component: StarterViewComponent },
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
    ]
  },

  // Handle all other routes
  {path: '**',  redirectTo: 'inicio'}
];
