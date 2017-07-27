import { ICheckModule } from './../../components/icheck/icheck.module';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { DatatablesModule } from "app/components/datatables/datatables.module";

import { RoleService } from "app/services/role.service";
import { UserService } from "app/services/user.service";
import { GroupService } from "app/services/group.service";
import { FunctionalityService } from "app/services/functionality.service";

import { RolesComponent } from './roles/roles.component';
import { RolAddComponent } from './roles/rol-add/rol-add.component';
import { RolEditComponent } from './roles/rol-edit/rol-edit.component';

import { UsersComponent } from './users/users.component';

import { GroupsComponent } from './groups/groups.component';

import { FunctionalitiesComponent } from './functionalities/functionalities.component';

@NgModule({
  declarations: [RolesComponent, RolAddComponent, RolEditComponent, UsersComponent, GroupsComponent, FunctionalitiesComponent],
  imports     : [CommonModule, DatatablesModule, RouterModule, FormsModule, ICheckModule],
  providers   : [RoleService, UserService, GroupService, FunctionalityService],
  exports     : [RolesComponent, RolAddComponent, RolEditComponent, UsersComponent, GroupsComponent, FunctionalitiesComponent],
})

export class AdminModule {}
