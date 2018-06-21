import { Routes, RouterModule } from "@angular/router";
import { SettingsComponent } from "app/views/admin/settings/settings.component";
import { AuthGuard } from "app/guards/auth.guard";
import { TaskEditComponent } from "app/views/admin/tasks/edit/task-edit.component";
import { TaskAddComponent } from "app/views/admin/tasks/add/task-add.component";
import { TaskListComponent } from "app/views/admin/tasks/list/task-list.component";
import { CategoryEditComponent } from "app/views/admin/category/edit/category-edit.component";
import { CategoryAddComponent } from "app/views/admin/category/add/category-add.component";
import { CategoryListComponent } from "app/views/admin/category/list/category-list.component";
import { ModuleEditComponent } from "app/views/admin/modules/module-edit/module-edit.component";
import { ModulesComponent } from "app/views/admin/modules/module-list/modules.component";
import { FunctionalitiesComponent } from "app/views/admin/functionalities/functionalities.component";
import { UserDetailComponent } from "app/views/admin/users/user-detail/user-detail.component";
import { UserAddComponent } from "app/views/admin/users/user-add/user-add.component";
import { UsersComponent } from "app/views/admin/users/user-list/users.component";
import { GroupEditComponent } from "app/views/admin/groups/group-edit/group-edit.component";
import { GroupAddComponent } from "app/views/admin/groups/group-add/group-add.component";
import { GroupsComponent } from "app/views/admin/groups/group-list/groups.component";
import { RolEditComponent } from "app/views/admin/roles/rol-edit/rol-edit.component";
import { RolAddComponent } from "app/views/admin/roles/rol-add/rol-add.component";
import { RolesComponent } from "app/views/admin/roles/rol-list/roles.component";

const ADMIN_ROUTER: Routes = [
    { path: 'roles', children:[
        { path: '', component: RolesComponent, canActivate: [AuthGuard], data: { module: "ROL", functionality: "QUERY" } },
        { path: 'add', component: RolAddComponent, canActivate: [AuthGuard], data: { module: "ROL", functionality: "ALTA" } },
        { path: 'edit/:id', component: RolEditComponent, canActivate: [AuthGuard], data: { module: "ROL", functionality: "UPDAT" } }
      ]},

      { path: 'groups', children:[
        {path: '', component: GroupsComponent, canActivate: [AuthGuard], data: { module: "GRP", functionality: "QUERY" } },
        {path: 'add', component: GroupAddComponent, canActivate: [AuthGuard], data: { module: "GRP", functionality: "ALTA" } },
        {path: 'edit/:id', component: GroupEditComponent, canActivate: [AuthGuard], data: { module: "GRP", functionality: "UPDAT" } }
      ]},

      { path: "users", children: [
         { path: '', component: UsersComponent, canActivate: [AuthGuard], data: { module: "USR", functionality: "QUERY" } },
         { path: 'add', component: UserAddComponent, canActivate: [AuthGuard] },
         { path: 'detail/:id', component: UserDetailComponent, canActivate: [AuthGuard], data: { module: "USR", functionality: "UPDAT" } }
      ]},

      { path: "functionalities", component: FunctionalitiesComponent, canActivate: [AuthGuard], data: { module: "FUNC", functionality: "QUERY" } },

      { path: "entities", children: [
        { path: '', component: ModulesComponent, canActivate: [AuthGuard], data: { module: "MOD", functionality: "QUERY" } },
        { path: 'edit/:id', component: ModuleEditComponent, canActivate: [AuthGuard], data: { module: "MOD", functionality: "UPDAT" } }
      ]},

      { path: "categories", children: [
        { path: '', component: CategoryListComponent, canActivate: [AuthGuard], data: { module: "CATEG", functionality: "QUERY" } },
        { path: 'add', component: CategoryAddComponent, canActivate: [AuthGuard], data: { module: "CATEG", functionality: "ADD" } },
        { path: ':id/edit', component: CategoryEditComponent, canActivate: [AuthGuard], data: { module: "CATEG", functionality: "EDIT" } }
      ]},

      { path: "tasks", children: [
        { path: '', component: TaskListComponent, canActivate: [AuthGuard], data: { module: "TASKS", functionality: "QUERY" } },
        { path: 'add', component: TaskAddComponent, canActivate: [AuthGuard], data: { module: "TASKS", functionality: "ADD" } },
        { path: ':id/edit', component: TaskEditComponent, canActivate: [AuthGuard], data: { module: "TASKS", functionality: "EDIT" } }
      ]},

      { path: 'settings', children: [
        { path: '', component: SettingsComponent, canActivate: [AuthGuard], data: { module: "PARMS", functionality: "UPDAT" } }
      ]}
];

export const AdminRouter = RouterModule.forChild(ADMIN_ROUTER);
