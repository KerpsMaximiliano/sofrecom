import { RolesComponent } from './roles/roles.component';
import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { DatatablesModule } from "app/components/datatables/datatables.module";
import { RoleService } from "app/services/role.service";

@NgModule({
  declarations: [RolesComponent],
  imports     : [CommonModule, DatatablesModule],
  providers   : [RoleService],
  exports     : [RolesComponent],
})

export class AdminModule {}
