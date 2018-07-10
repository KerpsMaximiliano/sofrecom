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
import { SettingsComponent } from './settings/settings.component';

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
import { SettingsService } from "app/services/admin/settings.service";
import { UserAddComponent } from 'app/views/admin/users/user-add/user-add.component';
import { SpinnerModule } from 'app/components/spinner/spinner.module';
import { DatePickerModule } from 'app/components/date-picker/date-picker.module';
import { CategoryService } from 'app/services/admin/category.service';
import { CategoryAddComponent } from 'app/views/admin/category/add/category-add.component';
import { CategoryEditComponent } from 'app/views/admin/category/edit/category-edit.component';
import { CategoryListComponent } from 'app/views/admin/category/list/category-list.component';
import { TaskService } from 'app/services/admin/task.service';
import { TaskAddComponent } from 'app/views/admin/tasks/add/task-add.component';
import { TaskListComponent } from 'app/views/admin/tasks/list/task-list.component';
import { TaskEditComponent } from 'app/views/admin/tasks/edit/task-edit.component';
import { Select2Module } from 'app/components/select2/select2';
import { AdminRouter } from 'app/views/admin/admin.router';
import { AreaListComponent } from 'app/views/admin/areas/list/area-list.component';
import { AreaEditComponent } from 'app/views/admin/areas/edit/area-edit.component';
import { AreaAddComponent } from 'app/views/admin/areas/add/area-add.component';
import { AreaService } from 'app/services/admin/area.service';
import { SectorService } from 'app/services/admin/sector.service';
import { SectorAddComponent } from 'app/views/admin/sectors/add/sector-add.component';
import { SectorEditComponent } from 'app/views/admin/sectors/edit/sector-edit.component';
import { SectorListComponent } from 'app/views/admin/sectors/list/sector-list.components';

@NgModule({
  declarations: [RolesComponent, RolAddComponent, RolEditComponent, UsersComponent, GroupsComponent, FunctionalitiesComponent, 
                 UserDetailComponent, GroupEditComponent, GroupAddComponent, ModulesComponent, ModuleEditComponent, UserAddComponent, 
                 SettingsComponent, CategoryAddComponent, CategoryEditComponent, CategoryListComponent, TaskAddComponent, TaskListComponent,
                 TaskEditComponent, AreaListComponent, AreaEditComponent, AreaAddComponent, SectorAddComponent, SectorEditComponent,
                 SectorListComponent],

  imports     : [CommonModule, Ng2DatatablesModule, RouterModule, FormsModule, ICheckModule, Ng2ModalModule, TranslateModule, 
                 SpinnerModule, DatePickerModule, Select2Module, AdminRouter],

  providers   : [RoleService, UserService, GroupService, FunctionalityService, ModuleService, SettingsService, CategoryService,
                 TaskService, AreaService, SectorService],

  exports     : [RolesComponent, RolAddComponent, RolEditComponent, UsersComponent, GroupsComponent, FunctionalitiesComponent, 
                 UserDetailComponent, SettingsComponent]
})

export class AdminModule {}
