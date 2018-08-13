import { Routes, RouterModule } from "@angular/router";
import { WorkTimeApprovalComponent } from "./approval/worktime-approval.component";
import { WorkTimeReportComponent } from "./report/worktime-report.component";
import { WorkTimeSearchComponent } from "./search/worktime-search.component";
import { HolidaysComponent } from "./holidays/holidays.component";
import { AuthGuard } from "../../guards/auth.guard";

const WORKTIME_ROUTER: Routes = [
    {
        path: 'workTime', children:[
          { path: "approval", component: WorkTimeApprovalComponent, canActivate: [AuthGuard], data: { module: "WOTIM", functionality: "APPRO" } },
          { path: "report", component: WorkTimeReportComponent, canActivate: [AuthGuard], data: { module: "WOTIM", functionality: "REPOR" } },
          { path: "search", component: WorkTimeSearchComponent, canActivate: [AuthGuard] }
        ]
      },
      {
        path: 'holidays',
        children: [
          { path: "", component: HolidaysComponent, canActivate: [AuthGuard], data: { module: "WOTIM", functionality: "HOLID" } }
        ]
      },
];

export const WorkTimeRouter = RouterModule.forChild(WORKTIME_ROUTER);