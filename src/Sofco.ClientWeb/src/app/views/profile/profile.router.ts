import { Routes, RouterModule } from "@angular/router";
import { WorkTimeComponent } from "../worktime-management/worktime/worktime.component";
import { ResourceDetailComponent } from "../allocation-management/resources/detail/resource-detail.component";
import { AuthGuard } from "../../guards/auth.guard";
import { AddLicenseComponent } from "../human-resources/licenses/add/add-license.componente";
import { AdvancementAddComponent } from "app/views/advancementAndRefund/advancement/add/advancement-add.component";
import { AdvancementEditComponent } from "../advancementAndRefund/advancement/edit/advancement-edit.component";

const PROFILE_ROUTER: Routes = [
    { path: "workTime", component: WorkTimeComponent, canActivate: [AuthGuard], data: { module: "PROFI", functionality: "WORKT" } },
    { path: ":id", component: ResourceDetailComponent, canActivate: [AuthGuard], data: { fromRrhh: false } },
    {
        path: "licenses",
        children: [
            { path: "add", component: AddLicenseComponent, canActivate: [AuthGuard], data: { fromProfile: true, module: "PROFI", functionality: "ALTA" } } ,
        ]
    },
    {
        path: "advancement",
        children: [
            { path: "add", component: AdvancementAddComponent, canActivate: [AuthGuard], data: { module: "ADVAN", functionality: "ADD" } } ,
            { path: "edit/:id", component: AdvancementEditComponent, canActivate: [AuthGuard], data: { module: "ADVAN", functionality: "ADD" } } 
        ]
    }
]

export const ProfileRouter = RouterModule.forChild(PROFILE_ROUTER);