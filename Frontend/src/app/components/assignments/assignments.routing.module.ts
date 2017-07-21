import { NgModule } from '@angular/core';
import { AssignmentsComponent } from './assignments.component';
import { Routes, RouterModule } from '@angular/router';


const routes: Routes = [
    {
        path: '', children: [
            {path: '', component: AssignmentsComponent}
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class AssignmentsRoutingModule{}
