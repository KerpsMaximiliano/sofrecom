
import { AuthGuard } from './guards/auth.guard';
import { Routes, RouterModule } from "@angular/router";
import { BlankLayoutComponent } from "./components/common/layouts/blankLayout.component";
import { BasicLayoutComponent } from "./components/common/layouts/basicLayout.component";
import { ForbiddenComponent } from "./views/appviews/errors/403/forbidden.component";
import { StarterViewComponent } from "./views/appviews/home/starterview.component";
import { LoginComponent } from "./views/appviews/login/login.component";
import { ModuleWithProviders } from '@angular/core';
import { PdfComponent } from './views/appviews/pdf/pdf.component';

export const ROUTES:Routes = [
  { path: '', redirectTo: 'inicio', pathMatch: 'full', canActivate: [AuthGuard] },

  {
    path: '', component: BasicLayoutComponent,
    children: [
      { path: 'inicio', component: StarterViewComponent, canActivate: [AuthGuard] }
    ]
  },

  {
    path: '', component: BlankLayoutComponent,
    children: [
      { path: 'login', component: LoginComponent },
      { path: '403', component: ForbiddenComponent, canActivate: [AuthGuard] },
    ]
  },

  {
    path: 'pdf', component: BasicLayoutComponent,
    children: [
      { path: ':id/:type', component: PdfComponent, canActivate: [AuthGuard] }
    ]
  },

  {
    path: 'admin', 
    component: BasicLayoutComponent,
    loadChildren: 'app/views/admin/admin.module#AdminModule'
  },

  {
    path: 'billing', 
    component: BasicLayoutComponent,
    loadChildren: 'app/views/billing/billing.module#BillingModule'
  },

  {
    path: 'contracts', 
    component: BasicLayoutComponent,
    loadChildren: 'app/views/contracts/contracts.module#ContractsModule'
  },

  {
    path: 'allocationManagement', 
    component: BasicLayoutComponent,
    loadChildren: 'app/views/allocation-management/allocation-management.module#AllocationManagementModule'
  },
 
  {
    path: 'rrhh', 
    component: BasicLayoutComponent,
    loadChildren: 'app/views/human-resources/human-resources.module#HumanResourcesModule'
  },

  {
    path: 'profile', 
    component: BasicLayoutComponent,
    loadChildren: 'app/views/profile/profile.module#ProfileModule'
  },

  {
    path: 'workTimeManagement', 
    component: BasicLayoutComponent,
    loadChildren: 'app/views/worktime-management/worktime-management.module#WorkTimeManagementModule'
  },

  // {
  //   path: 'advancementAndRefund', 
  //   component: BasicLayoutComponent,
  //   loadChildren: 'app/views/advancementAndRefund/advancementAndRefund.module#AdvancementAndRefundModule'
  // },

  // Handle all other routes
  { path: '**',  redirectTo: 'inicio' }
];

export const appRouter: ModuleWithProviders = RouterModule.forRoot(ROUTES);