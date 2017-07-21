import { Dashboard41Component } from './dashboard41.component';
import { Dashboard5Component } from './dashboard5.component';
import { Dashboard4Component } from './dashboard4.component';
import { Dashboard3Component } from './dashboard3.component';
import { Dashboard2Component } from './dashboard2.component';
import { Dashboard1Component } from './dashboard1.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: '', children: 
    [
      {path: '', component: Dashboard1Component},
      {path: 'dashboard1', component: Dashboard1Component},
      {path: 'dashboard2', component: Dashboard2Component},
      {path: 'dashboard3', component: Dashboard3Component},
      {path: 'dashboard4', component: Dashboard4Component},
      {path: 'dashboard5', component: Dashboard5Component},
      {path: 'dashboard41', component: Dashboard41Component}
      //{path: 'project/:id', component: ProjectDetailComponent, data: { title: 'Detalles de Proyecto'}},
      
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardsRoutingModule {}