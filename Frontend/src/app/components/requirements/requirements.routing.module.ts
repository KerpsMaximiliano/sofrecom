import { RequirementsComponent } from './requirements.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: '', children: 
    [
      {path: '', component: RequirementsComponent}
      //{path: 'project/:id', component: ProjectDetailComponent, data: { title: 'Detalles de Proyecto'}},
      
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RequirementsRoutingModule {}