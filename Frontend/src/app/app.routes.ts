import { RolesComponent } from './views/admin/roles/roles.component';
import { UsersComponent } from './views/admin/users/users.component';
import { UserDetailComponent } from './views/admin/users/user-detail/user-detail.component';
import { GroupsComponent } from './views/admin/groups/groups.component';
import { FunctionalitiesComponent } from './views/admin/functionalities/functionalities.component';
import {Routes} from "@angular/router";

import {Dashboard1Component} from "./views/dashboards/dashboard1.component";
import {Dashboard2Component} from "./views/dashboards/dashboard2.component";
import {Dashboard3Component} from "./views/dashboards/dashboard3.component";
import {Dashboard4Component} from "./views/dashboards/dashboard4.component";
import {Dashboard41Component} from "./views/dashboards/dashboard41.component";
import {Dashboard5Component} from "./views/dashboards/dashboard5.component";

import {StarterViewComponent} from "./views/appviews/starterview.component";
import {LoginComponent} from "./views/appviews/login.component";

import {BlankLayoutComponent} from "./components/common/layouts/blankLayout.component";
import {BasicLayoutComponent} from "./components/common/layouts/basicLayout.component";
import { TopNavigationLayoutComponent } from "./components/common/layouts/topNavigationlayout.component";
import { RolEditComponent } from "app/views/admin/roles/rol-edit/rol-edit.component";
import { RolAddComponent } from "app/views/admin/roles/rol-add/rol-add.component";

export const ROUTES:Routes = [
  // Main redirect
  {path: '', redirectTo: 'starterview', pathMatch: 'full'},

  // App views
  {
    path: 'dashboards', component: BasicLayoutComponent,
    children: [
      {path: 'dashboard1', component: Dashboard1Component},
      {path: 'dashboard2', component: Dashboard2Component},
      {path: 'dashboard3', component: Dashboard3Component},
      {path: 'dashboard4', component: Dashboard4Component},
      {path: 'dashboard5', component: Dashboard5Component}
    ]
  },
  {
    path: 'dashboards', component: TopNavigationLayoutComponent,
    children: [
      {path: 'dashboard41', component: Dashboard41Component}
    ]
  },
  {
    path: 'admin', component: BasicLayoutComponent,
    children: [
      { path: 'roles', children:[
        { path: '', component: RolesComponent },
        { path: 'add', component: RolAddComponent },
        { path: 'edit/:id', component: RolEditComponent }
      ]},

      { path: "users", children: [
         { path: '', component: UsersComponent },
         { path: ':id', component: UserDetailComponent },
      ]},

      { path: "groups", component: GroupsComponent },
      { path: "functionalities", component: FunctionalitiesComponent }
    ]
  },
  {
    path: '', component: BasicLayoutComponent,
    children: [
      {path: 'starterview', component: StarterViewComponent}
    ]
  },
  {
    path: '', component: BlankLayoutComponent,
    children: [
      { path: 'login', component: LoginComponent },
    ]
  },

  // Handle all other routes
  {path: '**',  redirectTo: 'starterview'}
];
