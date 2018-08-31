import { Routes, RouterModule } from "@angular/router";
import { WorkTimeApprovalComponent } from "./approval/worktime-approval.component";
import { WorkTimeReportComponent } from "./report/worktime-report.component";
import { WorkTimeSearchComponent } from "./search/worktime-search.component";
import { HolidaysComponent } from "./holidays/holidays.component";
import { AuthGuard } from "../../guards/auth.guard";
import { ImportWorkTimesComponent } from "app/views/worktime-management/import/import-worktime.component";
import { WorkTimeControlComponent } from "./worktime-control/worktime-control.component";

const WORKTIME_ROUTER: Routes = [
    {
        path: 'workTime', children:[
          { path: "approval", component: WorkTimeApprovalComponent, canActivate: [AuthGuard], data: { module: "WOTIM", functionality: "APPRO" } },
          { path: "report", component: WorkTimeReportComponent, canActivate: [AuthGuard], data: { module: "WOTIM", functionality: "REPOR" } },
          { path: "search", component: WorkTimeSearchComponent, canActivate: [AuthGuard], data: { module: "WOTIM", functionality: "QUERY" } },
          { path: "import", component: ImportWorkTimesComponent, canActivate: [AuthGuard], data: { module: "WOTIM", functionality: "IMPORT" } },
          { path: "control", component: WorkTimeControlComponent, canActivate: [AuthGuard], data: { module: "WOTIM", functionality: "WORKTIMECONTROL" } }
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