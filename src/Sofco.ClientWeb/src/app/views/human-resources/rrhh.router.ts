import { Routes, RouterModule } from "@angular/router";
import { AuthGuard } from "app/guards/auth.guard";
import { UnemployeesSearchComponent } from "app/views/human-resources/resources/search-unemployees/unemployees-search.component";
import { AddLicenseComponent } from "app/views/human-resources/licenses/add/add-license.componente";
import { LicenseListRrhh } from "app/views/human-resources/licenses/license-dahsboard-rrhh/license-list-rrhh.component";
import { LicenseListManager } from "app/views/human-resources/licenses/license-list-manager/license-list-manager.component";
import { LicenseDetailComponent } from "app/views/human-resources/licenses/detail/license-detail.component";
import { NewsComponent } from "app/views/human-resources/news/news.component";
import { LicenseViewDelegateComponent } from "./licenses/license-view/license-view-delegate/license-view-delegate.component";
import { LicenseViewDelegateEditComponent } from "./licenses/license-view/license-view-delegate/edit/license-view-delegate-edit.component";

const RRHH_ROUTER: Routes = [
    { path: "news", component: NewsComponent, canActivate: [AuthGuard], data: { module: "ALLOC", functionality: "NEWSQ" } } ,

    { path: "unemployees", component: UnemployeesSearchComponent, canActivate: [AuthGuard] },

    {
        path: "licenses",
        children: [
          { path: "add", component: AddLicenseComponent, canActivate: [AuthGuard], data: { fromProfile: false, module: "PROFI", functionality: "ALTA" } } ,
          { path: "", component: LicenseListRrhh, canActivate: [AuthGuard], data: { module: "CTRLI", functionality: "QUERY" } },
          { path: "managers", component: LicenseListManager, canActivate: [AuthGuard], data: { module: "CTRLI", functionality: "AUTH" } },
          { path: "views/delegates", component: LicenseViewDelegateComponent, canActivate: [AuthGuard], data: { module: "CTRLI", functionality: "LIC_VIEW_DELEGATE" } },
          { path: "views/delegates/edit", component: LicenseViewDelegateEditComponent, canActivate: [AuthGuard], data: { module: "CTRLI", functionality: "LIC_VIEW_DELEGATE" } },
          { path: ":id/detail", component: LicenseDetailComponent, canActivate: [AuthGuard] }
        ]
    },
];

export const RrhhRouter = RouterModule.forChild(RRHH_ROUTER);