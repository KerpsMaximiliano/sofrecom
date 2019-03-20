import { RouterModule, Routes } from "@angular/router";
import { AuthGuard } from "app/guards/auth.guard";
import { ManagementReportDetailComponent } from "./detail/mr-detail";

const MANAGEMENT_REPORT_ROUTER: Routes = [
    { path: ":customerId/service/:serviceId/detail",  component: ManagementReportDetailComponent, canActivate: [AuthGuard] },
];

export const ManagementReportRouter = RouterModule.forChild(MANAGEMENT_REPORT_ROUTER);