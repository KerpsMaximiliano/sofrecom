import { TranslateModule } from '@ngx-translate/core';
import { Ng2ModalModule } from '../../components/modal/ng2modal.module';
import { ICheckModule } from '../../components/icheck/icheck.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { Ng2DatatablesModule } from "../../components/datatables/ng2-datatables.module";

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
import { FunctionalityService } from "../../services/admin/functionality.service";
import { GroupService } from "../../services/admin/group.service";
import { ModuleService } from "../../services/admin/module.service";
import { RoleService } from "../../services/admin/role.service";
import { UserService } from "../../services/admin/user.service";
import { SettingsService } from "../../services/admin/settings.service";
import { UserAddComponent } from './users/user-add/user-add.component';
import { SpinnerModule } from '../../components/spinner/spinner.module';
import { DatePickerModule } from '../../components/date-picker/date-picker.module';
import { CategoryService } from '../../services/admin/category.service';
import { CategoryAddComponent } from './category/add/category-add.component';
import { CategoryEditComponent } from './category/edit/category-edit.component';
import { CategoryListComponent } from './category/list/category-list.component';
import { TaskService } from '../../services/admin/task.service';
import { TaskAddComponent } from './tasks/add/task-add.component';
import { TaskListComponent } from './tasks/list/task-list.component';
import { TaskEditComponent } from './tasks/edit/task-edit.component';
import { Select2Module } from '../../components/select2/select2';
import { AdminRouter } from './admin.router';
import { AreaListComponent } from './areas/list/area-list.component';
import { AreaEditComponent } from './areas/edit/area-edit.component';
import { AreaAddComponent } from './areas/add/area-add.component';
import { AreaService } from '../../services/admin/area.service';
import { SectorService } from '../../services/admin/sector.service';
import { SectorAddComponent } from './sectors/add/sector-add.component';
import { SectorEditComponent } from './sectors/edit/sector-edit.component';
import { SectorListComponent } from './sectors/list/sector-list.components';
import { NgSelectModule } from '@ng-select/ng-select';
import { WorkflowListComponent } from './workflow/workflows-list/workflow-list.component';
import { WorkflowService } from 'app/services/workflow/workflow.service';
import { WorkflowDetailComponent } from './workflow/workflow-detail/workflow-detail.component';
import { WorkflowAddComponent } from './workflow/workflow-add/workflow-add.component';
import { WorkflowTransitionAddComponent } from './workflow/transition-add/transition-add';
import { UtilsService } from 'app/services/common/utils.service';
import { WorkflowTransitionFormComponent } from './workflow/transition-form/transition-form';
import { WorkflowTransitionEditComponent } from './workflow/transition-edit/transition-edit';
import { WorkflowStateListComponent } from './workflow/state-list/state-list.component';
import { WorkflowStateAddComponent } from './workflow/state-add/state-add.component';
import { WorkflowStateEditComponent } from './workflow/state-edit/state-edit.component';

@NgModule({
  declarations: [RolesComponent, RolAddComponent, RolEditComponent, UsersComponent, GroupsComponent, FunctionalitiesComponent,
                 UserDetailComponent, GroupEditComponent, GroupAddComponent, ModulesComponent, ModuleEditComponent, UserAddComponent, 
                 SettingsComponent, CategoryAddComponent, CategoryEditComponent, CategoryListComponent, TaskAddComponent, TaskListComponent,
                 TaskEditComponent, AreaListComponent, AreaEditComponent, AreaAddComponent, SectorAddComponent, SectorEditComponent, WorkflowTransitionEditComponent,
                 SectorListComponent, WorkflowListComponent, WorkflowDetailComponent, WorkflowAddComponent, WorkflowTransitionAddComponent, WorkflowTransitionFormComponent,
                 WorkflowStateListComponent, WorkflowStateAddComponent, WorkflowStateEditComponent],

  imports     : [CommonModule, Ng2DatatablesModule, RouterModule, FormsModule, ICheckModule, Ng2ModalModule, TranslateModule, 
                 SpinnerModule, DatePickerModule, Select2Module, AdminRouter, NgSelectModule, ReactiveFormsModule],

  providers   : [RoleService, UserService, GroupService, FunctionalityService, ModuleService, SettingsService, CategoryService,
                 TaskService, AreaService, SectorService, WorkflowService, UtilsService],

  exports     : [RolesComponent, RolAddComponent, RolEditComponent, UsersComponent, GroupsComponent, FunctionalitiesComponent, 
                 UserDetailComponent, SettingsComponent]
})

export class AdminModule {}
 