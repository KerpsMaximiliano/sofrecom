import { ModuleService } from './../../services/module.service';
import { TranslateModule } from '@ngx-translate/core';
import { Ng2ModalModule } from './../../components/modal/ng2modal.module';
import { ICheckModule } from './../../components/icheck/icheck.module';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { Ng2DatatablesModule } from "app/components/datatables/ng2-datatables.module";
import { RoleService } from "app/services/role.service";
import { UserService } from "app/services/user.service";
import { GroupService } from "app/services/group.service";
import { FunctionalityService } from "app/services/functionality.service";

import { RolesComponent } from './roles/roles.component';
import { RolAddComponent } from './roles/rol-add/rol-add.component';
import { RolEditComponent } from './roles/rol-edit/rol-edit.component';

import { UsersComponent } from './users/users.component';
import { UserDetailComponent } from './users/user-detail/user-detail.component';

import { GroupsComponent } from './groups/groups.component';

import { FunctionalitiesComponent } from './functionalities/functionalities.component';
import { GroupEditComponent } from './groups/group-edit/group-edit.component';
import { GroupAddComponent } from './groups/group-add/group-add.component';
import { ModulesComponent } from './modules/modules.component';

@NgModule({
  declarations: [RolesComponent, RolAddComponent, RolEditComponent, UsersComponent, GroupsComponent, FunctionalitiesComponent, UserDetailComponent, GroupEditComponent, GroupAddComponent, ModulesComponent],
  imports     : [CommonModule, Ng2DatatablesModule, RouterModule, FormsModule, ICheckModule, Ng2ModalModule, TranslateModule],
  providers   : [RoleService, UserService, GroupService, FunctionalityService, ModuleService],
  exports     : [RolesComponent, RolAddComponent, RolEditComponent, UsersComponent, GroupsComponent, FunctionalitiesComponent, UserDetailComponent]
})

export class AdminModule {}
