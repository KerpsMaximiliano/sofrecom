import { Routes, RouterModule } from "@angular/router";
import { AuthGuard } from "../../guards/auth.guard";
import { UnemployeesSearchComponent } from "./resources/search-unemployees/unemployees-search.component";
import { AddLicenseComponent } from "./licenses/add/add-license.componente";
import { LicenseListRrhh } from "./licenses/license-dahsboard-rrhh/license-list-rrhh.component";
import { LicenseListManager } from "./licenses/license-list-manager/license-list-manager.component";
import { LicenseDetailComponent } from "./licenses/detail/license-detail.component";
import { NewsComponent } from "./news/news.component";
import { EndNotificationComponent } from "./end-notification/end-notification.component";
import { PrepaidImportComponent } from "./prepaid-import/prepaid-import";
import { PrepaidVerificationComponent } from "./prepaid-verification/prepaid-verification";
import { ReportUpdownComponent } from "./report-up-down/report-up-down";
import { SalaryReportComponent } from "./resources/salary-report/salary-report";

const RRHH_ROUTER: Routes = [
    { path: "news", component: NewsComponent, canActivate: [AuthGuard], data: { module: "ALLOC", functionality: "NEWSQ" } } ,

    { path: "unemployees", component: UnemployeesSearchComponent, canActivate: [AuthGuard], data: { fromRrhh: true, module: "ALLOC", functionality: "VWUEM" } },
    
    { path: "updown", component: ReportUpdownComponent, canActivate: [AuthGuard], data: { fromRrhh: true, module: "ALLOC", functionality: "VWUEM" } },
    
    { path: "salary-report", component: SalaryReportComponent, canActivate: [AuthGuard], data: { fromRrhh: true, module: "ALLOC", functionality: "VWUEM" } },

    {
        path: "licenses",
        children: [
          { path: "add", component: AddLicenseComponent, canActivate: [AuthGuard], data: { fromProfile: false, module: "PROFI", functionality: "ALTA" } } ,
          { path: "", component: LicenseListRrhh, canActivate: [AuthGuard], data: { module: "CTRLI", functionality: "QUERY" } },
          { path: "managers", component: LicenseListManager, canActivate: [AuthGuard], data: { module: "CTRLI", functionality: "AUTH" } },
          { path: ":id/detail", component: LicenseDetailComponent, canActivate: [AuthGuard] }
        ]
    },

    {
      path: "prepaid",
      children: [
        { path: "import", component: PrepaidImportComponent, canActivate: [AuthGuard], data: { module: "ALLOC", functionality: "PREPAID-IMPORT" } } ,
        { path: "verification", component: PrepaidVerificationComponent, canActivate: [AuthGuard], data: { module: "ALLOC", functionality: "PREPAID-CONTROL" }},
      ]
    },

    { path: "endNotification", component: EndNotificationComponent, canActivate: [AuthGuard] },
];

export const RrhhRouter = RouterModule.forChild(RRHH_ROUTER);