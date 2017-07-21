import { ProfilesComponent } from './components/admin/profiles/profiles.component';
import { DashboardComponent } from './layout/dashboard/dashboard.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule, PreloadAllModules } from '@angular/router';
import { BasicLayoutComponent } from "app/components/common/layouts/basicLayout.component";

const APP_ROUTES: Routes = [
    { path: 'home', component: BasicLayoutComponent,
        children: [
          {path: '', component: DashboardComponent}
        ]
    },//DashboardComponent}, //loadChildren: './layout/dashboard/dashboard.module#DashboardModule' },
    { path: 'services', component: BasicLayoutComponent, loadChildren: './components/services-c/services-c.module#ServicesCModule'},//component: ServicesCComponent},
    { path: 'requirements', component: BasicLayoutComponent, loadChildren: './components/requirements/requirements.module#RequirementsModule'},
    { path: 'assignments', component: BasicLayoutComponent, loadChildren: './components/assignments/assignments.module#AssignmentsModule'},
    { path: 'users', component: BasicLayoutComponent, loadChildren: './components/admin/users/users.module#UsersModule'},
    { path: 'profiles', component: BasicLayoutComponent, loadChildren: './components/admin/profiles/profiles.module#ProfilesModule'},
    { path: 'dashboards', component: BasicLayoutComponent, loadChildren: './views/dashboards/dashboards.module#DashboardsModule'},
    { path: '', redirectTo: 'home', pathMatch:'full'}
];


@NgModule({
  imports: [ RouterModule.forRoot(APP_ROUTES, {preloadingStrategy: PreloadAllModules}) ],
  exports: [ RouterModule ]
})
export class AppRoutingModule {}