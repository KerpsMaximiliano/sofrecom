import { TranslateModule } from '@ngx-translate/core';
import { Ng2ModalModule } from 'app/components/modal/ng2modal.module';
import { ICheckModule } from 'app/components/icheck/icheck.module';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { Ng2DatatablesModule } from "app/components/datatables/ng2-datatables.module";

import { RolesComponent } from './roles/rol-list/roles.component';
import { RolAddComponent } from './roles/rol-add/rol-add.component';
import { RolEditComponent } from './roles/rol-edit/rol-edit.component';

import { UsersComponent } from './users/user-list/users.component';
import { UserDetailComponent } from './users/user-detail/user-detail.component';

import { GroupsComponent } from './groups/group-list/groups.component';

import { FunctionalitiesComponent } from './functionalities/functionalities.component';
import { GroupEditComponent } from './groups/group-edit/group-edit.component';
import { GroupAddComponent } from './groups/group-add/group-add.component';
import { ModulesComponent } from './modules/module-list/modules.component';
import { ModuleEditComponent } from './modules/module-edit/module-edit.component';
import { FunctionalityService } from "app/services/admin/functionality.service";
import { GroupService } from "app/services/admin/group.service";
import { ModuleService } from "app/services/admin/module.service";
import { RoleService } from "app/services/admin/role.service";
import { UserService } from "app/services/admin/user.service";

@NgModule({
  declarations: [RolesComponent, RolAddComponent, RolEditComponent, UsersComponent, GroupsComponent, FunctionalitiesComponent, UserDetailComponent, GroupEditComponent, GroupAddComponent, ModulesComponent, ModuleEditComponent],
  imports     : [CommonModule, Ng2DatatablesModule, RouterModule, FormsModule, ICheckModule, Ng2ModalModule, TranslateModule],
  providers   : [RoleService, UserService, GroupService, FunctionalityService, ModuleService],
  exports     : [RolesComponent, RolAddComponent, RolEditComponent, UsersComponent, GroupsComponent, FunctionalitiesComponent, UserDetailComponent]
})

export class AdminModule {}
