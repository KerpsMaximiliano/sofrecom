import { ICheckModule } from './../../components/icheck/icheck.module';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { RolesComponent } from './roles/roles.component';
import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { DatatablesModule } from "app/components/datatables/datatables.module";
import { RoleService } from "app/services/role.service";
import { RolAddComponent } from './roles/rol-add/rol-add.component';
import { RolEditComponent } from './roles/rol-edit/rol-edit.component';

@NgModule({
  declarations: [RolesComponent, RolAddComponent, RolEditComponent],
  imports     : [CommonModule, DatatablesModule, RouterModule, FormsModule, ICheckModule],
  providers   : [RoleService],
  exports     : [RolesComponent, RolAddComponent, RolEditComponent],
})

export class AdminModule {}
