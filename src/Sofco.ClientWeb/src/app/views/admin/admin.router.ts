import { Routes, RouterModule } from "@angular/router";
import { SettingsComponent } from "./settings/settings.component";
import { AuthGuard } from "../../guards/auth.guard";
import { TaskEditComponent } from "./tasks/edit/task-edit.component";
import { TaskAddComponent } from "./tasks/add/task-add.component";
import { TaskListComponent } from "./tasks/list/task-list.component";
import { CategoryEditComponent } from "./category/edit/category-edit.component";
import { CategoryAddComponent } from "./category/add/category-add.component";
import { CategoryListComponent } from "./category/list/category-list.component";
import { ModuleEditComponent } from "./modules/module-edit/module-edit.component";
import { ModulesComponent } from "./modules/module-list/modules.component";
import { FunctionalitiesComponent } from "./functionalities/functionalities.component";
import { UserDetailComponent } from "./users/user-detail/user-detail.component";
import { UserAddComponent } from "./users/user-add/user-add.component";
import { UsersComponent } from "./users/user-list/users.component";
import { GroupEditComponent } from "./groups/group-edit/group-edit.component";
import { GroupAddComponent } from "./groups/group-add/group-add.component";
import { GroupsComponent } from "./groups/group-list/groups.component";
import { RolEditComponent } from "./roles/rol-edit/rol-edit.component";
import { RolAddComponent } from "./roles/rol-add/rol-add.component";
import { RolesComponent } from "./roles/rol-list/roles.component";
import { AreaListComponent } from "./areas/list/area-list.component";
import { AreaAddComponent } from "./areas/add/area-add.component";
import { AreaEditComponent } from "./areas/edit/area-edit.component";
import { SectorListComponent } from "./sectors/list/sector-list.components";
import { SectorAddComponent } from "./sectors/add/sector-add.component";
import { SectorEditComponent } from "./sectors/edit/sector-edit.component";

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

      { path: "areas", children: [
        { path: '', component: AreaListComponent, canActivate: [AuthGuard], data: { module: "AREAS", functionality: "QUERY" } },
        { path: 'add', component: AreaAddComponent, canActivate: [AuthGuard], data: { module: "AREAS", functionality: "ALTA" } },
        { path: ':id/edit', component: AreaEditComponent, canActivate: [AuthGuard], data: { module: "AREAS", functionality: "EDIT" } }
      ]},

      { path: "sectors", children: [
        { path: '', component: SectorListComponent, canActivate: [AuthGuard], data: { module: "SECTO", functionality: "QUERY" } },
        { path: 'add', component: SectorAddComponent, canActivate: [AuthGuard], data: { module: "SECTO", functionality: "ALTA" } },
        { path: ':id/edit', component: SectorEditComponent, canActivate: [AuthGuard], data: { module: "SECTO", functionality: "EDIT" } }
      ]},

      { path: 'settings', children: [
        { path: '', component: SettingsComponent, canActivate: [AuthGuard], data: { module: "PARMS", functionality: "UPDAT" } }
      ]}
];

export const AdminRouter = RouterModule.forChild(ADMIN_ROUTER);
