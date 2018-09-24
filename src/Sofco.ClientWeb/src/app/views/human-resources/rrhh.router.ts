import { Routes, RouterModule } from "@angular/router";
import { AuthGuard } from "../../guards/auth.guard";
import { UnemployeesSearchComponent } from "./resources/search-unemployees/unemployees-search.component";
import { AddLicenseComponent } from "./licenses/add/add-license.componente";
import { LicenseListRrhh } from "./licenses/license-dahsboard-rrhh/license-list-rrhh.component";
import { LicenseListManager } from "./licenses/license-list-manager/license-list-manager.component";
import { LicenseDetailComponent } from "./licenses/detail/license-detail.component";
import { NewsComponent } from "./news/news.component";
import { LicenseDelegateComponent } from "./licenses/license-delegate/license-delegate.component";

const RRHH_ROUTER: Routes = [
    { path: "news", component: NewsComponent, canActivate: [AuthGuard], data: { module: "ALLOC", functionality: "NEWSQ" } } ,

    { path: "unemployees", component: UnemployeesSearchComponent, canActivate: [AuthGuard] },

    {
        path: "licenses",
        children: [
          { path: "add", component: AddLicenseComponent, canActivate: [AuthGuard], data: { fromProfile: false, module: "PROFI", functionality: "ALTA" } } ,
          { path: "", component: LicenseListRrhh, canActivate: [AuthGuard], data: { module: "CTRLI", functionality: "QUERY" } },
          { path: "managers", component: LicenseListManager, canActivate: [AuthGuard], data: { module: "CTRLI", functionality: "AUTH" } },
          { path: "views/delegates", component: LicenseDelegateComponent, canActivate: [AuthGuard], data: { module: "CTRLI", functionality: "LIC_VIEW_DELEGATE" } },
          { path: ":id/detail", component: LicenseDetailComponent, canActivate: [AuthGuard] }
        ]
    },
];

export const RrhhRouter = RouterModule.forChild(RRHH_ROUTER);