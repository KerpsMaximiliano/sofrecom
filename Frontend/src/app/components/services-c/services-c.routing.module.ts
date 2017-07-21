import { ServicesCComponent } from './services-c.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: '', children: 
    [
      {path: '', component: ServicesCComponent}
      //{path: 'project/:id', component: ProjectDetailComponent, data: { title: 'Detalles de Proyecto'}},
      
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ServicesCRoutingModule {}